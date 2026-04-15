using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class OnlineDuration_RewardConfigCategory : ACategory<OnlineDuration_RewardConfig>
	{
	}

	///<summary>在线奖励 </summary>
	public class OnlineDuration_RewardConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>在线时长 </summary>
		public int Time;
		 ///<summary>带[主宰无双]称号 </summary>
		public int Title;
		 ///<summary>奖励金币 </summary>
		public int RewardCoins;
		 ///<summary>奖励U币 </summary>
		public int RewardMiracleCoin;
		 ///<summary>奖励道具ID </summary>
		public List<int> ItemID;
		 ///<summary>奖励道具 </summary>
		public string Item;
	}
}
