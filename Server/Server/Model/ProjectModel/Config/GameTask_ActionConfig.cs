using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork.Component;

namespace ETModel
{
    /// <summary>
    /// R任务-点卡2.0.xlsx-任务行为
    /// </summary>
    public partial class GameTask_ActionConfig : C_ConfigInfo
    {
        /// <summary>
        /// 任务行为Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 任务进度更新行为
        /// </summary>
        public int TaskProgressType { get; set; }
    }
    /// <summary>
    /// 配置数据:R任务-点卡2.0.xlsx-任务行为
    /// </summary>
    [ReadConfigAttribute(typeof(GameTask_ActionConfig), new AppType[] { AppType.Game })]
    public class GameTask_ActionConfigJson : C_ConfigJson
    {
        /// <summary>
        /// 反序列化后的配置数据
        /// </summary>
        public readonly Dictionary<int, GameTask_ActionConfig> JsonDic = new Dictionary<int, GameTask_ActionConfig>();
        /// <summary>
        /// 初始化配置表 更新表的信息
        /// </summary>
        /// <param name="b_ReadStr">新的数据集合</param>
        public GameTask_ActionConfigJson(string b_ReadStr)
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
            List<GameTask_ActionConfig> jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<GameTask_ActionConfig>>(b_ReadStr);
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
        public override Type ConfigType => typeof(GameTask_ActionConfigJson);
    }
}