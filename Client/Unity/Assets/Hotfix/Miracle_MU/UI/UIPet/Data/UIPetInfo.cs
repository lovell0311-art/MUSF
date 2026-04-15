using System.Collections.Generic;
using UnityEngine;

namespace ETHotfix
{
    public class PetRecommend
    {
        public Dictionary<string, int> recommendkeyValue;
    }
    public static class PetRecommendData
    {
        public static Dictionary<long,PetRecommend> PetsRecommendList = new Dictionary<long, PetRecommend>();
    }
    /// <summary>
    /// 宠物属性系统
    /// </summary>
    public enum PetAttributeSystem
    {
        /// <summary>
        /// 魔法系
        /// </summary>
        Magic,
        /// <summary>
        /// 物理系
        /// </summary>
        Physics
    }
    public enum AttributeSkill
    {
        Attribute,
        Skill,
        Enhance,
        Advance,
        Null
    }
    public enum PetWarState
    {
        War,
        Rest
    }
    public class PetInfo_Obj
    {
        public GameObject petIcomObj;
        public UIPetInfo uIPetInfo;
    }
    public class UIPetInfo
    {
        /// <summary>
        /// 宠物配置表ID
        /// </summary>
        public int petsConfigID = 0;
        /// <summary>
        /// 宠物ID
        /// </summary>
        public long petId = 0;
        /// <summary>
        /// 宠物名字
        /// </summary>
        public string petName = "宠物名";
        /// <summary>
        /// 宠物类型
        /// </summary>
        public int petsType;//0:陆地，1:飞行
        /// <summary>
        /// 宠物图标资源名
        /// </summary>
        public string petAsset = string.Empty;
        /// <summary>
        /// 宠物图标
        /// </summary>
        public Sprite sprite = null;
        /// <summary>
        /// 宠物等级
        /// </summary>
        public int petLevel = 0;
        /// <summary>
        /// 自由点数
        /// </summary>
        public int petsLVpoint = 0;
        /// <summary>
        /// 休息或者出战
        /// </summary>
        public PetWarState restOrWar = PetWarState.Rest;
        /// <summary>
        /// 被点击
        /// </summary>
        public bool Click = false;
        /// <summary>
        /// 初始技能ID
        /// </summary>
        public int initSkillId = 0;
        /// <summary>
        /// 当前经验
        /// </summary>
        public float curExp = 0f;
        /// <summary>
        /// 当前最高经验
        /// </summary>
        public float curHighestExp = 1f;
        /// <summary>
        /// 当前血量
        /// </summary>
        public float curHp = 0f;
        /// <summary>
        /// 当前最高血量
        /// </summary>
        public float maxHp = 1;
        /// <summary>
        /// 当前蓝量
        /// </summary>
        public float curMp = 0f;
        /// <summary>
        /// 当前最高蓝量
        /// </summary>
        public float maxMp = 1;
        /// <summary>
        /// 正在使用中的技能ID
        /// </summary>
        public int usingPetsSkillID = 0;
        /// <summary>
        /// 是否死亡 0：活着 1：死亡
        /// </summary>
        public int IsDeath = 0;
        /// <summary>
        /// 体验时间
        /// </summary>
        public long petsTrialTime = -1;
        /// <summary>
        /// 死亡等待时间
        /// </summary>
        public long deathTime = 0;
        /// <summary>
        /// 强化等级
        /// </summary>
        public int Elv = 0;
        /// <summary>
        /// 阶数
        /// </summary>
        public int Anvance = 0;
        /// <summary>
        /// 宠物所属系
        /// </summary>
        public PetAttributeSystem petAttributeSystem = PetAttributeSystem.Physics;
        /// <summary>
        /// 宠物属性
        /// </summary>
        public PetAttribute petAttribute = new PetAttribute();
    }
    public class PetAttribute
    {
        /// <summary>
        /// 力量
        /// </summary>
        public int power;
        /// <summary>
        ///最小攻击力
        /// </summary>
        public int attackMinForce;
        /// <summary>
        /// 最大攻击力
        /// </summary>
        public int attackMaxForce;
        /// <summary>
        /// 攻击成功率
        /// </summary>
        public int attackSuccess;
        /// <summary>
        /// PVP攻击率
        /// </summary>
        public int pvpAttack;
        /// <summary>
        /// 敏捷
        /// </summary>
        public int Agile;
        /// <summary>
        /// 防御力
        /// </summary>
        public int defense;
        /// <summary>
        /// 攻击速度
        /// </summary>
        public int attackSpeed;
        /// <summary>
        /// 防御率
        /// </summary>
        public int speedSkating;
        /// <summary>
        /// PVP防御率
        /// </summary>
        public int pvpSpeedSkating;
        /// <summary>
        /// 体力
        /// </summary>
        public int PhysicalStrength;
        /// <summary>
        /// 智力
        /// </summary>
        public int wit;
        /// <summary>
        /// 技能攻击力
        /// </summary>
        public float skillAttackPower = -1;
        /// <summary>
        /// 最小魔力
        /// </summary>
        public float minMagicValue = -1;
        /// <summary>
        /// 最大魔力
        /// </summary>
        public float maxMagicValue = -1;
    }
}
