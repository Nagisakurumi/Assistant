using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
        [JsonIgnore]
        internal SmartQQBot SmartQQBot { get; set; }
        /// <summary>
        /// 消息内容
        /// </summary>
        [JsonIgnore]
        public string Content { get; set; }
        /// <summary>
        /// 字体
        /// </summary>
        public Font Font { get; set; }
        /// <summary>
        /// 消息类型
        /// </summary>
        [JsonIgnore]
        public MessageType MessageType => MessageType.PrivateMessage;
        /// <summary>
        /// 接受的时间
        /// </summary>
        [JsonProperty("time")]
        public int ReciveTime { get; set; }
        /// <summary>
        /// 消息来源
        /// </summary>
        public IMessageSource MessageSource => SmartQQBot.Friends.Where(p => p.Uin == SenderId).First();
        /// <summary>
        /// 发送者的id
        /// </summary>
        [JsonProperty("from_uin")]
        public long SenderId { get; set; }
        /// <summary>
        /// 接受者id
        /// </summary>
        [JsonProperty("to_uin")]
        public long ReciveId { get; set; }
        /// <summary>
        ///     用于parse消息和字体的对象。
        /// </summary>
        [JsonProperty("content")]
        internal JArray ContentAndFont
        {
            set
            {
                Font = ((JArray)value.First).Last.ToObject<Font>();
                value.RemoveAt(0);
                foreach (var shit in value)
                    Content += SmartQQStaticString.ParseEmoticons(shit);
            }
        }
        /// <summary>
        /// 回复消息
        /// </summary>
        /// <param name="message"></param>
        public void Reply(string message)
        {
            LogLib.Log.Write("发送私有信息->", message);
            SmartQQBot.SendPrivateMessage(message, this.MessageSource.Id);
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public PrivateMessage()
        {

        }
    }
}
