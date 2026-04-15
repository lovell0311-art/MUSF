using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class ItemCustomDrop_InfoConfigCategory : ACategory<ItemCustomDrop_InfoConfig>
	{
	}

	///<summary>自定义掉落信息 </summary>
	public class ItemCustomDrop_InfoConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>掉落类型 </summary>
		public int DropType;
		 ///<summary>怪物id </summary>
		public int MonsterId;
		 ///<summary>怪物名 </summary>
		public string MonsterName;
		 ///<summary>怪物id范围 </summary>
		public List<int> MonsterIdRange;
		 ///<summary>怪物等级范围 </summary>
		public List<int> MonsterLevel;
		 ///<summary>物品id </summary>
		public int ItemId;
		 ///<summary>物品名 </summary>
		public string ItemName;
		 ///<summary>强化等级 </summary>
		public int Level;
		 ///<summary>数量 </summary>
		public int Quantity;
		 ///<summary>追加列表id </summary>
		public int OptListId;
		 ///<summary>追加等级 </summary>
		public int OptLevel;
		 ///<summary>拥有技能 </summary>
		public int HasSkill;
		 ///<summary>套装id </summary>
		public int SetId;
		 ///<summary>绑定物品 </summary>
		public int IsBind;
		 ///<summary>卓越属性 </summary>
		public List<int> OptionExcellent;
		 ///<summary>自定义属性方法 </summary>
		public string CustomAttrMathod;
		 ///<summary>权重 </summary>
		public int DropRate;
	}
}
