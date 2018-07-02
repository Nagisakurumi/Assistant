using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SmartQQ
{
    public class Client : IDisposable
    {
        /// <summary>
        /// 存放cookie
        /// </summary>
        private Dictionary<string, Cookie> cookies = new Dictionary<string, Cookie>();
        /// <summary>
        /// http请求客户端
        /// </summary>
        private HttpClient httpClient = new HttpClient();

        /// <summary>
        /// 构造函数
        /// </summary>
        public Client()
        {
            httpClient.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/67.0.3396.99 Safari/537.36");
            httpClient.DefaultRequestHeaders.Add("accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
            //httpClient.DefaultRequestHeaders.Add("Referer", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/67.0.3396.99 Safari/537.36");
        }
        /// <summary>
        /// 清理
        /// </summary>
        public void Dispose()
        {
            httpClient.Dispose();
        }
        /// <summary>
        /// 进行get请求，并且返回url的请求消息
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public HttpResponseMessage GetResponse(string url)
        {
            HttpResponseMessage response = httpClient.GetAsync(new Uri(url)).Result;
            return response;
        }


        public void PostResponse(string url, string content)
        {

        }
        


    }
}
