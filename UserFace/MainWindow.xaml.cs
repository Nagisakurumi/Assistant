﻿using InterfaceLib.PlugsInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static UserFace.Interface;
using static LogLib.LogInfo;
using System.Collections.ObjectModel;
using System.Windows.Forms;

namespace UserFace
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// 最小化任务栏图标
        /// </summary>
        private System.Windows.Forms.NotifyIcon notifyIcon = new System.Windows.Forms.NotifyIcon();
        /// <summary>
        /// 控件面板集合
        /// </summary>
        private ObservableCollection<MovePanel> movePanels = new ObservableCollection<MovePanel>();
        /// <summary>
        /// 当前置顶的
        /// </summary>
        private MovePanel currentZindex = null;
        /// <summary>
        /// 其他信息打印
        /// </summary>
        private MovePanel otherPanel = new MovePanel()
        {
            Notice = "其他",
        };

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
            foreach (var item in PlugInfoInterfaces)
            {
                MovePanel ml = new MovePanel()
                {
                    MainId = item.Id,
                    PlugInfoInterface = item,
                };
                ml.Notice = item.Name;
                addControl(ml);
            }
            addControl(otherPanel);
            MessageCallBack += MainWindow_MessageCallBack;
            this.Closed += MainWindow_Closed;
        }
        /// <summary>
        /// 窗体关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_Closed(object sender, EventArgs e)
        {
            notifyIcon.Visible = false;
            notifyIcon.Dispose();
            notifyIcon = null;
        }

        /// <summary>
        /// 加载事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // 设置全屏

            //this.Left = 0.0;
            //this.Top = 0.0;
            //this.Width = System.Windows.SystemParameters.PrimaryScreenWidth;
            //this.Height = System.Windows.SystemParameters.PrimaryScreenHeight;

            controlTitles.ItemsSource = movePanels;


            InitIcon();
        }
        #region IconOption

        private void InitIcon()
        {
            this.notifyIcon.BalloonTipText = "助手"; //设置程序启动时显示的文本
            this.notifyIcon.Text = "Assistant";//最小化到托盘时，鼠标点击时显示的文本
            this.notifyIcon.Icon = Properties.Resources.icon;
            this.notifyIcon.Visible = true;
            //notifyIcon.MouseDoubleClick += OnNotifyIconDoubleClick;
            this.notifyIcon.ShowBalloonTip(1000);

            System.Windows.Forms.MenuItem m1 = new System.Windows.Forms.MenuItem("关闭");
            m1.Click += Server_Close;
            System.Windows.Forms.MenuItem m2 = new System.Windows.Forms.MenuItem("全部隐藏");
            m2.Click += Hidden_All;
            System.Windows.Forms.MenuItem m3 = new System.Windows.Forms.MenuItem("显示");
            m2.Click += Show_All;
            System.Windows.Forms.MenuItem[] m = new System.Windows.Forms.MenuItem[] { m1, m2, m3 };
            this.notifyIcon.ContextMenu = new System.Windows.Forms.ContextMenu(m);
        }

        private void Show_All(object sender, EventArgs e)
        {
            this.Visibility = Visibility.Visible;
        }

        private void Hidden_All(object sender, EventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
        }

        private void Server_Close(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        /// <summary>
        /// 消息回调
        /// </summary>
        /// <param name="obj"></param>
        private void MainWindow_MessageCallBack(InterfaceLib.MsgInterface.MsgInfo.IInfoBase obj)
        {
            try
            {
                if(obj.MessageType == InterfaceLib.MsgInterface.MsgInfo.Enums.MessageType.File)
                {
                    otherPanel.AddMessage(obj);
                }
                else
                {
                    movePanels.Where(p => p.MainId.Equals(obj.SendId)).First().AddMessage(obj);
                }
            }
            catch (Exception ex)
            {
                otherPanel.AddMessage(obj);
                //Log.Write("接受消息出错!", ex, "obj.SendId-->", obj.SendId, "obj.ReciverId-->", obj.ReciverId);
            }
        }

        /// <summary>
        /// titles选项改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void controlTitles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MovePanel movePanel = controlTitles.SelectedItem as MovePanel;
            Canvas.SetZIndex(movePanel, 9999);
            if(currentZindex != null)
            {
                Canvas.SetZIndex(currentZindex, 1);
            }
            currentZindex = movePanel;
            movePanel.Visibility = Visibility.Visible;
        }

        private void addControl(MovePanel movePanel)
        {
            this.controls.Children.Add(movePanel);
            movePanel.Md += notice_Richbox_MouseDown;
            movePanel.MV += notice_Richbox_MouseMove;
            movePanel.MU += notice_Richbox_MouseUp;
            movePanel.ML += TitleBox_MouseLeave;
            movePanels.Add(movePanel);
        }

        

        private void removeControl(MovePanel movePanel)
        {
            if(movePanels.Contains(movePanel))
            {
                this.controls.Children.Remove(movePanel);
                movePanels.Remove(movePanel);
            }
        }



        #region 移动
        /// <summary>
        /// 当前鼠标位置
        /// </summary>
        private Point currentPoint = new Point();
        /// <summary>
        /// 是否进入移动
        /// </summary>
        private bool isMove = false;
        /// <summary>
        /// 鼠标事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void notice_Richbox_MouseDown(object sender, MouseButtonEventArgs e)
        {
            isMove = true;
            currentPoint = e.GetPosition(this);
        }

        private void notice_Richbox_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Point point = e.GetPosition(this);
            if (isMove)
            {
                Point decPoint = new Point(point.X - currentPoint.X
                    , point.Y - currentPoint.Y);
                if (Math.Sqrt(Math.Pow(decPoint.X, 2) + Math.Pow(decPoint.Y, 2)) > 10)
                {
                    (sender as MovePanel).PanelX += decPoint.X;
                    (sender as MovePanel).PanelY += decPoint.Y;
                    currentPoint = e.GetPosition(this);
                }
            }
        }
        private void TitleBox_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            isMove = false;
        }
        private void notice_Richbox_MouseUp(object sender, MouseButtonEventArgs e)
        {
            isMove = false;
        }
        #endregion
    }
}
