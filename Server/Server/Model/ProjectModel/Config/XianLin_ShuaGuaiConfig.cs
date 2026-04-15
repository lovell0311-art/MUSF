using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork.Component;

namespace ETModel
{
    /// <summary>
    /// H活动-点卡2.0.xlsx-仙踪林刷怪点
    /// </summary>
    public partial class XianLin_ShuaGuaiConfig : C_ConfigInfo
    {
        /// <summary>
        /// 序号
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 坐标X
        /// </summary>
        public int PosX { get; set; }
        /// <summary>
        /// 坐标Y
        /// </summary>
        public int PosY { get; set; }
    }
    /// <summary>
    /// 配置数据:H活动-点卡2.0.xlsx-仙踪林刷怪点
    /// </summary>
    [ReadConfigAttribute(typeof(XianLin_ShuaGuaiConfig), new AppType[] { AppType.Game })]
    public class XianLin_ShuaGuaiConfigJson : C_ConfigJson
    {
        /// <summary>
        /// 反序列化后的配置数据
        /// </summary>
        public readonly Dictionary<int, XianLin_ShuaGuaiConfig> JsonDic = new Dictionary<int, XianLin_ShuaGuaiConfig>();
        /// <summary>
        /// 初始化配置表 更新表的信息
        /// </summary>
        /// <param name="b_ReadStr">新的数据集合</param>
        public XianLin_ShuaGuaiConfigJson(string b_ReadStr)
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
            List<XianLin_ShuaGuaiConfig> jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<XianLin_ShuaGuaiConfig>>(b_ReadStr);
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
        public override Type ConfigType => typeof(XianLin_ShuaGuaiConfigJson);
    }
}