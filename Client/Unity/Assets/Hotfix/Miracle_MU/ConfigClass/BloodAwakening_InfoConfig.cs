using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class BloodAwakening_InfoConfigCategory : ACategory<BloodAwakening_InfoConfig>
	{
	}

	///<summary>血脉觉醒 </summary>
	public class BloodAwakening_InfoConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>武魂名称 </summary>
		public string BloodAwakening;
		 ///<summary>显示资源 </summary>
		public string DisplayResource;
		 ///<summary>激活需求 </summary>
		public string ActivateNeed;
		 ///<summary>净化需求 </summary>
		public string PurityNeed;
		 ///<summary>净化时间(秒) </summary>
		public string PurityTime;
		 ///<summary>一环属性节点 </summary>
		public string AttributeNode1;
		 ///<summary>二环属性节点 </summary>
		public string AttributeNode2;
		 ///<summary>三环属性节点 </summary>
		public string AttributeNode3;
		 ///<summary>四环属性节点 </summary>
		public string AttributeNode4;
		 ///<summary>五环属性节点 </summary>
		public string AttributeNode5;
	}
}
