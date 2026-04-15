using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class ItemAttrEntry_SetConfigCategory : ACategory<ItemAttrEntry_SetConfig>
	{
	}

	///<summary>套装属性词条用于套装的属性 </summary>
	public class ItemAttrEntry_SetConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>属性名 </summary>
		public string Name;
		 ///<summary>万分比 </summary>
		public int IsBP;
		 ///<summary>属性id </summary>
		public int PropId;
		 ///<summary>0级 </summary>
		public int Value0;
	}
}
