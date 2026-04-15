using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class TrialTower_RewardsConfigCategory : ACategory<TrialTower_RewardsConfig>
	{
	}

	///<summary>试炼塔奖励 </summary>
	public class TrialTower_RewardsConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>奖励 </summary>
		public string Reward;
	}
}
