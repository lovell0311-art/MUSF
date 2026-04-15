using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETHotfix
{
    public enum PetSkillType
    {

        PassiveSkill = 0,
        InitiativeSkill,
        CanLearnPassiveSkill,
        CanLearnInitiativeSkill
    }
    public class PetSkillInfo
    {
        /// <summary>
        /// 宠物技能ID
        /// </summary>
        public long petSkillID = 0;
        /// <summary>
        /// 宠物技能名
        /// </summary>
        public string petSkillName;
        /// <summary>
        /// 宠物资源名
        /// </summary>
        public string petAsset;
        /// <summary>
        /// 宠物精灵
        /// </summary>
        public Sprite sprite;
        /// <summary>
        /// 宠物技能描述
        /// </summary>
        public string petSkillDes;
        /// <summary>
        /// 技能数量
        /// </summary>
        public int petSkillCount;
        /// <summary>
        /// 技能类型 1->主动 2->被动
        /// </summary>
        public int skillType;
        /// <summary>
        /// 是否已学习
        /// </summary>
        public bool learned = false;
        /// <summary>
        /// 是否使用该技能
        /// </summary>
        public bool isUse = false;
        /// <summary>
        /// 魔法值
        /// </summary>
        public float magicValue;
        /// <summary>
        /// 距离
        /// </summary>
        public float distance;
        /// <summary>
        /// 基本技能攻击力
        /// </summary>
        public float skillAttackValue;
        public PetsItem petsItem;
    }
}

