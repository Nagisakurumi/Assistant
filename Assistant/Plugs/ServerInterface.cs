using InterfaceLib.MsgInterface;
using InterfaceLib.PlugsInterface;
using InterfaceLib.ServerInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Assistant.Plugs.PluginsManager;

namespace Assistant.Plugs
{
    /// <summary>
    /// 服务器的接口
    /// </summary>
    partial class ServerInterface : IServerInterface
    {
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
        public void SendMsgToDispla(string msg)
        {
            
        }
        /// <summary>
        /// 路由消息
        /// </summary>
        /// <param name="msgInterface"></param>
        public void SendToMessage(IMsgInterface msgInterface)
        {

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
        public void WriteLog(string logMsg)
        {

        }
    }
}
