using InterfaceLib.MsgInterface;
using InterfaceLib.PlugsInterface;
using InterfaceLib.ServerInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Assistant.Plugs.PluginsManager;
using static Assistant.MessageRoute.MessageRoute;
using Assistant.MessageRoute;
using static Assistant.ServerLog;

namespace Assistant.Plugs
{
    /// <summary>
    /// 服务器的接口
    /// </summary>
    partial class ServerInterface : IServerInterface
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        private ServerInterface() {
            Log.ErroStringEvent += Log_ErroStringEvent;
        }
        /// <summary>
        /// 日志写入回调
        /// </summary>
        /// <param name="obj"></param>
        private void Log_ErroStringEvent(string obj)
        {
            Server.WriteLog("0", obj);
        }

        /// <summary>
        /// 服务
        /// </summary>
        public readonly static ServerInterface Server = new ServerInterface();
        /// <summary>
        /// 获取本地已经安装的插件列表
        /// </summary>
        /// <returns></returns>
        public IPlugInfoInterface[] GetLocalPlugsList()
        {
            return Manager.PlugInfoInterfaces;
        }
        /// <summary>
        /// 获取远程插件列表
        /// </summary>
        /// <returns></returns>
        public IPlugInfoInterface[] GetRemotePlugsList()
        {
            return null;
        }
        /// <summary>
        /// 安装插件
        /// </summary>
        /// <param name="plugInterface"></param>
        /// <returns></returns>
        public bool InstallPlugFromRemote(IPlugInfoInterface plugInterface)
        {
            return true;
        }
        /// <summary>
        /// 运行本地插件
        /// </summary>
        /// <param name="plugInterface"></param>
        /// <returns></returns>

        public bool RunLocalPlug(IPlugInfoInterface plugInterface)
        {

            return true;
        }
        /// <summary>
        /// 发送给界面插件显示
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="id">发送者id</param>
        public void SendMsgToDispla(string id, string msg)
        {
            MessageRouteInfo.AddMessage(new TextMsgInfo()
            { Text = msg, ReciverId = PluginsURL.FaceInterfaceId,
            SendId = id,});
        }
        /// <summary>
        /// 路由消息
        /// </summary>
        /// <param name="msgInterface"></param>
        public void SendToMessage(IMsgInterface msgInterface)
        {
            MessageRouteInfo.AddMessage(msgInterface);
        }
        /// <summary>
        /// 停止本地插件
        /// </summary>
        /// <param name="plugInterface"></param>
        /// <returns></returns>
        public bool StopLocalPlug(IPlugInfoInterface plugInterface)
        {
            return true;
        }
        /// <summary>
        /// 卸载本地插件
        /// </summary>
        /// <param name="plugInterface"></param>
        /// <returns></returns>
        public bool UnInstallLocalPlug(IPlugInfoInterface plugInterface)
        {
            return true;
        }
        /// <summary>
        /// 写入到日志
        /// </summary>
        /// <param name="logMsg"></param>
        /// <param name="id">发送者id</param>
        public void WriteLog(string id, string logMsg)
        {
            MessageRouteInfo.AddMessage(new TextMsgInfo()
            { Text = logMsg, ReciverId = PluginsURL.FaceInterfaceId,
                SendId = id,
            });
        }
    }
}
