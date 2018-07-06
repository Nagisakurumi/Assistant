using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Audio
{
    /// <summary>
    /// 采集语音的模式
    /// </summary>
    public enum CaputreMode
    {
        /// <summary>
        /// 以时间的模式进行
        /// </summary>
        Time,
        /// <summary>
        /// 以最大值不超过某个值结束
        /// </summary>
        Max,
        /// <summary>
        /// 以平均值不超过某个值结束
        /// </summary>
        Avg,
    }
}
