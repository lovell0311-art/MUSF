using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class Activity_InfoConfigCategory : ACategory<Activity_InfoConfig>
	{
	}

	///<summary>活动配置 </summary>
	public class Activity_InfoConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>活动名称 </summary>
		public string Name;
		 ///<summary>活动介绍 </summary>
		public string ActivityIntroduce;
		 ///<summary>奖励介绍 </summary>
		public string RewardIntroduce;
		 ///<summary>开启时间 </summary>
		public string OpenTime;
		 ///<summary>结束时间 </summary>
		public string EndTime;
	}
}
