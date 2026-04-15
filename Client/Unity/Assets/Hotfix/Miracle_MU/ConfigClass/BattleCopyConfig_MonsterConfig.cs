using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class BattleCopyConfig_MonsterConfigCategory : ACategory<BattleCopyConfig_MonsterConfig>
	{
	}

	///<summary>恶魔广场怪物 </summary>
	public class BattleCopyConfig_MonsterConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>等级 </summary>
		public int Level;
	}
}
