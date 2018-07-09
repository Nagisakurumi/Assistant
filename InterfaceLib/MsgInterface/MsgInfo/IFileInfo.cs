using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceLib.MsgInterface.MsgInfo
{
    /// <summary>
    /// 文件消息接口
    /// </summary>
    public interface IFileInfo : IInfoBase
    {
        /// <summary>
        /// 文件的地址
        /// </summary>
        string Path { get; }
        /// <summary>
        /// 文件流信息
        /// </summary>
        Stream Stream { get; }
        /// <summary>
        /// 文件扩展名
        /// </summary>
        string ExtendName { get; }
    }
}
