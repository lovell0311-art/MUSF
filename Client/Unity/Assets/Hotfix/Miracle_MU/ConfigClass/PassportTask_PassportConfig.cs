using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class PassportTask_PassportConfigCategory : ACategory<PassportTask_PassportConfig>
	{
	}

	///<summary>主线任务 </summary>
	public class PassportTask_PassportConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>名称 </summary>
		public string TaskName;
		 ///<summary>任务行为类型 </summary>
		public int TaskActionType;
		 ///<summary>任务介绍 </summary>
		public string TaskDes;
		 ///<summary>地图 </summary>
		public int MapId;
		 ///<summary>自动寻路坐标 </summary>
		public List<int> AutoPathPos;
		 ///<summary>是否需要自动打怪 </summary>
		public int IsAutoHitMonster;
		 ///<summary>怪物名称 </summary>
		public string MonsterName;
		 ///<summary>任务目标id </summary>
		public List<int> TaskTargetId;
		 ///<summary>任务目标数量 </summary>
		public List<int> TaskTargetCount;
		 ///<summary>自动领取奖励 </summary>
		public int AutoReceiveReward;
		 ///<summary>奖励经验 </summary>
		public int RewardExp;
		 ///<summary>奖励金币 </summary>
		public int RewardCoin;
		 ///<summary>奖励U币 </summary>
		public int RewardUCoin;
		 ///<summary>奖励物品 </summary>
		public string RewardItems;
		 ///<summary>前置任务ID </summary>
		public List<int> TaskBeforeId;
		 ///<summary>需要最小等级 </summary>
		public int ReqLevelMin;
		 ///<summary>最大等级 </summary>
		public int ReqLevelMax;
		 ///<summary>法师 </summary>
		public int Spell;
		 ///<summary>剑士 </summary>
		public int Swordsman;
		 ///<summary>弓箭手 </summary>
		public int Archer;
		 ///<summary>魔剑士 </summary>
		public int Spellsword;
		 ///<summary>圣导师 </summary>
		public int Holyteacher;
		 ///<summary>召唤术士 </summary>
		public int SummonWarlock;
		 ///<summary>格斗 </summary>
		public int Combat;
		 ///<summary>梦幻骑士 </summary>
		public int GrowLancer;
	}
}
