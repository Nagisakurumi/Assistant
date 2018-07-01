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


        [STAThread]
        public static void Main(string[] args)
        {
            ConsoleHelper.hideConsole();
            
            ConsoleHelper.showConsole();
            Console.ReadKey();
        }

        


    }
}
