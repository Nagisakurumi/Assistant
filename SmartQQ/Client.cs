using LogLib;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using static SmartQQ.SmartQQLog;

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
        private HttpClientHandler clientHandler = new HttpClientHandler() { AllowAutoRedirect = true, UseCookies = true };
        /// <summary>
        /// 请求头的参数
        /// </summary>
        public const string UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:51.0) Gecko/20100101 Firefox/51.0";
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
        /// cookie容器
        /// </summary>
        public CookieContainer CookieContainer { get; } = new CookieContainer();
        /// <summary>
        /// 构造函数
        /// </summary>
        public Client()
        {
            clientHandler.CookieContainer = CookieContainer;
            httpClient = new HttpClient(clientHandler);
            httpClient.DefaultRequestHeaders.Add("user-agent", UserAgent);
            //httpClient.DefaultRequestHeaders.Add("accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
            //httpClient.DefaultRequestHeaders.Add("Referer", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/67.0.3396.99 Safari/537.36");
            httpClient.DefaultRequestHeaders.Add("KeepAlive", "true");

            Log.Write("初始化请求客户端!");
        }
        /// <summary>
        /// 清理
        /// </summary>
        public void Dispose()
        {
            httpClient.Dispose();
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
        /// <summary>
        /// 获取对应url中的对应name的cookie
        /// </summary>
        /// <param name="url"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetCookieByUrl(SmartQQAPI smartQQAPI, string name, params object [] ags)
        {
            CookieCollection cookies = null;
            if (ags.Length != 0)
               cookies = CookieContainer.GetCookies(new Uri(smartQQAPI.BuildUrl(ags)));
            else
            {
                cookies = CookieContainer.GetCookies(new Uri(smartQQAPI.Url));
            }
            foreach (Cookie item in cookies)
            {
                if(item.Name.Equals(name))
                {
                    return item.Value;
                }
            }
            return "";
        }
        /// <summary>
        /// 获取get请求
        /// </summary>
        /// <param name="smartQQAPI"></param>
        /// <returns></returns>
        public HttpResponseMessage GetAsync(SmartQQAPI smartQQAPI, params object[] ags)
        {
            Uri refuri = addRef(smartQQAPI);
            HttpResponseMessage result = httpClient.GetAsync(smartQQAPI.BuildUrl(ags)).Result;
            reciveRef(refuri);
            return result;
        }

        /// <summary>
        /// 获取get请求的字符串流
        /// </summary>
        /// <param name="smartQQAPI"></param>
        /// <returns></returns>
        public string GetStringAsync(SmartQQAPI smartQQAPI, params object [] ags)
        {
            Uri refuri = addRef(smartQQAPI);
            string result = httpClient.GetStringAsync(smartQQAPI.BuildUrl(ags)).Result;
            reciveRef(refuri);
            return result;
        }

        /// <summary>
        /// 获取get请求的json
        /// </summary>
        /// <param name="smartQQAPI"></param>
        /// <returns></returns>
        public JObject GetJsonAsync(SmartQQAPI smartQQAPI, params object[] ags)
        {
            Uri refuri = addRef(smartQQAPI);
            string result = httpClient.GetStringAsync(smartQQAPI.BuildUrl(ags)).Result;
            reciveRef(refuri);
            return (JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(result);
        }
        /// <summary>
        /// 获取get请求中的字节流数据
        /// </summary>
        /// <param name="smartQQAPI"></param>
        /// <returns></returns>
        public byte [] GetBytesAsync(SmartQQAPI smartQQAPI, params object[] ags)
        {
            Uri refuri = addRef(smartQQAPI);
            byte[] result = httpClient.GetByteArrayAsync(smartQQAPI.BuildUrl(ags)).Result;
            reciveRef(refuri);
            return result;
        }
        /// <summary>
        /// 获取get请求中的字节流数据
        /// </summary>
        /// <param name="smartQQAPI"></param>
        /// <returns></returns>
        public Stream GetStreamAsync(SmartQQAPI smartQQAPI, params object[] ags)
        {
            Uri refuri = addRef(smartQQAPI);
            Stream result = httpClient.GetStreamAsync(smartQQAPI.BuildUrl(ags)).Result;
            reciveRef(refuri);
            return result;
        }
        /// <summary>
        /// Post请求
        /// </summary>
        /// <param name="smartQQAPI"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> PostResponseAsync(SmartQQAPI smartQQAPI, JObject content)
        {
            var hasOrigin = httpClient.DefaultRequestHeaders.TryGetValues("Origin", out IEnumerable<string> origin);

            Uri refrui =  addRef(smartQQAPI);
            if (httpClient.DefaultRequestHeaders.Contains("Origin"))
            {
                httpClient.DefaultRequestHeaders.Remove("Origin");
                httpClient.DefaultRequestHeaders.Add("Origin", smartQQAPI.Origin);
            }
            else
            {
                httpClient.DefaultRequestHeaders.Add("Origin", smartQQAPI.Origin);
            }
            string contentstring = $"r={WebUtility.UrlEncode(content.ToString(Formatting.None))}";
            HttpContent hc = new StringContent(contentstring, Encoding.UTF8);
            hc.Headers.ContentType = MediaTypeHeaderValue.Parse("application/ x-www-form-urlencoded; charset=UTF-8");
            var response = httpClient.PostAsync(smartQQAPI.Url, hc);
            response.Wait();

            // 复原httpClient
            if (hasOrigin)
            {
                httpClient.DefaultRequestHeaders.Remove("Origin");
                httpClient.DefaultRequestHeaders.Add("Origin", origin);
            }
            else
            {
                httpClient.DefaultRequestHeaders.Remove("Origin");
            }
            reciveRef(refrui);
            return await response;
        }
        /// <summary>
        /// Post请求获取返回字符串
        /// </summary>
        /// <param name="smartQQAPI"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public string PostStringAsync(SmartQQAPI smartQQAPI, JObject content)
        {
            HttpResponseMessage httpResponseMessage = PostResponseAsync(smartQQAPI, content).Result;
            return httpResponseMessage.Content.ReadAsStringAsync().Result;
        }

        /// <summary>
        /// Post请求获取返回json
        /// </summary>
        /// <param name="smartQQAPI"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public JObject PostJsonAsync(SmartQQAPI smartQQAPI, JObject content)
        {
            HttpResponseMessage httpResponseMessage = PostResponseAsync(smartQQAPI, content).Result;
            return Newtonsoft.Json.JsonConvert.DeserializeObject(httpResponseMessage.Content.ReadAsStringAsync().Result) as JObject;
        }
        /// <summary>
        /// Post请求获取返回json
        /// </summary>
        /// <param name="smartQQAPI"></param>
        /// <param name="content"></param>
        /// <param name="retryTimes">重新尝试次数</param>
        /// <returns></returns>
        public JObject PostJsonAsync(SmartQQAPI smartQQAPI, JObject content, int retryTimes)
        {
            HttpResponseMessage httpResponseMessage = null;
            httpResponseMessage = PostResponseAsync(smartQQAPI, content).Result;
            while (httpResponseMessage.StatusCode != HttpStatusCode.OK && retryTimes -- >= 0)
            {
                httpResponseMessage = PostResponseAsync(smartQQAPI, content).Result;
            }
            return Newtonsoft.Json.JsonConvert.DeserializeObject(httpResponseMessage.Content.ReadAsStringAsync().Result) as JObject;
        }

        #region 私有方法
        /// <summary>
        /// 添加Referrer
        /// </summary>
        /// <param name="smartQQAPI"></param>
        private Uri addRef(SmartQQAPI smartQQAPI)
        {
            var httpRef = httpClient.DefaultRequestHeaders.Referrer;
            if (smartQQAPI.Referer != null && !smartQQAPI.Referer.Equals(""))
            {
                httpClient.DefaultRequestHeaders.Referrer = new Uri(smartQQAPI.Referer);
            }
            return httpRef;
        }
        /// <summary>
        /// 恢复ref
        /// </summary>
        /// <param name="smartQQAPI"></param>
        private void reciveRef(Uri uri)
        {
            httpClient.DefaultRequestHeaders.Referrer = uri;
        }
        #endregion

    }
}
