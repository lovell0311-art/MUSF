using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class SelectionGiftBundle_TypeConfigCategory : ACategory<SelectionGiftBundle_TypeConfig>
	{
	}

	///<summary>甄选礼包类型 </summary>
	public class SelectionGiftBundle_TypeConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>名字 </summary>
		public string Name;
		 ///<summary>所需金额 </summary>
		public int Money;
	}
}
