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
        /// 加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MovePanel_Loaded(object sender, RoutedEventArgs e)
        {
            idCombox.ItemsSource = PlugInfoInterfaces;
            idCombox.DisplayMemberPath = "Id";
            idCombox.SelectedItem = PlugInfoInterfaces.Where(p=>p.Id == MainId).First();
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
            //content_Richbox.CaretPosition.InsertLineBreak();
            //content_Richbox.Focus();
            //content_Richbox.CaretPosition = content_Richbox.CaretPosition.InsertParagraphBreak();
            content.Inlines.Add(msg);
        }
        /// <summary>
        /// 添加图片的信息
        /// </summary>
        /// <param name="path"></param>
        private void AddImageMsg(string path)
        {
            FileInfo fileInfo = new FileInfo(path);
            if(fileInfo.Exists)
            {
                BitmapImage image = new BitmapImage();
                using (FileStream stream = fileInfo.OpenRead())
                {
                    image.StreamSource = new MemoryStream();
                    stream.CopyTo(image.StreamSource);
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
        }
        /// <summary>
        /// 添加图片的信息
        /// </summary>
        /// <param name="stream"></param>
        private void AddImageMsg(Stream stream)
        {
            BitmapImage image = new BitmapImage();
            image.StreamSource = new MemoryStream();
            stream.CopyTo(image.StreamSource);
            content.Inlines.Add(new Image()
            {
                Source = image,
                Stretch = Stretch.Fill,
                MaxWidth = 200,
                MaxHeight = 200,
            });
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
    }
}
