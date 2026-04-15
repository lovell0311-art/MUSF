using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class FluoreSet_StrengthenConfigCategory : ACategory<FluoreSet_StrengthenConfig>
	{
	}

	///<summary>荧光宝石强化表 </summary>
	public class FluoreSet_StrengthenConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>升级成功率 </summary>
		public int SuccessRate;
	}
}
