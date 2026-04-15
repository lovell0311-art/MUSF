using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork.Component;

namespace ETModel
{
    /// <summary>
    /// C充值礼包-点卡2.0.xlsx-充值礼包类型
    /// </summary>
    public partial class RechargeGiftBundle_TypeConfig : C_ConfigInfo
    {
        /// <summary>
        /// 捆绑包id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 名字
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 价格
        /// </summary>
        public int Price { get; set; }
        /// <summary>
        /// 原价
        /// </summary>
        public int OriginPrice { get; set; }
        /// <summary>
        /// 上架时间
        /// </summary>
        public string StartTime { get; set; }
        /// <summary>
        /// 持续时间(秒)
        /// </summary>
        public int DurationTime { get; set; }
        /// <summary>
        /// 购买次数
        /// </summary>
        public int BuyCount { get; set; }
        /// <summary>
        /// 购买次数绑定类型(0.账号 1.角色)
        /// </summary>
        public int BindType { get; set; }
    }
    /// <summary>
    /// 配置数据:C充值礼包-点卡2.0.xlsx-充值礼包类型
    /// </summary>
    [ReadConfigAttribute(typeof(RechargeGiftBundle_TypeConfig), new AppType[] { AppType.Game })]
    public class RechargeGiftBundle_TypeConfigJson : C_ConfigJson
    {
        /// <summary>
        /// 反序列化后的配置数据
        /// </summary>
        public readonly Dictionary<int, RechargeGiftBundle_TypeConfig> JsonDic = new Dictionary<int, RechargeGiftBundle_TypeConfig>();
        /// <summary>
        /// 初始化配置表 更新表的信息
        /// </summary>
        /// <param name="b_ReadStr">新的数据集合</param>
        public RechargeGiftBundle_TypeConfigJson(string b_ReadStr)
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
            List<RechargeGiftBundle_TypeConfig> jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<RechargeGiftBundle_TypeConfig>>(b_ReadStr);
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
        public override Type ConfigType => typeof(RechargeGiftBundle_TypeConfigJson);
    }
}