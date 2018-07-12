using InterfaceLib.MsgInterface.MsgInfo;
using InterfaceLib.PlugsInterface;
using System;
using System.Collections.Generic;
using System.IO;
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
using static UserFace.FaceInterfaceLog;
using System.Windows.Media.Animation;
using UserFace.Tools;
using System.Windows.Interop;

namespace UserFace
{
    /// <summary>
    /// MovePanel.xaml 的交互逻辑
    /// </summary>
    public partial class MovePanel : UserControl
    {
        public MovePanel()
        {
            InitializeComponent();
            this.Loaded += MovePanel_Loaded;
        }
        /// <summary>
        /// 最大显示数量
        /// </summary>
        public static readonly long MaxLogCount = 1000;
        /// <summary>
        /// 加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MovePanel_Loaded(object sender, RoutedEventArgs e)
        {
            hideIcon.Source = Imaging.CreateBitmapSourceFromHBitmap(Properties.Resources.hide.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            try
            {
                idCombox.ItemsSource = PlugInfoInterfaces;
                idCombox.DisplayMemberPath = "Name";
                idCombox.SelectedItem = PlugInfoInterfaces.Where(p => p.Id == MainId).First();
            }
            catch (Exception)
            {
                
            }
        }

        /// <summary>
        /// 面板在Canvas中 X坐标
        /// </summary>
        public static readonly DependencyProperty PanelXProperty = DependencyProperty.Register(
            "PanelX", typeof(double), typeof(MovePanel));
        /// <summary>
        /// 面板在Canvas中 Y坐标
        /// </summary>
        public static readonly DependencyProperty PanelYProperty = DependencyProperty.Register(
            "PanelY", typeof(double), typeof(MovePanel));
        /// <summary>
        /// Title
        /// </summary>
        private string title = "";
        /// <summary>
        /// 标题
        /// </summary>
        public string Notice
        {
            get
            {
                return title;
            }
            set
            {
                title = value;
                SetNotice(value);
            }
        }
        /// <summary>
        /// 面板在Canvas中 X坐标
        /// </summary>
        public double PanelX
        {
            get
            {
                return (double)GetValue(PanelXProperty);
            }
            set
            {
                SetValue(PanelXProperty, value);
            }
        }
        /// <summary>
        /// 面板在Canvas中 Y坐标
        /// </summary>
        public double PanelY
        {
            get
            {
                return (double)GetValue(PanelYProperty);
            }
            set
            {
                SetValue(PanelYProperty, value);
            }
        }
        /// <summary>
        /// 鼠标按下
        /// </summary>
        public Action<object, MouseButtonEventArgs> Md = null;
        /// <summary>
        /// 鼠标移动
        /// </summary>
        public Action<object, MouseEventArgs> MV = null;
        /// <summary>
        /// 鼠标抬起
        /// </summary>
        public Action<object, MouseButtonEventArgs> MU = null;
        /// <summary>
        /// 鼠标离开
        /// </summary>
        public Action<object, MouseEventArgs> ML = null;

        /// <summary>
        /// 主要的id
        /// </summary>
        public string MainId { get; set; }
        /// <summary>
        /// 主要插件的详细信息
        /// </summary>
        public IPlugInfoInterface PlugInfoInterface { get; set; }

        /// <summary>
        /// 设置公告
        /// </summary>
        /// <param name="notice"></param>
        public void SetNotice(string noticemsg)
        {
            notice.Text += noticemsg;
            //notice_Richbox.ScrollToEnd();
        }

        /// <summary>
        /// 添加显示用的信息
        /// </summary>
        /// <param name="infoBase">信息</param>
        public void AddMessage(IInfoBase infoBase)
        {
            switch (infoBase.MessageType)
            {
                case InterfaceLib.MsgInterface.MsgInfo.Enums.MessageType.Text:
                    AddTextMsg((infoBase as ITextInfo).Text);
                    break;
                case InterfaceLib.MsgInterface.MsgInfo.Enums.MessageType.File:
                    IFileInfo fileInfo = infoBase as IFileInfo;
                    if(IsImg(fileInfo))
                    {
                        if (fileInfo.Stream == null)
                        {
                            AddImageMsg(fileInfo.Path);
                        }
                        else
                        {
                            AddImageMsg(fileInfo.Stream);
                        }
                    }
                    break;
                case InterfaceLib.MsgInterface.MsgInfo.Enums.MessageType.Command:
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 添加文字
        /// </summary>
        /// <param name="msg"></param>
        private void AddTextMsg(string msg)
        {
            this.Dispatcher.Invoke(new Action<string>((m) => {
                try
                {
                    if (content.Inlines.Count > MaxLogCount)
                    {
                        content.Inlines.Clear();
                    }
                    content.Inlines.Add(m);
                    this.content_Richbox.ScrollToEnd();
                }
                catch (Exception ex)
                {
                    Log.Write(ex);
                }
            }), msg);
        }
        /// <summary>
        /// 添加图片的信息
        /// </summary>
        /// <param name="path"></param>
        private void AddImageMsg(string pathstring)
        {
            this.Dispatcher.Invoke(new Action<string>((path) => {
                try
                {
                    if (content.Inlines.Count > MaxLogCount)
                    {
                        content.Inlines.Clear();
                    }
                    FileInfo fileInfo = new FileInfo(path);
                    if (fileInfo.Exists)
                    {
                        BitmapImage image = new BitmapImage();
                        using (FileStream stream = fileInfo.OpenRead())
                        {
                            image.BeginInit();
                            image.StreamSource = new MemoryStream();
                            stream.CopyTo(image.StreamSource);
                            image.EndInit();
                        }
                        content.Inlines.Add(new Image()
                        {
                            Source = image,
                            Stretch = Stretch.Fill,
                            MaxWidth = 200,
                            MaxHeight = 200,
                        });

                    }
                    fileInfo = null;
                    this.content_Richbox.ScrollToEnd();
                }
                catch (Exception ex)
                {
                    Log.Write(ex);
                }
            }), pathstring);
            
        }
        /// <summary>
        /// 添加图片的信息
        /// </summary>
        /// <param name="stream"></param>
        private void AddImageMsg(Stream imgstream)
        {
            this.Dispatcher.Invoke(new Action<Stream>((stream) => 
            {
                try
                {
                    if (content.Inlines.Count > MaxLogCount)
                    {
                        content.Inlines.Clear();
                    }

                    BitmapImage image = new BitmapImage();
                    image.BeginInit();
                    image.StreamSource = new MemoryStream();
                    stream.CopyTo(image.StreamSource);
                    image.EndInit();
                    content.Inlines.Add(new Image()
                    {
                        Source = image,
                        Stretch = Stretch.Fill,
                        MaxWidth = 200,
                        MaxHeight = 200,
                    });
                    this.content_Richbox.ScrollToEnd();
                }
                catch (Exception ex)
                {
                    Log.Write(ex);
                }
            }), imgstream);
        }
        /// <summary>
        /// 判断是否是图片信息
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        private bool IsImg(IFileInfo fileInfo)
        {
            if(fileInfo != null && !fileInfo.ExtendName.Equals(""))
            {
                return fileInfo.ExtendName.Contains("png") ||
                    fileInfo.ExtendName.Contains("jpg") ||
                    fileInfo.ExtendName.Contains("jpeg") ||
                    fileInfo.ExtendName.Contains("gif") ||
                    fileInfo.ExtendName.Contains("bmp");
            }
            else if(fileInfo != null && !fileInfo.Path.Equals(""))
            {
                string name = System.IO.Path.GetFileName(fileInfo.Path);
                return name.Contains("png") ||
                    name.Contains("jpg") ||
                    name.Contains("jpeg") ||
                    name.Contains("gif") ||
                    name.Contains("bmp");
            }
            else
            {
                return false;
            }
        }

        #region 鼠标事件
        private void notice_Richbox_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Md?.Invoke(this, e);
        }

        private void notice_Richbox_MouseLeave(object sender, MouseEventArgs e)
        {
            this.ML?.Invoke(this, e);
        }

        private void notice_Richbox_MouseMove(object sender, MouseEventArgs e)
        {
            this.MV?.Invoke(this, e);
        }

        private void notice_Richbox_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.MU?.Invoke(this, e);
        }
        #endregion

        #region 失去获得焦点
        /// <summary>
        /// 是否已经升上来了
        /// </summary>
        private bool isUp = false;
        /// <summary>
        /// 位置动画
        /// </summary>
        private ThicknessAnimation thicknessAnimation = new ThicknessAnimation();
        /// <summary>
        /// 获取焦点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void content_Richbox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (!isUp)
            {
                thicknessAnimation.From = new Thickness(0, 0, 0, -inputBox.Height);
                thicknessAnimation.To = new Thickness(0, 0, 0, 2);
                thicknessAnimation.Duration = TimeSpan.FromMilliseconds(500);
                inputBox.BeginAnimation(Grid.MarginProperty, thicknessAnimation);
                isUp = true;
            }
            inputTextBox.Focus();
        }
        /// <summary>
        /// 失去焦点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void content_Richbox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (isUp)
            {
                thicknessAnimation.From = new Thickness(0, 0, 0, 2);
                thicknessAnimation.To = new Thickness(0, 0, 0, -inputBox.Height);
                thicknessAnimation.Duration = TimeSpan.FromMilliseconds(500);
                inputBox.BeginAnimation(Grid.MarginProperty, thicknessAnimation);
                isUp = false;
            }
        }
        #endregion
        /// <summary>
        /// 隐藏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hidenIcon_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
        }
    }
}
