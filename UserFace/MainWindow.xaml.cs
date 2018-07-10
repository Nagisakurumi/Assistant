using InterfaceLib.PlugsInterface;
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

namespace UserFace
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// 控件面板集合
        /// </summary>
        private ObservableCollection<MovePanel> movePanels = new ObservableCollection<MovePanel>();

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }
        /// <summary>
        /// 加载事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {

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
            controlTitles.ItemsSource = movePanels;
            MessageCallBack += MainWindow_MessageCallBack;
        }
        /// <summary>
        /// 消息回调
        /// </summary>
        /// <param name="obj"></param>
        private void MainWindow_MessageCallBack(InterfaceLib.MsgInterface.MsgInfo.IInfoBase obj)
        {
            try
            {
                movePanels.Where(p => p.MainId == obj.ReciverId).First().AddMessage(obj);
            }
            catch (Exception)
            {
                Log.Write("接受消息出错!");
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

        private void notice_Richbox_MouseMove(object sender, MouseEventArgs e)
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
        private void TitleBox_MouseLeave(object sender, MouseEventArgs e)
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
