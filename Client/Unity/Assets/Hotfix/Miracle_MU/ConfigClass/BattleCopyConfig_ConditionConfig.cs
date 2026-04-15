using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class BattleCopyConfig_ConditionConfigCategory : ACategory<BattleCopyConfig_ConditionConfig>
	{
	}

	///<summary>副本条件 </summary>
	public class BattleCopyConfig_ConditionConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>挑战次数 </summary>
		public int Challenge;
		 ///<summary>经验倍数 </summary>
		public int EXPMultiple;
		 ///<summary>分数倍数 </summary>
		public int ScoreRate;
		 ///<summary>经验系数 </summary>
		public int EXPRate;
		 ///<summary>金币系数 </summary>
		public int CoinRate;
	}
}
