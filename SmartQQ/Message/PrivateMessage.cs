﻿using Newtonsoft.Json;
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
        [JsonProperty("content")]
        public string Content { get; set; }
        /// <summary>
        /// 消息类型
        /// </summary>
        [JsonIgnore]
        public MessageType MessageType => MessageType.PrivateMessage;
        /// <summary>
        /// 接受的时间
        /// </summary>
        [JsonProperty("time")]
        public DateTime ReciveTime { get; set; }
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
        public PrivateMessage(JObject jObject)
        {
            ReciveTime = DateTime.Now;

        }
    }
}
