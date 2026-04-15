using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class TrialTower_MonsterConfigCategory : ACategory<TrialTower_MonsterConfig>
	{
	}

	///<summary>试炼塔怪物 </summary>
	public class TrialTower_MonsterConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>怪物ID </summary>
		public int MobId;
		 ///<summary>怪物 </summary>
		public string Monster;
		 ///<summary>怪物数量 </summary>
		public int Number;
	}
}
