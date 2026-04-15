using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class GoldLottery_ItemInfoConfigCategory : ACategory<GoldLottery_ItemInfoConfig>
	{
	}

	///<summary>金币抽奖 </summary>
	public class GoldLottery_ItemInfoConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>活动ID </summary>
		public int ActivityID;
		 ///<summary>图标名 </summary>
		public string IconName;
		 ///<summary>物品id </summary>
		public int ItemId;
		 ///<summary>物品名 </summary>
		public string ItemName;
		 ///<summary>强化等级 </summary>
		public int Level;
		 ///<summary>数量 </summary>
		public int Quantity;
		 ///<summary>拥有技能 </summary>
		public int HasSkill;
		 ///<summary>有幸运属性 </summary>
		public int HasLucky;
		 ///<summary>自定义属性方法 </summary>
		public string CustomAttrMathod;
		 ///<summary>命中权重 </summary>
		public int Weight;
	}
}
