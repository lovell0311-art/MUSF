using CustomFrameWork.Baseic;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ETModel
{
    public class TeamComponent: TCustomComponent<Player>
    {
        public Player mPlayer
        {
            get
            {
                return Parent;
            }
        }

        public long TeamID;
        public bool IsCaptain;

        public long TimerId = 0;

        /// <summary>
        /// 需要同步的属性
        /// </summary>
        public int LastHPMax = 0;
        public int LastHP = 0;
        public int LastMPMax = 0;
        public int LastMP = 0;
        public int LastLevel = 0;

        public bool IsChanged = false;

        /// <summary>
        /// 缓存，提升性能
        /// </summary>
        public GamePlayer __GamePlayer = null;

        public override void Dispose()
        {
            if (IsDisposeable) return;

            TeamID = 0;
            IsCaptain = false;

            LastHPMax = 0;
            LastHP = 0;
            LastMPMax = 0;
            LastMP = 0;
            LastLevel = 0;

            IsChanged = false;
            __GamePlayer = null;

            base.Dispose();
        }
    }
}
