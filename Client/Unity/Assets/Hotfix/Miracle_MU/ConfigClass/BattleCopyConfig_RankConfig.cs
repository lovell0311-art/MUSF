using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class BattleCopyConfig_RankConfigCategory : ACategory<BattleCopyConfig_RankConfig>
	{
	}

	///<summary>恶魔广场排名 </summary>
	public class BattleCopyConfig_RankConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>排名倍数 </summary>
		public int Multiple;
	}
}
