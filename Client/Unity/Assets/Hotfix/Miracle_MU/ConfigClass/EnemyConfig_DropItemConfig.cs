using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class EnemyConfig_DropItemConfigCategory : ACategory<EnemyConfig_DropItemConfig>
	{
	}

	///<summary>掉落物品 </summary>
	public class EnemyConfig_DropItemConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>怪物等级 </summary>
		public int MonsterLevel;
		 ///<summary>装备 </summary>
		public string Equip;
		 ///<summary>项链 </summary>
		public string Necklace;
		 ///<summary>戒指 </summary>
		public string Rings;
		 ///<summary>技能书 </summary>
		public string SkillBooks;
		 ///<summary>守护 </summary>
		public string Guard;
		 ///<summary>消耗品 </summary>
		public string Consumables;
	}
}
