using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace Audio
{
    /// <summary> 载入资源中的动态链接库(dll)文件
    /// </summary>
    internal static class LoadResourceDll
    {

        /// <summary>
        /// 所有依赖的dll集合
        /// </summary>
        private static Dictionary<string, Assembly> refAssembly = new Dictionary<string, Assembly>();
        /// <summary>
        /// 注册资源中的dll
        /// </summary>
        public static void RegistDLL()
        {
            //获取调用者的程序集
            Assembly assembly = new StackTrace(0).GetFrame(1).GetMethod().Module.Assembly;
            System.Resources.ResourceManager resourceManager = new System.Resources.ResourceManager(
                assembly.GetName().Name + ".Properties.Resources", assembly);
            AssemblyName [] refAssemblies = assembly.GetReferencedAssemblies();
            foreach (var item in refAssemblies)
            {
                if (!refAssembly.ContainsKey(item.Name))
                {
                    object ass = resourceManager.GetObject(item.Name.Replace('.', '_'));
                    if (ass != null)
                    {
                        //(ass as Assembly).load
                        refAssembly.Add(item.Name, Assembly.Load(ass as byte[]));
                    }
                }
            }
        }
        /// <summary>
        /// dll引用异常回调
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            string name = args.Name.Split(',')[0];
            if (refAssembly.ContainsKey(name))
            {
                Assembly assembly = refAssembly[name];
                refAssembly.Remove(name);
                name = null;
                return assembly;
            }
            else
            {
                throw new Exception(args.Name + "在资源文件中找不到!");
            }
        }
    }
}