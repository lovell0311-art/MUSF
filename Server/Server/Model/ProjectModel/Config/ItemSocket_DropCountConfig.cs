using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork.Component;

namespace ETModel
{
    /// <summary>
    /// X镶嵌-点卡2.0.xlsx-镶嵌孔洞掉落数
    /// </summary>
    public partial class ItemSocket_DropCountConfig : C_ConfigInfo
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 孔数
        /// </summary>
        public int SocketCnt { get; set; }
        /// <summary>
        /// 概率(权重)
        /// </summary>
        public int Rate { get; set; }
    }
    /// <summary>
    /// 配置数据:X镶嵌-点卡2.0.xlsx-镶嵌孔洞掉落数
    /// </summary>
    [ReadConfigAttribute(typeof(ItemSocket_DropCountConfig), new AppType[] { AppType.Game })]
    public class ItemSocket_DropCountConfigJson : C_ConfigJson
    {
        /// <summary>
        /// 反序列化后的配置数据
        /// </summary>
        public readonly Dictionary<int, ItemSocket_DropCountConfig> JsonDic = new Dictionary<int, ItemSocket_DropCountConfig>();
        /// <summary>
        /// 初始化配置表 更新表的信息
        /// </summary>
        /// <param name="b_ReadStr">新的数据集合</param>
        public ItemSocket_DropCountConfigJson(string b_ReadStr)
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
            List<ItemSocket_DropCountConfig> jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ItemSocket_DropCountConfig>>(b_ReadStr);
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
        public override Type ConfigType => typeof(ItemSocket_DropCountConfigJson);
    }
}