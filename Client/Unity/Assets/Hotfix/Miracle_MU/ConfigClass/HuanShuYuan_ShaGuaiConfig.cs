using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class HuanShuYuan_ShaGuaiConfigCategory : ACategory<HuanShuYuan_ShaGuaiConfig>
	{
	}

	///<summary>幻术园活动刷怪点 </summary>
	public class HuanShuYuan_ShaGuaiConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>坐标X </summary>
		public int PosX;
		 ///<summary>坐标Y </summary>
		public int PosY;
	}
}
