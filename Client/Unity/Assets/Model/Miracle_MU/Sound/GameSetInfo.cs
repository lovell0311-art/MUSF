using ETModel;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETModel
{
    [Serializable]
    public class GameSetInfo
    {
        private bool closeEffect;
        private bool closeSound;
        private bool closeMusic;
        private bool refuseTream;//拒绝组队
        private bool hiderole;//拒绝好友申请
        /// <summary>
        /// 关闭特效
        /// </summary>
        public bool CloseEffect
        {
            get { return closeEffect; }
            set { closeEffect = value; }
        }
        /// <summary>
        /// 关闭音效
        /// </summary>
        public bool CloseSound
        {
            get
            {
                return closeSound;
            }
            set
            {
                closeSound = value;
                SoundComponent.Instance.SetAudioSoundMute(closeSound);
            }
        }
        /// <summary>
        /// 关闭场景音乐
        /// </summary>
        public bool CloseMusic
        {
            get
            {
                return closeMusic;
            }
            set
            {
                closeMusic = value;
                SoundComponent.Instance.SetBgSoundMute(closeMusic);
            }
        }

        public bool RefuseTream
        {
            get { return refuseTream; }
            set { refuseTream = value; }
        }
        public bool HideRole
        {
            get { return hiderole; }
            set { hiderole = value; }
        }


        public GameSetInfo()
        {
            CloseEffect = false;
            CloseSound = false;
            CloseMusic = false;
            RefuseTream = false;
            HideRole = false;
        }
    }
}
