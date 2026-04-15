using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class Item_SkillBooksConfigCategory : ACategory<Item_SkillBooksConfig>
	{
	}

	///<summary>29.技能书/石 </summary>
	public class Item_SkillBooksConfig: IConfig
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
		 ///<summary>普通装备掉落 </summary>
		public int NormalDropWeight;
		 ///<summary>使用方法 </summary>
		public string UseMethod;
		 ///<summary>操作值 </summary>
		public string Value;
		 ///<summary>需求等级 </summary>
		public int ReqLvl;
		 ///<summary>力量 </summary>
		public int ReqStr;
		 ///<summary>敏捷 </summary>
		public int ReqAgi;
		 ///<summary>体力 </summary>
		public int ReqVit;
		 ///<summary>智力 </summary>
		public int ReqEne;
		 ///<summary>统率 </summary>
		public int ReqCom;
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
