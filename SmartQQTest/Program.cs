using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using SmartQQ;

namespace SmartQQTest
{
    class Program
    {
        /// <summary>
        /// 显示图片
        /// </summary>
        /// <param name="filePathName"></param>
        private static Process ShowImage(string filePathName)
        {
            //建立新的系统进程    
            Process process = new Process();

            //设置图片的真实路径和文件名    
            process.StartInfo.FileName = filePathName;

            //设置进程运行参数，这里以最大化窗口方法显示图片。    
            process.StartInfo.Arguments = "rundl132.exe C://WINDOWS//system32//shimgvw.dll,ImageView_Fullscreen";

            //此项为是否使用Shell执行程序，因系统默认为true，此项也可不设，但若设置必须为true    
            process.StartInfo.UseShellExecute = true;

            //此处可以更改进程所打开窗体的显示样式，可以不设    
            process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            process.Start();

            return process;
            //Process process = Process.Start(filePathName, "rundl132.exe C://WINDOWS//system32//shimgvw.dll,ImageView_Fullscreen");
        }
        static void Main(string[] args)
        {
            SmartQQBot smartQQBot = new SmartQQBot();
            smartQQBot.CheckLoginForntInit();
            Thread.Sleep(20);
            smartQQBot.Login();
            Thread.Sleep(20);
            ShowImage(smartQQBot.SaveLoginImagePath);
            while (true)
            {
                Thread.Sleep(5000);
                if (smartQQBot.CheckLogin())
                {
                    break;
                }
            }
            smartQQBot.ForLogion2();
            Thread.Sleep(20);
            smartQQBot.Login2();
            //string value = "qrsig=0kRs0JaQyY*Fmhsq2Zy3PRR2wkA-pdj9YyCRPcnityOFmi5cGoTKS63K7oKkmAnv;Path=/;Domain=ptlogin2.qq.com;";
            //string value = "qrsig=0kRs0JaQyY*Fmhsq2Zy3PRR2wkA-pdj9YyCRPcnityOFmi5cGoTKS63K7oKkmAnv;Path=dddadadasdasd;";

            //Regex regex = new Regex("=(.*?);");
            //string v =  regex.Match(value).Groups[1].Value;
            //Console.WriteLine(v);
            Console.ReadKey();
        }
    }
}
