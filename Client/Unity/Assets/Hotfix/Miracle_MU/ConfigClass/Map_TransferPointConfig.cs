using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class Map_TransferPointConfigCategory : ACategory<Map_TransferPointConfig>
	{
	}

	///<summary>地图传送点对应关系 </summary>
	public class Map_TransferPointConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>传送点名 </summary>
		public string Name;
		 ///<summary>小地图中显示名字 </summary>
		public string NameInMinimap;
		 ///<summary>地图Id </summary>
		public int MapId;
		 ///<summary>传送点目标Id </summary>
		public int TargetIndex;
		 ///<summary>地图传送花费 </summary>
		public int MapCostGold;
		 ///<summary>进入等级限制 </summary>
		public int MapMinLevel;
	}
}
