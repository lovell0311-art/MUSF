using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork.Component;

namespace ETModel
{
    /// <summary>
    /// J角色-点卡2.0.xlsx-角色
    /// </summary>
    public partial class CreateRole_InfoConfig : C_ConfigInfo
    {
        /// <summary>
        /// 唯一id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 名字
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Desc { get; set; }
        /// <summary>
        /// 性別
        /// </summary>
        public int Sex { get; set; }
        /// <summary>
        /// 生命值
        /// </summary>
        public int Hp { get; set; }
        /// <summary>
        /// 魔法值
        /// </summary>
        public int Mp { get; set; }
        /// <summary>
        /// 力量
        /// </summary>
        public int Strength { get; set; }
        /// <summary>
        /// 智力
        /// </summary>
        public int Willpower { get; set; }
        /// <summary>
        /// 体力
        /// </summary>
        public int BoneGas { get; set; }
        /// <summary>
        /// 敏捷
        /// </summary>
        public int Agility { get; set; }
        /// <summary>
        /// 统率
        /// </summary>
        public int Command { get; set; }
        /// <summary>
        /// 攻击距离
        /// </summary>
        public int AttackDistance { get; set; }
        /// <summary>
        /// 技能攻击力(百分比)
        /// </summary>
        public int SkillAddition { get; set; }
        /// <summary>
        /// 初始地图位置
        /// </summary>
        public int InitMap { get; set; }
        /// <summary>
        /// 升级获得
        /// </summary>
        public string AppendLevel { get; set; }
        /// <summary>
        /// 大师点数
        /// </summary>
        public string MasterPoints { get; set; }
        /// <summary>
        /// 死亡时间(毫秒)
        /// </summary>
        public int DeathSleepTime { get; set; }
    }
    /// <summary>
    /// 配置数据:J角色-点卡2.0.xlsx-角色
    /// </summary>
    [ReadConfigAttribute(typeof(CreateRole_InfoConfig), new AppType[] { AppType.Game,AppType.Robot })]
    public class CreateRole_InfoConfigJson : C_ConfigJson
    {
        /// <summary>
        /// 反序列化后的配置数据
        /// </summary>
        public readonly Dictionary<int, CreateRole_InfoConfig> JsonDic = new Dictionary<int, CreateRole_InfoConfig>();
        /// <summary>
        /// 初始化配置表 更新表的信息
        /// </summary>
        /// <param name="b_ReadStr">新的数据集合</param>
        public CreateRole_InfoConfigJson(string b_ReadStr)
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
            List<CreateRole_InfoConfig> jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CreateRole_InfoConfig>>(b_ReadStr);
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
        public override Type ConfigType => typeof(CreateRole_InfoConfigJson);
    }
}