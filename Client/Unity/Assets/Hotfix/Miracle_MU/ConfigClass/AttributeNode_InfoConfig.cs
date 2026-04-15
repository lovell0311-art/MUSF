using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class AttributeNode_InfoConfigCategory : ACategory<AttributeNode_InfoConfig>
	{
	}

	///<summary>属性节点 </summary>
	public class AttributeNode_InfoConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>激活需求 </summary>
		public string ActivateNeed;
		 ///<summary>节点属性 </summary>
		public List<int> AttributeNode;
	}
}
