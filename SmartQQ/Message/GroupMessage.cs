using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
        [JsonIgnore]
        internal SmartQQBot SmartQQBot { get; set; }
        /// <summary>
        /// 消息来源
        /// </summary>
        [JsonIgnore]
        public IMessageSource MessageSource => getSource();
        /// <summary>
        /// 消息内容
        /// </summary>
        [JsonIgnore]
        public string Content { get; internal set; }
        /// <summary>
        /// 字体
        /// </summary>
        [JsonIgnore]
        public Font Font { get; set; }
        /// <summary>
        /// 群编号
        /// </summary>
        [JsonProperty("group_code")]
        public long GroupCode { get; set; }
        /// <summary>
        /// 消息类型
        /// </summary>
        [JsonIgnore]
        public MessageType MessageType => MessageType.GroupMessage;
        /// <summary>
        /// 接受消息的时间
        /// </summary>
        [JsonProperty("time")]
        public int ReciveTime { get; internal set; }
        /// <summary>
        /// 发送者id
        /// </summary>
        [JsonProperty("send_uin")]
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
            LogLib.Log.Write("发送群信息->", message);
            SmartQQBot.SendGroupMessage(message, this.MessageSource.Id);
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public GroupMessage()
        {
        }
        /// <summary>
        /// 获取消息源
        /// </summary>
        /// <returns></returns>
        private IMessageSource getSource()
        {
            try
            {
                return SmartQQBot.Groups.Find(p => p.Id == this.GroupCode).GroupInfo.Members.Find(p => p.Uin == this.SenderId);
            }
            catch (Exception)
            {
                LogLib.Log.Write("获取群消息失败!");
                return null;
            }

        }
    }
}
