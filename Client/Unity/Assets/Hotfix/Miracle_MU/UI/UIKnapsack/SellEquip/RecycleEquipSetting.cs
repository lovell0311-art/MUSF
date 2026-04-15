using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETHotfix
{
    /// <summary>
    /// 装备回收设置
    /// </summary>
    [Serializable]
    public class RecycleEquipSetting
    {
        public bool SingleExcellence;//单词条卓越
        public bool WhiteSuit;//白装
        public bool DoubleExcellence;//双词条卓越
        public bool SkillBook;//技能书
        public bool IntensifyEquip;//强化装备
        public bool Red_BlueMedicine;//小瓶红、蓝药水
        public bool AutoRecycle;//自动回收
        public bool IsLucky;//幸运装备
        public bool SkillId;//技能
        public RecycleEquipSetting()
        {
            SingleExcellence = false;
            WhiteSuit = true;
            DoubleExcellence = false;
            SkillBook = false;
            IntensifyEquip = false;
            Red_BlueMedicine = false;
            AutoRecycle = false;
            IsLucky = false;
            SkillId = false;
        }
    }

}