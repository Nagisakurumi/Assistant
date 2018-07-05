using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using SmartQQ;
using System.Drawing;
using SmartQQ.Message;
using LogLib;

namespace SmartQQTest
{
    class Program
    {
        static SmartQQBot smartQQBot = new SmartQQBot();
        static void Main(string[] args)
        {
            LogLib.Log.ErroStringEvent += Log_ErroStringEvent;
            
            smartQQBot.GetLoginParamter();
            Thread.Sleep(20);
            smartQQBot.Login();
            Thread.Sleep(20);

            ConsoleHelper.ConsoleWriteImage(new Bitmap(smartQQBot.SaveLoginImagePath));
            Log.Write("已经完成二维码的下载，请尽快扫码!");
            while (true)
            {
                Thread.Sleep(2000);
                if (smartQQBot.CheckLogin())
                {
                    break;
                }
            }
            smartQQBot.ForLogion2();
            smartQQBot.Login2();
            smartQQBot.UpdateFrindsList();
            smartQQBot.UpdateGroup();
            smartQQBot.MessageCallBack += MessageCallBack;
            smartQQBot.StartMessageLoop();

            Console.ReadKey();
        }
        /// <summary>
        /// 日志回写
        /// </summary>
        /// <param name="erro"></param>
        private static void Log_ErroStringEvent(string erro)
        {
            Console.WriteLine(erro);
        }
        /// <summary>
        /// 消息获取回调
        /// </summary>
        /// <param name="message"></param>
        private static void MessageCallBack(IMessage message)
        {
            FriendInfo friend = smartQQBot[message.SenderId];
            Log.Write(message.MessageType, friend.Nickname,  "--->",  message.Content);
        }
    }
}
