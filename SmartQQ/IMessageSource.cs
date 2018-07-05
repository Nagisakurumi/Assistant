using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartQQ
{
    /// <summary>
    /// 消息源
    /// </summary>
    public interface IMessageSource
    {
        /// <summary>
        /// 用于消息发送的id
        /// </summary>
        long Id { get; }

    }
}
