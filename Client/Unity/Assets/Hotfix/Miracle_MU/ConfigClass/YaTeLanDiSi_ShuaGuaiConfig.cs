using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class YaTeLanDiSi_ShuaGuaiConfigCategory : ACategory<YaTeLanDiSi_ShuaGuaiConfig>
	{
	}

	///<summary>亚特兰蒂斯 </summary>
	public class YaTeLanDiSi_ShuaGuaiConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>坐标X </summary>
		public int PosX;
		 ///<summary>坐标Y </summary>
		public int PosY;
	}
}
