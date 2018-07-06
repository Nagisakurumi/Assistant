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
    public interface IPlugBaseInterface
    {
        /// <summary>
        /// 插件的名称
        /// </summary>
        string Name { get; }
        /// <summary>
        /// 日志写入回调
        /// </summary>
        Action<string> LogWrite { get; set; }
        ///// <summary>
        ///// 插件的作者
        ///// </summary>
        //string Author { get; }
        ///// <summary>
        ///// 插件的创作日期
        ///// </summary>
        //DateTime DateTime { get;  }
        ///// <summary>
        ///// 插件版本
        ///// </summary>
        //string Version { get; }
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
