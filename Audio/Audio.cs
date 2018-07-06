using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oraycn.MCapture;
using System.Threading.Tasks;
using System.Threading;
using Oraycn.MPlayer;
using System.IO;

namespace Audio
{
    /// <summary>
    /// 音频插件
    /// </summary>
    public class Audio
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        private Audio() { }
        /// <summary>
        /// 麦克风采集器
        /// </summary>
        private IMicrophoneCapturer microphoneCapturer = null;
        /// <summary>
        /// 播放组件
        /// </summary>
        private IAudioPlayer audioPlayer = PlayerFactory.CreateAudioPlayer(0, 16000, 1, 16, 3);
        /// <summary>
        /// 实例
        /// </summary>
        private static Audio audio = null;
        /// <summary>
        /// 是否正在采集
        /// </summary>
        public bool IsCapturering { get; private set; }
        /// <summary>
        /// 采集模式
        /// </summary>
        public CaputreMode CaputreMode { get; private set; }
        /// <summary>
        /// 采集时间
        /// </summary>
        private long caputreTime = 0;
        /// <summary>
        /// 接受数据次数
        /// </summary>
        private long reciverTimes = 0;
        public static Audio Instacne => audio != null ? audio : (audio = new Audio());
        /// <summary>
        /// 采集的数据
        /// </summary>
        private List<byte> datas = new List<byte>();
        /// <summary>
        /// 用于线程阻塞公共资源
        /// </summary>
        private AutoResetEvent distributionThreadAre = new AutoResetEvent(false);
        /// <summary>
        /// 数据源
        /// </summary>
        public byte[] Datas => datas.ToArray();
        /// <summary>
        /// 开启采集
        /// </summary>
        public void Start()
        {
            this.microphoneCapturer = CapturerFactory.CreateMicrophoneCapturer(0);
            this.microphoneCapturer.AudioCaptured += new ESBasic.CbGeneric<byte[]>(MicrophoneCapturer_AudioCaptured);
            this.microphoneCapturer.Start();
            datas.Clear();
            IsCapturering = true;
        }

        /// <summary>
        /// 捕获麦克风数据的回调
        /// </summary>
        /// <param name="audioData"></param>
        void MicrophoneCapturer_AudioCaptured(byte[] audioData)
        {
            datas.AddRange(audioData);
            if(CaputreMode == CaputreMode.Time)
            {
                reciverTimes++;
                if(reciverTimes * 20 > caputreTime)
                {
                    distributionThreadAre.Set();
                }
            }
        }
        /// <summary>
        /// 停止采集
        /// </summary>
        public void Stop()
        {
            if (this.microphoneCapturer != null)
            {
                this.microphoneCapturer.Stop();
                this.microphoneCapturer.Dispose();
                this.microphoneCapturer = null;
            }
            IsCapturering = false;
        }
        /// <summary>
        /// 按照时间采集
        /// </summary>
        /// <param name="time">采集的时间秒数</param>
        /// <returns></returns>
        public Task<byte []> StartBySecondTime(int time)
        {
            if (IsCapturering) return null;
            CaputreMode = CaputreMode.Time;
            caputreTime = time * 1000;
            Task<byte[]> task = new Task<byte[]>(() =>
            {
                Start();
                reciverTimes = 0;
                distributionThreadAre.WaitOne();
                Stop();
                return this.datas.ToArray();
            });
            task.Start();
            return task;
        }
        /// <summary>
        /// 播放音频数据
        /// </summary>
        /// <param name="playDatas">数据源</param>
        public void Play(byte [] playDatas)
        {
            audioPlayer.Play(playDatas);
        }
        /// <summary>
        /// 播放wav文件
        /// </summary>
        /// <param name="wavPath"></param>
        public void Play(string wavPath)
        {
            if(!File.Exists(wavPath))
            {
                return;
            }

            AudioInformation info = PlayerFactory.ParseWaveFile(wavPath);
            this.audioPlayer.Play(info.AudioData);
        }
    }

}
