using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class Inherit_InfoConfigCategory : ACategory<Inherit_InfoConfig>
	{
	}

	///<summary>继承 </summary>
	public class Inherit_InfoConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>所需材料 </summary>
		public string Material;
		 ///<summary>所需金币 </summary>
		public int GoldCoin;
		 ///<summary>描述 </summary>
		public string Name;
	}
}
