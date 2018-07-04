using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartQQ
{
    /// <summary>
    ///     群详细信息。
    /// </summary>
    public class GroupInfo
    {
        /// <summary>
        ///     用于发送信息的编号。不等于群号。
        /// </summary>
        [JsonProperty("gid")]
        public long Id { get; set; }

        /// <summary>
        ///     创建时间。
        /// </summary>
        [JsonProperty("createtime")]
        public long CreateTime { get; set; }

        /// <summary>
        ///     「本群须知」公告。(大概……）
        /// </summary>
        [JsonProperty("memo")]
        public string Announcement { get; set; }

        /// <summary>
        ///     群名。
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        ///     备注名称。
        /// </summary>
        [JsonProperty("markname")]
        [Obsolete("此属性没有用处。")]
        public string Alias { get; set; }

        /// <summary>
        ///     群主ID。
        /// </summary>
        [JsonProperty("owner")]
        public long OwnerId { get; set; }

        /// <summary>
        ///     成员。
        /// </summary>
        [JsonProperty("users")]
        public List<GroupMemberInfo> Members { get; set; } = new List<GroupMemberInfo>();

        /// <summary>
        ///     已登录账户在此群的群名片。
        /// </summary>
        [JsonIgnore]
        public string MyAlias { get; set; }
    }
}
