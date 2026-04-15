using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETHotfix
{
	/// <summary>
	/// 动画转换参数
	/// </summary>
	public  class MotionType
	{
		public const string None = "None";
		public const string RoleIndex = "RoleIndex";//角色类型
		
		public const string IsMan = "IsMan";//是否是男性角色
		
		public const string IsWeapon = "IsWeapon";//是否装备单手武器
		
		public const string IsLeftWeapon = "IsLeftWeapon";//是否装备做手武器
		
		public const string IsShield = "IsShield";//是否装备盾牌
	
		public const string IsDoubleWeapon = "IsDoubleWeapon";//是否装备双手武器
	
		public const string IsDoubelCane = "IsDoubelCane";//魔剑士 双手杖
		
		public const string IsNu = "IsNu";//是否装备弩（弓箭手专属）
		
		public const string IsGong = "IsGong";//是否装备弓（弓箭手专属）
		
		public const string IsMove = "IsMove";//移动
		
		public const string IsRun = "IsRun";//奔跑
		public const string Stand = "Stand";//靠墙
		public const string IsSwim = "IsSwim";//游泳
		public const string IsSwimIdle = "IsSwimIdle";//游泳待机
	
		public const string IsWing = "IsWing";//是否有翅膀
	
		public const string IsMount = "IsMount";//是否有坐骑

		public const string IsMountIdel = "IsMountIdel";//坐骑是否是站立的
		
		public const string Dead = "Dead";//是否死亡
	
		public const string Revive = "Revive";//复活
	
		public const string Attack = "Attack";//攻击
		
		public const string Hit = "Hit";//被击
	
		public const string Skill = "Skill";
		public const string DengChang = "DengChang";//Boss登场
	
		public const string BossSkill = "BossSkill";//Boss技能
							  //魔法师

        public const string IsSingleCane = "IsSingleCane";//单手杖

    }
}
