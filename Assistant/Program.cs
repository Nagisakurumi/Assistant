﻿using Assistant.Plugs;
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

namespace Assistant
{
    class Program
    {
        /// <summary>
        /// 插件管理器
        /// </summary>
        public static PluginsManager PluginsManager = new PluginsManager();


        /// <summary>
        /// 主函数
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            List<IPlugInfoInterface> audioInterfaces = PluginsManager.AudioPlugInfos;
            Console.WriteLine("插件列表!");
            foreach (var item in audioInterfaces)
            {
                WriteObjectToJson(item);
            }
            ///加载插件
            PluginsManager.LoadPlugins();
            IAudioInterface plugin = PluginsManager[0].Value.PluginInstance as IAudioInterface;
            plugin.LogWrite += WriteLog;
            plugin.Init();
            Console.Write("完成插件" + plugin.Name + "加载!");
            //using (FileStream fileStream = File.Open("wav.wav", FileMode.Create))
            //{
            //    byte[] datas = plugin.StartBySecondTime(2);
            //    fileStream.Write(datas, 0, datas.Length);
            //}
            //plugin.Play("wav.wav");
            //plugin.Play(plugin.StartBySecondTime(2));
            //using (FileStream stream = File.Open(@"C:\Users\78633\Desktop\baiduai\test.wav", FileMode.Open))
            //{
            //    byte[] datas = new byte[stream.Length];
            //    stream.Read(datas, 0, datas.Length);
            //    plugin.Play(datas);
                
            //}
            plugin.Play(@"C:\Users\78633\Desktop\baiduai\test.wav");
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
