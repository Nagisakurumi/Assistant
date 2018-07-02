using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceLib.MsgInterface
{
    /// <summary>
    /// 插件接口
    /// </summary>
    public interface PlugInterface
    {
        /// <summary>
        /// 是否已经安装过的插件
        /// </summary>
        bool IsInstall { get; }
        /// <summary>
        /// 远程仓库插件的地址
        /// </summary>
        string RemoteURL { get; }
        /// <summary>
        /// 本地插件安装下载的地址
        /// </summary>
        string LocalURL { get; }
        /// <summary>
        /// 插件名称
        /// </summary>
        string Name { get; }
        /// <summary>
        /// 插件作者
        /// </summary>
        string Author { get; }
        /// <summary>
        /// 插件大小
        /// </summary>
        string Size { get; }
        /// <summary>
        /// 插件创作日期
        /// </summary>
        DateTime DateTime { get; }
        /// <summary>
        /// 插件版本
        /// </summary>
        string Version { get; }
    }
}
