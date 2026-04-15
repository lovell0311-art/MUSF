using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class ItemCustomDrop_TypeConfigCategory : ACategory<ItemCustomDrop_TypeConfig>
	{
	}

	///<summary>自定义掉落类型 </summary>
	public class ItemCustomDrop_TypeConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>权重 </summary>
		public int DropRate;
	}
}
