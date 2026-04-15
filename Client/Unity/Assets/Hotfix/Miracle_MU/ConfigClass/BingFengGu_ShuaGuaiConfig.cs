using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class BingFengGu_ShuaGuaiConfigCategory : ACategory<BingFengGu_ShuaGuaiConfig>
	{
	}

	///<summary>冰风谷 </summary>
	public class BingFengGu_ShuaGuaiConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>坐标X </summary>
		public int PosX;
		 ///<summary>坐标Y </summary>
		public int PosY;
	}
}
