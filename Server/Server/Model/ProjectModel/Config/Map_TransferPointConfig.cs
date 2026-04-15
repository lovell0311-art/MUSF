using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork.Component;

namespace ETModel
{
    /// <summary>
    /// D地图-点卡2.0.xlsx-传送点对应关系
    /// </summary>
    public partial class Map_TransferPointConfig : C_ConfigInfo
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 传送点名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 小地图中显示名字
        /// </summary>
        public string NameInMinimap { get; set; }
        /// <summary>
        /// 地图Id
        /// </summary>
        public int MapId { get; set; }
        /// <summary>
        /// 传送点目标Id
        /// </summary>
        public int TargetIndex { get; set; }
        /// <summary>
        /// 地图传送花费
        /// </summary>
        public int MapCostGold { get; set; }
        /// <summary>
        /// 进入等级限制
        /// </summary>
        public int MapMinLevel { get; set; }
    }
    /// <summary>
    /// 配置数据:D地图-点卡2.0.xlsx-传送点对应关系
    /// </summary>
    [ReadConfigAttribute(typeof(Map_TransferPointConfig), new AppType[] { AppType.Game,AppType.Map,AppType.Robot })]
    public class Map_TransferPointConfigJson : C_ConfigJson
    {
        /// <summary>
        /// 反序列化后的配置数据
        /// </summary>
        public readonly Dictionary<int, Map_TransferPointConfig> JsonDic = new Dictionary<int, Map_TransferPointConfig>();
        /// <summary>
        /// 初始化配置表 更新表的信息
        /// </summary>
        /// <param name="b_ReadStr">新的数据集合</param>
        public Map_TransferPointConfigJson(string b_ReadStr)
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
            List<Map_TransferPointConfig> jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Map_TransferPointConfig>>(b_ReadStr);
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
        public override Type ConfigType => typeof(Map_TransferPointConfigJson);
    }
}