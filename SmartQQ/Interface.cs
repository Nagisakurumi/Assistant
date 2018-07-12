using InterfaceLib;
using InterfaceLib.MsgInterface;
using InterfaceLib.MsgInterface.MsgInfo;
using InterfaceLib.PlugsInterface.CurrencyInterface;
using InterfaceLib.ServerInterface;
using SmartQQ.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static SmartQQ.SmartQQLog;


namespace SmartQQ
{
    [Export]
    public class Interface : ICurrencyInterface
    {

        //private List<>
        /// <summary>
        /// 插件被分配的id
        /// </summary>
        private string id = "";
        /// <summary>
        /// SmartQQ控制对象
        /// </summary>
        private SmartQQBot SmartQQ = new SmartQQBot();
        /// <summary>
        /// 登录线程
        /// </summary>
        private Thread loginThread = null;
        /// <summary>
        /// 服务
        /// </summary>
        public IServerInterface ServerInterface { get; private set; } 
        /// <summary>
        /// 插件的名称
        /// </summary>
        public string Name => "SmartQQ";
        /// <summary>
        /// 插件初始化
        /// </summary>
        /// <returns></returns>
        public bool Init()
        {
            SmartQQ.MessageCallBack += MessageCall;
            loginThread = new Thread(() =>
            {
                SmartQQ.GetLoginParamter();
                Thread.Sleep(20);
                SmartQQ.Login();
                Thread.Sleep(20);

                //ConsoleHelper.ConsoleWriteImage(new Bitmap(smartQQBot.SaveLoginImagePath));
                IMsgInterface messageInterface = new MessageInterface();
                messageInterface.AddMessage(new FileMsgInfo() {
                    Path = SmartQQ.SaveLoginImagePath,
                    ReciverId = "1",
                    SendId = id,
                    ExtendName = "png",
                    Stream = null,
                });
                ServerInterface.SendToMessage(messageInterface);
                while (true)
                {
                    Thread.Sleep(2000);
                    //Log.Write("已经完成二维码的下载，请尽快扫码!");
                    if (SmartQQ.CheckLogin())
                    {
                        break;
                    }
                }
                SmartQQ.ForLogion2();
                SmartQQ.Login2();
                SmartQQ.UpdateFrindsList();
                SmartQQ.UpdateGroup();
                SmartQQ.StartMessageLoop();
                ServerInterface.SendMsgToDispla(id, "SmartQQ,开始进入消息收发循环," +
                    "操作步骤->发送命令:" +
                    "1.给某个特定的用户发送好友消息格式，\"sf\" : {\"uin\", \"message\"} -> {好友的uin, 发送给好友的消息}\n" +
                    "2.发送消息给群格式, \"sg\" : {\"uin\", \"message\"} - > {群的uin, 要发送的消息}\n" +
                    "3.获取好友的详细信息格式, getfriend {\"uin\"} -> {好友的uin}\n" +
                    "4.获取所有好友的uin, getfriendlist {}");
            })
            { IsBackground = true };
            loginThread.Start();

            return true;
        }
        /// <summary>
        /// 日志写回调
        /// </summary>
        /// <param name="obj"></param>
        private void Log_ErroStringEvent(string obj)
        {
            ServerInterface.WriteLog(id, obj);
        }

        /// <summary>
        /// 接受QQ消息
        /// </summary>
        /// <param name="obj"></param>
        private void MessageCall(IMessage message)
        {
            try
            {
                string name = "";
                if (message.MessageType == MessageType.PrivateMessage)
                {
                    Friend friend = (message as PrivateMessage).MessageSource as Friend;
                    if (friend != null)
                    {
                        name = friend.Nickname;
                    }
                    else
                    {
                        name = "null";
                    }
                }
                else if (message.MessageType == MessageType.GroupMessage)
                {
                    SmartQQ.GroupMemberInfo group = (message as GroupMessage).MessageSource as SmartQQ.GroupMemberInfo;
                    if (group != null)
                    {
                        name = group.Nickname;
                    }
                }
                Log.Write(message.MessageType, " : ", message.MessageSource.Id ,  name, "--->", message.Content);
            }
            catch (Exception)
            {
                Log.Write(message.MessageType, message.SenderId, "--->", message.Content);
            }
        }

        /// <summary>
        /// 接受到服务的消息
        /// </summary>
        /// <param name="msgInterface"></param>
        public void ReciverFromServerMsg(IMsgInterface msgInterface)
        {
            foreach (var item in msgInterface.MsgInfos.Values)
            {
                if(item.MessageType == InterfaceLib.MsgInterface.MsgInfo.Enums.MessageType.File)
                {
                    ServerInterface.SendMsgToDispla(id, "SmartQQ插件不支持，非文本信息处理!");
                }
                else
                {
                    dealWithMessage(item as ITextInfo);
                }
            }
        }
        /// <summary>
        /// 启动插件
        /// </summary>
        /// <param name="serverInterface"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Start(IServerInterface serverInterface, string id)
        {
            ServerInterface = serverInterface;
            Log.ErroStringEvent += Log_ErroStringEvent;
            this.id = id;
            return true;
        }
        /// <summary>
        /// 停止插件
        /// </summary>
        /// <returns></returns>
        public bool Stop()
        {
            
            return true;
        }

        /// <summary>
        /// 处理文本信息
        /// </summary>
        /// <param name="textMsgInfo"></param>
        private void dealWithMessage(ITextInfo textMsgInfo)
        {

        }
    }
}
