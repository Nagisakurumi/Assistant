using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartQQ
{
    public class Friend
    {
        /// <summary>
        /// 好友名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 好友的uin
        /// </summary>
        public long Uin { get; set; }
        /// <summary>
        /// 是否在线
        /// </summary>
        public bool IsLogin { get; set; }
        /// <summary>
        /// 所属的组号
        /// </summary>
        public long GroupId { get; set; }
        /// <summary>
        /// 意义不明
        /// </summary>
        public long Flag { get; set; }
        /// <summary>
        /// 好友的详细信息
        /// </summary>
        public FriendInfo FriendInfo { get; set; }
        /// <summary>
        /// 昵称
        /// </summary>
        [JsonProperty("nick")]
        public string Nickname { get; internal set; }
        /// <summary>
        ///     备注姓名。
        /// </summary>
        [JsonProperty("markname")]
        public string MarkName { get; internal set; }
        /// <summary>
        /// 类型(意义不明)
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 意义不明
        /// </summary>
        public long Face { get; set; }
        /// <summary>
        ///     QQ会员状态。
        /// </summary>
        [JsonProperty("vip")]
        public bool IsVip { get; internal set; }

        /// <summary>
        ///     会员等级。
        /// </summary>
        [JsonProperty("vipLevel")]
        public int VipLevel { get; internal set; }
        /// <inheritdoc />
        [JsonIgnore]
        public long QQNumber { get; internal set; }
        /// <inheritdoc />
        [JsonProperty("userId")]
        public long Id { get; internal set; }
    }
}
