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
    }
}
