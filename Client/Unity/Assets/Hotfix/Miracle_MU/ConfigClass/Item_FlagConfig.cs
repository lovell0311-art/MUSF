using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class Item_FlagConfigCategory : ACategory<Item_FlagConfig>
	{
	}

	///<summary>34.旗帜 </summary>
	public class Item_FlagConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>名字 </summary>
		public string Name;
		 ///<summary>资源名 </summary>
		public string ResName;
		 ///<summary>装备卡槽 </summary>
		public int Slot;
		 ///<summary>技能 </summary>
		public int Skill;
		 ///<summary>宽 </summary>
		public int X;
		 ///<summary>高 </summary>
		public int Y;
		 ///<summary>单组数量 </summary>
		public int StackSize;
		 ///<summary>品质类型 </summary>
		public int QualityAttr;
		 ///<summary>物品等级 </summary>
		public int Level;
		 ///<summary>追加属性 </summary>
		public List<int> AppendAttrId;
		 ///<summary>基础属性 </summary>
		public List<int> BaseAttrId;
		 ///<summary>耐久 </summary>
		public int Durable;
		 ///<summary>需求等级 </summary>
		public int ReqLvl;
		 ///<summary>使用职业 </summary>
		public string UseRole;
		 ///<summary>提示 </summary>
		public string Prompt;
		 ///<summary>更新属性方法 </summary>
		public string UpdatePropMethod;
		 ///<summary>备注 </summary>
		public string BZ;
		 ///<summary>是否出售 </summary>
		public int Sell;
	}
}
