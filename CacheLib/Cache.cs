using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheLib
{
    /// <summary>
    /// 缓存
    /// </summary>
    public class Cache
    {
        /// <summary>
        /// 缓存的容器
        /// </summary>
        private Dictionary<string, CacheStruct> cacheObjs = new Dictionary<string, CacheStruct>();
        /// <summary>
        /// 默认缓存过期的秒时间
        /// </summary>
        public long DefultTimeOutSecond { get; set; } = 360;

        /// <summary>
        /// 设置缓存的对象
        /// </summary>
        /// <param name="key">key值</param>
        /// <param name="obj">缓存对象值</param>
        /// <param name="timeOutSecond">超时时间</param>
        /// <returns></returns>
        public bool SetValueCache(string key, object obj, long timeOutSecond = -1)
        {
            long timeout = timeOutSecond == -1 ? DefultTimeOutSecond : timeOutSecond;
            if(cacheObjs.ContainsKey(key))
            {
                return false;
            }
            else
            {
                cacheObjs.Add(key, new CacheStruct(obj, DateTime.MinValue, timeout));
                return true;
            }
        }
        /// <summary>
        /// 获取缓存的值
        /// </summary>
        /// <param name="key">缓存的key</param>
        /// <returns></returns>
        public object GetValueCache(string key)
        {
            if(!cacheObjs.ContainsKey(key))
            {
                return null;
            }
            else
            {
                if(cacheObjs[key].IsTimeOut)
                {
                    ///删除超时的缓存
                    cacheObjs.Remove(key);
                    return null;
                }
                else
                {
                    return cacheObjs[key].CacheObject;
                }
            }
        }
        /// <summary>
        /// 获取缓存的值
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">缓存的key</param>
        /// <returns></returns>
        public T GetValueCache<T>(string key)
        {
            object obj = GetValueCache(key);
            return obj == null ? default(T) : (T)obj;
        }
        /// <summary>
        /// 更新缓存里的对象，如果不存在则添加
        /// </summary>
        /// <param name="key">key值</param>
        /// <param name="obj">缓存对象值</param>
        /// <param name="timeOutSecond">超时时间</param>
        /// <returns></returns>
        public void UpdateValueCache(string key, object obj, long timeOutSecond = -1)
        {
            long timeout = timeOutSecond == -1 ? DefultTimeOutSecond : timeOutSecond;
            if (cacheObjs.ContainsKey(key))
            {
                cacheObjs[key] = null;
                cacheObjs[key] = new CacheStruct(obj, DateTime.MinValue, timeout);
            }
            else
            {
                SetValueCache(key, obj, timeOutSecond);
            }
        }
    }

}
