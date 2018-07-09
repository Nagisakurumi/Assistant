using InterfaceLib.MsgInterface.MsgInfo;
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
        /// 文件消息
        /// </summary>
        Dictionary<string, IInfoBase> MsgInfos { get; }
        /// <summary>
        /// 添加一条消息
        /// </summary>
        /// <param name="infoBase"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        bool AddMessage(IInfoBase infoBase, string key = null);
    }
}
