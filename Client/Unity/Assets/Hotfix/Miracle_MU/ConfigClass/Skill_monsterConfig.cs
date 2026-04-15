using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class Skill_monsterConfigCategory : ACategory<Skill_monsterConfig>
	{
	}

	///<summary>技能-怪物 </summary>
	public class Skill_monsterConfig: IConfig
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
		 ///<summary>技能释放提示 </summary>
		public int TimeLeft;
		 ///<summary>技能提示类型0圆形1矩形2射线3扇形 </summary>
		public int TipsType;
		 ///<summary>技能类型0随机1怪物2玩家 </summary>
		public int skillType;
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
		 ///<summary>附加数值 </summary>
		public string OtherData;
		 ///<summary>使用要求 </summary>
		public string UseStandard;
	}
}
