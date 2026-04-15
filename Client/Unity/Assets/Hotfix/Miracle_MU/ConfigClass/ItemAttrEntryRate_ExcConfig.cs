using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class ItemAttrEntryRate_ExcConfigCategory : ACategory<ItemAttrEntryRate_ExcConfig>
	{
	}

	///<summary>卓越属性词条概率 </summary>
	public class ItemAttrEntryRate_ExcConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>条数 </summary>
		public int StripCnt;
		 ///<summary>装备概率(万分比) </summary>
		public int Rate;
		 ///<summary>宠物概率(万分比) </summary>
		public int PetsRate;
		 ///<summary>旗帜概率(万分比) </summary>
		public int FlagRate;
	}
}
