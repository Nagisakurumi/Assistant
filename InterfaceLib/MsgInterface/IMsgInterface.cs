using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceLib.MsgInterface
{
    /// <summary>
    /// 消息接口
    /// </summary>
    public interface IMsgInterface
    {
        /// <summary>
        /// 文本消息
        /// </summary>
        Dictionary<string, string> TextMsg { get; }
        /// <summary>
        /// 文件消息
        /// </summary>
        Dictionary<string, IFileInterface> FileInfos { get; }
    }
}
