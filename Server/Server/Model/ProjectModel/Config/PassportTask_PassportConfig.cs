using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork.Component;

namespace ETModel
{
    /// <summary>
    /// R任务-点卡2.0.xlsx-通行证任务
    /// </summary>
    public partial class PassportTask_PassportConfig : C_ConfigInfo
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
        /// 任务行为类型
        /// </summary>
        public int TaskActionType { get; set; }
        /// <summary>
        /// 任务介绍
        /// </summary>
        public string TaskDes { get; set; }
        /// <summary>
        /// 地图
        /// </summary>
        public int MapId { get; set; }
        /// <summary>
        /// 自动寻路坐标
        /// </summary>
        public List<int> AutoPathPos { get; set; }
        /// <summary>
        /// 是否需要自动打怪
        /// </summary>
        public int IsAutoHitMonster { get; set; }
        /// <summary>
        /// 怪物名称
        /// </summary>
        public string MonsterName { get; set; }
        /// <summary>
        /// 任务目标id
        /// </summary>
        public List<int> TaskTargetId { get; set; }
        /// <summary>
        /// 任务目标数量
        /// </summary>
        public List<int> TaskTargetCount { get; set; }
        /// <summary>
        /// 自动领取奖励
        /// </summary>
        public int AutoReceiveReward { get; set; }
        /// <summary>
        /// 奖励经验
        /// </summary>
        public int RewardExp { get; set; }
        /// <summary>
        /// 奖励金币
        /// </summary>
        public int RewardCoin { get; set; }
        /// <summary>
        /// 奖励U币
        /// </summary>
        public int RewardUCoin { get; set; }
        /// <summary>
        /// 奖励物品
        /// </summary>
        public string RewardItems { get; set; }
        /// <summary>
        /// 前置任务ID
        /// </summary>
        public List<int> TaskBeforeId { get; set; }
        /// <summary>
        /// 需要最小等级
        /// </summary>
        public int ReqLevelMin { get; set; }
        /// <summary>
        /// 最大等级
        /// </summary>
        public int ReqLevelMax { get; set; }
        /// <summary>
        /// 法师
        /// </summary>
        public int Spell { get; set; }
        /// <summary>
        /// 剑士
        /// </summary>
        public int Swordsman { get; set; }
        /// <summary>
        /// 弓箭手
        /// </summary>
        public int Archer { get; set; }
        /// <summary>
        /// 魔剑士
        /// </summary>
        public int Spellsword { get; set; }
        /// <summary>
        /// 圣导师
        /// </summary>
        public int Holyteacher { get; set; }
        /// <summary>
        /// 召唤术士
        /// </summary>
        public int SummonWarlock { get; set; }
        /// <summary>
        /// 格斗
        /// </summary>
        public int Combat { get; set; }
        /// <summary>
        /// 梦幻骑士
        /// </summary>
        public int GrowLancer { get; set; }
    }
    /// <summary>
    /// 配置数据:R任务-点卡2.0.xlsx-通行证任务
    /// </summary>
    [ReadConfigAttribute(typeof(PassportTask_PassportConfig), new AppType[] { AppType.Game })]
    public class PassportTask_PassportConfigJson : C_ConfigJson
    {
        /// <summary>
        /// 反序列化后的配置数据
        /// </summary>
        public readonly Dictionary<int, PassportTask_PassportConfig> JsonDic = new Dictionary<int, PassportTask_PassportConfig>();
        /// <summary>
        /// 初始化配置表 更新表的信息
        /// </summary>
        /// <param name="b_ReadStr">新的数据集合</param>
        public PassportTask_PassportConfigJson(string b_ReadStr)
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
            List<PassportTask_PassportConfig> jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<PassportTask_PassportConfig>>(b_ReadStr);
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
        public override Type ConfigType => typeof(PassportTask_PassportConfigJson);
    }
}