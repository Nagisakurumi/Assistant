using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InterfaceLib.MsgInterface.MsgInfo
{
    /// <summary>
    /// 文本消息接口
    /// </summary>
    public interface ITextInfo : IInfoBase
    {
        /// <summary>
        /// 文本消息
        /// </summary>
        string Text { get; }
    }
}
