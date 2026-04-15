using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class Announc_InfoConfigCategory : ACategory<Announc_InfoConfig>
	{
	}

	///<summary> </summary>
	public class Announc_InfoConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>公告名称 </summary>
		public string AnnounName;
		 ///<summary>类别 </summary>
		public int AnnounType;
		 ///<summary>公告内容 </summary>
		public string AnnounContent;
	}
}
