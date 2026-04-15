using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class Kill_RewardConfigCategory : ACategory<Kill_RewardConfig>
	{
	}

	///<summary>击杀奖励 </summary>
	public class Kill_RewardConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>活动ID </summary>
		public int ActivityID;
		 ///<summary>刷怪时间点 </summary>
		public string BrushTime;
		 ///<summary>怪物生成时间 </summary>
		public List<int> ExistenceTime;
		 ///<summary>刷怪ID </summary>
		public int ModID;
		 ///<summary>数量 </summary>
		public int MiracleCoin;
		 ///<summary>掉落 </summary>
		public string Drop;
	}
}
