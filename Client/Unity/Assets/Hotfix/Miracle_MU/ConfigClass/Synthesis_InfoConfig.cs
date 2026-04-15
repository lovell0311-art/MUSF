using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class Synthesis_InfoConfigCategory : ACategory<Synthesis_InfoConfig>
	{
	}

	///<summary>H合成 </summary>
	public class Synthesis_InfoConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>合成方法 </summary>
		public string Method;
		 ///<summary>需求物品 </summary>
		public string NeedItems;
		 ///<summary>所需金币 </summary>
		public long NeedGold;
		 ///<summary>基础成功率 </summary>
		public int BaseSuccessRate;
		 ///<summary>最高成功率 </summary>
		public int MaxSuccessRate;
		 ///<summary>备注 </summary>
		public string Info;
	}
}
