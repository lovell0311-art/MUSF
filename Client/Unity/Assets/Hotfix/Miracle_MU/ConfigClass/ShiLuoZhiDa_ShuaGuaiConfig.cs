using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class ShiLuoZhiDa_ShuaGuaiConfigCategory : ACategory<ShiLuoZhiDa_ShuaGuaiConfig>
	{
	}

	///<summary>失落之塔 </summary>
	public class ShiLuoZhiDa_ShuaGuaiConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>坐标X </summary>
		public int PosX;
		 ///<summary>坐标Y </summary>
		public int PosY;
	}
}
