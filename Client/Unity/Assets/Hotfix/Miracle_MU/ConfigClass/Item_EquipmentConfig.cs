using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class Item_EquipmentConfigCategory : ACategory<Item_EquipmentConfig>
	{
	}

	///<summary>装备 </summary>
	public class Item_EquipmentConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>名字 </summary>
		public string Name;
		 ///<summary>种类名 </summary>
		public string KindName;
		 ///<summary>资源名 </summary>
		public string ResName;
		 ///<summary>装备卡槽 </summary>
		public int Slot;
		 ///<summary>类型 </summary>
		public int Type;
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
		 ///<summary>品质类型 </summary>
		public int QualityAttr;
		 ///<summary>物品等级 </summary>
		public int Level;
		 ///<summary>普通装备掉落 </summary>
		public int NormalDropWeight;
		 ///<summary>追加装备掉落 </summary>
		public int AppendDropWeight;
		 ///<summary>技能装备掉落 </summary>
		public int SkillDropWeight;
		 ///<summary>幸运装备掉落 </summary>
		public int LuckyDropWeight;
		 ///<summary>卓越装备掉落 </summary>
		public int ExcellentDropWeight;
		 ///<summary>套装装备掉落 </summary>
		public int SetDropWeight;
		 ///<summary>镶嵌装备掉落 </summary>
		public int SocketDropWeight;
		 ///<summary>橙光装备掉落 </summary>
		public int PurpleDropWeight;
		 ///<summary>基础属性 </summary>
		public List<int> BaseAttrId;
		 ///<summary>追加属性 </summary>
		public List<int> AppendAttrId;
		 ///<summary>额外属性 </summary>
		public List<int> ExtraAttrId;
		 ///<summary>额外属性2 </summary>
		public List<int> ExtraAttrId2;
		 ///<summary>400 </summary>
		public int Is400;
		 ///<summary>双手武器 </summary>
		public int TwoHand;
		 ///<summary>最小伤害 </summary>
		public int DamageMin;
		 ///<summary>最大伤害 </summary>
		public int DamageMax;
		 ///<summary>诅咒 </summary>
		public int Curse;
		 ///<summary>宠物提升 </summary>
		public int UpPet;
		 ///<summary>魔力百分比 </summary>
		public int MagicPct;
		 ///<summary>攻击速度 </summary>
		public int AttackSpeed;
		 ///<summary>移动速度 </summary>
		public int WalkSpeed;
		 ///<summary>防御 </summary>
		public int Defense;
		 ///<summary>防御率 </summary>
		public int DefenseRate;
		 ///<summary>耐久 </summary>
		public int Durable;
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
		 ///<summary>备注 </summary>
		public string BZ;
		 ///<summary>是否出售 </summary>
		public int Sell;
	}
}
