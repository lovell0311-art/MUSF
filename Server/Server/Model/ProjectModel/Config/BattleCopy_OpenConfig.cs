using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork.Component;

namespace ETModel
{
    /// <summary>
    /// F副本-点卡2.0.xlsx-副本开放
    /// </summary>
    public partial class BattleCopy_OpenConfig : C_ConfigInfo
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 副本名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 开启时间1
        /// </summary>
        public string OpenTime1 { get; set; }
        /// <summary>
        /// 开启时间2
        /// </summary>
        public string OpenTime2 { get; set; }
        /// <summary>
        /// 开启时间3
        /// </summary>
        public string OpenTime3 { get; set; }
        /// <summary>
        /// 开启时间4
        /// </summary>
        public string OpenTime4 { get; set; }
        /// <summary>
        /// 开启时间5
        /// </summary>
        public string OpenTime5 { get; set; }
        /// <summary>
        /// 开启时间6
        /// </summary>
        public string OpenTime6 { get; set; }
        /// <summary>
        /// 开启时间7
        /// </summary>
        public string OpenTime7 { get; set; }
        /// <summary>
        /// 开启时间8
        /// </summary>
        public string OpenTime8 { get; set; }
        /// <summary>
        /// 开启时间9
        /// </summary>
        public string OpenTime9 { get; set; }
        /// <summary>
        /// 开启时间10
        /// </summary>
        public string OpenTime10 { get; set; }
        /// <summary>
        /// 开启时间11
        /// </summary>
        public string OpenTime11 { get; set; }
        /// <summary>
        /// 开启时间12
        /// </summary>
        public string OpenTime12 { get; set; }
        /// <summary>
        /// 开启时间13
        /// </summary>
        public string OpenTime13 { get; set; }
        /// <summary>
        /// 开启时间14
        /// </summary>
        public string OpenTime14 { get; set; }
        /// <summary>
        /// 开启时间15
        /// </summary>
        public string OpenTime15 { get; set; }
        /// <summary>
        /// 开启时间16
        /// </summary>
        public string OpenTime16 { get; set; }
        /// <summary>
        /// 开启时间17
        /// </summary>
        public string OpenTime17 { get; set; }
        /// <summary>
        /// 开启时间18
        /// </summary>
        public string OpenTime18 { get; set; }
        /// <summary>
        /// 开启时间19
        /// </summary>
        public string OpenTime19 { get; set; }
        /// <summary>
        /// 开启时间20
        /// </summary>
        public string OpenTime20 { get; set; }
        /// <summary>
        /// 开启时间21
        /// </summary>
        public string OpenTime21 { get; set; }
        /// <summary>
        /// 开启时间22
        /// </summary>
        public string OpenTime22 { get; set; }
        /// <summary>
        /// 开启时间23
        /// </summary>
        public string OpenTime23 { get; set; }
        /// <summary>
        /// 开启时间24
        /// </summary>
        public string OpenTime24 { get; set; }
        /// <summary>
        /// 持续时间（分钟）
        /// </summary>
        public string Duration { get; set; }
    }
    /// <summary>
    /// 配置数据:F副本-点卡2.0.xlsx-副本开放
    /// </summary>
    [ReadConfigAttribute(typeof(BattleCopy_OpenConfig), new AppType[] { AppType.Game })]
    public class BattleCopy_OpenConfigJson : C_ConfigJson
    {
        /// <summary>
        /// 反序列化后的配置数据
        /// </summary>
        public readonly Dictionary<int, BattleCopy_OpenConfig> JsonDic = new Dictionary<int, BattleCopy_OpenConfig>();
        /// <summary>
        /// 初始化配置表 更新表的信息
        /// </summary>
        /// <param name="b_ReadStr">新的数据集合</param>
        public BattleCopy_OpenConfigJson(string b_ReadStr)
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
            List<BattleCopy_OpenConfig> jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<BattleCopy_OpenConfig>>(b_ReadStr);
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
        public override Type ConfigType => typeof(BattleCopy_OpenConfigJson);
    }
}