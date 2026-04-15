using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class Npc_InfoConfigCategory : ACategory<Npc_InfoConfig>
	{
	}

	///<summary>非玩家角色 </summary>
	public class Npc_InfoConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>Npc名字 </summary>
		public string Name;
		 ///<summary>资源名 </summary>
		public string ResName;
		 ///<summary>小地图图标 </summary>
		public string Icon;
		 ///<summary>Npc类型 </summary>
		public int NpcType;
		 ///<summary>描述,简介 </summary>
		public string Info;
		 ///<summary>附加信息 </summary>
		public string OtherData;
		 ///<summary>商店物品ID </summary>
		public string EquipData;
	}
}
