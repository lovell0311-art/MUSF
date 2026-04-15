using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class Buff_UnitConfigCategory : ACategory<Buff_UnitConfig>
	{
	}

	///<summary>Buff-特效 </summary>
	public class Buff_UnitConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>名字 </summary>
		public string Name;
		 ///<summary>介绍 </summary>
		public string Describe;
		 ///<summary>技能图标 </summary>
		public string Icon;
		 ///<summary>攻击特效 </summary>
		public string AttackEffect;
		 ///<summary>受击特效 </summary>
		public string HitEffect;
	}
}
