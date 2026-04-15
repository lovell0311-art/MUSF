using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ETHotfix
{
    /// <summary>
    /// 挂机设置 工具类
    /// </summary>
    public static class OnHookSetInfoTools
    {
        private static int range;//攻击范围
        private static bool isAuto_30;//自动治疗
        private static bool isAuto_50;
        private static bool isAuto_80;
        private static bool isUseAttack;//使用普攻
        private static bool isTargetMonster;//使用普攻
        private static bool isUseSkill;//使用技能
        private static bool isReturnOrigin;//返回原点
        private static bool isOriginOnHook;//原点挂机


        private static bool isPickUpAllEquip;//拾取所有装备道具
        private static bool isPickUpSuitEquip;//拾取套装装备
        private static bool isPickUpGems;//拾取宝石 原石
        private static bool isPickUpMountMat;//拾取坐骑材料
        private static bool isPickUpYuMao;//拾取羽毛
        private static bool isPickUpLuck;//拾取幸运装备
        private static bool isPickUpSkill;//拾取技能装备
        private static bool isDiyPicUp;//是否开启自定义拾取
        private static string diyPickUpName;//自定义 拾取的物品名

        private static bool isAutoAccpet;//自动接受好友组队
        private static bool isAutoAccpetTeam;//自动接受战队组队

        private static bool isAutoBufferCd_10;//BufferCd
        private static bool isAutoBufferCd_20;
        private static bool isAutoBufferCd_30;

        static readonly char[] PickUpNameChars = new char[] { ',', '.', '，', '。', ' ', ' ', '、', '`', ';', ':', '；' };

        public static Action ShowEffectAction;
        public static Action HideEffectAction;

        public static int Range { get => range; set => range = value; }
        public static bool IsAuto_30 
        { 
            get => isAuto_30;
            set 
            { 
                isAuto_30 = value;
            } 
        }
        public static bool IsAuto_50 { get => isAuto_50; set => isAuto_50 = value; }
        public static bool IsAuto_80 { get => isAuto_80; set => isAuto_80 = value; }
        public static bool IsUseAttack { get => isUseAttack; set => isUseAttack = value; }
        public static bool IsTargetMonster { get => isTargetMonster; set => isTargetMonster = value; }
        public static bool IsUseSkill { get => isUseSkill; set => isUseSkill = value; }
        public static bool IsReturnOrigin 
        { 
            get => isReturnOrigin;
            set 
            { 
                isReturnOrigin = value;
                
                if (value)
                {
                    ShowEffectAction?.Invoke();
                }
                else 
                {
                    HideEffectAction?.Invoke();
                }
            }
        }
        public static bool IsPickUpAllEquip { get => isPickUpAllEquip; set => isPickUpAllEquip = value; }
        public static bool IsPickUpSuitEquip { get => isPickUpSuitEquip; set => isPickUpSuitEquip = value; }
        public static bool IsPickUpGems
        {
            get => isPickUpGems;
            set
            { 
                isPickUpGems = value;
            }
        }
        public static bool IsPickUpMountMat { get => isPickUpMountMat; set => isPickUpMountMat = value; }
        public static bool IsPickUpYuMao { get => isPickUpYuMao; set => isPickUpYuMao = value; }
        public static bool IsPickUpLuck { get => isPickUpLuck; set => isPickUpLuck = value; }
        public static bool IsPickUpSkill { get => isPickUpSkill; set => isPickUpSkill = value; }
        public static string DiyPickUpName
        {
            get => diyPickUpName;
            set
            {
                DIYPickList.Clear();
                diyPickUpName = value;
                var list = value.Split(PickUpNameChars, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0, length = list.Length; i < length; i++)
                {
                    if (string.IsNullOrEmpty(list[i])) continue;
                    DIYPickList.Add(list[i]);
                }
            }

        }
        public static bool IsDiyPicUp { get => isDiyPicUp; set => isDiyPicUp = value; }
        public static bool IsAutoAccpet { get => isAutoAccpet; set => isAutoAccpet = value; }
        public static bool IsAutoAccpetTeam { get => isAutoAccpetTeam; set => isAutoAccpetTeam = value; }
        public static bool IsAutoBufferCd_10 { get => isAutoBufferCd_10; set => isAutoBufferCd_10 = value; }
        public static bool IsAutoBufferCd_20 { get => isAutoBufferCd_20; set => isAutoBufferCd_20 = value; }
        public static bool IsAutoBufferCd_30 { get => isAutoBufferCd_30; set => isAutoBufferCd_30 = value; }
        public static bool IsOriginOnHook { get => isOriginOnHook; set => isOriginOnHook = value; }

        public static List<string> DIYPickList = new List<string>();


        private static OnHookSetInfoData infoData;
        static OnHookSetInfoTools()
        {
            infoData = LocalDataJsonComponent.Instance.LoadData<OnHookSetInfoData>(LocalJsonDataKeys.OnHookConfigInfo) ?? new OnHookSetInfoData();
            Range = infoData.range;
            IsAuto_30 = infoData.IsAuto_30;
            IsAuto_50 = infoData.IsAuto_50;
            IsAuto_80 = infoData.IsAuto_80;
            IsUseAttack = infoData.IsUseAttack;
            IsTargetMonster = infoData.IsTargetMonster;
            IsUseSkill = infoData.IsUseSkill;
            IsReturnOrigin = infoData.IsReturnOrigin;
            IsOriginOnHook = infoData.IsOriginOnHook;
            IsPickUpAllEquip = infoData.IsPickUpAllEquip;
            IsPickUpSuitEquip = infoData.IsPickUpSuitEquip;
            IsPickUpGems = infoData.IsPickUpGems;
            IsPickUpMountMat = infoData.IsPickUpMountMat;
            IsPickUpYuMao = infoData.IsPickUpYuMao;
            IsPickUpLuck = infoData.IsPickUpLuck;
            IsPickUpSkill = infoData.IsPickUpSkill;
            DiyPickUpName = infoData.DiyPickUpName;
            IsDiyPicUp = infoData.IsDiyPicUp;
            IsAutoAccpet = infoData.IsAutoAccpet;
            IsAutoAccpetTeam = infoData.IsAutoAccpetTeam;
            IsAutoBufferCd_10 = infoData.IsAutoBufferCd_10;
            IsAutoBufferCd_20 = infoData.IsAutoBufferCd_20;
            IsAutoBufferCd_30 = infoData.IsAutoBufferCd_30;

        }

        public static void Init() { }

        public static void Save()
        {
            infoData.range = range;
            infoData.IsAuto_30 = isAuto_30;
            infoData.IsAuto_50 = isAuto_50;
            infoData.IsAuto_80 = isAuto_80;
            infoData.IsUseAttack = isUseAttack;
            infoData.IsTargetMonster = isTargetMonster;
            infoData.IsUseSkill = isUseSkill;
            infoData.IsReturnOrigin = isReturnOrigin;
            infoData.IsOriginOnHook = isOriginOnHook;
            infoData.IsPickUpAllEquip = IsPickUpAllEquip;
            infoData.IsDiyPicUp = isDiyPicUp;
            infoData.IsPickUpSuitEquip = IsPickUpSuitEquip;
            infoData.IsPickUpLuck = isPickUpLuck;
            infoData.IsPickUpSkill = isPickUpSkill;
            infoData.IsPickUpGems = isPickUpGems;
            infoData.IsPickUpYuMao = isPickUpYuMao;
            infoData.DiyPickUpName = diyPickUpName;
            infoData.IsPickUpMountMat = isPickUpMountMat;
            infoData.IsAutoAccpet = isAutoAccpet;
            infoData.IsAutoAccpetTeam = isAutoAccpetTeam;
            LocalDataJsonComponent.Instance.SavaData(infoData, LocalJsonDataKeys.OnHookConfigInfo);
        }
    }
}
