
using System.Collections.Generic;

using System;

using ETModel;

namespace ETHotfix
{

    /// <summary>
    /// 账号配置
    /// </summary>
    [System.Serializable]
    public class AccountConfigInfo
    {
        public string account;//账号
        public string password;//密码
        public bool IsRead;//是否阅读并允许相关许可协议

        //JsonUtlity对自定义类不要求有无参构造，LitJson需要
        //JsonUtlity不支持字典,LitJson支持(但是键只能是字符串)

        public AccountConfigInfo()
        {
            account = string.Empty;//账号
            password = string.Empty;//密码
            IsRead = false;//是否阅读并允许相关许可协议
        }
        public static AccountConfigInfo GetConfig()
        {
            AccountConfigInfo accountConfigInfo = LocalDataJsonComponent.Instance.LoadData<AccountConfigInfo>(LocalJsonDataKeys.UserAccount) ?? new AccountConfigInfo();
            return accountConfigInfo;
        }
    }
    /// <summary>
    /// 上次 登录的服务器信息
    /// </summary>
    [Serializable]
    public class LastLineInfo
    {
        public string lineName ="主宰二区1线";//上次登录的线路名
        public int linestate = 1;//上次登录时 服务器的状态\
        public int ZoneId =2;//大区ID
        public int LineId =1;//线路Id
        public static LastLineInfo GetLastLineInfo()
        {
            return LocalDataJsonComponent.Instance.LoadData<LastLineInfo>(LocalJsonDataKeys.LastLineInfo);
        }
    }
    /// <summary>
    /// 上一次使用的角色
    /// </summary>
    [Serializable]
    public class LastRoelInfo 
    {
        public long LastRoleUUID;
    }
    /// <summary>
    /// 技能选择配置
    /// </summary
    [Serializable]
    public class Skillconfiguration
    {
        public Dictionary<string, int> SKilDic;//ListJson 字典虽然支持 但是键在使用时需要使用字符串类型
        public Skillconfiguration()//类结构 需要无参构造函数 否则反序列化时报错
        {
            SKilDic = new Dictionary<string, int>();
        }
    }

    /// <summary>
    /// 挂机配置
    /// </summary>
    [Serializable]
    public class OnHookSetInfoData
    {
        public int range;//攻击范围
        public bool IsAuto_30;//自动治疗
        public bool IsAuto_50;
        public bool IsAuto_80;
        public bool IsUseAttack;//使用普攻
        public bool IsTargetMonster;//使用普攻
        public bool IsUseSkill;//使用技能
        public bool IsReturnOrigin;//返回原点
        public bool IsOriginOnHook;//原点挂机

      
        public bool IsPickUpAllEquip;//拾取所有装备道具
        public bool IsPickUpSuitEquip;//拾取套装装备
        public bool IsPickUpGems;//拾取宝石 原石
        public bool IsPickUpMountMat;//拾取坐骑材料
        public bool IsPickUpYuMao;//拾取羽毛
        public bool IsPickUpLuck;//拾取幸运装备
        public bool IsPickUpSkill;//拾取技能装备
        public bool IsDiyPicUp;//是否开启自定义拾取
        public string DiyPickUpName;//自定义 拾取的物品名

        public bool IsAutoAccpet;//自动接受好友组队
        public bool IsAutoAccpetTeam;//自动接受战队组队

        public bool IsAutoBufferCd_10;//BufferCd
        public bool IsAutoBufferCd_20;
        public bool IsAutoBufferCd_30;

        public OnHookSetInfoData()
        {
            range = 5;
            IsAuto_30 = false;
            IsAuto_50 = true;
            IsAuto_80 = false;

            IsAutoBufferCd_10 = false;
            IsAutoBufferCd_20 = false;
            IsAutoBufferCd_30 = true;

            IsTargetMonster = false;
            IsUseAttack = true;
            IsUseSkill = true;
            IsReturnOrigin = true;
            IsOriginOnHook = false;

            IsDiyPicUp = false;
            IsPickUpAllEquip = true;
            IsPickUpSuitEquip = false;
            IsPickUpLuck = false;
            IsPickUpSkill = false;
            IsPickUpGems = false;
            IsPickUpYuMao = false;
            DiyPickUpName = string.Empty;

            IsAutoAccpet = false;
            IsAutoAccpetTeam = false;
           


        }
    }
    /// <summary>
    /// 摄像机的的信息
    /// </summary>
    [Serializable]
    public class CameraInfo
    {
        public float curAngleH;
        public float curAngleV;
        public float distance;

        public CameraInfo()
        {
            curAngleH = -135f;//Y
            curAngleV = 45f;//X
            distance = 35;
        }
    }

    /// <summary>
    /// 红点信息
    /// </summary>
    public class RedDotInfo
    {
        public string redDotName;
        public int redDotCount;
    }

    /// <summary>
    /// 摊位信息
    /// </summary>
    public class StallUpInfo 
    {
        public string StallName;
    }

    [Serializable]
    public class LuguageInfo
    {
        public int index;
        public LuguageInfo()
        {
          index = 0;
        }
        public static LuguageInfo GetLuguageInfo()
        {
            return LocalDataJsonComponent.Instance.LoadData<LuguageInfo>(LocalJsonDataKeys.LuguageInfo);
        }
    }

}
