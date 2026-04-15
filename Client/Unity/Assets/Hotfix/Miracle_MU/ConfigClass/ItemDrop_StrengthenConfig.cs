using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class ItemDrop_StrengthenConfigCategory : ACategory<ItemDrop_StrengthenConfig>
	{
	}

	///<summary>物品掉落强化概率 </summary>
	public class ItemDrop_StrengthenConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>强化等级 </summary>
		public int Level;
		 ///<summary>权重 </summary>
		public int Weight;
	}
}
