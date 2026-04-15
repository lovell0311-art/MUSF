using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class EnemyConfig_infoConfigConfigCategory : ACategory<EnemyConfig_infoConfigConfig>
	{
	}

	///<summary>怪物 </summary>
	public class EnemyConfig_infoConfigConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>名字 </summary>
		public string Name;
		 ///<summary>资源名 </summary>
		public string ResName;
		 ///<summary>攻击特效 </summary>
		public string AttackEffect;
		 ///<summary>待机音效 </summary>
		public string Sound_Idle;
		 ///<summary>攻击音效 </summary>
		public string Sound_Attack;
		 ///<summary>被击音效 </summary>
		public string Sound_Hit;
		 ///<summary>死亡音效 </summary>
		public string Sound_Dead;
		 ///<summary>怪物类型 </summary>
		public int Monster_Type;
		 ///<summary>等级 </summary>
		public int Lvl;
		 ///<summary>生命 </summary>
		public int HP;
		 ///<summary>被击次数 </summary>
		public int BeHitCnt;
		 ///<summary>魔法 </summary>
		public int MP;
		 ///<summary>最小攻击 </summary>
		public int DmgMin;
		 ///<summary>最大攻击 </summary>
		public int DmgMax;
		 ///<summary>防御 </summary>
		public int Def;
		 ///<summary>魔法防御 </summary>
		public int mDe;
		 ///<summary>攻击成功率 </summary>
		public int AttRate;
		 ///<summary>防御成功率 </summary>
		public int BloRate;
		 ///<summary>减伤百分比 </summary>
		public int DetractPct;
		 ///<summary>破甲百分比 </summary>
		public int SumderArmorPct;
		 ///<summary>巡逻范围 </summary>
		public int Ran;
		 ///<summary>攻击类型 </summary>
		public string AttackType;
		 ///<summary>攻击范围 </summary>
		public int AR;
		 ///<summary>视野范围 </summary>
		public int VR;
		 ///<summary>移动速度 </summary>
		public int MoSpeed;
		 ///<summary>攻击速度 </summary>
		public int AtSpeed;
		 ///<summary>复活时间(毫秒) </summary>
		public int Regen;
		 ///<summary>掉落 </summary>
		public string DropDic;
		 ///<summary> </summary>
		public int Att;
		 ///<summary> </summary>
		public int IT;
		 ///<summary> </summary>
		public int MR;
		 ///<summary> </summary>
		public int MIL;
		 ///<summary> </summary>
		public int SKI;
		 ///<summary>冰抗 </summary>
		public int ICE;
		 ///<summary>毒抗 </summary>
		public int POI;
		 ///<summary>雷抗 </summary>
		public int LIG;
		 ///<summary>火抗 </summary>
		public int FIR;
	}
}
