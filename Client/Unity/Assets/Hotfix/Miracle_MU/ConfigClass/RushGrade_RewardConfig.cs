using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class RushGrade_RewardConfigCategory : ACategory<RushGrade_RewardConfig>
	{
	}

	///<summary>冲级奖励 </summary>
	public class RushGrade_RewardConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>领取限制 </summary>
		public int Limit;
		 ///<summary>活动ID </summary>
		public int ActivityID;
		 ///<summary>奖励金币 </summary>
		public int GoldCoin;
		 ///<summary>奖励U币 </summary>
		public int MiracleCoin;
		 ///<summary>奖励道具ID </summary>
		public List<int> ItemID;
	}
}
