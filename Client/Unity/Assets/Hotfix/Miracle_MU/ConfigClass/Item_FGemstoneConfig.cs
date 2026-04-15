using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class Item_FGemstoneConfigCategory : ACategory<Item_FGemstoneConfig>
	{
	}

	///<summary>27.荧光宝石 </summary>
	public class Item_FGemstoneConfig: IConfig
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
		 ///<summary>提示 </summary>
		public string Prompt;
		 ///<summary>更新属性方法 </summary>
		public string UpdatePropMethod;
		 ///<summary>是否出售 </summary>
		public int Sell;
	}
}
