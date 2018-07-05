using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartQQ.Message
{
    public interface IMessage
    {
        /// <summary>
        /// 消息来源
        /// </summary>
        IMessageSource MessageSource { get; }
        /// <summary>
        /// 消息内容
        /// </summary>
        string Content { get; }
        /// <summary>
        /// 消息类型
        /// </summary>
        MessageType MessageType { get; }
        /// <summary>
        /// 接受的时间
        /// </summary>
        DateTime ReciveTime { get; }
        /// <summary>
        ///     发送者ID。
        /// </summary>
        long SenderId { get; set; }
        /// <summary>
        /// 回复消息
        /// </summary>
        /// <param name="message"></param>
        void Reply(string message);
    }


    public enum MessageType
    {
        PrivateMessage,
        GroupMessage,
        UnKnowMessage,
    }
}
