using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class ItemAttrEntry_AppendConfigCategory : ACategory<ItemAttrEntry_AppendConfig>
	{
	}

	///<summary>8.追加属性词条   </summary>
	public class ItemAttrEntry_AppendConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>属性名 </summary>
		public string Name;
		 ///<summary>万分比 </summary>
		public int IsBP;
		 ///<summary>属性id </summary>
		public int PropId;
		 ///<summary>值 </summary>
		public int Value1;
		 ///<summary>值2 </summary>
		public int Value2;
		 ///<summary>值3 </summary>
		public int Value3;
		 ///<summary>值4 </summary>
		public int Value4;
	}
}
