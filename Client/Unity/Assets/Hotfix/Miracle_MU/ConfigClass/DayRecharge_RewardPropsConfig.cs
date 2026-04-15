using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class DayRecharge_RewardPropsConfigCategory : ACategory<DayRecharge_RewardPropsConfig>
	{
	}

	///<summary>每日充值 </summary>
	public class DayRecharge_RewardPropsConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>活动ID </summary>
		public int ActivityID;
		 ///<summary>充值金额 </summary>
		public int Limit;
		 ///<summary>奖励金币 </summary>
		public int RewardCoins;
		 ///<summary>奖励U币 </summary>
		public int RewardMiracleCoin;
		 ///<summary>奖励道具ID </summary>
		public List<int> ItemID;
		 ///<summary>奖励道具2 </summary>
		public string Item;
	}
}
