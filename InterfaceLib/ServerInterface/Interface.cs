using InterfaceLib.MsgInterface;
using InterfaceLib.PlugsInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceLib.ServerInterface
{
    /// <summary>
    /// 界面插件接口
    /// </summary>
    public interface Interface : IPlugBaseInterface
    {
        /// <summary>
        /// 获取远程仓库插件列表
        /// </summary>
        /// <returns></returns>
        IPlugInfoInterface[] GetRemotePlugsList();
        /// <summary>
        /// 获取本地已经安装的插件列表
        /// </summary>
        /// <returns></returns>
        IPlugInfoInterface[] GetLocalPlugsList();
        /// <summary>
        /// 安装远程插件
        /// </summary>
        /// <param name="plugInterface">远程插件的信息</param>
        /// <returns></returns>
        bool InstallPlugFromRemote(IPlugInfoInterface plugInterface);
        /// <summary>
        /// 卸载本地插件
        /// </summary>
        /// <param name="plugInterface">本地插件信息</param>
        /// <returns></returns>
        bool UnInstallLocalPlug(IPlugInfoInterface plugInterface);
        /// <summary>
        /// 运行一个本地的插件
        /// </summary>
        /// <param name="plugInterface"></param>
        /// <returns></returns>
        bool RunLocalPlug(IPlugInfoInterface plugInterface);
        /// <summary>
        /// 停止正在运行的一个本地插件
        /// </summary>
        /// <param name="plugInterface"></param>
        /// <returns></returns>
        bool StopLocalPlug(IPlugInfoInterface plugInterface);
        /// <summary>
        /// 设置消息回调函数
        /// </summary>
        /// <param name="msgCallBackEventHandler"></param>
        /// <returns></returns>
        bool SetMsgCallBackFunction(Action<IMsgInterface> msgCallBackEventHandler);
    }
}
