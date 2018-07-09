using InterfaceLib.MsgInterface.MsgInfo.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InterfaceLib.MsgInterface.MsgInfo
{
    /// <summary>
    /// Infobase
    /// </summary>
    public interface IInfoBase
    {
        /// <summary>
        /// 接受的id -1 : 为消息路由，发送给所有插件, 0 : 默认接受者为服务, 1 : 默认为界面插件 其他为
        /// 各自插件对应的id,服务在安装好插件后会分配一个固定的id,也可以
        /// 从服务中获取所有的id列表
        /// </summary>
        string ReciverId { get; }
        /// <summary>
        /// 发送者的id
        /// </summary>
        string SendId { get; set; }
        /// <summary>
        /// 消息类型
        /// </summary>
        MessageType MessageType { get; }
    }
}
