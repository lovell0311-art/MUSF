using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class ItemAttrEntry_BaseConfigCategory : ACategory<ItemAttrEntry_BaseConfig>
	{
	}

	///<summary>基础属性词条会根据物品强化等级的不同，选择对应等级的属性值 </summary>
	public class ItemAttrEntry_BaseConfig: IConfig
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
		 ///<summary>卓越属性 </summary>
		public int Outsattrib;
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
