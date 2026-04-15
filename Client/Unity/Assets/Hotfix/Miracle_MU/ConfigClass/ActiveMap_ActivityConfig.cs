using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class ActiveMap_ActivityConfigCategory : ACategory<ActiveMap_ActivityConfig>
	{
	}

	///<summary>活动地图 </summary>
	public class ActiveMap_ActivityConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>名称 </summary>
		public string TaskName;
		 ///<summary>可以进入次数 </summary>
		public int IntoCount;
	}
}
