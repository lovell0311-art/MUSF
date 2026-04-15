using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class Npc_Shop_InfoConfigCategory : ACategory<Npc_Shop_InfoConfig>
	{
	}

	///<summary>NPC商店 </summary>
	public class Npc_Shop_InfoConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>物品ID </summary>
		public int itemID;
		 ///<summary>技能 </summary>
		public int Skill;
		 ///<summary>强化等级 </summary>
		public int Level;
		 ///<summary>追加ID </summary>
		public int OptValue;
		 ///<summary>追加等级 </summary>
		public int OptLevel;
		 ///<summary>幸运属性 </summary>
		public int LuckyAttribute;
		 ///<summary>套装属性 </summary>
		public int SetID;
		 ///<summary>卓越属性 </summary>
		public string OptionExcellent;
		 ///<summary>380属性 </summary>
		public int Option380;
		 ///<summary>再生属性 </summary>
		public int OptionRebirth;
		 ///<summary>再生属性等级 </summary>
		public int OptionRebirthLevel;
		 ///<summary>数量 </summary>
		public int Quantity;
		 ///<summary>价格 </summary>
		public int Price;
		 ///<summary>备注 </summary>
		public string Info;
	}
}
