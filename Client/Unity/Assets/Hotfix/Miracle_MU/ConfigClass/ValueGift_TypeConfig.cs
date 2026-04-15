using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class ValueGift_TypeConfigCategory : ACategory<ValueGift_TypeConfig>
	{
	}

	///<summary>超值礼包类型 </summary>
	public class ValueGift_TypeConfig: IConfig
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
