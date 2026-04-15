using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork.Component;

namespace ETModel
{
    /// <summary>
    /// J技能升级-点卡2.0.xlsx-技能升级
    /// </summary>
    public partial class SkillUp_InfoConfig : C_ConfigInfo
    {
        /// <summary>
        /// 技能Id
        /// </summary>
        public int SKillId { get; set; }
        /// <summary>
        /// 升级需求
        /// </summary>
        public string LvItemInfo { get; set; }
        /// <summary>
        /// 升级获得
        /// </summary>
        public string LvUpGet { get; set; }
    }
    /// <summary>
    /// 配置数据:J技能升级-点卡2.0.xlsx-技能升级
    /// </summary>
    [ReadConfigAttribute(typeof(SkillUp_InfoConfig), new AppType[] { AppType.Game })]
    public class SkillUp_InfoConfigJson : C_ConfigJson
    {
        /// <summary>
        /// 反序列化后的配置数据
        /// </summary>
        public readonly Dictionary<int, SkillUp_InfoConfig> JsonDic = new Dictionary<int, SkillUp_InfoConfig>();
        /// <summary>
        /// 初始化配置表 更新表的信息
        /// </summary>
        /// <param name="b_ReadStr">新的数据集合</param>
        public SkillUp_InfoConfigJson(string b_ReadStr)
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
            List<SkillUp_InfoConfig> jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SkillUp_InfoConfig>>(b_ReadStr);
            for (int i = 0; i < jsonData.Count; i++)
            {
                var mConfig = jsonData[i];
                mConfig.InitExpand();
                JsonDic[mConfig.SKillId] = mConfig;
            }
        }
        /// <summary>
        /// ConfigType
        /// </summary>
        public override Type ConfigType => typeof(SkillUp_InfoConfigJson);
    }
}