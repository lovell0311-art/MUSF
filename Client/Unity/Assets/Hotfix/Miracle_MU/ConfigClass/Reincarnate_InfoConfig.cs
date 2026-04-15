using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class Reincarnate_InfoConfigCategory : ACategory<Reincarnate_InfoConfig>
	{
	}

	///<summary>荧光属性 </summary>
	public class Reincarnate_InfoConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>需求金币 </summary>
		public int DemandGold;
		 ///<summary>需求魔晶 </summary>
		public int DemandCrystal;
		 ///<summary>限制等级 </summary>
		public int RestrictionLevel;
		 ///<summary>转生材料 </summary>
		public string ReincarnationMaterial;
		 ///<summary>大师点数 </summary>
		public int MasterPoints;
		 ///<summary>转生点数 </summary>
		public int ReincarnatePoints;
		 ///<summary>转身属性 </summary>
		public string ReincarnateBuf;
	}
}
