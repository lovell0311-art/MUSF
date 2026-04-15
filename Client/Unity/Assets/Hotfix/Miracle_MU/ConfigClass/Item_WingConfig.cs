using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class Item_WingConfigCategory : ACategory<Item_WingConfig>
	{
	}

	///<summary>22.翅膀 </summary>
	public class Item_WingConfig: IConfig
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
		 ///<summary>追加属性 </summary>
		public List<int> AppendAttrId;
		 ///<summary>基础属性 </summary>
		public List<int> BaseAttrId;
		 ///<summary>特殊属性 </summary>
		public List<int> SpecialAttrId;
		 ///<summary>耐久 </summary>
		public int Durable;
		 ///<summary>魔力百分比 </summary>
		public int MagicPct;
		 ///<summary>诅咒 </summary>
		public int Curse;
		 ///<summary>攻击力 </summary>
		public int Attack;
		 ///<summary>防御 </summary>
		public int Defense;
		 ///<summary>伤害提高百分比 </summary>
		public int DamagePct;
		 ///<summary>伤害吸收 </summary>
		public int DamageAbsPct;
		 ///<summary>需求等级 </summary>
		public int ReqLvl;
		 ///<summary>几代翅膀 </summary>
		public int WingLevel;
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
