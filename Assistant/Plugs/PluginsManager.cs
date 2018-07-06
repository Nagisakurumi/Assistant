﻿using CacheLib;
using InterfaceLib.PlugsInterface;
using LogLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Assistant.Plugs
{
    /// <summary>
    /// 插件管理容器
    /// </summary>
    public class PluginsManager
    {
        /// <summary>
        /// 插件容器
        /// </summary>
        private Dictionary<string, Plugin> PluginsContainer { get; } = new Dictionary<string, Plugin>();
        /// <summary>
        /// 缓存
        /// </summary>
        private Cache Cache = new Cache();
        /// <summary>
        /// 获取音频插件列表
        /// </summary>
        public List<IPlugInfoInterface> AudioPlugInfos => updateToMemory(PluginsURL.AudioPlugsSharp, getPlugInfoInterfaces);
        /// <summary>
        /// 加载插件
        /// </summary>
        /// <param name="path"></param>
        private void loadCSharpPlugin(string path)
        {
            string name = Path.GetFileName(path);
            if(!File.Exists(path))
            {
                Log.Write("加载插件", name, "失败, 原因 --> ", "插件路劲下不存在该插件!");
                return;
            }
            try
            {
                Assembly assembly = Assembly.LoadFrom(path);
                Plugin plugin = new Plugin();
                plugin.PluginInstance = Activator.CreateInstance(assembly.GetExportedTypes().Where(p => p.Name.Equals(PluginsURL.PlugInterfaceClassName)).First()) as IPlugBaseInterface;
                if(plugin is null)
                {
                    Log.Write("加载插件", name, "失败, 原因 --> ", "插件未提供有效的接口!");
                    return;
                }
                PlugInfo plugInfo = new PlugInfo();
                FileInfo info = new FileInfo(path);
                plugin.PlugInfo = plugInfo;
                plugInfo.Name = plugin.PluginInstance.Name;
                plugInfo.LocalURL = path;
                plugInfo.RemoteURL = "";
                plugInfo.IsInstall = true;
                plugInfo.Size = info.Length.ToString();
                plugInfo.Version = assembly.ImageRuntimeVersion;
                plugInfo.DateTime = info.LastWriteTime;
                plugInfo.Author = "";

                string id = EncryptWithMD5(name);
                if(!PluginsContainer.ContainsKey(id))
                {
                    PluginsContainer.Add(id, plugin);
                }
                else
                {
                    Log.Write("加载插件", name, "失败, 原因 --> 插件已经加载，重复加载!");
                    return;
                }
            }
            catch (Exception ex)
            {
                Log.Write("加载插件", name, "失败, 原因 --> ", ex.Message);
            }
        }
        /// <summary>
        /// 获取MD5
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private static string EncryptWithMD5(string source)
        {
            byte[] sor = Encoding.UTF8.GetBytes(source);
            MD5 md5 = MD5.Create();
            byte[] result = md5.ComputeHash(sor);
            StringBuilder strbul = new StringBuilder(40);
            for (int i = 0; i < result.Length; i++)
            {
                strbul.Append(result[i].ToString("x2"));//加密结果"x2"结果为32位,"x3"结果为48位,"x4"结果为64位

            }
            return strbul.ToString();
        }
        /// <summary>
        /// 获取或更新缓存的信息
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private T updateToMemory<T>(string path, Func<string, T> func)
        {
            T memory = Cache.GetValueCache<T>(path);
            if(memory == null)
            {
                memory = func(path);
                Cache.UpdateValueCache(path, memory, 3600);
            }
            return memory;
        }
        /// <summary>
        /// 获取本地指定路径下的插件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private List<IPlugInfoInterface> getPlugInfoInterfaces(string path)
        {
            List<IPlugInfoInterface> plugs = new List<IPlugInfoInterface>();
            string[] directorys = Directory.GetDirectories(path);
            foreach (var directorysitem in directorys)
            {
                string[] fileNames = Directory.GetFiles(directorysitem);
                foreach (var item in fileNames)
                {
                    if (!Path.GetFileName(item).Contains(".dll") || !Path.GetFileName(item).Contains(PluginsURL.PlugNameStartContainer)
                        || Path.GetFileName(item).IndexOf(PluginsURL.PlugNameStartContainer) != 0)
                        continue;

                    FileInfo info = new FileInfo(item);
                    PlugInfo plugInfo = new PlugInfo();
                    plugInfo.DateTime = info.LastWriteTime;
                    plugInfo.IsInstall = true;
                    plugInfo.LocalURL = item;
                    plugInfo.Name = Assembly.LoadFrom(item).GetName().Name;
                    plugInfo.Size = info.Length.ToString();
                    plugInfo.Version = Assembly.LoadFrom(item).ImageRuntimeVersion;
                    plugInfo.Author = "";
                    info = null;
                    plugs.Add(plugInfo);
                }
            }
            return plugs;
        }
    }
}