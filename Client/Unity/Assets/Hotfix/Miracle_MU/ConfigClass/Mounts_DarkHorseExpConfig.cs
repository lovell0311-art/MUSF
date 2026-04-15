using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class Mounts_DarkHorseExpConfigCategory : ACategory<Mounts_DarkHorseExpConfig>
	{
	}

	///<summary>黑王马升级经验 </summary>
	public class Mounts_DarkHorseExpConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>经验 </summary>
		public int Exp;
	}
}
