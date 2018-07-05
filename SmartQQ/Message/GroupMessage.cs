using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartQQ.Message
{
    /// <summary>
    /// 群消息
    /// </summary>
    public class GroupMessage : IMessage
    {
        /// <summary>
        /// qq管理
        /// </summary>
        internal SmartQQBot SmartQQBot { get; set; }
        /// <summary>
        /// 消息来源
        /// </summary>
        public IMessageSource MessageSource => SmartQQBot.Groups.Where(p => p.Id == SenderId).First();
        /// <summary>
        /// 消息内容
        /// </summary>
        [JsonProperty("content")]
        public string Content { get; internal set; }
        /// <summary>
        /// 消息类型
        /// </summary>
        public MessageType MessageType => MessageType.GroupMessage;
        /// <summary>
        /// 接受消息的时间
        /// </summary>
        [JsonProperty("time")]
        public DateTime ReciveTime { get; internal set; }
        /// <summary>
        /// 发送者id
        /// </summary>
        [JsonProperty("from_uin")]
        public long SenderId { get; set; }

        /// <summary>
        /// 回复消息
        /// </summary>
        /// <param name="message"></param>
        public void Reply(string message)
        {
            LogLib.Log.Write("发送群信息->", message);
            SmartQQBot.SendGroupMessage(message, this.MessageSource.Id);
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public GroupMessage()
        {
            
            this.ReciveTime = DateTime.Now;
        }
    }
}
