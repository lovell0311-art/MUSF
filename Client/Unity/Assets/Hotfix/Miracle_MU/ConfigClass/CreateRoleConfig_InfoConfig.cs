using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class CreateRoleConfig_InfoConfigCategory : ACategory<CreateRoleConfig_InfoConfig>
	{
	}

	///<summary>角色 </summary>
	public class CreateRoleConfig_InfoConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>名字 </summary>
		public string Name;
		 ///<summary>描述 </summary>
		public string Desc;
		 ///<summary>性別 </summary>
		public int Sex;
		 ///<summary>生命值 </summary>
		public int Hp;
		 ///<summary>魔法值 </summary>
		public int Mp;
		 ///<summary>力量 </summary>
		public int Strength;
		 ///<summary>智力 </summary>
		public int Willpower;
		 ///<summary>体力 </summary>
		public int BoneGas;
		 ///<summary>敏捷 </summary>
		public int Agility;
		 ///<summary>统率 </summary>
		public int Command;
		 ///<summary>攻击距离 </summary>
		public int AttackDistance;
		 ///<summary>技能攻击力(百分比) </summary>
		public int SkillAddition;
		 ///<summary>初始地图位置 </summary>
		public int InitMap;
		 ///<summary>升级获得 </summary>
		public string AppendLevel;
		 ///<summary>大师点数 </summary>
		public string MasterPoints;
		 ///<summary>死亡时间(毫秒) </summary>
		public int DeathSleepTime;
	}
}
