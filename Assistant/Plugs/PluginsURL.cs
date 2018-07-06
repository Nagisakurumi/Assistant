﻿using System;
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
        /// 所有插件的名称起始
        /// </summary>
        public static string PlugNameStartContainer => "Interface_";
        /// <summary>
        /// 所有插件公开交互类的类型名称
        /// </summary>
        public static string PlugInterfaceClassName => "Interface";
        /// <summary>
        /// 音频交互的插件路径
        /// </summary>
        public static string AudioPlugsSharp => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Plugs/Audio/CSharp/");
    }
}
