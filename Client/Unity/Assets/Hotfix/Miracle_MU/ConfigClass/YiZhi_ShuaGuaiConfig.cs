using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class YiZhi_ShuaGuaiConfigCategory : ACategory<YiZhi_ShuaGuaiConfig>
	{
	}

	///<summary>坎特鲁遗址 </summary>
	public class YiZhi_ShuaGuaiConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>坐标X </summary>
		public int PosX;
		 ///<summary>坐标Y </summary>
		public int PosY;
	}
}
