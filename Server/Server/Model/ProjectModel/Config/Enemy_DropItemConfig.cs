using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork.Component;

namespace ETModel
{
    /// <summary>
    /// G怪物_点卡2.0.xlsx-掉落物品
    /// </summary>
    public partial class Enemy_DropItemConfig : C_ConfigInfo
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 怪物等级
        /// </summary>
        public int MonsterLevel { get; set; }
        /// <summary>
        /// 装备
        /// </summary>
        public string Equip { get; set; }
        /// <summary>
        /// 项链
        /// </summary>
        public string Necklace { get; set; }
        /// <summary>
        /// 戒指
        /// </summary>
        public string Rings { get; set; }
        /// <summary>
        /// 技能书
        /// </summary>
        public string SkillBooks { get; set; }
        /// <summary>
        /// 守护
        /// </summary>
        public string Guard { get; set; }
        /// <summary>
        /// 消耗品
        /// </summary>
        public string Consumables { get; set; }
    }
    /// <summary>
    /// 配置数据:G怪物_点卡2.0.xlsx-掉落物品
    /// </summary>
    [ReadConfigAttribute(typeof(Enemy_DropItemConfig), new AppType[] { AppType.Game,AppType.Map })]
    public class Enemy_DropItemConfigJson : C_ConfigJson
    {
        /// <summary>
        /// 反序列化后的配置数据
        /// </summary>
        public readonly Dictionary<int, Enemy_DropItemConfig> JsonDic = new Dictionary<int, Enemy_DropItemConfig>();
        /// <summary>
        /// 初始化配置表 更新表的信息
        /// </summary>
        /// <param name="b_ReadStr">新的数据集合</param>
        public Enemy_DropItemConfigJson(string b_ReadStr)
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
            List<Enemy_DropItemConfig> jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Enemy_DropItemConfig>>(b_ReadStr);
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
        public override Type ConfigType => typeof(Enemy_DropItemConfigJson);
    }
}