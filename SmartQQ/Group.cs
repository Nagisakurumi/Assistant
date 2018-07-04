using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartQQ
{
    /// <summary>
    /// QQ群
    /// </summary>
    public class Group
    {
        /// <summary>
        ///     意义尚不明确。
        /// </summary>
        [JsonProperty("flag")]
        public long Flag { get; internal set; }

        /// <summary>
        ///     用于查询详细信息信息的编号。
        /// </summary>
        [JsonProperty("code")]
        internal long Code { get; set; }

        /// <summary>
        ///     创建时间。
        /// </summary>
        [JsonIgnore]
        public long CreateTime { get; set; }

        /// <summary>
        ///     「本群须知」公告。(大概……）
        /// </summary>
        [JsonIgnore]
        public string Announcement { get; set; }

        /// <summary>
        ///     备注名称。
        /// </summary>
        [JsonIgnore]
        [Obsolete("此属性没有用处。")]
        public string Alias { get; set; }

        /// <summary>
        ///     成员。
        /// </summary>
        [JsonIgnore]
        public List<Friend> Members { get; set; }

        /// <summary>
        ///     已登录账户在此群的群名片。
        /// </summary>
        [JsonIgnore]
        public string MyAlias { get; set; }

        /// <inheritdoc />
        [JsonProperty("name")]
        public string Name { get; internal set; }

        /// <inheritdoc />
        [JsonProperty("gid")]
        public long Id { get; internal set; }
    }
}
