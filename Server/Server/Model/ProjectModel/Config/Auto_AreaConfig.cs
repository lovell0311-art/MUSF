using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork.Component;

namespace ETModel
{
    /// <summary>
    /// Z自动合区开区.xlsx-AutoArea
    /// </summary>
    public partial class Auto_AreaConfig : C_ConfigInfo
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 区名
        /// </summary>
        public string AreaName { get; set; }
        /// <summary>
        /// 开启时间
        /// </summary>
        public string OpeningTime { get; set; }
        /// <summary>
        /// 提前准备时间
        /// </summary>
        public int PreparationTime { get; set; }
        /// <summary>
        /// 线路参数
        /// </summary>
        public string LinesParameter { get; set; }
        /// <summary>
        /// 合并时间
        /// </summary>
        public string MergeTime { get; set; }
        /// <summary>
        /// 合并到几区
        /// </summary>
        public int MergeArea { get; set; }
        /// <summary>
        /// 数据地址
        /// </summary>
        public string DBAddreas { get; set; }
    }
    /// <summary>
    /// 配置数据:Z自动合区开区.xlsx-AutoArea
    /// </summary>
    [ReadConfigAttribute(typeof(Auto_AreaConfig), new AppType[] { AppType.Realm })]
    public class Auto_AreaConfigJson : C_ConfigJson
    {
        /// <summary>
        /// 反序列化后的配置数据
        /// </summary>
        public readonly Dictionary<int, Auto_AreaConfig> JsonDic = new Dictionary<int, Auto_AreaConfig>();
        /// <summary>
        /// 初始化配置表 更新表的信息
        /// </summary>
        /// <param name="b_ReadStr">新的数据集合</param>
        public Auto_AreaConfigJson(string b_ReadStr)
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
            List<Auto_AreaConfig> jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Auto_AreaConfig>>(b_ReadStr);
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
        public override Type ConfigType => typeof(Auto_AreaConfigJson);
    }
}