using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Assistant
{
    class Program
    {
        /// <summary>
        /// C# 插件目录
        /// </summary>
        public static readonly string CSharpPlugs = AppDomain.CurrentDomain.BaseDirectory + "\\C#Plugs\\";

        //public static Dictionary<>


        public static void Main(string[] args)
        {
            ConsoleHelper.hideConsole();
            
            ConsoleHelper.showConsole();
            Console.ReadKey();
        }

        


    }
}
