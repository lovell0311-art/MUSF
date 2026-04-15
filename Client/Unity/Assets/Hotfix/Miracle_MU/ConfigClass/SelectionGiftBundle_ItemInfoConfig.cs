using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class SelectionGiftBundle_ItemInfoConfigCategory : ACategory<SelectionGiftBundle_ItemInfoConfig>
	{
	}

	///<summary>甄选礼包 </summary>
	public class SelectionGiftBundle_ItemInfoConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>甄选类型id </summary>
		public int TypeId;
		 ///<summary>职业类型(0.全部职业) </summary>
		public int RoleType;
		 ///<summary>是否可选 </summary>
		public int IsSelectable;
		 ///<summary>物品id </summary>
		public int ItemId;
		 ///<summary>物品名 </summary>
		public string ItemName;
		 ///<summary>物品资源名 </summary>
		public string ResourceName;
		 ///<summary>强化等级 </summary>
		public int Level;
		 ///<summary>追加等级 </summary>
		public int OptLevel;
		 ///<summary>锻造等级 </summary>
		public int SmithLevel;
		 ///<summary>数量 </summary>
		public int Quantity;
		 ///<summary>拥有技能 </summary>
		public int HasSkill;
		 ///<summary>有幸运属性 </summary>
		public int HasLucky;
		 ///<summary>卓越属性 </summary>
		public List<int> OptionExcellent;
		 ///<summary>套装id </summary>
		public int SetId;
		 ///<summary>绑定物品 </summary>
		public int IsBind;
		 ///<summary>自定义属性方法 </summary>
		public string CustomAttrMathod;
	}
}
