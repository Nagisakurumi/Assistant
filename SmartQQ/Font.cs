﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SmartQQ.SmartQQLog;

namespace SmartQQ
{
    /// <summary>
    ///     字体。
    /// </summary>
    public class Font
    {
        /// <summary>
        ///     默认字体。
        /// </summary>
        public static Font DefaultFont = new Font { Style = new[] { 0, 0, 0 }, Color = "000000", Name = "微软雅黑", Size = 10 };

        /// <summary>
        ///     样式。
        /// </summary>
        [JsonProperty("style")]
        public int[] Style { get; set; }

        /// <summary>
        ///     颜色。
        /// </summary>
        [JsonProperty("color")]
        public string Color { get; set; }

        /// <summary>
        ///     名称。
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        ///     字号。
        /// </summary>
        [JsonProperty("size")]
        public int Size { get; set; }
    }
}
