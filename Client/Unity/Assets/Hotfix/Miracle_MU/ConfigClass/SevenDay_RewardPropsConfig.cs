using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class SevenDay_RewardPropsConfigCategory : ACategory<SevenDay_RewardPropsConfig>
	{
	}

	///<summary>国庆签到奖励 </summary>
	public class SevenDay_RewardPropsConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>活动ID </summary>
		public int ActivityID;
		 ///<summary>奖励魔晶 </summary>
		public int MagicCrystal;
	}
}
