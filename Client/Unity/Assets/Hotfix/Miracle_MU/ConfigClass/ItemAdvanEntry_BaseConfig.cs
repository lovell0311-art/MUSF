using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class ItemAdvanEntry_BaseConfigCategory : ACategory<ItemAdvanEntry_BaseConfig>
	{
	}

	///<summary>坐骑进阶属性词条会根据物品强化等级的不同，选择对应等级的属性值 </summary>
	public class ItemAdvanEntry_BaseConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>属性名 </summary>
		public string Name;
		 ///<summary>会衰弱 </summary>
		public int WillWeaken;
		 ///<summary>万分比 </summary>
		public int IsBP;
		 ///<summary>属性id </summary>
		public int PropId;
		 ///<summary>0级 </summary>
		public int Value0;
		 ///<summary>1级 </summary>
		public int Value1;
	}
}
