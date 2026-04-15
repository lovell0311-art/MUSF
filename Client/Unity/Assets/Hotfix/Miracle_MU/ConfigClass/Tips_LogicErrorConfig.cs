using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class Tips_LogicErrorConfigCategory : ACategory<Tips_LogicErrorConfig>
	{
	}

	///<summary>提示语 </summary>
	public class Tips_LogicErrorConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>提示语 </summary>
		public string TipsDescribe;
		 ///<summary>使用场景 </summary>
		public string str;
	}
}
