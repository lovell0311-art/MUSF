using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork.Component;

namespace ETModel
{
    /// <summary>
    /// D地图-点卡2.0.xlsx-地图
    /// </summary>
    public partial class Map_InfoConfig : C_ConfigInfo
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 地图名
        /// </summary>
        public string SceneName { get; set; }
        /// <summary>
        /// 是不是副本
        /// </summary>
        public int IsCopyMap { get; set; }
        /// <summary>
        /// 最低进入等级
        /// </summary>
        public int GotoMapByLevel { get; set; }
        /// <summary>
        /// 怪物击杀掉落物品
        /// </summary>
        public int ItemDropByMonsterKill { get; set; }
        /// <summary>
        /// 角色击杀掉落物品
        /// </summary>
        public int ItemDropByRoleKill { get; set; }
        /// <summary>
        /// 地形配置文件路径
        /// </summary>
        public string TerrainPath { get; set; }
        /// <summary>
        /// 安全区路径
        /// </summary>
        public string SafeAreaPath { get; set; }
        /// <summary>
        /// 出生点
        /// </summary>
        public string SpawnPath { get; set; }
        /// <summary>
        /// 传送点
        /// </summary>
        public string TransferPoint { get; set; }
        /// <summary>
        /// #怪物出生点
        /// </summary>
        public string MonsterPath { get; set; }
        /// <summary>
        /// npc出生点
        /// </summary>
        public string NpcPath { get; set; }
        /// <summary>
        /// 坐标偏移值X
        /// </summary>
        public int PosOffsetX { get; set; }
        /// <summary>
        /// 坐标偏移值X
        /// </summary>
        public int PosOffsetY { get; set; }
        /// <summary>
        /// 坐标偏移值X
        /// </summary>
        public float ScaleOffset { get; set; }
        /// <summary>
        /// 坐标偏移值X
        /// </summary>
        public string Minimap { get; set; }
    }
    /// <summary>
    /// 配置数据:D地图-点卡2.0.xlsx-地图
    /// </summary>
    [ReadConfigAttribute(typeof(Map_InfoConfig), new AppType[] { AppType.Game,AppType.Map,AppType.Robot })]
    public class Map_InfoConfigJson : C_ConfigJson
    {
        /// <summary>
        /// 反序列化后的配置数据
        /// </summary>
        public readonly Dictionary<int, Map_InfoConfig> JsonDic = new Dictionary<int, Map_InfoConfig>();
        /// <summary>
        /// 初始化配置表 更新表的信息
        /// </summary>
        /// <param name="b_ReadStr">新的数据集合</param>
        public Map_InfoConfigJson(string b_ReadStr)
        {
            ReadData(b_ReadStr);
        }
        /// <summary>
        /// 更新表的信息
        /// </summary>
        /// <param name="b_ReadStr">新的数据集合</param>
        public override void ReadData(string b_ReadStr)
        {
            JsonDic.Clear();
            List<Map_InfoConfig> jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Map_InfoConfig>>(b_ReadStr);
            for (int i = 0; i < jsonData.Count; i++)
            {
                var mConfig = jsonData[i];
                mConfig.InitExpand();
                JsonDic[mConfig.Id] = mConfig;
            }
        }
        /// <summary>
        /// ConfigType
        /// </summary>
        public override Type ConfigType => typeof(Map_InfoConfigJson);
    }
}