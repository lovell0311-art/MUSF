using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class Pets_SkillConfigCategory : ACategory<Pets_SkillConfig>
	{
	}

	///<summary>宠物技能 </summary>
	public class Pets_SkillConfig: IConfig
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
		 ///<summary>技能目标 </summary>
		public int SkillTarget;
		 ///<summary>技能类型0无1主动2被动 </summary>
		public int skillType;
		 ///<summary>伤害延迟(毫秒) </summary>
		public int DamageWait;
		 ///<summary>最大释放距离 </summary>
		public int Distance;
		 ///<summary>技能CD（毫秒） </summary>
		public int CoolTime;
		 ///<summary>持续时间(毫秒) </summary>
		public int PersistentTime;
		 ///<summary>技能间隔（毫秒） </summary>
		public int SkillInterval;
		 ///<summary>消耗 </summary>
		public string Consume;
		 ///<summary>附加数值 </summary>
		public string OtherData;
		 ///<summary>使用要求 </summary>
		public string UseStandard;
	}
}
