using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class Yong_ShuaGuaiConfigCategory : ACategory<Yong_ShuaGuaiConfig>
	{
	}

	///<summary>勇者刷怪点 </summary>
	public class Yong_ShuaGuaiConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>坐标X </summary>
		public int PosX;
		 ///<summary>坐标Y </summary>
		public int PosY;
	}
}
