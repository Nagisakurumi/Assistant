using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assistant.Plugs
{
    /// <summary>
    /// 插件加载默认路径
    /// </summary>
    public static class PluginsURL
    {
        /// <summary>
        /// 界面id
        /// </summary>
        public static string FaceInterfaceId = "1";
        /// <summary>
        /// 服务的id
        /// </summary>
        public static string ServerInterfaceId = "0";
        /// <summary>
        /// 所有插件公开交互类的类型名称
        /// </summary>
        public static string PlugInterfaceClassName => "Interface";
        /// <summary>
        /// 音频交互的插件路径
        /// </summary>
        public static string AudioPlugsSharp => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Plugs/Audio/CSharp/");
        /// <summary>
        /// 通常插件路径
        /// </summary>
        public static string CurrencyPlugsSharp => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Plugs/Currency/CSharp/");
        /// <summary>
        /// 界面插件
        /// </summary>
        public static string FacePlugsSharp => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Plugs/Face/CSharp/");
    }
}
