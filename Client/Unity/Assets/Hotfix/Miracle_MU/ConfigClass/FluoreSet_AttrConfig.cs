using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class FluoreSet_AttrConfigCategory : ACategory<FluoreSet_AttrConfig>
	{
	}

	///<summary>荧光属性 </summary>
	public class FluoreSet_AttrConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>属性随机概率（权重） </summary>
		public int weight;
		 ///<summary>属性宝石ID </summary>
		public int fluore;
		 ///<summary>描述 </summary>
		public string Info;
		 ///<summary>属性 </summary>
		public int Attribute;
		 ///<summary>等级0 </summary>
		public int Level0;
		 ///<summary>等级1 </summary>
		public int Level1;
		 ///<summary>等级2 </summary>
		public int Level2;
		 ///<summary>等级3 </summary>
		public int Level3;
		 ///<summary>等级4 </summary>
		public int Level4;
		 ///<summary>等级5 </summary>
		public int Level5;
		 ///<summary>等级6 </summary>
		public int Level6;
		 ///<summary>等级7 </summary>
		public int Level7;
		 ///<summary>等级8 </summary>
		public int Level8;
		 ///<summary>等级9 </summary>
		public int Level9;
		 ///<summary>判断 </summary>
		public int Judgment;
	}
}
