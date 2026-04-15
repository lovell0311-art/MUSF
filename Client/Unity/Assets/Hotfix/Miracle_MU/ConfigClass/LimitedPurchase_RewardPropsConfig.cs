using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class LimitedPurchase_RewardPropsConfigCategory : ACategory<LimitedPurchase_RewardPropsConfig>
	{
	}

	///<summary>限时购买 </summary>
	public class LimitedPurchase_RewardPropsConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>活动ID </summary>
		public int ActivityID;
		 ///<summary>充值金额 </summary>
		public int TypeId;
		 ///<summary>职业类型(0.全部职业) </summary>
		public int RoleType;
		 ///<summary>物品id </summary>
		public int ItemId;
		 ///<summary>物品名 </summary>
		public string ItemName;
		 ///<summary>物品资源名 </summary>
		public string ResourceName;
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
