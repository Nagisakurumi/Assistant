using SmartQQ.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartQQ.Rule
{
    public class Rule
    {

        #region keyword
        /// <summary>
        /// 规则
        /// </summary>
        private string rule = "";
        /// <summary>
        /// 数据源关键词
        /// </summary>
        public Keyword OriginKeyword { get; private set; }
        /// <summary>
        /// 要回复或者回复的消息
        /// </summary>
        private string sendMsg { get; set; }
        /// <summary>
        /// 命令集合
        /// </summary>
        private List<string> commands = new List<string>();
        #endregion


        /// <summary>
        /// 构造函数
        /// </summary>
        private Rule() { }


        /// <summary>
        /// 创建一条规则
        /// </summary>
        /// <param name="rulemsg"></param>
        /// <returns></returns>
        public static Rule Create(string rulemsg)
        {
            Rule rule = new Rule();
            rule.rule = rulemsg;
            Keyword key = LogLib.TClassOption.GetEnumTypeByString<Keyword>(rulemsg.Split(' ')[0]);
            switch (key)
            {
                case Keyword.looptime:
                    rule.OriginKeyword = Keyword.looptime;
                    break;
                case Keyword.looptimes:
                    rule.OriginKeyword = Keyword.looptimes;
                    break;
                case Keyword.send:
                    rule.OriginKeyword = Keyword.send;
                    break;
                case Keyword.reply:
                    rule.OriginKeyword = Keyword.reply;
                    break;
                default:
                    return null;
            }
            rule.commands = rulemsg.Split(' ').ToList();
            return rule;
        }
        /// <summary>
        /// 执行回复命令
        /// </summary>
        /// <param name="rule"></param>
        /// <param name="message"></param>
        public static bool DoReplyRule(Rule rule, IMessage message)
        {
            if(rule.sendMsg.Equals(""))
            {
                rule.sendMsg = rule.commands[1];
            }
            if(rule.commands[2].Equals(Keyword.where) == false)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 获取条件
        /// </summary>
        /// <param name="rule"></param>
        /// <param name="idx"></param>
        /// <returns></returns>
        private static bool getWhere(Rule rule, int idx, IMessage message)
        {
            bool isOk = true;
            while (isOk && rule.commands[idx].Contains("=="))
            {

            }
            return true;
        }
    }

    /// <summary>
    /// 关键字
    /// </summary>
    public enum Keyword
    {
        /// <summary>
        /// 回复
        /// </summary>
        reply,
        /// <summary>
        /// 发送
        /// </summary>
        send,
        /// <summary>
        /// 循环
        /// </summary>
        looptime,
        /// <summary>
        /// 循环次数
        /// </summary>
        looptimes,
        /// <summary>
        /// 延迟时间
        /// </summary>
        delytime,
        /// <summary>
        /// 群
        /// </summary>
        group,
        /// <summary>
        /// 好友
        /// </summary>
        friends,
        /// <summary>
        /// 消息
        /// </summary>
        message,
        /// <summary>
        /// 且
        /// </summary>
        and,
        /// <summary>
        /// 或
        /// </summary>
        or,
        /// <summary>
        /// 条件
        /// </summary>
        where,
        /// <summary>
        /// 目标
        /// </summary>
        to,
        /// <summary>
        /// 获取
        /// </summary>
        get,
    }
}
