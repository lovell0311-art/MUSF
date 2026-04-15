using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class AI_ConfigCategory : ACategory<AI_Config>
	{
	}

	///<summary>AI </summary>
	public class AI_Config: IConfig
	{
		public long Id { get; set; }
		 ///<summary>所属ai </summary>
		public int AIConfigId;
		 ///<summary>此ai中的顺序 </summary>
		public int Order;
		 ///<summary>名字 </summary>
		public string Name;
		 ///<summary>节点参数 </summary>
		public List<int> NodeParams;
	}
}
