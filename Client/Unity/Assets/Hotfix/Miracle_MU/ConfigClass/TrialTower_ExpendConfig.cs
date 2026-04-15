using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class TrialTower_ExpendConfigCategory : ACategory<TrialTower_ExpendConfig>
	{
	}

	///<summary>恐惧之地消耗 </summary>
	public class TrialTower_ExpendConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>消耗(魔晶) </summary>
		public int Expend;
		 ///<summary>消耗道具1 </summary>
		public int ItemInfo1;
		 ///<summary>消耗数量1 </summary>
		public int ItemCnt1;
		 ///<summary>消耗道具2 </summary>
		public int ItemInfo2;
		 ///<summary>消耗数量2 </summary>
		public int ItemCnt2;
	}
}
