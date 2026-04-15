using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork.Component;

namespace ETModel
{
    /// <summary>
    /// H活动-点卡2.0.xlsx-年兽活动配置
    /// </summary>
    public partial class Kill_RewardConfig : C_ConfigInfo
    {
        /// <summary>
        /// 地图ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 活动ID
        /// </summary>
        public int ActivityID { get; set; }
        /// <summary>
        /// 刷怪时间点
        /// </summary>
        public string BrushTime { get; set; }
        /// <summary>
        /// 怪物生成时间
        /// </summary>
        public List<int> ExistenceTime { get; set; }
        /// <summary>
        /// 刷怪ID
        /// </summary>
        public int ModID { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public int MiracleCoin { get; set; }
        /// <summary>
        /// 掉落
        /// </summary>
        public string Drop { get; set; }
    }
    /// <summary>
    /// 配置数据:H活动-点卡2.0.xlsx-年兽活动配置
    /// </summary>
    [ReadConfigAttribute(typeof(Kill_RewardConfig), new AppType[] { AppType.Game })]
    public class Kill_RewardConfigJson : C_ConfigJson
    {
        /// <summary>
        /// 反序列化后的配置数据
        /// </summary>
        public readonly Dictionary<int, Kill_RewardConfig> JsonDic = new Dictionary<int, Kill_RewardConfig>();
        /// <summary>
        /// 初始化配置表 更新表的信息
        /// </summary>
        /// <param name="b_ReadStr">新的数据集合</param>
        public Kill_RewardConfigJson(string b_ReadStr)
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
            List<Kill_RewardConfig> jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Kill_RewardConfig>>(b_ReadStr);
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
        public override Type ConfigType => typeof(Kill_RewardConfigJson);
    }
}