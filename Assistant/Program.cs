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
using System.IO;
using static Assistant.Plugs.PluginsManager;
using static Assistant.ServerLog;
using static Assistant.MessageRoute.MessageRoute;

namespace Assistant
{
    class Program
    {
        /// <summary>
        /// 主函数
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            ///加载插件
            Manager.LoadPlugins();
            Manager.InitPlugins();
            Log.Write("完成插件加载!");
            Log.Write("所有初始化后的插件列表!");
            MessageRouteInfo.Start();
            Log.Write("开启消息路由!");
            foreach (var item in Manager.PluginsContainer.Values)
            {
                WriteObjectToJson(item);
            }

            //ConsoleHelper.hideConsole();
            //using (FileStream fileStream = File.Open("wav.wav", FileMode.Create))
            //{
            //    byte[] datas = plugin.StartBySecondTime(2);
            //    fileStream.Write(datas, 0, datas.Length);
            //}
            //plugin.Play("wav.wav");
            //plugin.Play(plugin.StartBySecondTime(8));
            //using (FileStream stream = File.Open(@"C:\Users\78633\Desktop\baiduai\test.wav", FileMode.Open))
            //{
            //    byte[] datas = new byte[stream.Length];
            //    stream.Read(datas, 0, datas.Length);
            //    plugin.Play(datas);

            //}
            //plugin.Play(@"C:\Users\78633\Desktop\baiduai\test.wav");
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

        public static void WriteLog(string content)
        {
            Console.WriteLine(content);
        }
    }
}
