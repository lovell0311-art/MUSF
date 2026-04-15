using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork.Component;

namespace ETModel
{
    /// <summary>
    /// C宠物-点卡2.0.xlsx-宠物
    /// </summary>
    public partial class Pets_InfoConfig : C_ConfigInfo
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
        /// 宠物模型资源名
        /// </summary>
        public string PetModleAsset { get; set; }
        /// <summary>
        /// 宠物图标资源名
        /// </summary>
        public string PetAsset { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public int PetsType { get; set; }
        /// <summary>
        /// 攻击类型
        /// </summary>
        public int AttackType { get; set; }
        /// <summary>
        /// 力量
        /// </summary>
        public int Strength { get; set; }
        /// <summary>
        /// 敏捷
        /// </summary>
        public int Agility { get; set; }
        /// <summary>
        /// 体力
        /// </summary>
        public int BoneGas { get; set; }
        /// <summary>
        /// 智力
        /// </summary>
        public int Willpower { get; set; }
        /// <summary>
        /// 初始技能
        /// </summary>
        public List<int> SkillID { get; set; }
        /// <summary>
        /// 试用时间(秒)
        /// </summary>
        public int TrialTime { get; set; }
        /// <summary>
        /// 升级获得
        /// </summary>
        public int AppendLevel { get; set; }
        /// <summary>
        /// 巡逻范围
        /// </summary>
        public int Ran { get; set; }
        /// <summary>
        /// 视野范围
        /// </summary>
        public int VR { get; set; }
        /// <summary>
        /// 攻击距离
        /// </summary>
        public int AttackDistance { get; set; }
        /// <summary>
        /// 移动速度
        /// </summary>
        public int MoSpeed { get; set; }
        /// <summary>
        /// 攻击间隔(毫秒)
        /// </summary>
        public int AtSpeed { get; set; }
        /// <summary>
        /// 复活时间(毫秒)
        /// </summary>
        public int Regen { get; set; }
        /// <summary>
        /// 背包物品
        /// </summary>
        public int BeakId { get; set; }
        /// <summary>
        /// 强化属性
        /// </summary>
        public List<int> EA { get; set; }
    }
    /// <summary>
    /// 配置数据:C宠物-点卡2.0.xlsx-宠物
    /// </summary>
    [ReadConfigAttribute(typeof(Pets_InfoConfig), new AppType[] { AppType.Game,AppType.Map,AppType.Robot })]
    public class Pets_InfoConfigJson : C_ConfigJson
    {
        /// <summary>
        /// 反序列化后的配置数据
        /// </summary>
        public readonly Dictionary<int, Pets_InfoConfig> JsonDic = new Dictionary<int, Pets_InfoConfig>();
        /// <summary>
        /// 初始化配置表 更新表的信息
        /// </summary>
        /// <param name="b_ReadStr">新的数据集合</param>
        public Pets_InfoConfigJson(string b_ReadStr)
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
            List<Pets_InfoConfig> jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Pets_InfoConfig>>(b_ReadStr);
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
        public override Type ConfigType => typeof(Pets_InfoConfigJson);
    }
}