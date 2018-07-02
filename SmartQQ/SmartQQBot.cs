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
        private string loginURL = "https://ssl.ptlogin2.qq.com/ptqrshow?appid=501004106&e=2&l=M&s=3&d=72&v=4&t=0.";
        /// <summary>
        /// 获取二维码登录请求的地址的尾部
        /// </summary>
        private string loginURLEnd = "&daid=164&pt_3rd_aid=0";
        /// <summary>
        /// 保存登录图片的路径
        /// </summary>
        private string saveLoginImagePath = "login.png";
        /// <summary>
        /// 检测是否已经登录和二维码是否有效
        /// </summary>
        private string checkLogin = "https://ssl.ptlogin2.qq.com/ptqrlogin?u1=http://web2.qq.com/proxy.html&ptqrtoken=";
        private string checklogincenter = "&ptredirect=0&h=1&t=1&g=1&from_ui=1&ptlang=2052&action=0-0-";
        private string checkloginEnd = "&js_ver=10275&js_type=1&login_sig=d3x*Vx2yJ4DrYI6O01ENdxJ9bDGHXawivhMrOTCZi1HE2Dc2j79W8nG1iJkAf-XP&pt_uistyle=40&aid=501004106&daid=164&mibao_css=m_webqq&";
        private long checkloginnum = 1530515955825;
        private string cookieqrsig = "";

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
            HttpResponseMessage response = client.GetResponse(loginURL + getRandomNum() + loginURLEnd);
            Stream stream = response.Content.ReadAsStreamAsync().Result;
            cookieqrsig = new Regex("=\\S*;").Match((response.Headers.GetValues("Set-Cookie") as string[])[0]).Value.Split(';')[0].Substring(1);

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
            return true;
        }
        /// <summary>
        /// 检测是否已经成功登录
        /// </summary>
        /// <returns></returns>
        public bool CheckLogin()
        {
            string url = checkLogin + getCheckptqrtoken() + checklogincenter + (checkloginnum++).ToString() + checkloginEnd;
            HttpResponseMessage httpResponseMessage = client.GetResponse(url);
            Console.WriteLine(httpResponseMessage.Content.ReadAsStringAsync().Result);
            int value = Convert.ToInt32(new Regex("'\\S*'").Match(httpResponseMessage.Content.ReadAsStringAsync().Result).Value.Split(',')[0].Substring(1, 2));
            if(value == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
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
