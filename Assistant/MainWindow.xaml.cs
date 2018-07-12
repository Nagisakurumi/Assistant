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
using static Assistant.ServerLog;
using static Assistant.Plugs.PluginsManager;
using static Assistant.MessageRoute.MessageRoute;
using static Assistant.Plugs.ServerInterface;

namespace Assistant
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
        /// 构造函数
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            this.Closed += MainWindow_Closed;
            this.Closing += MainWindow_Closing;
            Log.ErroStringEvent += Log_ErroStringEvent;

            ///加载插件
            Manager.LoadPlugins();
            Manager.InitPlugins();
            Log.Write("完成插件加载!");
            Log.Write("所有初始化后的插件列表!");
            MessageRouteInfo.Start();
            Log.Write("开启消息路由!");

            foreach (var item in Manager.PluginsContainer.Values)
            {
                Log.Write(Newtonsoft.Json.JsonConvert.SerializeObject(item));
            }

            this.Loaded += MainWindow_Loaded;
            
        }
        /// <summary>
        /// 关闭前
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Hidden_All(null, null);
        }

        /// <summary>
        /// 窗体关闭的时候
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
        /// 日志写
        /// </summary>
        /// <param name="obj"></param>
        private void Log_ErroStringEvent(string obj)
        {
            this.Dispatcher.Invoke(new Action<string>((msg) =>
            {
                if(content.Inlines.Count > 10000)
                {
                    content.Inlines.Clear();
                }
                content.Inlines.Add(msg);
                content_Richbox.ScrollToEnd();
            }), obj);
        }

        /// <summary>
        /// 加载事件        
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.Hide();
            InitIcon();
        }

        #region IconOption

        private void InitIcon()
        {
            this.notifyIcon.BalloonTipText = "助手服务开启"; //设置程序启动时显示的文本
            this.notifyIcon.Text = "Assistant";//最小化到托盘时，鼠标点击时显示的文本
            this.notifyIcon.Icon = Properties.Resources.icon;
            this.notifyIcon.Visible = true;
            //notifyIcon.MouseDoubleClick += OnNotifyIconDoubleClick;
            this.notifyIcon.ShowBalloonTip(1000);

            System.Windows.Forms.MenuItem m1 = new System.Windows.Forms.MenuItem("关闭");
            m1.Click += Server_Close;
            System.Windows.Forms.MenuItem m2 = new System.Windows.Forms.MenuItem("隐藏");
            m2.Click += Hidden_All;
            System.Windows.Forms.MenuItem m3 = new System.Windows.Forms.MenuItem("显示");
            m2.Click += Show_All;
            System.Windows.Forms.MenuItem[] m = new System.Windows.Forms.MenuItem[] { m1, m2, m3 };
            this.notifyIcon.ContextMenu = new System.Windows.Forms.ContextMenu(m);
        }

        private void Show_All(object sender, EventArgs e)
        {
            this.Show();
            ShowInTaskbar = true;
        }

        private void Hidden_All(object sender, EventArgs e)
        {
            this.Hide();
            ShowInTaskbar = false;
        }

        private void Server_Close(object sender, EventArgs e)
        {
            App.Current.Shutdown();
        }

        #endregion
    }
}
