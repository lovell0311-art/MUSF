using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork.Component;

namespace ETModel
{
    /// <summary>
    /// W物品-点卡2.0.xlsx-项链
    /// </summary>
    public partial class Item_NecklaceConfig : C_ConfigInfo
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
        /// 品质类型
        /// </summary>
        public int QualityAttr { get; set; }
        /// <summary>
        /// 物品等级
        /// </summary>
        public int Level { get; set; }
        /// <summary>
        /// 普通装备掉落
        /// </summary>
        public int NormalDropWeight { get; set; }
        /// <summary>
        /// 追加装备掉落
        /// </summary>
        public int AppendDropWeight { get; set; }
        /// <summary>
        /// 卓越装备掉落
        /// </summary>
        public int ExcellentDropWeight { get; set; }
        /// <summary>
        /// 套装装备掉落
        /// </summary>
        public int SetDropWeight { get; set; }
        /// <summary>
        /// 镶嵌装备掉落
        /// </summary>
        public int SocketDropWeight { get; set; }
        /// <summary>
        /// 追加属性
        /// </summary>
        public List<int> AppendAttrId { get; set; }
        /// <summary>
        /// 基础属性
        /// </summary>
        public List<int> BaseAttrId { get; set; }
        /// <summary>
        /// 额外属性
        /// </summary>
        public List<int> ExtraAttrId { get; set; }
        /// <summary>
        /// 耐久
        /// </summary>
        public int Durable { get; set; }
        /// <summary>
        /// 需求等级
        /// </summary>
        public int ReqLvl { get; set; }
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
    /// 配置数据:W物品-点卡2.0.xlsx-项链
    /// </summary>
    [ReadConfigAttribute(typeof(Item_NecklaceConfig), new AppType[] { AppType.Game,AppType.Robot,AppType.GM })]
    public class Item_NecklaceConfigJson : C_ConfigJson
    {
        /// <summary>
        /// 反序列化后的配置数据
        /// </summary>
        public readonly Dictionary<int, Item_NecklaceConfig> JsonDic = new Dictionary<int, Item_NecklaceConfig>();
        /// <summary>
        /// 初始化配置表 更新表的信息
        /// </summary>
        /// <param name="b_ReadStr">新的数据集合</param>
        public Item_NecklaceConfigJson(string b_ReadStr)
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
            List<Item_NecklaceConfig> jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Item_NecklaceConfig>>(b_ReadStr);
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
        public override Type ConfigType => typeof(Item_NecklaceConfigJson);
    }
}