using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class langhunyaosai_ShuaGuaiConfigCategory : ACategory<langhunyaosai_ShuaGuaiConfig>
	{
	}

	///<summary>狼魂要塞 </summary>
	public class langhunyaosai_ShuaGuaiConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>坐标X </summary>
		public int PosX;
		 ///<summary>坐标Y </summary>
		public int PosY;
	}
}
