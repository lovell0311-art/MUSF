using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class BattleCopyConfig_RedCastleConfigCategory : ACategory<BattleCopyConfig_RedCastleConfig>
	{
	}

	///<summary>副本血色评分配置 </summary>
	public class BattleCopyConfig_RedCastleConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>攻破城门 </summary>
		public int Door;
		 ///<summary>打开水晶 </summary>
		public int Crystal;
		 ///<summary>交换武器 </summary>
		public int Weapon;
		 ///<summary>剩余系数 </summary>
		public int Time;
	}
}
