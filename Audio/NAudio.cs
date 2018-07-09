using NAudio.Wave;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Audio
{
    /// <summary>
    /// 音频播放
    /// </summary>
    public class NAudio
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        private NAudio()
        {
            CaputreMode = CaputreMode.Time;
        }
        /// <summary>
        /// 麦克风采集器
        /// </summary>
        private WaveInEvent waveIn = null;
        /// <summary>
        /// 频率
        /// </summary>
        public int SimapleRate { get; set; } = 16000;
        /// <summary>
        /// 声道数
        /// </summary>
        public int Channel { get; set; } = 1;
        //public static string FilePath = 
        /// <summary>
        /// 播放组件
        /// </summary>
        private IWavePlayer audioPlayer = new WaveOut();
        /// <summary>
        /// 实例
        /// </summary>
        private static NAudio audio = null;
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
        public static NAudio Instacne => audio != null ? audio : (audio = new NAudio());
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
            waveIn = new WaveInEvent() { WaveFormat = new WaveFormat(SimapleRate, Channel) };
            waveIn.DataAvailable += waveIn_DataAvailable;
            waveIn.RecordingStopped += OnRecordingStopped;
            waveIn.StartRecording();
            datas.Clear();
            IsCapturering = true;
        }

        /// <summary>
        /// 录音中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void waveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            datas.AddRange(e.Buffer);
            int secondsRecorded = (int)(datas.Count / waveIn.WaveFormat.AverageBytesPerSecond);//录音时间获取
            if (secondsRecorded >= caputreTime && CaputreMode == CaputreMode.Time)
            {
                Stop();
            }
        }
        /// <summary>
        /// 停止录音
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnRecordingStopped(object sender, StoppedEventArgs e)
        {
            if (waveIn != null) // 关闭录音对象
            {
                waveIn.Dispose();
                waveIn = null;
            }
            distributionThreadAre.Set();
        }
        /// <summary>
        /// 停止录音
        /// </summary>
        private void Stop()
        {
            waveIn.StopRecording();
            waveIn.Dispose();
            IsCapturering = false;
        }
        /// <summary>
        /// 按照时间采集
        /// </summary>
        /// <param name="time">采集的时间秒数</param>
        /// <returns></returns>
        public Task<byte[]> StartBySecondTime(int time)
        {
            if (IsCapturering) return null;
            CaputreMode = CaputreMode.Time;
            caputreTime = time;
            Task<byte[]> task = new Task<byte[]>(() =>
            {
                Start();
                distributionThreadAre.WaitOne();
                return this.datas.ToArray();
            });
            task.Start();
            return task;
        }
        /// <summary>
        /// 播放音频数据
        /// </summary>
        /// <param name="playDatas">数据源</param>
        public void Play(byte[] playDatas)
        {
            BufferedWaveProvider bufferedWaveProvider = new BufferedWaveProvider(new WaveFormat(SimapleRate, Channel));
            bufferedWaveProvider.BufferLength = playDatas.Length;
            bufferedWaveProvider.AddSamples(playDatas, 0, playDatas.Length);
            audioPlayer.Init(bufferedWaveProvider);
            audioPlayer.Play();
            //bufferedWaveProvider.ClearBuffer();
            //bufferedWaveProvider = null;
        }
        /// <summary>
        /// 播放wav文件
        /// </summary>
        /// <param name="wavPath"></param>
        public void Play(string wavPath)
        {
            if (!File.Exists(wavPath))
            {
                return;
            }
            AudioFileReader audioFileReader = new AudioFileReader(wavPath);
            audioPlayer.Init(audioFileReader);
            this.audioPlayer.Play();
            //audioFileReader.Dispose();
            //audioFileReader = null;
        }
    }
}