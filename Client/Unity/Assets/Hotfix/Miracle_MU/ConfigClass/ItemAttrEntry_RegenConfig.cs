using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class ItemAttrEntry_RegenConfigCategory : ACategory<ItemAttrEntry_RegenConfig>
	{
	}

	///<summary>套装属性词条用于套装的属性 </summary>
	public class ItemAttrEntry_RegenConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>词条类型 </summary>
		public int EntryType;
		 ///<summary>属性名 </summary>
		public string Name;
		 ///<summary>概率 </summary>
		public int Rate;
		 ///<summary>需求等级 </summary>
		public int ReqLevel;
		 ///<summary>最大等级 </summary>
		public int MaxLevel;
		 ///<summary>万分比 </summary>
		public int IsBP;
		 ///<summary>属性id </summary>
		public int PropId;
		 ///<summary>0级 </summary>
		public int Value0;
		 ///<summary>1级 </summary>
		public int Value1;
		 ///<summary>2级 </summary>
		public int Value2;
		 ///<summary>3级 </summary>
		public int Value3;
		 ///<summary>4级 </summary>
		public int Value4;
		 ///<summary>5级 </summary>
		public int Value5;
		 ///<summary>6级 </summary>
		public int Value6;
		 ///<summary>7级 </summary>
		public int Value7;
		 ///<summary>8级 </summary>
		public int Value8;
		 ///<summary>9级 </summary>
		public int Value9;
		 ///<summary>10级 </summary>
		public int Value10;
		 ///<summary>11级 </summary>
		public int Value11;
		 ///<summary>12级 </summary>
		public int Value12;
		 ///<summary>13级 </summary>
		public int Value13;
		 ///<summary>14级 </summary>
		public int Value14;
		 ///<summary>15级 </summary>
		public int Value15;
	}
}
