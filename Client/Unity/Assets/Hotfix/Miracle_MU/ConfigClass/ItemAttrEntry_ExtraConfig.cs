using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class ItemAttrEntry_ExtraConfigCategory : ACategory<ItemAttrEntry_ExtraConfig>
	{
	}

	///<summary>7.额外属性词条   </summary>
	public class ItemAttrEntry_ExtraConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>属性名 </summary>
		public string Name;
		 ///<summary>万分比 </summary>
		public int IsBP;
		 ///<summary>属性id </summary>
		public int PropId;
		 ///<summary>值 </summary>
		public int Value0;
		 ///<summary>出现权重 </summary>
		public int Rate0;
		 ///<summary>值2 </summary>
		public int Value1;
		 ///<summary>出现权重2 </summary>
		public int Rate1;
		 ///<summary>值3 </summary>
		public int Value2;
		 ///<summary>出现权重3 </summary>
		public int Rate2;
	}
}
