using CacheLib;
using InterfaceLib;
using InterfaceLib.PlugsInterface;
using InterfaceLib.PlugsInterface.AudioInterface;
using InterfaceLib.PlugsInterface.CurrencyInterface;
using InterfaceLib.PlugsInterface.FaceInterface;
using InterfaceLib.ServerInterface;
using LogLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static Assistant.Plugs.ServerInterface;
using static Assistant.ServerLog;

namespace Assistant.Plugs
{
    /// <summary>
    /// 插件管理容器
    /// </summary>
    partial class PluginsManager
    {
        /// <summary>
        /// 运行中的插件容器
        /// </summary>
        internal Dictionary<string, Plugin> PluginsContainer { get; } = new Dictionary<string, Plugin>();
        /// <summary>
        /// 缓存
        /// </summary>
        private Cache Cache = new Cache();
        /// <summary>
        /// 所有插件信息
        /// </summary>
        public IPlugInfoInterface [] PlugInfoInterfaces
        {
            get
            {
                IPlugInfoInterface[] plugInfoInterfaces = new IPlugInfoInterface[PluginsContainer.Count()];
                int idx = 0;
                foreach (var item in PluginsContainer)
                {
                    plugInfoInterfaces[idx++] = item.Value.PlugInfo;
                }
                return plugInfoInterfaces;
            }
        }
        /// <summary>
        /// 获取音频插件列表
        /// </summary>
        public List<IPlugInfoInterface> AudioPlugInfos => updateToMemory(PluginsURL.AudioPlugsSharp, getPlugInfoInterfaces);
        /// <summary>
        /// 通用插件列表
        /// </summary>
        public List<IPlugInfoInterface> CurrencyPlugInfos => updateToMemory(PluginsURL.CurrencyPlugsSharp, getPlugInfoInterfaces);
        /// <summary>
        /// 界面插件
        /// </summary>
        public List<IPlugInfoInterface> FacePlugInfos => updateToMemory(PluginsURL.FacePlugsSharp, getPlugInfoInterfaces);
        #region Run
        /// <summary>
        /// 获取运行中的界面插件
        /// </summary>
        public IFaceInterface RunningFaceInterface => getFirstTInterfaceFromRunningPluginsContainer<IFaceInterface>();
        /// <summary>
        /// 获取运行中的音频插件
        /// </summary>
        public IAudioInterface RunningAudioInterface => getFirstTInterfaceFromRunningPluginsContainer<IAudioInterface>();
        #endregion
        /// <summary>
        /// 插件集合
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public KeyValuePair<string, Plugin> this[int idx]
        {
            get
            {
                int i = 0;
                foreach (var item in PluginsContainer)
                {
                    if(i++ == idx)
                    {
                        return item;
                    }
                }
                throw new Exception("idx的值不存在!");
            }
        }
        /// <summary>
        /// 插件数量
        /// </summary>
        public int Length => PluginsContainer.Count;
        /// <summary>
        /// 插件管理器
        /// </summary>
        public readonly static PluginsManager Manager = new PluginsManager();
        /// <summary>
        /// 单例构造函数
        /// </summary>
        private PluginsManager() { }
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
                plugin.PluginInstance = Activator.CreateInstance(
                    assembly.GetExportedTypes().Where(p => p.GetCustomAttribute<ExportAttribute>() != null).First()
                ) as IPlugBaseInterface;
                if (plugin is null)
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
                Log.Write("加载插件", name, "失败, 原因 --> ", ex.Message, "或者导出插件接口没有添加特性ExportAttribute");
            }
        }
        /// <summary>
        /// 加载插件
        /// </summary>
        /// <param name="info">插件信息</param>
        private Plugin loadCSharpPlugin(PlugInfo info)
        {
            string name = Path.GetFileName(info.LocalURL);
            if (!File.Exists(info.LocalURL))
            {
                Log.Write("加载插件", name, "失败, 原因 --> ", "插件路劲下不存在该插件!");
                return null;
            }
            try
            {
                Assembly assembly = Assembly.LoadFrom(info.LocalURL);
                Plugin plugin = new Plugin();
                plugin.PluginInstance = Activator.CreateInstance(
                    assembly.GetExportedTypes().Where(p=>p.GetCustomAttribute<ExportAttribute>() != null).First()
                ) as IPlugBaseInterface;
                if (plugin is null)
                {
                    Log.Write("加载插件", name, "失败, 原因 --> ", "插件未提供有效的接口!");
                    return null;
                }
                PlugInfo plugInfo = info.Clone();
                plugInfo.Name = plugin.PluginInstance.Name;
                info.Name = plugInfo.Name;
                plugin.PlugInfo = plugInfo;
                return plugin;
            }
            catch (Exception ex)
            {
                Log.Write("加载插件", name, "失败, 原因 --> ", ex.Message, "或者导出插件接口没有添加特性ExportAttribute");
                return null;
            }
        }
        /// <summary>
        /// 获取MD5
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string EncryptWithMD5(string source)
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
                Cache.UpdateValueCache(path, memory, 7200);
            }
            return memory;
        }
        /// <summary>
        /// 从运行的容器中获取第一个T类型的插件
        /// </summary>
        /// <typeparam name="T">插件类型</typeparam>
        /// <returns></returns>
        private T getFirstTInterfaceFromRunningPluginsContainer<T>()
        {
            return (T)PluginsContainer.Values.Where(p => p.PluginInstance is T).First().PluginInstance;
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
                    try
                    {
                        if(Assembly.LoadFrom(item).GetExportedTypes().Where(p=>
                        p.GetCustomAttribute<ExportAttribute>() != null).First() == null)
                        {
                            continue;
                        }
                    }
                    catch (Exception ex)
                    {
                        continue;
                    }

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
                    plugInfo.Id = Guid.NewGuid().ToString() + DateTime.Now.ToString();
                    plugs.Add(plugInfo);
                }
            }
            return plugs;
        }
        /// <summary>
        /// 加载插件
        /// </summary>
        public void LoadPlugins()
        {
            Plugin plugin = null;
            if(AudioPlugInfos.Count == 0)
            {
                Log.Write("缺少音频插件!");
            }
            else
            {
                plugin = loadCSharpPlugin(AudioPlugInfos[0] as PlugInfo);
                PluginsContainer.Add(plugin.PlugInfo.Id, plugin);
            }
            foreach (var item in CurrencyPlugInfos)
            {
                plugin = loadCSharpPlugin(item as PlugInfo);
                PluginsContainer.Add(plugin.PlugInfo.Id, plugin);
            }
            if (FacePlugInfos.Count == 0)
            {
                Log.Write("缺少界面插件!");
            }
            else
            {
                plugin = loadCSharpPlugin(FacePlugInfos[0] as PlugInfo);
                PluginsContainer.Add(plugin.PlugInfo.Id, plugin);
            }
        }
        /// <summary>
        /// 初始化插件
        /// </summary>
        public void InitPlugins()
        {
            foreach (var item in PluginsContainer.Values)
            {
                item.PluginInstance.Start(Server);
                item.PluginInstance.Init();
            }
        }
    }
}
