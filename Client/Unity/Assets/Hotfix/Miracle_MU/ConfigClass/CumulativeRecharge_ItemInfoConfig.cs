using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class CumulativeRecharge_ItemInfoConfigCategory : ACategory<CumulativeRecharge_ItemInfoConfig>
	{
	}

	///<summary>累计充值物品信息 </summary>
	public class CumulativeRecharge_ItemInfoConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>累计充值类型id </summary>
		public int TypeId;
		 ///<summary>小类ID </summary>
		public int Id2;
		 ///<summary>职业类型(0.全部职业) </summary>
		public int RoleType;
		 ///<summary>是否可选 </summary>
		public int IsSelectable;
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
		 ///<summary>套装id </summary>
		public int SetId;
		 ///<summary>绑定物品 </summary>
		public int IsBind;
		 ///<summary>物品到期时间 </summary>
		public int ItemExpirationTime;
		 ///<summary>自定义属性方法 </summary>
		public string CustomAttrMathod;
	}
}
