using InterfaceLib.MsgInterface;
using InterfaceLib.MsgInterface.MsgInfo;
using InterfaceLib.MsgInterface.MsgInfo.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartQQ
{
    public class MessageInterface : IMsgInterface
    {
        /// <summary>
        /// 消息内容
        /// </summary>
        public Dictionary<string, IInfoBase> MsgInfos { get; } = new Dictionary<string, IInfoBase>();
        /// <summary>
        /// 添加一条消息
        /// </summary>
        /// <param name="infoBase"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool AddMessage(IInfoBase infoBase, string key = null)
        {
            if(key == null)
            {
                key = Guid.NewGuid().ToString(); ;
            }
            if (MsgInfos.ContainsKey(key) == false)
            {
                MsgInfos.Add(key, infoBase);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    /// <summary>
    /// 文件消息信息
    /// </summary>
    public class FileMsgInfo : IFileInfo
    {
        /// <summary>
        /// 文件路径
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// 文件流
        /// </summary>
        public Stream Stream { get; set; }
        /// <summary>
        /// 文件扩展名
        /// </summary>
        public string ExtendName { get; set; }
        /// <summary>
        /// 消息接受者的id
        /// </summary>
        public string ReciverId { get; set; }
        /// <summary>
        /// 消息发送者的id
        /// </summary>
        public string SendId { get; set; }
        /// <summary>
        /// 消息类型
        /// </summary>
        public MessageType MessageType => MessageType.File;
    }
    /// <summary>
    /// 文本消息
    /// </summary>
    public class TextMsgInfo : ITextInfo
    {
        /// <summary>
        /// 文本消息内容
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// 接受者id
        /// </summary>
        public string ReciverId { get; set; }
        /// <summary>
        /// 发送者id
        /// </summary>
        public string SendId { get; set; }
        /// <summary>
        /// 消息类型
        /// </summary>
        public MessageType MessageType => MessageType.Text;
    }
}
