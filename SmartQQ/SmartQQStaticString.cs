using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartQQ
{
    public static class SmartQQStaticString
    {
        /// <summary>
        /// 所有好友
        /// </summary>
        public static string Friends = "Friends";
        /// <summary>
        /// 所有分组
        /// </summary>
        public static string FriendsGroup = "FriendsGroup";
        /// <summary>
        /// QQ群
        /// </summary>
        public static string Group = "Group";
        /// <summary>
        /// 自己信息
        /// </summary>
        public static string SelfInfo = "Self";
        /// <summary>
        /// 组的信息
        /// </summary>
        public static string GroupInfo = "GroupInfo";

        /// <summary>
        ///     将消息中的表情文字（e.g. "/微笑"）转换为节点内容以实现发送内置表情。
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static JToken[] TranslateEmoticons(string message)
        {
            // TODO: 实现将文本中的表情转换为JSON节点
            return new[]
            {
                JToken.FromObject(message)
            };
        }

        /// <summary>
        /// 根据消息类型获取 回复时候的参数
        /// </summary>
        /// <param name="messageType"></param>
        /// <returns></returns>
        public static string GetParamNameByMessageType(Message.MessageType messageType)
        {
            switch (messageType)
            {
                case Message.MessageType.PrivateMessage:
                    return "to";
                case Message.MessageType.GroupMessage:
                    return "group_uin";
                case Message.MessageType.UnKnowMessage:
                    return "";
                default:
                    return null;
            }
        }
        /// <summary>
        ///     将一个消息JSON中的节点转换为表情的文字描述或文字本身。
        /// </summary>
        /// <param name="token">JSON节点。</param>
        /// <returns>文字。</returns>
        public static string ParseEmoticons(JToken token)
        {
            if (token is JArray)
                return token.ToString(Formatting.None);
            return token.Value<string>();
        }
    }
}
