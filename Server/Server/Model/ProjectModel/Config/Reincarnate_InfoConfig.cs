using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork.Component;

namespace ETModel
{
    /// <summary>
    /// Z转身-点卡2.0.xlsx-Sheet1
    /// </summary>
    public partial class Reincarnate_InfoConfig : C_ConfigInfo
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 需求金币
        /// </summary>
        public int DemandGold { get; set; }
        /// <summary>
        /// 需求魔晶
        /// </summary>
        public int DemandCrystal { get; set; }
        /// <summary>
        /// 限制等级
        /// </summary>
        public int RestrictionLevel { get; set; }
        /// <summary>
        /// 转生材料
        /// </summary>
        public string ReincarnationMaterial { get; set; }
        /// <summary>
        /// 大师点数
        /// </summary>
        public int MasterPoints { get; set; }
        /// <summary>
        /// 转生点数
        /// </summary>
        public int ReincarnatePoints { get; set; }
    }
    /// <summary>
    /// 配置数据:Z转身-点卡2.0.xlsx-Sheet1
    /// </summary>
    [ReadConfigAttribute(typeof(Reincarnate_InfoConfig), new AppType[] { AppType.Game })]
    public class Reincarnate_InfoConfigJson : C_ConfigJson
    {
        /// <summary>
        /// 反序列化后的配置数据
        /// </summary>
        public readonly Dictionary<int, Reincarnate_InfoConfig> JsonDic = new Dictionary<int, Reincarnate_InfoConfig>();
        /// <summary>
        /// 初始化配置表 更新表的信息
        /// </summary>
        /// <param name="b_ReadStr">新的数据集合</param>
        public Reincarnate_InfoConfigJson(string b_ReadStr)
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
            List<Reincarnate_InfoConfig> jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Reincarnate_InfoConfig>>(b_ReadStr);
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
        public override Type ConfigType => typeof(Reincarnate_InfoConfigJson);
    }
}