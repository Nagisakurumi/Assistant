using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using LogLib;
using CacheLib;
using System.Threading;
using SmartQQ.Message;

namespace SmartQQ
{
    public class SmartQQBot
    {
        #region 字段
        /// <summary>
        /// 消息id
        /// </summary>
        private long messageId = 43690001;
        /// <summary>
        /// http请求客户端
        /// </summary>
        private Client client = new Client();
        /// <summary>
        /// 开始检测登录的时间
        /// </summary>
        private DateTime checkStartTime = DateTime.Now;
        /// <summary>
        /// 检测是否登录时候需要的数据
        /// </summary>
        private Dictionary<string, string> checkloginData = new Dictionary<string, string>()
        {
            { "js_ver", "10275"},
            { "js_type", "1"},
            { "login_sig", "d3x*Vx2yJ4DrYI6O01ENdxJ9bDGHXawivhMrOTCZi1HE2Dc2j79W8nG1iJkAf-XP"},
            { "pt_uistyle", "40"},
            { "aid", "501004106"},
            { "daid", "164"},
            { "mibao_css", "m_webqq"}
        };
        /// <summary>
        /// 检测是否登录时候使用
        /// </summary>
        private string cookieqrsig = "";
        /// <summary>
        /// 验证登录扫码成功后的回调地址
        /// </summary>
        private string checksigURL = "";
        /// <summary>
        /// clientid
        /// </summary>
        private long clientid = 53999199;
        /// <summary>
        /// 为了获取QQ好友列表的code
        /// </summary>
        private string vfwebqq = "";
        /// <summary>
        /// ptwebqq
        /// </summary>
        private string ptwebqq = "";
        /// <summary>
        /// ptwebqq第二次登录时候获取的 
        /// </summary>
        private string vfwebqq2 = "";
        /// <summary>
        /// psessionid
        /// </summary>
        private string psessionid = "";
        /// <summary>
        /// uin
        /// </summary>
        private long uin = 0;
        /// <summary>
        /// hash
        /// </summary>
        private string hash = "";
        /// <summary>
        /// 缓存
        /// </summary>
        internal Cache cache = new Cache();
        /// <summary>
        /// 消息循环线程
        /// </summary>
        private Thread messageLoopThread = null;
        /// <summary>
        /// 派发消息线程
        /// </summary>
        private Thread messageDistribution = null;
        /// <summary>
        /// 用于线程阻塞公共资源
        /// </summary>
        private AutoResetEvent distributionThreadAre = new AutoResetEvent(false);
        /// <summary>
        /// 消息队列
        /// </summary>
        private Queue<IMessage> messages = new Queue<IMessage>();
        #endregion
        #region 访问器
        /// <summary>
        /// 重试重新发送的次数
        /// </summary>
        public int RetryTimes { get; set; } = 5;
        /// <summary>
        /// 登录图片保存路径
        /// </summary>
        public string SaveLoginImagePath { get; set; } = "login.png";
        /// <summary>
        /// 所有的好友信息
        /// </summary>
        public List<Friend> Friends => getCache<List<Friend>>(SmartQQStaticString.Friends, UpdateFrindsList);
        /// <summary>
        /// 所有的分组信息
        /// </summary>
        public List<FriendGroup> FriendGroups => getCache<List<FriendGroup>>(SmartQQStaticString.FriendsGroup, UpdateFrindsList);
        /// <summary>
        /// 所有的QQ群信息
        /// </summary>
        public List<Group> Groups => getCache<List<Group>>(SmartQQStaticString.Group, UpdateGroup);
        /// <summary>
        /// 自己账号的信息
        /// </summary>
        public FriendInfo SelfInfo => updateSelfAccountInfo();
        /// <summary>
        /// 自己的qq号
        /// </summary>
        public long MyQQNum => GetQQByUin(SelfInfo.Uin);
        /// <summary>
        /// 是否已经登录
        /// </summary>
        public bool IsLogin => isLogin();
        /// <summary>
        /// 获取群的详细信息
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public GroupInfo this[Group group] => GetGroupInfo(group);
        /// <summary>
        /// 消息派发
        /// </summary>
        public Action<IMessage> MessageCallBack { get; set; }
        /// <summary>
        /// 获取好友信息
        /// </summary>
        /// <param name="uin"></param>
        /// <returns></returns>
        public FriendInfo this[long uin]
        {
            get
            {
                List<Friend> fs = Friends.Where(p => p.Uin == uin).ToList();
                if (fs.Count == 0)
                    return SelfInfo.Uin == uin ? SelfInfo : null;
                else
                {
                    return fs.First().FriendInfo;
                }
            }
        }
        #endregion
        #region 公开方法
        /// <summary>
        /// 登录qq
        /// </summary>
        /// <returns></returns>
        public bool Login()
        {
            Stream stream = client.GetStreamAsync(SmartQQAPI.GetQrCode);
            if (File.Exists(SaveLoginImagePath))
            {
                File.Delete(SaveLoginImagePath);
            }
            using (FileStream filestream = File.Open(SaveLoginImagePath, FileMode.Create))
            {
                stream.CopyTo(filestream);
            }
            stream.Dispose();
            cookieqrsig = client.GetCookieByUrl(SmartQQAPI.GetQrCode, "qrsig");
            checkStartTime = DateTime.Now;
            return true;
        }

        /// <summary>
        /// 检测是否登录成功前的检测
        /// </summary>
        /// <returns></returns>
        public bool GetLoginParamter()
        {
            string html = client.GetStringAsync(SmartQQAPI.GetLoginParamter);
            checkloginData["aid"] = new Regex("<input type=\"hidden\" name=\"aid\" value=\"(\\d+?)\" />").Match(html).Groups[1].Value;
            checkloginData["mibao_css"] = new Regex("g_mibao_css=encodeURIComponent\\(\"(.+?)\"\\)").Match(html).Groups[1].Value;
            string vsig = new Regex("g_login_sig=encodeURIComponent\\(\"(.*?)\"\\)").Match(html).Groups[1].Value;
            string vjver = new Regex("g_pt_version=encodeURIComponent\\(\"(\\d +?)\"\\)").Match(html).Groups[1].Value;
            if(!vsig.Equals(""))
            {
                checkloginData["login_sig"] = vsig;
            }
            if (!vjver.Equals(""))
            {
                checkloginData["js_ver"] = vjver;
            }
            return true;
        }

        /// <summary>
        /// 检测是否已经成功登录
        /// </summary>
        /// <returns></returns>
        public bool CheckLogin()
        {
            string result = client.GetStringAsync(SmartQQAPI.VerifyQrCode, hash33(cookieqrsig));
            if(result.Contains("未失效"))
            {
                Log.Write("验证码未失效，请尽快扫码登录");
            }
            else if(result.Contains("登录成功"))
            {
                Log.Write("检测扫码登录成功!");
            }
            string[] values = new Regex("'.*'").Match(result).Value.Split(',');
            int value = Convert.ToInt32(values[0].Substring(1, values[0].Length - 2));
            
            if (value == 0)
            {
                ptwebqq = client.GetCookieByUrl(SmartQQAPI.VerifyQrCode, "ptwebqq", hash33(cookieqrsig));
                Log.Write("ptwebqq -> " + ptwebqq);
                checksigURL = values[2].Substring(1, values[2].Length - 2);
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 为了二次登录做准备
        /// </summary>
        /// <returns></returns>
        public bool ForLogion2()
        {
            vfwebqq = "";
            HttpResponseMessage httpResponseMessage  = client.GetAsync(new SmartQQAPI(checksigURL, null));
            vfwebqq = "";
            //client.GetAsync(new SmartQQAPI(httpResponseMessage.Headers.Location.ToString(), null));
            JObject jObject = client.GetJsonAsync(SmartQQAPI.GetVfwebqq, ptwebqq);
            Log.Write("Result:" + jObject.ToString());
            vfwebqq = jObject["result"]["vfwebqq"].ToString();
            if(vfwebqq.Equals(""))
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// 二次登录
        /// </summary>
        /// <returns></returns>
        public bool Login2()
        {
            var r = new JObject
            {
                {"ptwebqq", ptwebqq},
                {"clientid", clientid},
                {"psessionid", ""},
                {"status", "online"}
            };

            JObject jObject = client.PostJsonAsync(SmartQQAPI.GetUinAndPsessionid, r);
            psessionid = jObject["result"]["psessionid"].ToString();
            vfwebqq2 = jObject["result"]["vfwebqq"].ToString();
            uin = Convert.ToInt64(jObject["result"]["uin"].ToString());
            if(testLogin())
            {
                hash = getHash(uin, ptwebqq);
            }
            else
            {
                Log.Write("测试登录失败!");
                return false;
            }
            return true;
        }
        /// <summary>
        /// 更新好友列表信息
        /// </summary>
        /// <returns></returns>
        public bool UpdateFrindsList()
        {
            try
            {
                Log.Write("开始更新好友列表信息,和分组信息!");
                /// <summary>
                /// 所有好友
                /// </summary>
                List<Friend> friends = new List<Friend>();
                /// <summary>
                /// 好友的分组
                /// </summary>
                List<FriendGroup> friendGroups = new List<FriendGroup>();
                
                JObject jObject = new JObject
                {
                    { "vfwebqq", vfwebqq }, { "hash", hash }
                };

                JToken response = client.PostJsonAsync(SmartQQAPI.GetFriendList,
                    jObject)["result"];
                //Log.Write(response.ToString());
                ///好友基本信息
                friends.Clear();
                ///好友info信息
                foreach (var item in response["info"] as JArray)
                {
                    friends.Add(new Friend()
                    {
                        SmartQQBot = this,
                        Face = Convert.ToInt64(item["face"].ToString()),
                        Nickname = item["nick"].ToString(),
                        Uin = Convert.ToInt64(item["uin"].ToString()),
                    });
                    
                }
                foreach (var item in response["friends"] as JArray)
                {
                    Friend friend = friends.Where(p => p.Uin == Convert.ToInt64(item["uin"].ToString())).First();
                    if (friend != null)
                    {
                        friend.Flag = Convert.ToInt64(item["flag"].ToString());
                        friend.GroupId = Convert.ToInt64(item["categories"].ToString());
                    }

                }
                ///好友备注信息
                foreach (var item in response["marknames"] as JArray)
                {
                    Friend friend = friends.Where(p => p.Uin == Convert.ToInt64(item["uin"].ToString())).First();
                    if (friend != null)
                    {
                        friend.MarkName = item["markname"].ToString();
                        friend.Type = item["type"].ToString();
                    }
                }
                ///好友分组信息
                friendGroups.Clear();
                foreach (var item in response["categories"] as JArray)
                {
                    friendGroups.Add(new FriendGroup(friends)
                    {
                        Index = Convert.ToInt32(item["index"].ToString()),
                        Name = item["name"].ToString()
                    });
                }
                ///vip信息
                foreach (var item in response["vipinfo"] as JArray)
                {
                    Friend friend = friends.Where(p => p.Uin == Convert.ToInt64(item["u"].ToString())).First();
                    if (friend != null)
                    {
                        friend.VipLevel = Convert.ToInt32(item["vip_level"].ToString());
                        friend.IsVip = item["is_vip"].ToString().Equals("1");
                    }
                }
                

                cache.UpdateValueCache(SmartQQStaticString.Friends, friends, 1200);
                cache.UpdateValueCache(SmartQQStaticString.FriendsGroup, friendGroups, 3000);
                return true;
            }
            catch (Exception)
            {
                Log.Write("获取好友信息失败!");
                return false;
            }
        }
        /// <summary>
        /// 更新QQ群信息
        /// </summary>
        /// <returns></returns>
        public bool UpdateGroup()
        {
            try
            {   
                /// <summary>
                /// QQ群
                /// </summary>
                List<Group> groups = new List<Group>();
                Log.Write("开始获取群列表");
                JToken response = client.PostJsonAsync(SmartQQAPI.GetGroupList,
                    new JObject { { "vfwebqq", vfwebqq }, { "hash", hash } })["result"];
                //Log.Write(response.ToString());
                ///QQ群信息
                groups.Clear();
                JArray jArray = response["gnamelist"] as JArray;
                float idx = 0;
                float max = jArray.Count;
                foreach (var item in jArray)
                {
                    groups.Add(new Group()
                    {
                        Flag = Convert.ToInt64(item["flag"].ToString()),
                        Id = Convert.ToInt64(item["gid"].ToString()),
                        Name = item["name"].ToString(),
                        Code = Convert.ToInt64(item["code"].ToString())
                    });
                    Log.Progress(idx++ / max, "更新QQ群：");
                }
                cache.UpdateValueCache(SmartQQStaticString.Group, groups, 1000);
                Log.Write("结束更新群!");
                return true;
            }
            catch (Exception)
            {
                Log.Write("获取群信息失败!");
                return false;
            }
        }
        /// <summary>
        /// 获取qq号由uin
        /// </summary>
        /// <param name="uin"></param>
        /// <returns></returns>
        public long GetQQByUin(long uin)
        {
            long qq = ((JObject)client.GetJsonAsync(SmartQQAPI.GetQQById, 
                uin, vfwebqq, new Random().NextDouble())[
                    "result"])["account"].Value<long>();
            return qq;
        }
        /// <summary>
        /// 获取qq号
        /// </summary>
        /// <param name="friend"></param>
        /// <returns></returns>
        public long GetQQ(Friend friend)
        {
            return GetQQByUin(friend.Uin);
        }
        /// <summary>
        /// 获取群成员信息
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public GroupInfo GetGroupInfo(Group group)
        {
            GroupInfo groupInfo = cache.GetValueCache<GroupInfo>(group.Code.ToString());
            if(groupInfo == null)
            {
                JObject jObject = client.GetJsonAsync(SmartQQAPI.GetGroupInfo, group.Code, vfwebqq);
                groupInfo = jObject["result"]["ginfo"].ToObject<GroupInfo>();
                cache.UpdateValueCache(group.Code.ToString(), groupInfo, 3600);
            }
            return groupInfo;
        }
        /// <summary>
        /// 发送私有消息
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="friend">好友信息</param>
        /// <returns></returns>
        public bool SendPrivateMessage(string message, Friend friend)
        {
            return SendMessage(Message.MessageType.PrivateMessage, friend.Uin, message);
        }
        /// <summary>
        /// 发送私有消息
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="nin">id</param>
        /// <returns></returns>
        public bool SendPrivateMessage(string message, long uin)
        {
            return SendMessage(Message.MessageType.PrivateMessage, uin, message);
        }
        /// <summary>
        /// 发送群消息
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="uin">群id</param>
        /// <returns></returns>
        public bool SendGroupMessage(string message, long uin)
        {
            return SendMessage(Message.MessageType.GroupMessage, uin, message);
        }
        /// <summary>
        /// 发送给群消息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        public bool SendGroupMessage(string message, Group group)
        {
            return SendMessage(Message.MessageType.GroupMessage, group.Id, message);
        }
        /// <summary>
        /// 启动消息循环系统
        /// </summary>
        public void StartMessageLoop()
        {
            if(messageLoopThread != null && messageLoopThread.IsAlive)
            {
                return;
            }
            if (messageDistribution != null && messageDistribution.IsAlive)
            {
                messageDistribution.Abort();
                messageDistribution = null;
            }
            
            messageLoopThread = new Thread(pollAllMessageLoop);
            messageLoopThread.IsBackground = true;
            messageLoopThread.Start();

            messageDistribution = new Thread(distributionLoop);
            messageDistribution.IsBackground = true;
            messageDistribution.Start();
        }
        /// <summary>
        /// 停止消息循环
        /// </summary>
        public void StopMessageLoop()
        {
            if(messageLoopThread != null && messageLoopThread.IsAlive)
            {
                messageLoopThread.Abort();
            }
            if (messageDistribution != null && messageDistribution.IsAlive)
            {
                messageDistribution.Abort();
            }
            messageLoopThread = null;
            messageDistribution = null;
        }
        #endregion
        #region 私有方法
        /// <summary>
        /// 获取length长度的随机数字
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        private string getRandomNum(int length = 17)
        {
            string nums = "";
            Random random = new Random();
            for (int i = 0; i < length; i++)
            {
                nums += random.Next(0, 10);
            }
            return nums;
        }
        /// <summary>
        /// 根据登录cookie中的qrsig生成ptqrtoken
        /// </summary>
        /// <param name="cookieqrsig"></param>
        /// <returns></returns>
        private string hash33(string cookieqrsig)
        {
            int value = 0;
            foreach (var item in cookieqrsig)
            {
                value += (value << 5) + (byte)item;
            }
            value = value & 2147483647;
            return value.ToString();
        }
        /// <summary>
        /// 获取hash value
        /// </summary>
        /// <param name="uin"></param>
        /// <param name="ptwebqq"></param>
        /// <returns></returns>
        private string getHash(long uin, string ptwebqq)
        {
            var n = new int[4];
            for (var T = 0; T < ptwebqq.Length; T++)
                n[T % 4] ^= ptwebqq[T];
            string[] u = { "EC", "OK" };
            var v = new long[4];
            v[0] = ((uin >> 24) & 255) ^ u[0][0];
            v[1] = ((uin >> 16) & 255) ^ u[0][1];
            v[2] = ((uin >> 8) & 255) ^ u[1][0];
            v[3] = (uin & 255) ^ u[1][1];

            var u1 = new long[8];

            for (var t = 0; t < 8; t++)
                u1[t] = t % 2 == 0 ? n[t >> 1] : v[t >> 1];

            string[] n1 = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "A", "B", "C", "D", "E", "F" };
            var v1 = "";
            foreach (var aU1 in u1)
            {
                v1 += n1[(int)((aU1 >> 4) & 15)];
                v1 += n1[(int)(aU1 & 15)];
            }
            return v1;
        }
        /// <summary>
        /// 获取缓存的对象
        /// </summary>
        /// <typeparam name="T">缓存的类型</typeparam>
        /// <param name="key">缓存的key</param>
        /// <param name="action">缓存更新的函数</param>
        /// <returns></returns>
        private T getCache<T>(string key, Func<bool> action)
        {
            T t= cache.GetValueCache<T>(key);
            if(t == null)
            {
                action();
            }
            return cache.GetValueCache<T>(key);
        }

        /// <summary>
        /// 解决103错误的问题
        /// </summary>
        /// <returns></returns>
        private bool testLogin()
        {
            Log.Write("开始向服务器发送测试连接请求");
            var result = client.GetStringAsync(SmartQQAPI.TestLogin, vfwebqq, clientid, psessionid, new Random().NextDouble());
            return JObject.Parse(result)["retcode"].Value<int?>() == 0;
        }
        /// <summary>
        /// 检查是否登录
        /// </summary>
        /// <returns></returns>
        private bool isLogin()
        {
            return true;
        }
        /// <summary>
        /// 更新好友的详细信息
        /// </summary>
        /// <param name="friend">好友</param>
        /// <returns></returns>
        internal FriendInfo updateFriendInfo(Friend friend)
        {
            try
            {
                FriendInfo info = null;
                info = cache.GetValueCache<FriendInfo>(friend.Id.ToString());
                if (info == null)
                {
                    JObject jObject = client.GetJsonAsync(SmartQQAPI.GetFriendInfo, friend.Uin, vfwebqq, psessionid);
                    info = jObject["result"].ToObject<FriendInfo>();
                    cache.UpdateValueCache(friend.Id.ToString(), info);
                }
                return info;
            }
            catch (Exception ex)
            {
                Log.Write("更新好友:", friend.Nickname, " ，详细信息失败!");
                return null;
            }
        }
        /// <summary>
        /// 更新自己账户的信息
        /// </summary>
        /// <returns></returns>
        internal FriendInfo updateSelfAccountInfo()
        {
            FriendInfo info = cache.GetValueCache<FriendInfo>(SmartQQStaticString.SelfInfo);
            if(info == null)
            {
                info = ((JObject)client.GetJsonAsync(SmartQQAPI.GetAccountInfo)["result"]).ToObject<FriendInfo>();
            }
            return info;
        }
        /// <summary>
        /// 更新自己QQ群信息
        /// </summary>
        /// <param name="group">QQ群信息</param>
        /// <returns></returns>
        internal GroupInfo updateGroupInfo(Group group)
        {
            GroupInfo info = cache.GetValueCache<GroupInfo>(group.Code.ToString());
            try
            {
                if (info == null)
                {
                    JObject result = ((JObject)client.GetJsonAsync(SmartQQAPI.GetGroupInfo, group.Code, vfwebqq)["result"]);

                    info = result["ginfo"].ToObject<GroupInfo>();

                    // 获得群成员信息
                    var members = new Dictionary<long, GroupMemberInfo>();
                    var minfo = (JArray)result["minfo"];
                    for (var i = 0; minfo != null && i < minfo.Count; i++)
                    {
                        var member = minfo[i].ToObject<GroupMemberInfo>();
                        members.Add(member.Uin, member);
                        info.Members.Add(member);
                    }
                    var stats = (JArray)result["stats"];
                    for (var i = 0; stats != null && i < stats.Count; i++)
                    {
                        var item = (JObject)stats[i];
                        var member = members[item["uin"].Value<long>()];
                        member.ClientType = item["client_type"].Value<int>();
                        member.Status = item["stat"].Value<int>();
                    }
                    var cards = (JArray)result["cards"];
                    for (var i = 0; cards != null && i < cards.Count; i++)
                    {
                        var item = (JObject)cards[i];
                        members[item["muin"].Value<long>()].Alias = item["card"].Value<string>();
                        if (item["muin"].Value<long>() == SelfInfo.Uin)
                            info.MyAlias = item["card"].Value<string>();
                    }
                    var vipinfo = (JArray)result["vipinfo"];
                    for (var i = 0; vipinfo != null && i < vipinfo.Count; i++)
                    {
                        var item = (JObject)vipinfo[i];
                        var member = members[item["u"].Value<long>()];
                        member.IsVip = item["is_vip"].Value<int>() == 1;
                        member.VipLevel = item["vip_level"].Value<int>();
                    }
                }
                return info;
            }
            catch (Exception)
            {
                Log.Write("拉取群的详细信息失败!");
                return null;
            }
        }
        /// <summary>
        /// 消息循环
        /// </summary>
        private void pollAllMessageLoop()
        {
            var r = new JObject
            {
                {"ptwebqq", ptwebqq},
                {"clientid", clientid},
                {"psessionid", psessionid},
                {"key", ""}
            };

            while (true)
            {
                var response = client.PostJsonAsync(SmartQQAPI.PollMessage, r);
                JArray messageArray = response["result"] as JArray;

                foreach (var item in messageArray)
                {
                    var message = (JObject)item;
                    var type = message["poll_type"].Value<string>();

                    IMessage imessage = null;
                    switch (type)
                    {
                        case "message":
                            imessage = message["value"].ToObject<PrivateMessage>();
                            (imessage as PrivateMessage).SmartQQBot = this;
                            //(imessage as PrivateMessage).Content = (message["value"]["content"] as JArray)[1].ToObject<string>();
                            //(imessage as PrivateMessage).Font = (message["value"]["content"] as JArray)[0].ToObject<Font>();
                            break;
                        case "group_message":
                            imessage = message["value"].ToObject<GroupMessage>();
                            (imessage as GroupMessage).SmartQQBot = this;
                            //(imessage as GroupMessage).Content = (message["value"]["content"] as JArray)[1].ToObject<string>();
                            //(imessage as GroupMessage).Font = (message["value"]["content"] as JArray)[0].ToObject<Font>();
                            break;
                        case "discu_message":
                            Log.Write("收到暂时不做处理的讨论组消息!");
                            break;
                        default:
                            Log.Write("意外的消息类型：" + type);
                            break;
                    }
                    if (imessage != null)
                    {
                        messages.Enqueue(imessage);
                    }
                }
                if(messages.Count > 0)
                {
                    ///开启阻塞的线程
                    distributionThreadAre.Set();
                }
                Thread.Sleep(300);
            }

        }
        /// <summary>
        /// 派发消息循环
        /// </summary>
        private void distributionLoop()
        {
            while (true)
            {
                ///如果没有消息则阻塞线程
                if(messages.Count == 0)
                {
                    distributionThreadAre.WaitOne();
                }
                else
                {
                    IMessage message = messages.Dequeue();
                    MessageCallBack?.Invoke(message);
                    message = null;
                }
            }
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="messageType">消息类型</param>
        /// <param name="id">消息发送方向的id</param>
        /// <param name="msg">消息内容</param>
        /// <returns></returns>
        internal bool SendMessage(Message.MessageType messageType, long id, string msg)
        {
            try
            {
                SmartQQAPI url = null;
                switch (messageType)
                {
                    case Message.MessageType.PrivateMessage:
                        url = SmartQQAPI.SendMessageToFriend;
                        break;
                    case Message.MessageType.GroupMessage:
                        url = SmartQQAPI.SendMessageToGroup;
                        break;
                    case Message.MessageType.UnKnowMessage:
                        Log.Write("未知的消息类型!");
                        return false;
                    default:
                        Log.Write("未知的类型!");
                        return false;
                }   
                var response = client.PostJsonAsync(url, new JObject
                {
                    {SmartQQStaticString.GetParamNameByMessageType(messageType), id},
                    {
                        "content",
                        new JArray
                            {
                                SmartQQStaticString.TranslateEmoticons(msg),
                                new JArray {"font", JObject.FromObject(Font.DefaultFont)}
                            }
                            .ToString(Formatting.None)
                    },
                    {"face", 573},
                    {"clientid", clientid},
                    {"msg_id", messageId++},
                    {"psessionid", psessionid}
                }, RetryTimes);

                int? code = response["retcode"].ToObject<int?>();
                if(code == 0)
                {
                    Log.Write("消息发送成功!");
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                Log.Write("发送信息失败!");
                return false;
            }
            return true;
        }
        #endregion
    }
}
