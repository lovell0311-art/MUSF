using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork.Component;

namespace ETModel
{
    /// <summary>
    /// F副本-点卡2.0.xlsx-血色评分
    /// </summary>
    public partial class BattleCopy_RedCastleConfig : C_ConfigInfo
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 攻破城门
        /// </summary>
        public int Door { get; set; }
        /// <summary>
        /// 打开水晶
        /// </summary>
        public int Crystal { get; set; }
        /// <summary>
        /// 交换武器
        /// </summary>
        public int Weapon { get; set; }
        /// <summary>
        /// 剩余系数
        /// </summary>
        public int Time { get; set; }
    }
    /// <summary>
    /// 配置数据:F副本-点卡2.0.xlsx-血色评分
    /// </summary>
    [ReadConfigAttribute(typeof(BattleCopy_RedCastleConfig), new AppType[] { AppType.Game })]
    public class BattleCopy_RedCastleConfigJson : C_ConfigJson
    {
        /// <summary>
        /// 反序列化后的配置数据
        /// </summary>
        public readonly Dictionary<int, BattleCopy_RedCastleConfig> JsonDic = new Dictionary<int, BattleCopy_RedCastleConfig>();
        /// <summary>
        /// 初始化配置表 更新表的信息
        /// </summary>
        /// <param name="b_ReadStr">新的数据集合</param>
        public BattleCopy_RedCastleConfigJson(string b_ReadStr)
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
            List<BattleCopy_RedCastleConfig> jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<BattleCopy_RedCastleConfig>>(b_ReadStr);
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
        public override Type ConfigType => typeof(BattleCopy_RedCastleConfigJson);
    }
}