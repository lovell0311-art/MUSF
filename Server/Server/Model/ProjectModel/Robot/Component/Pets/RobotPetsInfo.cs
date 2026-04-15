using System;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork;

namespace ETModel.Robot
{
    public class RobotPetsInfo : Entity
    {
        public Pets_InfoConfig Config;
        public long PetsTrialTime;
        private bool isDeath;
        public bool IsDeath
        {
            set { isDeath = value; }
            get
            {
                if (isDeath == false) return false;
                if ((UpdateTimestamp + deathTime) >= Help_TimeHelper.GetNow()) return false;
                return true;
            }
        }
        private long deathTime;

        public long DeathTime
        {
            get { return deathTime; }
            set 
            { 
                deathTime = value;
                UpdateTimestamp = Help_TimeHelper.GetNow();
            }
        }
        public bool IsToWar;
        /// <summary>
        /// 数据更新时间
        /// </summary>
        public long UpdateTimestamp;
    }
}
