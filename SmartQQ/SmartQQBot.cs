using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LogLib;
using CacheLib;

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
        /// 是否已经登录
        /// </summary>
        public bool IsLogin => isLogin();
        /// <summary>
        /// 获取群的详细信息
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public GroupInfo this[Group group] => GetGroupInfo(group);
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
                foreach (var item in response["friends"] as JArray)
                {

                    friends.Add(new Friend()
                    {
                        Flag = Convert.ToInt64(item["flag"].ToString()),
                        Uin = Convert.ToInt64(item["uin"].ToString()),
                        GroupId = Convert.ToInt64(item["categories"].ToString())
                    });
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
                ///好友info信息
                foreach (var item in response["info"] as JArray)
                {
                    Friend friend = friends.Where(p => p.Uin == Convert.ToInt64(item["uin"].ToString())).First();
                    if (friend != null)
                    {
                        friend.Face = Convert.ToInt32(item["face"].ToString());
                        friend.Nickname = item["nick"].ToString();
                    }
                }
                ///更新所有好友的详细信息
                foreach (var item in friends)
                {
                    updateFriendInfo(item);
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
                Log.Write(response.ToString());
                ///QQ群信息
                groups.Clear();
                foreach (var item in response["gnamelist"] as JArray)
                {
                    groups.Add(new Group()
                    {
                        Flag = Convert.ToInt64(item["flag"].ToString()),
                        Id = Convert.ToInt64(item["gid"].ToString()),
                        Name = item["name"].ToString(),
                        Code = Convert.ToInt64(item["code"].ToString())
                    });
                }
                cache.UpdateValueCache(SmartQQStaticString.Group, groups, 1000);
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
        
        public bool SendPrivateMessage(string message, Friend friend)
        {
            return true;
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
        private bool updateFriendInfo(Friend friend)
        {
            JObject jObject = client.GetJsonAsync(SmartQQAPI.GetFriendInfo, friend.Uin, vfwebqq, psessionid);
            friend.FriendInfo = null;
            friend.FriendInfo = jObject["result"].ToObject<FriendInfo>();
            return true;
        }
        /// <summary>
        /// 更新自己账户的信息
        /// </summary>
        /// <returns></returns>
        private FriendInfo updateSelfAccountInfo()
        {
            FriendInfo info = cache.GetValueCache<FriendInfo>(SmartQQStaticString.SelfInfo);
            if(info == null)
            {
                info = ((JObject)client.GetJsonAsync(SmartQQAPI.GetAccountInfo)["result"]).ToObject<FriendInfo>();
            }
            return info;
        }
        /// <summary>
        /// 消息循环
        /// </summary>
        private void pollAllMessageLoop()
        {

        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="url">请求的url</param>
        /// <param name="messageType">消息类型</param>
        /// <param name="id">消息发送方向的id</param>
        /// <param name="msg">消息内容</param>
        /// <returns></returns>
        internal bool SendMessage(SmartQQAPI url, Message.MessageType messageType, long id, string msg)
        {
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

            return true;
        }
        #endregion
    }
}
