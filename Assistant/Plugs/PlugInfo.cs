using InterfaceLib.PlugsInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assistant.Plugs
{
    /// <summary>
    /// 插件的详细信息
    /// </summary>
    public class PlugInfo : IPlugInfoInterface
    {
        public bool IsInstall { get; set; }

        public string RemoteURL { get; set; }

        public string LocalURL { get; set; }

        public string Name { get; set; }

        public string Author { get; set; }

        public string Size { get; set; }

        public DateTime DateTime { get; set; }

        public string Version { get; set; }
        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public PlugInfo Clone()
        {
            return new PlugInfo()
            {
                IsInstall = this.IsInstall,
                Author = this.Author,
                DateTime = this.DateTime,
                LocalURL = this.LocalURL,
                Name = this.Name,
                RemoteURL = this.RemoteURL,
                Size = this.Size,
                Version = this.Version,
            };
        }
    }
}
