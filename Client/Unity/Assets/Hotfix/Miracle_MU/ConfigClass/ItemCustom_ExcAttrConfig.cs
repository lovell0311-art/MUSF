using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class ItemCustom_ExcAttrConfigCategory : ACategory<ItemCustom_ExcAttrConfig>
	{
	}

	///<summary>自定义卓越属性 </summary>
	public class ItemCustom_ExcAttrConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>最小伤害 </summary>
		public int DamageMin;
		 ///<summary>最大伤害 </summary>
		public int DamageMax;
		 ///<summary>诅咒 </summary>
		public int Curse;
		 ///<summary>魔力百分比 </summary>
		public int MagicPct;
		 ///<summary>攻击速度 </summary>
		public int AttackSpeed;
		 ///<summary>防御 </summary>
		public int Defense;
		 ///<summary>防御率 </summary>
		public int DefenseRate;
		 ///<summary>耐久 </summary>
		public int Durable;
		 ///<summary>需求等级 </summary>
		public int ReqLvl;
		 ///<summary>力量 </summary>
		public int ReqStr;
		 ///<summary>敏捷 </summary>
		public int ReqAgi;
		 ///<summary>体力 </summary>
		public int ReqVit;
		 ///<summary>智力 </summary>
		public int ReqEne;
		 ///<summary>统率 </summary>
		public int ReqCom;
	}
}
