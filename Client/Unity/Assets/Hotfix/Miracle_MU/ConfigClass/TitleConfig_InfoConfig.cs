using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class TitleConfig_InfoConfigCategory : ACategory<TitleConfig_InfoConfig>
	{
	}

	///<summary>称号信息 </summary>
	public class TitleConfig_InfoConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>称号 </summary>
		public string Name;
		 ///<summary>描述 </summary>
		public string Describe;
		 ///<summary>属性1 </summary>
		public string TitleAttribute;
		 ///<summary>持续时长 </summary>
		public int Duration;
		 ///<summary>资源名 </summary>
		public string AsstetName;
		 ///<summary>获取途径 </summary>
		public string GetWay;
		 ///<summary>附加属性描述 </summary>
		public string AttributeDescribe;
	}
}
