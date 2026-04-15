using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class dixiacheng_ShuaGuaiConfigCategory : ACategory<dixiacheng_ShuaGuaiConfig>
	{
	}

	///<summary>地下城 </summary>
	public class dixiacheng_ShuaGuaiConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>坐标X </summary>
		public int PosX;
		 ///<summary>坐标Y </summary>
		public int PosY;
	}
}
