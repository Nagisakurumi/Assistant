using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceLib;
using InterfaceLib.MsgInterface;
using InterfaceLib.PlugsInterface;
using InterfaceLib.PlugsInterface.AudioInterface;
using InterfaceLib.ServerInterface;

namespace Audio
{
    /// <summary>
    /// 插件接口
    /// </summary>
    [Export]
    public class Interface : IAudioInterface
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public Interface()
        {
            ///加载资源dll
            //AppDomain.CurrentDomain.AssemblyResolve += LoadResourceDll.AssemblyResolve;
        }
        /// <summary>
        /// 实例
        /// </summary>
        private NAudio audio => NAudio.Instacne;
        /// <summary>
        /// 录音的数据
        /// </summary>
        public byte[] Datas => audio.Datas;
        /// <summary>
        /// 插件的名称
        /// </summary>
        public string Name => "Audio";
        /// <summary>
        /// 频率
        /// </summary>
        public int SimapleRate { get => audio.SimapleRate;
            set => audio.SimapleRate = value;
        }
        /// <summary>
        /// 声道数
        /// </summary>
        public int Channel { get => audio.Channel;
            set => audio.Channel = value;
        }

        /// <summary>
        /// 播放音频
        /// </summary>
        /// <param name="audioDatas">数据源</param>
        public void Play(byte[] audioDatas)
        {
            audio.Play(audioDatas);
        }
        /// <summary>
        /// 播放wav文件
        /// </summary>
        /// <param name="wavPath">wav文件路径</param>
        public void Play(string wavPath)
        {
            audio.Play(wavPath);
        }
        /// <summary>
        /// 录音并返回数据
        /// </summary>
        /// <param name="time">录音的时长(单位S)</param>
        /// <returns>录音的数据</returns>
        public byte[] StartBySecondTime(int time)
        {
            return audio.StartBySecondTime(time).Result;
        }
        /// <summary>
        /// 启动插件
        /// </summary>
        /// <returns></returns>
        public bool Start(IServerInterface serverInterface)
        {
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
        /// 初始化插件
        /// </summary>
        /// <returns></returns>
        public bool Init()
        {
            return true;
        }

        /// <summary>
        /// 接受服务的消息
        /// </summary>
        /// <param name="msgInterface"></param>
        public void ReciverFromServerMsg(IMsgInterface msgInterface)
        {
        }
    }
}
