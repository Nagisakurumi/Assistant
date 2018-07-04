using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheLib
{
    /// <summary>
    /// 缓存结构
    /// </summary>
    internal class CacheStruct
    {
        /// <summary>
        /// 缓存的起始时间
        /// </summary>
        private DateTime startTime = DateTime.Now;
        /// <summary>
        /// 缓存的终止结束时间
        /// </summary>
        private DateTime endTime = DateTime.MinValue;
        /// <summary>
        /// 超时的缓存时间
        /// </summary>
        private long CacheSecondTimeOut { get; } = -1;
        /// <summary>
        /// 缓存的小时超时时间
        /// </summary>
        private long CacheHourTimeOut { get; } = -1;
        /// <summary>
        /// 缓存的内容
        /// </summary>
        public object CacheObject { get; } = null;
        /// <summary>
        /// 是否缓存的内容已经超时
        /// </summary>
        public bool IsTimeOut
        {
            get
            {
                if(!endTime.Equals(DateTime.MinValue))
                {
                    return endTime < startTime;
                }
                if(CacheSecondTimeOut != -1)
                {
                    return (DateTime.Now - startTime).TotalSeconds > CacheSecondTimeOut;
                }
                if(CacheHourTimeOut != -1)
                {
                    return (DateTime.Now - startTime).TotalHours > CacheHourTimeOut;
                }
                return true;
            }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="obj">缓存的对象</param>
        /// <param name="endTime">缓存过期时间(MinValue为不作为)</param>
        /// <param name="cacheSecondTimeOut">缓存超时秒(-1为不作为)</param>
        /// <param name="cacheHourTimeOut">缓存超时小时(-1为不作为)</param>
        public CacheStruct(object obj, DateTime endTime, long cacheSecondTimeOut = 360, long cacheHourTimeOut = -1)
        {
            this.CacheSecondTimeOut = cacheHourTimeOut;
            this.CacheObject = obj;
            this.CacheHourTimeOut = cacheHourTimeOut;
            this.startTime = DateTime.Now;
            this.endTime = endTime;
        }
    }
}
