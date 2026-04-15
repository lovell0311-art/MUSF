using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class Pets_InfoConfigCategory : ACategory<Pets_InfoConfig>
	{
	}

	///<summary>宠物 </summary>
	public class Pets_InfoConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>名字 </summary>
		public string Name;
		 ///<summary>描述 </summary>
		public string Desc;
		 ///<summary>宠物模型资源名 </summary>
		public string PetModleAsset;
		 ///<summary>宠物图标资源名 </summary>
		public string PetAsset;
		 ///<summary>类型 </summary>
		public int PetsType;
		 ///<summary>攻击类型 </summary>
		public int AttackType;
		 ///<summary>力量 </summary>
		public int Strength;
		 ///<summary>敏捷 </summary>
		public int Agility;
		 ///<summary>体力 </summary>
		public int BoneGas;
		 ///<summary>智力 </summary>
		public int Willpower;
		 ///<summary>初始技能 </summary>
		public List<int> SkillID;
		 ///<summary>试用时间(秒) </summary>
		public int TrialTime;
		 ///<summary>升级获得 </summary>
		public int AppendLevel;
		 ///<summary>巡逻范围 </summary>
		public int Ran;
		 ///<summary>视野范围 </summary>
		public int VR;
		 ///<summary>攻击距离 </summary>
		public int AttackDistance;
		 ///<summary>移动速度 </summary>
		public int MoSpeed;
		 ///<summary>攻击间隔(毫秒) </summary>
		public int AtSpeed;
		 ///<summary>复活时间(毫秒) </summary>
		public int Regen;
		 ///<summary>背包物品 </summary>
		public int BeakId;
		 ///<summary>强化属性 </summary>
		public List<int> EA;
	}
}
