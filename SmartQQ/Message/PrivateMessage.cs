using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartQQ.Message
{
    public class PrivateMessage : IMessage
    {
        /// <summary>
        /// qq管理
        /// </summary>
        internal SmartQQBot SmartQQBot { get; set; }
        /// <summary>
        /// 消息内容
        /// </summary>
        public string Content { get; internal set; }
        /// <summary>
        /// 消息类型
        /// </summary>
        public MessageType MessageType { get; internal set; }
        /// <summary>
        /// 接受的时间
        /// </summary>
        public DateTime ReciveTime { get; private set; }
        /// <summary>
        /// 回复消息
        /// </summary>
        /// <param name="message"></param>
        public void Reply(string message)
        {
            //SmartQQBot.
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public PrivateMessage()
        {
            ReciveTime = DateTime.Now;
        }
    }
}
