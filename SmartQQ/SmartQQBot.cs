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

namespace SmartQQ
{
    public class SmartQQBot
    {
        #region 字段
        /// <summary>
        /// http请求客户端
        /// </summary>
        private Client client = new Client();
        /// <summary>
        /// 获取二维码登录请求的地址
        /// </summary>
        private string loginURL = "https://ssl.ptlogin2.qq.com/ptqrshow?appid=";
        private string loginURLCneter = "&e=2&l=M&s=3&d=72&v=4&t=0.";
        /// <summary>
        /// 获取二维码登录请求的地址的尾部
        /// </summary>
        private string loginURLEnd = "&daid=164&pt_3rd_aid=0";
        /// <summary>
        /// 保存登录图片的路径
        /// </summary>
        private string saveLoginImagePath = "login.png";
        /// <summary>
        /// 开始检测登录的时间
        /// </summary>
        private DateTime checkStartTime = DateTime.Now;
        /// <summary>
        /// 检测是否已经登录和二维码是否有效
        /// </summary>
        private string checkLogin = "https://ssl.ptlogin2.qq.com/ptqrlogin?u1=" +
            "http://web2.qq.com/proxy.html&ptqrtoken=";
        private string checklogincenter = "&ptredirect=0&h=1&t=1&g=1&from_ui=1&ptlang=2052&action=0-0-";
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
        //private long checkloginnum = 1530515955825;
        private string cookieqrsig = "";
        /// <summary>
        /// 验证登录扫码成功后的回调地址
        /// </summary>
        private string checksigURL = "";
        /// <summary>
        /// 用于拉取qq好友时候的验证码获取地址
        /// </summary>
        private string vfwebqqURL = "http://s.web2.qq.com/api/getvfwebqq?ptwebqq=&clientid=53999199&psessionid=&t=1530601105595";

        /// <summary>
        /// 检测状态前的初始化
        /// </summary>
        private string init_url = "https://ui.ptlogin2.qq.com/cgi-bin/login?" +
                   "daid=164&target=self&style=16&mibao_css=m_webqq" +
                   "&appid=501004106&enable_qlogin=0&no_verifyimg=1" +
                   "&s_url=http%3A%2F%2Fw.qq.com%2Fproxy.html" +
                   "&f_url=loginerroralert&strong_login=1" +
                   "&login_state=10&t=20131024001";
        /// <summary>
        /// 二次登录地址
        /// </summary>
        private string login2URL = "http://d1.web2.qq.com/channel/login2";
        /// <summary>
        /// 二次登录的数据
        /// </summary>
        private Dictionary<string, string> login2Data = new Dictionary<string, string>()
        {
            { "ptwebqq", "" },
            { "clientid", "53999199"},
            { "psessionid","" },
            { "status", "online"}
        };
        /// <summary>
        /// 为了获取QQ好友列表的code
        /// </summary>
        private string forgetfriendlistcode = "";
        #endregion
        #region 访问器
        /// <summary>
        /// 登录图片保存路径
        /// </summary>
        public string SaveLoginImagePath { get => saveLoginImagePath; set => saveLoginImagePath = value; }
        #endregion
        #region 公开方法
        /// <summary>
        /// 登录qq
        /// </summary>
        /// <returns></returns>
        public bool Login()
        {
            HttpResponseMessage response = client.GetResponse(loginURL + checkloginData["aid"] + loginURLCneter + getRandomNum() + loginURLEnd);
            Stream stream = response.Content.ReadAsStreamAsync().Result;
            cookieqrsig = new Regex("=(.*?);").Match((response.Headers.GetValues("Set-Cookie") as string[])[0]).Groups[1].Value;
            Console.WriteLine(client.GetCookieValueByName("qrsig", response));
            if (File.Exists(SaveLoginImagePath))
            {
                File.Delete(SaveLoginImagePath);
            }
            using (FileStream filestream = File.Open(SaveLoginImagePath, FileMode.Create))
            {
                stream.CopyTo(filestream);
            }
            stream.Dispose();
            response.Dispose();
            checkStartTime = DateTime.Now;
            return true;
        }

        /// <summary>
        /// 检测是否登录成功前的检测
        /// </summary>
        /// <returns></returns>
        public bool CheckLoginForntInit()
        {
            HttpResponseMessage httpResponseMessage = client.GetResponse(init_url);
            string html = httpResponseMessage.Content.ReadAsStringAsync().Result;
            httpResponseMessage.Dispose();
            httpResponseMessage = null;
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
            string urlend = "";
            foreach (var item in checkloginData)
            {
                urlend += "&" + item.Key + "=" + item.Value;
            }
            urlend += "&";
            string url = checkLogin + getCheckptqrtoken() + checklogincenter + (DateTime.Now - checkStartTime).TotalMilliseconds.ToString() + urlend;
            client.AddHeader("referer", init_url);
            HttpResponseMessage httpResponseMessage = client.GetResponse(url);
            Console.WriteLine(httpResponseMessage.Content.ReadAsStringAsync().Result);
            string[] values = new Regex("'.*'").Match(httpResponseMessage.Content.ReadAsStringAsync().Result).Value.Split(',');
            int value = Convert.ToInt32(values[0].Substring(1, values[0].Length - 2));
            urlend = null;
            url = null;
            if (value == 0)
            {
                Console.WriteLine(client.GetCookieValueByName("ptwebqq", httpResponseMessage));
                client.RemoveHeader("referer");
                checksigURL = values[2].Substring(1, values[2].Length - 2);
                values = null;
                return true;
            }
            else
            {
                values = null;
                return false;
            }
        }
        /// <summary>
        /// 为了二次登录做准备
        /// </summary>
        /// <returns></returns>
        public bool ForLogion2()
        {
            forgetfriendlistcode = "";
            HttpResponseMessage httpResponseMessage = null;
            httpResponseMessage = client.GetResponse(checksigURL);
            httpResponseMessage.Dispose();
            httpResponseMessage = null;


            httpResponseMessage = client.GetResponse(vfwebqqURL);
            //httpResponseMessage = client.PostResponse(vfwebqqURL, vfwebqqData);
            Console.WriteLine(httpResponseMessage.Content.ReadAsStringAsync().Result);
            //string result = httpResponseMessage.Content.ReadAsStringAsync().Result;
            JObject jObject = (JObject)JsonConvert.DeserializeObject(httpResponseMessage.Content.ReadAsStringAsync().Result);
            forgetfriendlistcode = jObject["retcode"].ToString();
            httpResponseMessage.Dispose();
            if(forgetfriendlistcode.Equals(""))
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
            HttpResponseMessage httpResponseMessage = client.PostResponse(login2URL, login2Data);
            Console.WriteLine(httpResponseMessage.Content.ReadAsStringAsync().Result);
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
        private string getCheckptqrtoken()
        {
            int value = 0;
            foreach (var item in cookieqrsig)
            {
                value += (value << 5) + (byte)item;
            }
            value = value & 2147483647;
            return value.ToString();
        }
        #endregion
    }
}
