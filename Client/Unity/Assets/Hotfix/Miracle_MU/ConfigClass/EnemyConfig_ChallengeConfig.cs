using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class EnemyConfig_ChallengeConfigCategory : ACategory<EnemyConfig_ChallengeConfig>
	{
	}

	///<summary>掉落物品 </summary>
	public class EnemyConfig_ChallengeConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>怪物ID </summary>
		public int MonsterId;
		 ///<summary>怪物名字 </summary>
		public string MonsterName;
		 ///<summary>怪物类型 </summary>
		public int MonsterType;
		 ///<summary>刷新地点 </summary>
		public string RefreshPlace;
		 ///<summary>刷新时间 </summary>
		public string RefreshTime;
		 ///<summary>掉落物品 </summary>
		public List<int> Equip;
	}
}
