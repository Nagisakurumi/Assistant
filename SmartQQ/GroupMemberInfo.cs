using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartQQ
{
    /// <summary>
    /// 群成员信息
    /// </summary>
    public class GroupMemberInfo : IMessageSource
    {
        /// <summary>
        /// QQ号
        /// </summary>
        [JsonIgnore]
        public long QQNumber { get; internal set; }

        /// <inheritdoc />
        [JsonProperty("uin")]
        public long Uin { get; internal set; }

        /// <inheritdoc />
        [JsonProperty("nick")]
        public string Nickname { get; internal set; }

        /// <summary>
        ///     群名片。
        /// </summary>
        [JsonProperty("card")]
        public string Alias { get; set; }

        /// <summary>
        ///     客户端类型。
        /// </summary>
        [JsonProperty("clientType")]
        public int ClientType { get; set; }

        /// <summary>
        ///     当前状态。
        /// </summary>
        [JsonProperty("status")]
        public int Status { get; set; }

        /// <summary>
        ///     国家。
        /// </summary>
        [JsonProperty("country")]
        public string Country { get; set; }

        /// <summary>
        ///     省份。
        /// </summary>
        [JsonProperty("province")]
        public string Province { get; set; }

        /// <summary>
        ///     城市。
        /// </summary>
        [JsonProperty("city")]
        public string City { get; set; }

        /// <summary>
        ///     性别。
        /// </summary>
        [JsonProperty("gender")]
        public string Gender { get; set; }

        /// <summary>
        ///     QQ会员状态。
        /// </summary>
        [JsonProperty("vip")]
        public bool IsVip { get; set; }

        /// <summary>
        ///     会员等级。
        /// </summary>
        [JsonProperty("vipLevel")]
        public int VipLevel { get; set; }
        /// <summary>
        /// 信息员的id
        /// </summary>
        [JsonIgnore]
        public long Id => this.Uin;
    }
}
