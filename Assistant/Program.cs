using Assistant.Plugs;
using InterfaceLib.PlugsInterface;
using InterfaceLib.PlugsInterface.AudioInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Assistant
{
    class Program
    {
        /// <summary>
        /// 插件管理器
        /// </summary>
        public static PluginsManager PluginsManager = new PluginsManager();



        public static void Main(string[] args)
        {
            //ConsoleHelper.hideConsole();

            //ConsoleHelper.showConsole();
            List<IPlugInfoInterface> audioInterfaces = PluginsManager.AudioPlugInfos;

            foreach (var item in audioInterfaces)
            {
                WriteObjectToJson(item);
            }

            Console.ReadKey();
        }

        /// <summary>
        /// 写出对象的json信息
        /// </summary>
        /// <param name="obj"></param>
        public static void WriteObjectToJson(object obj)
        {
            Console.WriteLine(JsonConvert.SerializeObject(obj).ToString());
        }

    }
}
