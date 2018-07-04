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
        /// 昵称
        /// </summary>
        [JsonProperty("Nickname")]
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

        /// <summary>
        ///     个性签名。
        /// </summary>
        [JsonIgnore]
        public string Bio { get; internal set; }

        /// <summary>
        ///     生日。
        /// </summary>
        [JsonIgnore]
        public string Birthday { get; internal set; }

        /// <summary>
        ///     座机号码。
        /// </summary>
        [JsonIgnore]
        public string Phone { get; internal set; }

        /// <summary>
        ///     手机号码。
        /// </summary>
        [JsonIgnore]
        public string Cellphone { get; internal set; }

        /// <summary>
        /// 邮箱地址。
        /// </summary>
        [JsonIgnore]
        public string Email { get; internal set; }

        /// <summary>
        /// 职业。
        /// </summary>
        [JsonIgnore]
        public string Job { get; internal set; }

        /// <summary>
        /// 个人主页。
        /// </summary>
        [JsonIgnore]
        public string Homepage { get; internal set; }

        /// <summary>
        /// 学校。
        /// </summary>
        [JsonIgnore]
        public string School { get; internal set; }

        /// <summary>
        /// 国家。
        /// </summary>
        [JsonIgnore]
        public string Country { get; internal set; }

        /// <summary>
        /// 省份。
        /// </summary>
        [JsonIgnore]
        public string Province { get; internal set; }

        /// <summary>
        /// 城市。
        /// </summary>
        [JsonIgnore]
        public string City { get; internal set; }

        /// <summary>
        /// 性别。
        /// </summary>
        [JsonIgnore]
        public string Gender { get; internal set; }

        /// <summary>
        /// 生肖。
        /// </summary>
        [JsonIgnore]
        public int Shengxiao { get; internal set; }

        /// <summary>
        ///     某信息字段。意义暂不明确。
        /// </summary>
        [JsonIgnore]
        public string Personal { get; internal set; }

        /// <summary>
        ///     某信息字段。意义暂不明确。
        /// </summary>
        [JsonIgnore]
        public int VipInfo { get; internal set; }

        /// <inheritdoc />
        [JsonIgnore]
        public long QQNumber { get; internal set; }

        /// <inheritdoc />
        [JsonProperty("userId")]
        public long Id { get; internal set; }
    }
}
