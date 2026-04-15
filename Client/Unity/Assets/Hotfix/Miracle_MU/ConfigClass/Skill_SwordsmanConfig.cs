using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class Skill_SwordsmanConfigCategory : ACategory<Skill_SwordsmanConfig>
	{
	}

	///<summary>技能-剑士 </summary>
	public class Skill_SwordsmanConfig: IConfig
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
		 ///<summary>技能位置 </summary>
		public int skillType;
		 ///<summary>伤害延迟 </summary>
		public int DamageWait;
		 ///<summary>伤害延迟2 </summary>
		public int DamageWait2;
		 ///<summary>最小动作时间 </summary>
		public int MinActionTime;
		 ///<summary>最大动作时间 </summary>
		public int MaxActionTime;
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
