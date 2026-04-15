using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class FunctionList_InfoConfigCategory : ACategory<FunctionList_InfoConfig>
	{
	}

	///<summary>G功能 </summary>
	public class FunctionList_InfoConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>功能名字 </summary>
		public string FunctionName;
		 ///<summary>物品ID </summary>
		public int id;
		 ///<summary>层级 </summary>
		public int Hierarchy;
		 ///<summary>父级Id </summary>
		public int ParentId;
		 ///<summary>描述 </summary>
		public string Desk;
		 ///<summary>地图 </summary>
		public int SceneName;
		 ///<summary>NPC坐标 </summary>
		public string TargetIndex;
		 ///<summary>进入等级限制 </summary>
		public int MapMinLevel;
	}
}
