using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class Maps_infoConfigCategory : ACategory<Maps_infoConfig>
	{
	}

	///<summary>地图 </summary>
	public class Maps_infoConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>地图名字 </summary>
		public string SceneName;
		 ///<summary>地图ID </summary>
		public int MapId;
	}
}
