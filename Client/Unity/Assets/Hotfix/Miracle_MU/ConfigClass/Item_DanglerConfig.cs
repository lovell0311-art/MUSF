using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class Item_DanglerConfigCategory : ACategory<Item_DanglerConfig>
	{
	}

	///<summary>25.耳环 </summary>
	public class Item_DanglerConfig: IConfig
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
		 ///<summary>可以掉落 </summary>
		public int Drop;
		 ///<summary>物品等级 </summary>
		public int Level;
		 ///<summary>是卓越物品 </summary>
		public int IsExc;
		 ///<summary>耐久 </summary>
		public int Durable;
		 ///<summary>攻击力/魔法攻击力/诅咒力 最小 </summary>
		public int AllAttackMin;
		 ///<summary>攻击力/魔法攻击力/诅咒力 最大 </summary>
		public int AllAttackMax;
		 ///<summary>属性防御 </summary>
		public int AttrDefense;
		 ///<summary>需求等级 </summary>
		public int ReqLvl;
		 ///<summary>使用职业 </summary>
		public string UseRole;
		 ///<summary>提示 </summary>
		public string Prompt;
		 ///<summary>更新属性方法 </summary>
		public string UpdatePropMethod;
		 ///<summary>是否出售 </summary>
		public int Sell;
	}
}
