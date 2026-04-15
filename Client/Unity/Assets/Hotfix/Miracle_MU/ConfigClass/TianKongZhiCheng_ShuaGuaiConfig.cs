using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class TianKongZhiCheng_ShuaGuaiConfigCategory : ACategory<TianKongZhiCheng_ShuaGuaiConfig>
	{
	}

	///<summary>天空之城 </summary>
	public class TianKongZhiCheng_ShuaGuaiConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>坐标X </summary>
		public int PosX;
		 ///<summary>坐标Y </summary>
		public int PosY;
	}
}
