using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class Lottery_ItemInfoConfigCategory : ACategory<Lottery_ItemInfoConfig>
	{
	}

	///<summary>抽奖-奖池类型 </summary>
	public class Lottery_ItemInfoConfig: IConfig
	{
		public long Id { get; set; }
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
		 ///<summary>绑定物品 </summary>
		public int IsBind;
		 ///<summary>自定义属性方法 </summary>
		public string CustomAttrMathod;
		 ///<summary>命中权重 </summary>
		public int Weight;
	}
}
