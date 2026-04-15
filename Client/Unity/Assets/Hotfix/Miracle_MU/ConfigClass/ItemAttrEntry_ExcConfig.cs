using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class ItemAttrEntry_ExcConfigCategory : ACategory<ItemAttrEntry_ExcConfig>
	{
	}

	///<summary>卓越属性词条 </summary>
	public class ItemAttrEntry_ExcConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>词条类型 </summary>
		public int EntryType;
		 ///<summary>属性名 </summary>
		public string Name;
		 ///<summary>装备概率 </summary>
		public int Rate;
		 ///<summary>宠物概率 </summary>
		public int PetsRate;
		 ///<summary>旗帜概率 </summary>
		public int FlagRate;
		 ///<summary>万分比 </summary>
		public int IsBP;
		 ///<summary>属性id </summary>
		public int PropId;
		 ///<summary>0级 </summary>
		public int Value0;
	}
}
