using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork.Component;

namespace ETModel
{
    /// <summary>
    /// G怪物_点卡2.0.xlsx-挑战Boss
    /// </summary>
    public partial class Enemy_ChallengeConfig : C_ConfigInfo
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 怪物ID
        /// </summary>
        public int MonsterId { get; set; }
        /// <summary>
        /// 怪物类型
        /// </summary>
        public int MonsterType { get; set; }
        /// <summary>
        /// 刷新地点
        /// </summary>
        public string RefreshPlace { get; set; }
        /// <summary>
        /// 刷新时间
        /// </summary>
        public string RefreshTime { get; set; }
        /// <summary>
        /// 掉落物品
        /// </summary>
        public List<int> Equip { get; set; }
    }
    /// <summary>
    /// 配置数据:G怪物_点卡2.0.xlsx-挑战Boss
    /// </summary>
    [ReadConfigAttribute(typeof(Enemy_ChallengeConfig), new AppType[] { AppType.Game,AppType.Map })]
    public class Enemy_ChallengeConfigJson : C_ConfigJson
    {
        /// <summary>
        /// 反序列化后的配置数据
        /// </summary>
        public readonly Dictionary<int, Enemy_ChallengeConfig> JsonDic = new Dictionary<int, Enemy_ChallengeConfig>();
        /// <summary>
        /// 初始化配置表 更新表的信息
        /// </summary>
        /// <param name="b_ReadStr">新的数据集合</param>
        public Enemy_ChallengeConfigJson(string b_ReadStr)
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
            List<Enemy_ChallengeConfig> jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Enemy_ChallengeConfig>>(b_ReadStr);
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
        public override Type ConfigType => typeof(Enemy_ChallengeConfigJson);
    }
}