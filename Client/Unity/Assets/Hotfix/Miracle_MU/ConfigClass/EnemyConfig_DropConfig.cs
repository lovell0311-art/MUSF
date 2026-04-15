using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class EnemyConfig_DropConfigCategory : ACategory<EnemyConfig_DropConfig>
	{
	}

	///<summary>怪物掉落概率 </summary>
	public class EnemyConfig_DropConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>怪物等级 </summary>
		public int MonsterLevel;
		 ///<summary>装备 </summary>
		public int Equip;
		 ///<summary>项链 </summary>
		public int Necklace;
		 ///<summary>戒指 </summary>
		public int Ring;
		 ///<summary>技能书|石 </summary>
		public int SkillBook;
		 ///<summary>消耗品 </summary>
		public int Consumables;
		 ///<summary>光之石 </summary>
		public int LightStone;
		 ///<summary>玛雅之石 </summary>
		public int MayaStone;
		 ///<summary>祝福宝石 </summary>
		public int BlessingGem;
		 ///<summary>灵魂宝石 </summary>
		public int SoulGem;
		 ///<summary>生命宝石 </summary>
		public int LifeGem;
		 ///<summary>创造宝石 </summary>
		public int CreateGem;
		 ///<summary>守护宝石 </summary>
		public int GuardGem;
		 ///<summary>再生原石 </summary>
		public int ReviveOriginalStone;
		 ///<summary>荧光宝石火 </summary>
		public int FluorescentDropsFire;
		 ///<summary>荧光宝石土 </summary>
		public int FluorescentDropsSoil;
		 ///<summary>荧光宝石雷 </summary>
		public int FluorescentDropsMine;
		 ///<summary>荧光宝石风 </summary>
		public int FluorescentDropsWind;
		 ///<summary>荧光宝石冰 </summary>
		public int FluorescentDropsIce;
		 ///<summary>荧光宝石水 </summary>
		public int FluorescentDropsWater;
		 ///<summary>幸运宝石 </summary>
		public int LuckyGem;
		 ///<summary>卓越宝石 </summary>
		public int ExcellentGem;
		 ///<summary>金币 </summary>
		public int MiracleCoin;
		 ///<summary>金币 </summary>
		public int GoldCoin;
		 ///<summary>无掉落 </summary>
		public int NoDrop;
		 ///<summary>之和 </summary>
		public int sum;
	}
}
