using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork.Component;

namespace ETModel
{
    /// <summary>
    /// W物品-点卡2.0.xlsx-消耗品(血瓶|药水|实力提升卷轴)
    /// </summary>
    public partial class Item_ConsumablesConfig : C_ConfigInfo
    {
        /// <summary>
        /// 物品ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 名字
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 资源名
        /// </summary>
        public string ResName { get; set; }
        /// <summary>
        /// 装备卡槽
        /// </summary>
        public int Slot { get; set; }
        /// <summary>
        /// 技能
        /// </summary>
        public int Skill { get; set; }
        /// <summary>
        /// 宽
        /// </summary>
        public int X { get; set; }
        /// <summary>
        /// 高
        /// </summary>
        public int Y { get; set; }
        /// <summary>
        /// 单组数量
        /// </summary>
        public int StackSize { get; set; }
        /// <summary>
        /// 可以掉落
        /// </summary>
        public int Drop { get; set; }
        /// <summary>
        /// 物品等级
        /// </summary>
        public int Level { get; set; }
        /// <summary>
        /// 普通装备掉落
        /// </summary>
        public int NormalDropWeight { get; set; }
        /// <summary>
        /// 使用方法
        /// </summary>
        public string UseMethod { get; set; }
        /// <summary>
        /// 操作值
        /// </summary>
        public int Value { get; set; }
        /// <summary>
        /// 操作值2
        /// </summary>
        public int Value2 { get; set; }
        /// <summary>
        /// 需求等级
        /// </summary>
        public int ReqLvl { get; set; }
        /// <summary>
        /// 力量
        /// </summary>
        public int ReqStr { get; set; }
        /// <summary>
        /// 敏捷
        /// </summary>
        public int ReqAgi { get; set; }
        /// <summary>
        /// 体力
        /// </summary>
        public int ReqVit { get; set; }
        /// <summary>
        /// 智力
        /// </summary>
        public int ReqEne { get; set; }
        /// <summary>
        /// 统率
        /// </summary>
        public int ReqCom { get; set; }
        /// <summary>
        /// 使用职业
        /// </summary>
        public string UseRole { get; set; }
        /// <summary>
        /// 提示
        /// </summary>
        public string Prompt { get; set; }
        /// <summary>
        /// 更新属性方法
        /// </summary>
        public string UpdatePropMethod { get; set; }
        /// <summary>
        /// 是否出售
        /// </summary>
        public int Sell { get; set; }
    }
    /// <summary>
    /// 配置数据:W物品-点卡2.0.xlsx-消耗品(血瓶|药水|实力提升卷轴)
    /// </summary>
    [ReadConfigAttribute(typeof(Item_ConsumablesConfig), new AppType[] { AppType.Game,AppType.Robot,AppType.GM })]
    public class Item_ConsumablesConfigJson : C_ConfigJson
    {
        /// <summary>
        /// 反序列化后的配置数据
        /// </summary>
        public readonly Dictionary<int, Item_ConsumablesConfig> JsonDic = new Dictionary<int, Item_ConsumablesConfig>();
        /// <summary>
        /// 初始化配置表 更新表的信息
        /// </summary>
        /// <param name="b_ReadStr">新的数据集合</param>
        public Item_ConsumablesConfigJson(string b_ReadStr)
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
            List<Item_ConsumablesConfig> jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Item_ConsumablesConfig>>(b_ReadStr);
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
        public override Type ConfigType => typeof(Item_ConsumablesConfigJson);
    }
}