using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class Fashion_InfoConfigCategory : ACategory<Fashion_InfoConfig>
	{
	}

	///<summary>时装 </summary>
	public class Fashion_InfoConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>魔法师时装 </summary>
		public string DW;
		 ///<summary>剑士时装 </summary>
		public string DK;
		 ///<summary>弓箭手时装 </summary>
		public string ELF;
		 ///<summary>魔剑时装 </summary>
		public string MG;
		 ///<summary>圣导师时装 </summary>
		public string DL;
		 ///<summary>召唤师时装 </summary>
		public string SUM;
		 ///<summary>梦幻骑士 </summary>
		public string DREAM;
		 ///<summary>格斗家时装 </summary>
		public string MONK;
	}
}
