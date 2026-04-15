using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class SiWangShaMo_ShuaGuaiConfigCategory : ACategory<SiWangShaMo_ShuaGuaiConfig>
	{
	}

	///<summary>死亡沙漠 </summary>
	public class SiWangShaMo_ShuaGuaiConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>坐标X </summary>
		public int PosX;
		 ///<summary>坐标Y </summary>
		public int PosY;
	}
}
