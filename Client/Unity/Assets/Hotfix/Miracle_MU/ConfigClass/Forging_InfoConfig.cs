using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class Forging_InfoConfigCategory : ACategory<Forging_InfoConfig>
	{
	}

	///<summary>锻造 </summary>
	public class Forging_InfoConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>必要道具 </summary>
		public string MustItem;
		 ///<summary>攻击 </summary>
		public int Attack;
		 ///<summary>防御 </summary>
		public int Defense;
		 ///<summary>要求数量 </summary>
		public int MustCnt;
		 ///<summary>可选道具 </summary>
		public string SelectItem;
		 ///<summary>要求数量2 </summary>
		public int SelectCnt;
		 ///<summary>辅助道具 </summary>
		public string AssistItem;
		 ///<summary>所需金币 </summary>
		public int GoldCoin;
		 ///<summary>提示语 </summary>
		public string Prompt;
		 ///<summary>描述 </summary>
		public string Name;
	}
}
