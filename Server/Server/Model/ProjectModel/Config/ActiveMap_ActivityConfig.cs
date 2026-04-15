using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork.Component;

namespace ETModel
{
    /// <summary>
    /// R任务-点卡2.0.xlsx-活动地图
    /// </summary>
    public partial class ActiveMap_ActivityConfig : C_ConfigInfo
    {
        /// <summary>
        /// 任务Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string TaskName { get; set; }
        /// <summary>
        /// 可以进入次数
        /// </summary>
        public int IntoCount { get; set; }
    }
    /// <summary>
    /// 配置数据:R任务-点卡2.0.xlsx-活动地图
    /// </summary>
    [ReadConfigAttribute(typeof(ActiveMap_ActivityConfig), new AppType[] { AppType.Game })]
    public class ActiveMap_ActivityConfigJson : C_ConfigJson
    {
        /// <summary>
        /// 反序列化后的配置数据
        /// </summary>
        public readonly Dictionary<int, ActiveMap_ActivityConfig> JsonDic = new Dictionary<int, ActiveMap_ActivityConfig>();
        /// <summary>
        /// 初始化配置表 更新表的信息
        /// </summary>
        /// <param name="b_ReadStr">新的数据集合</param>
        public ActiveMap_ActivityConfigJson(string b_ReadStr)
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
            List<ActiveMap_ActivityConfig> jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ActiveMap_ActivityConfig>>(b_ReadStr);
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
        public override Type ConfigType => typeof(ActiveMap_ActivityConfigJson);
    }
}