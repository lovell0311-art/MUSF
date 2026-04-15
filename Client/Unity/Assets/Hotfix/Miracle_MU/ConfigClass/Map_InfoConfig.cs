using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class Map_InfoConfigCategory : ACategory<Map_InfoConfig>
	{
	}

	///<summary>地图 </summary>
	public class Map_InfoConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>地图名 </summary>
		public string SceneName;
		 ///<summary>是不是副本 </summary>
		public int IsCopyMap;
		 ///<summary>最低进入等级 </summary>
		public int GotoMapByLevel;
		 ///<summary>怪物击杀掉落物品 </summary>
		public int ItemDropByMonsterKill;
		 ///<summary>角色击杀掉落物品 </summary>
		public int ItemDropByRoleKill;
		 ///<summary>地形配置文件路径 </summary>
		public string TerrainPath;
		 ///<summary>安全区路径 </summary>
		public string SafeAreaPath;
		 ///<summary>出生点 </summary>
		public string SpawnPath;
		 ///<summary>传送点 </summary>
		public string TransferPoint;
		 ///<summary>npc出生点 </summary>
		public string NpcPath;
		 ///<summary>坐标偏移值X </summary>
		public int PosOffsetX;
		 ///<summary>坐标偏移值X </summary>
		public int PosOffsetY;
		 ///<summary>坐标偏移值X </summary>
		public float ScaleOffset;
		 ///<summary>坐标偏移值X </summary>
		public string Minimap;
	}
}
