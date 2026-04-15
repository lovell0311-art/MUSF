using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork.Component;

namespace ETModel
{
    /// <summary>
    /// R任务-点卡2.0.xlsx-任务物品掉落
    /// </summary>
    public partial class GameTask_DropItemConfig : C_ConfigInfo
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 怪物id
        /// </summary>
        public int MonsterId { get; set; }
        /// <summary>
        /// 任务id
        /// </summary>
        public int TaskId { get; set; }
        /// <summary>
        /// 任务目标id
        /// </summary>
        public int TaskTargetId { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 物品id
        /// </summary>
        public int ItemId { get; set; }
        /// <summary>
        /// 强化等级
        /// </summary>
        public int Level { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public int Quantity { get; set; }
        /// <summary>
        /// 追加列表id
        /// </summary>
        public int OptListId { get; set; }
        /// <summary>
        /// 追加等级
        /// </summary>
        public int OptLevel { get; set; }
        /// <summary>
        /// 拥有技能
        /// </summary>
        public int HasSkill { get; set; }
        /// <summary>
        /// 套装id
        /// </summary>
        public int SetId { get; set; }
        /// <summary>
        /// 绑定物品
        /// </summary>
        public int IsBind { get; set; }
        /// <summary>
        /// 任务物品
        /// </summary>
        public int IsTask { get; set; }
        /// <summary>
        /// 卓越属性
        /// </summary>
        public List<int> OptionExcellent { get; set; }
        /// <summary>
        /// 自定义属性方法
        /// </summary>
        public string CustomAttrMathod { get; set; }
        /// <summary>
        /// 掉落率
        /// </summary>
        public int DropRate { get; set; }
    }
    /// <summary>
    /// 配置数据:R任务-点卡2.0.xlsx-任务物品掉落
    /// </summary>
    [ReadConfigAttribute(typeof(GameTask_DropItemConfig), new AppType[] { AppType.Game })]
    public class GameTask_DropItemConfigJson : C_ConfigJson
    {
        /// <summary>
        /// 反序列化后的配置数据
        /// </summary>
        public readonly Dictionary<int, GameTask_DropItemConfig> JsonDic = new Dictionary<int, GameTask_DropItemConfig>();
        /// <summary>
        /// 初始化配置表 更新表的信息
        /// </summary>
        /// <param name="b_ReadStr">新的数据集合</param>
        public GameTask_DropItemConfigJson(string b_ReadStr)
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
            List<GameTask_DropItemConfig> jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<GameTask_DropItemConfig>>(b_ReadStr);
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
        public override Type ConfigType => typeof(GameTask_DropItemConfigJson);
    }
}