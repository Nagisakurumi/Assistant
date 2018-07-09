using InterfaceLib.MsgInterface.MsgInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Assistant.MessageRoute
{
    /// <summary>
    /// 消息路由处理
    /// </summary>
    public class MessageRoute
    {
        /// <summary>
        /// 消息队列
        /// </summary>
        private Queue<IInfoBase> infoBases = new Queue<IInfoBase>();
        /// <summary>
        /// 消息路由
        /// </summary>
        public static MessageRoute MessageRouteInfo = new MessageRoute();
        /// <summary>
        /// 消息路由线程
        /// </summary>
        private Thread messageRouteThread = null;
        /// <summary>
        /// 用于线程阻塞公共资源
        /// </summary>
        private AutoResetEvent distributionThreadAre = new AutoResetEvent(false);
        /// <summary>
        /// 消息处理线程是否处理活跃状态
        /// </summary>
        private bool isActive { get; set; } = false;
        /// <summary>
        /// 单例模式
        /// </summary>
        private MessageRoute() { }

        /// <summary>
        /// 继续任务
        /// </summary>
        private void continue_task()
        {
            isActive = true;
            distributionThreadAre.Set();
        }
        /// <summary>
        /// 暂停任务
        /// </summary>
        private void pause_task()
        {
            isActive = false;
            distributionThreadAre.WaitOne();
        }

        /// <summary>
        /// 添加一条待处理的消息
        /// </summary>
        /// <param name="infoBase">消息</param>
        public void AddMessage(IInfoBase infoBase)
        {
            infoBases.Enqueue(infoBase);
            if(isActive == false)
            {
                continue_task();
            }
        }
    }
}
