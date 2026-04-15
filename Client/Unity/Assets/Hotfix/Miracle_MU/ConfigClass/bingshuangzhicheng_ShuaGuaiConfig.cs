using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class bingshuangzhicheng_ShuaGuaiConfigCategory : ACategory<bingshuangzhicheng_ShuaGuaiConfig>
	{
	}

	///<summary>冰霜之城 </summary>
	public class bingshuangzhicheng_ShuaGuaiConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>坐标X </summary>
		public int PosX;
		 ///<summary>坐标Y </summary>
		public int PosY;
	}
}
