using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class Item_MountsConfigCategory : ACategory<Item_MountsConfig>
	{
	}

	///<summary>26.坐骑 </summary>
	public class Item_MountsConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>名字 </summary>
		public string Name;
		 ///<summary>资源名 </summary>
		public string ResName;
		 ///<summary>坐骑模型资源名 </summary>
		public string MountResName;
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
		 ///<summary>品质类型 </summary>
		public int QualityAttr;
		 ///<summary>物品等级 </summary>
		public int Level;
		 ///<summary>基础属性 </summary>
		public List<int> BaseAttrId;
		 ///<summary>生命 </summary>
		public int Life;
		 ///<summary>使用方法 </summary>
		public string UseMethod;
		 ///<summary>操作值 </summary>
		public int Value;
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
