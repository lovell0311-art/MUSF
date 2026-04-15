using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class RechargeGiftBundle_ItemInfoConfigCategory : ACategory<RechargeGiftBundle_ItemInfoConfig>
	{
	}

	///<summary>充值礼包物品信息 </summary>
	public class RechargeGiftBundle_ItemInfoConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>宝箱id </summary>
		public int GiftBundleId;
		 ///<summary>物品id </summary>
		public int ItemId;
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
	}
}
