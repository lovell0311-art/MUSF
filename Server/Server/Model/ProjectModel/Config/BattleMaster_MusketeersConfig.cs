using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork.Component;

namespace ETModel
{
    /// <summary>
    /// D大师-点卡2.0.xlsx-火枪手
    /// </summary>
    public partial class BattleMaster_MusketeersConfig : C_ConfigInfo
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 名字
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 介绍
        /// </summary>
        public string Describe { get; set; }
        /// <summary>
        /// 附加数据使用
        /// </summary>
        public int OtherDataUse { get; set; }
        /// <summary>
        /// 等级
        /// </summary>
        public int LayerLevel { get; set; }
        /// <summary>
        /// 前置技能
        /// </summary>
        public List<int> FrontIds { get; set; }
        /// <summary>
        /// 上一阶技能
        /// </summary>
        public List<int> LastIds { get; set; }
        /// <summary>
        /// 升级消耗
        /// </summary>
        public int Consume { get; set; }
        /// <summary>
        /// 附加数值
        /// </summary>
        public string OtherData { get; set; }
    }
    /// <summary>
    /// 配置数据:D大师-点卡2.0.xlsx-火枪手
    /// </summary>
    [ReadConfigAttribute(typeof(BattleMaster_MusketeersConfig), new AppType[] { AppType.Game,AppType.Map,AppType.Robot })]
    public class BattleMaster_MusketeersConfigJson : C_ConfigJson
    {
        /// <summary>
        /// 反序列化后的配置数据
        /// </summary>
        public readonly Dictionary<int, BattleMaster_MusketeersConfig> JsonDic = new Dictionary<int, BattleMaster_MusketeersConfig>();
        /// <summary>
        /// 初始化配置表 更新表的信息
        /// </summary>
        /// <param name="b_ReadStr">新的数据集合</param>
        public BattleMaster_MusketeersConfigJson(string b_ReadStr)
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
            List<BattleMaster_MusketeersConfig> jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<BattleMaster_MusketeersConfig>>(b_ReadStr);
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
        public override Type ConfigType => typeof(BattleMaster_MusketeersConfigJson);
    }
}