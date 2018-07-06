using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceLib.PlugsInterface;

namespace Assistant.Plugs
{
    /// <summary>
    /// 插件
    /// </summary>
    public class Plugin
    {
        /// <summary>
        /// 插件实例
        /// </summary>
        public IPlugBaseInterface PluginInstance { get; set; }
        /// <summary>
        /// 插件的详细信息
        /// </summary>
        public IPlugInfoInterface PlugInfo { get; set; }
    }
}
