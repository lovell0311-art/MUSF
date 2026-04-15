using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class SetItem_TypeConfigCategory : ACategory<SetItem_TypeConfig>
	{
	}

	///<summary>套装属性 </summary>
	public class SetItem_TypeConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>套装名 </summary>
		public string SetName;
		 ///<summary>套装物品id </summary>
		public List<int> ItemsId;
		 ///<summary>选择概率 </summary>
		public int Rate;
		 ///<summary>2件套装属性 </summary>
		public List<int> AttrId2;
		 ///<summary>3件套装属性 </summary>
		public List<int> AttrId3;
		 ///<summary>4件套装属性 </summary>
		public List<int> AttrId4;
		 ///<summary>5件套装属性 </summary>
		public List<int> AttrId5;
		 ///<summary>6件套装属性 </summary>
		public List<int> AttrId6;
		 ///<summary>7件套装属性 </summary>
		public List<int> AttrId7;
		 ///<summary>8件套装属性 </summary>
		public List<int> AttrId8;
		 ///<summary>9件套装属性 </summary>
		public List<int> AttrId9;
		 ///<summary>10件套装属性 </summary>
		public List<int> AttrId10;
	}
}
