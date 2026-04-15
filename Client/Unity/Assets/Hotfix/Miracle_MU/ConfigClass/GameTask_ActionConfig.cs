using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class GameTask_ActionConfigCategory : ACategory<GameTask_ActionConfig>
	{
	}

	///<summary>任务行为 </summary>
	public class GameTask_ActionConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>名称 </summary>
		public string TaskName;
		 ///<summary>任务进度更新行为 </summary>
		public int TaskProgressType;
		 ///<summary>描述 </summary>
		public string Desc;
		 ///<summary>填写规则 </summary>
		public string Param;
	}
}
