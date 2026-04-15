using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class YouAnSenLin_ShuaGuaiConfigCategory : ACategory<YouAnSenLin_ShuaGuaiConfig>
	{
	}

	///<summary>幽暗深林 </summary>
	public class YouAnSenLin_ShuaGuaiConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>坐标X </summary>
		public int PosX;
		 ///<summary>坐标Y </summary>
		public int PosY;
	}
}
