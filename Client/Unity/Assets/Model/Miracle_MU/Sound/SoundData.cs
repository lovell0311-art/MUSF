using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace ETModel
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundData : MonoBehaviour
    {
        //音频源控件
        public  AudioSource AudioSource;

        /// <summary>
        /// 是否循环播放
        /// </summary>
        [HideInInspector]
        public bool isLoop = false;

        /// <summary>
        /// 音量
        /// </summary>
        [HideInInspector]
        public float volume = 0.5f;

        /// <summary>
        /// 延迟
        /// </summary>
        public int delay = 3;
        /// <summary>
        /// 间隔时间
        /// </summary>
        public int nextTime = 10;

        public void Awake()
        {
            this.AudioSource = this.gameObject.GetComponent<AudioSource>();
            nextTime = RandomHelper.RandomNumber(10,30);
            InvokeRepeating(nameof(PlayAudio),delay,nextTime);
        }

        public void PlayAudio() 
        {
            AudioSource.Play();
        }
        public AudioSource GetAudio()
        {
            return AudioSource;
        }

        public bool IsPlaying
        {
            get
            {
                return AudioSource != null && AudioSource.isPlaying;
            }
        }
        public bool IsPause
        {
            get;
            set;
        }
        /// <summary>
        /// 是否循环播放
        /// </summary>
        public bool IsLooP 
        {
            get { return AudioSource.loop; }
            set { AudioSource.loop = value; }
        }
         /// <summary>
         /// 是否静音
         /// </summary>
        public bool Mute
        {
            get { return AudioSource.mute; }
            set 
            {
                if (AudioSource == null)
                {
                    AudioSource = this.gameObject.GetComponent<AudioSource>();
                   
                   
                   
                }
                AudioSource.mute = value;
            }
        }
        /// <summary>
        /// 设置音乐音量
        /// </summary>
        public float Volume
        {
            get { return AudioSource.volume; }
            set {
                volume = value;
                AudioSource.volume = volume; }
        }

        private void Reset()
        {
            this.AudioSource = this.gameObject.GetComponent<AudioSource>();
        }
    }

   
}