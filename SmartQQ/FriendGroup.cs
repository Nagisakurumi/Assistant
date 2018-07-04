using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartQQ
{
    /// <summary>
    /// 好友分组
    /// </summary>
    public class FriendGroup
    {
        /// <summary>
        ///     序号。
        /// </summary>
        [JsonProperty("index")]
        public int Index { get; set; }
        /// <summary>
        ///     名称。
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
        /// <summary>
        /// 成员
        /// </summary>
        public List<Friend> Members { get => friends.Where(p=>p.GroupId == this.Index).ToList();}
        /// <summary>
        /// 默认好友组
        /// </summary>
        public static FriendGroup DefultFriendGroup
        {
            get
            {
                return new FriendGroup(null) {
                    Index = 0,
                    Name = "我的好友",
                };
            }
        }
        /// <summary>
        /// 所有好友
        /// </summary>
        private List<Friend> friends = null;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="friends"></param>
        internal FriendGroup(List<Friend> friends)
        {
            this.friends = friends;
        }
    }
}
