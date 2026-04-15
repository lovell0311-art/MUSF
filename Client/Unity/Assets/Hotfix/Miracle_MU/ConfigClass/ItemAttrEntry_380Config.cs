using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class ItemAttrEntry_380ConfigCategory : ACategory<ItemAttrEntry_380Config>
	{
	}

	///<summary>380属性词条 </summary>
	public class ItemAttrEntry_380Config: IConfig
	{
		public long Id { get; set; }
		 ///<summary>词条类型 </summary>
		public int EntryType;
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
