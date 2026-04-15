using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class KillRewards_InfoConfigCategory : ACategory<KillRewards_InfoConfig>
	{
	}

	///<summary>古战场红名奖励 </summary>
	public class KillRewards_InfoConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>奖励 </summary>
		public string Reward;
	}
}
