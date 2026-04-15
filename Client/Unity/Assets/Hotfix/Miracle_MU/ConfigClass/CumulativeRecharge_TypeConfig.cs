using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class CumulativeRecharge_TypeConfigCategory : ACategory<CumulativeRecharge_TypeConfig>
	{
	}

	///<summary>累计充值类型 </summary>
	public class CumulativeRecharge_TypeConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>名字 </summary>
		public string Name;
		 ///<summary>所需金额 </summary>
		public int Money;
		 ///<summary>物品描述 </summary>
		public string Description;
	}
}
