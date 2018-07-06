using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InterfaceLib.PlugsInterface.AudioInterface
{
    /// <summary>
    /// 音频交互接口
    /// </summary>
    public interface IAudioInterface : IPlugBaseInterface
    {
        /// <summary>
        /// 录音的数据
        /// </summary>
        byte[] Datas { get; }
        /// <summary>
        /// 播放音频
        /// </summary>
        /// <param name="audioDatas">数据源</param>
        void Play(byte[] audioDatas);
        /// <summary>
        /// 播放wav文件
        /// </summary>
        /// <param name="wavPath">wav文件路径</param>
        void Play(string wavPath);
        /// <summary>
        /// 录音并返回数据
        /// </summary>
        /// <param name="time">录音的时长(单位S)</param>
        /// <returns>录音的数据</returns>
        byte[] StartBySecondTime(int time);
    }
}
