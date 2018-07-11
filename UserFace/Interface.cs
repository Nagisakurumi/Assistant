using InterfaceLib;
using InterfaceLib.MsgInterface;
using InterfaceLib.MsgInterface.MsgInfo;
using InterfaceLib.PlugsInterface;
using InterfaceLib.PlugsInterface.FaceInterface;
using InterfaceLib.ServerInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static UserFace.FaceInterfaceLog;

namespace UserFace
{
    /// <summary>
    /// 界面插件接口
    /// </summary>
    [Export]
    public class Interface : IFaceInterface
    {
        /// <summary>
        /// 插件信息列表
        /// </summary>
        private static IPlugInfoInterface[] plugInfoInterfaces = null;
        /// <summary>
        /// 插件信息列表
        /// </summary>
        public static IPlugInfoInterface[] PlugInfoInterfaces
        {
            get
            {
                if (plugInfoInterfaces == null)
                {
                    plugInfoInterfaces = ServerInterface.GetLocalPlugsList();
                }
                return plugInfoInterfaces;
            }
        }
        /// <summary>
        /// 服务
        /// </summary>
        public static IServerInterface ServerInterface = null;
        private string id;

        /// <summary>
        /// 信息回调
        /// </summary>
        public static event Action<IInfoBase> MessageCallBack = null;
        /// <summary>
        /// 界面主线程
        /// </summary>
        private Thread faceMainThread = null;
        /// <summary>
        /// 插件名称
        /// </summary>
        public string Name => "界面";
        /// <summary>
        /// 初始化插件
        /// </summary>
        /// <returns></returns>
        public bool Init()
        {
            
            return true;
        }
        /// <summary>
        /// 从服务接受信息
        /// </summary>
        /// <param name="msgInterface"></param>
        public void ReciverFromServerMsg(IMsgInterface msgInterface)
        {
            try
            {
                foreach (var item in msgInterface.MsgInfos)
                {
                    //if(item.Value.MessageType == InterfaceLib.MsgInterface.MsgInfo.Enums.MessageType.File)
                    //{
                    //    Log.Write("FileMessage", item.Value.SendId);
                    //}
                    MessageCallBack?.Invoke(item.Value);
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
        }
        /// <summary>
        /// 开启插件
        /// </summary>
        /// <param name="serverInterface"></param>
        /// <returns></returns>
        public bool Start(IServerInterface serverInterface, string id)
        {
            ServerInterface = serverInterface;
            this.id = id;
            faceMainThread = new Thread(faceMainThreadRun);
            faceMainThread.IsBackground = true;
            faceMainThread.SetApartmentState(ApartmentState.STA);
            faceMainThread.Start();
            return true;
        }
        /// <summary>
        /// 停止插件
        /// </summary>
        /// <returns></returns>
        public bool Stop()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 运行界面主线程
        /// </summary>
        protected void faceMainThreadRun()
        {
            try
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.ShowDialog();
                Log.Write("界面插件被关闭!");
            }
            catch (Exception ex)
            {
                Log.Write("界面插件运行异常被关闭", ex);
            }

        }
    }
}
