using InterfaceLib.MsgInterface;
using InterfaceLib.ServerInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceLib.PlugsInterface
{
    /// <summary>
    /// 基础插件接口
    /// </summary>
    public interface IPlugBaseInterface
    {
        /// <summary>
        /// 插件的名称
        /// </summary>
        string Name { get; }

        ///// <summary>
        ///// 插件的作者
        ///// </summary>
        //string Author { get; }
        ///// <summary>
        ///// 插件的创作日期
        ///// </summary>
        //DateTime DateTime { get;  }
        ///// <summary>
        ///// 插件版本
        ///// </summary>
        //string Version { get; }
        /// <summary>
        /// 启动插件
        /// </summary>
        /// <param name="serverInterface">服务对象</param>
        /// <returns></returns>
        bool Start(IServerInterface serverInterface);
        /// <summary>
        /// 终止插件
        /// </summary>
        /// <returns></returns>
        bool Stop();
        /// <summary>
        /// 服务给插件路由消息
        /// </summary>
        /// <param name="msgInterface"></param>
        void ReciverFromServerMsg(IMsgInterface msgInterface);
        /// <summary>
        /// 初始化插件
        /// </summary>
        /// <returns></returns>
        bool Init();
    }
}
