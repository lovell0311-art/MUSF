using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace ETHotfix
{

	/// <summary>
	/// 技能信息类
	/// </summary>
    public class SkillInfos
    {
		public long Id { get; set; }
		///<summary>名字 </summary>
		public string Name;
		///<summary>介绍 </summary>
		public string Describe;
		///<summary>技能图标 </summary>
		public string Icon;
		///<summary>技能音效 </summary>
		public string SoundName;
		///<summary>动画技能ID </summary>
		public string AnimatorTriggerIndex;
		///<summary>攻击特效 </summary>
		public string AttackEffect;
		///<summary>受击特效 </summary>
		public string HitEffect;
		///<summary>技能类型0无1攻击2辅助 </summary>
		public int skillType;
		///<summary>学习要求 </summary>
		public int LearnStandard;
		///<summary>伤害延迟 </summary>
		public int DamageWait;
		///<summary>最大释放距离 </summary>
		public int Distance;
		///<summary>技能CD </summary>
		public int CoolTime;
		///<summary>持续时间(毫秒) </summary>
		public int PersistentTime;

		///<summary>消耗 </summary>
		public string Consume;
        ///<summary>消耗蓝量 1</summary>
        public int NeedBlue;

        ///<summary>附加数值 </summary>
        public string OtherData;
        ///<summary>基础伤害 1</summary>
        public int BaseDamage;

        ///<summary>使用要求 </summary>
        public string UseStandard;
        ///<summary>等级要求 1</summary>
        public int Lev_CanUser;
        ///<summary>力量要求 2</summary>
        public int Strength;
        ///<summary>智力要求 3</summary>
        public int Intell;
        ///<summary>敏捷要求 4</summary>
        public int Agile;
        ///<summary>统率要求 5</summary>
        public int Command;
        ///<summary>体力要求 6</summary>
        public int PhyStength;

        /* 
		 * 解构函数 
		 把不同的成员变量 拆分其具体的变量值 结构出来
		 一个类中 可以有多个Deconstruct 参数不能相同
		 */
        public void Deconstruct(out string Name,out long Id)
		{
			Name = this.Name;
			Id = this.Id;
		}
	}
}
