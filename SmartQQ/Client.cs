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
        /// http请求客户端
        /// </summary>
        private HttpClient httpClient = null;
        /// <summary>
        /// http请求头
        /// </summary>
        private HttpClientHandler clientHandler = new HttpClientHandler() { AllowAutoRedirect = false };
        /// <summary>
        /// 是否允许重定向
        /// </summary>
        public bool AllowAutoRedirect
        {
            get
            {
                return clientHandler.AllowAutoRedirect;
            }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public Client()
        {
            httpClient = new HttpClient(clientHandler);
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
        /// <summary>
        /// 添加响应头部
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void AddHeader(string name, string value)
        {
            if(httpClient.DefaultRequestHeaders.Contains(name) == false)
            {
                httpClient.DefaultRequestHeaders.Add(name, value);
            }
        }
        /// <summary>
        /// 删除响应头部
        /// </summary>
        /// <param name="name"></param>
        public void RemoveHeader(string name)
        {
            if (httpClient.DefaultRequestHeaders.Contains(name) == true)
            {
                httpClient.DefaultRequestHeaders.Remove(name);
            }
        }
        /// <summary>
        /// 获取name的cookie的值
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetCookieValueByName(string name, HttpResponseMessage httpResponseMessage)
        {
            string [] array = httpResponseMessage.Headers.GetValues("Set-Cookie").Where(p => p.Contains(name)).ToArray();
            if(array.Length == 0)
            {
                return "";
            }
            else
            {
                return array[0];
            }
        }

        public HttpResponseMessage PostResponse(string url, Dictionary<string, string> content)
        {
            HttpContent httpContent = new FormUrlEncodedContent(content);
            HttpResponseMessage response = httpClient.PostAsync(new Uri(url),
                httpContent).Result;
            return response;
        }
        


    }
}
