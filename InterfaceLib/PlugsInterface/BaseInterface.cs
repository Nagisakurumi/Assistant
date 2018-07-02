using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceLib.PlugsInterface
{
    /// <summary>
    /// 基础插件接口
    /// </summary>
    public interface BaseInterface
    {
        /// <summary>
        /// 启动插件
        /// </summary>
        /// <returns></returns>
        bool Start();
        /// <summary>
        /// 终止插件
        /// </summary>
        /// <returns></returns>
        bool Stop();

        /// <summary>
        /// 初始化插件
        /// </summary>
        /// <returns></returns>
        bool Init();
    }
}
