using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class FeiXu_ShuaGuaiConfigCategory : ACategory<FeiXu_ShuaGuaiConfig>
	{
	}

	///<summary>坎特鲁废墟 </summary>
	public class FeiXu_ShuaGuaiConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>坐标X </summary>
		public int PosX;
		 ///<summary>坐标Y </summary>
		public int PosY;
	}
}
