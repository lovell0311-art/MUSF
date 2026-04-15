using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork.Component;

namespace ETModel
{
    /// <summary>
    /// G怪物_点卡2.0.xlsx-怪物
    /// </summary>
    public partial class Enemy_InfoConfig : C_ConfigInfo
    {
        /// <summary>
        /// Id
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
        /// 攻击特效
        /// </summary>
        public string AttackEffect { get; set; }
        /// <summary>
        /// 待机音效
        /// </summary>
        public string Sound_Idle { get; set; }
        /// <summary>
        /// 攻击音效
        /// </summary>
        public string Sound_Attack { get; set; }
        /// <summary>
        /// 被击音效
        /// </summary>
        public string Sound_Hit { get; set; }
        /// <summary>
        /// 死亡音效
        /// </summary>
        public string Sound_Dead { get; set; }
        /// <summary>
        /// 怪物类型
        /// </summary>
        public int Monster_Type { get; set; }
        /// <summary>
        /// 等级
        /// </summary>
        public int Lvl { get; set; }
        /// <summary>
        /// 生命
        /// </summary>
        public int HP { get; set; }
        /// <summary>
        /// 被击次数
        /// </summary>
        public int BeHitCnt { get; set; }
        /// <summary>
        /// 魔法
        /// </summary>
        public int MP { get; set; }
        /// <summary>
        /// 最小攻击
        /// </summary>
        public int DmgMin { get; set; }
        /// <summary>
        /// 最大攻击
        /// </summary>
        public int DmgMax { get; set; }
        /// <summary>
        /// 防御
        /// </summary>
        public int Def { get; set; }
        /// <summary>
        /// 攻击成功率
        /// </summary>
        public int AttRate { get; set; }
        /// <summary>
        /// 防御成功率
        /// </summary>
        public int BloRate { get; set; }
        /// <summary>
        /// 巡逻范围
        /// </summary>
        public int Ran { get; set; }
        /// <summary>
        /// 攻击类型
        /// </summary>
        public string AttackType { get; set; }
        /// <summary>
        /// 攻击范围
        /// </summary>
        public int AR { get; set; }
        /// <summary>
        /// 视野范围
        /// </summary>
        public int VR { get; set; }
        /// <summary>
        /// 移动速度
        /// </summary>
        public int MoSpeed { get; set; }
        /// <summary>
        /// 攻击速度
        /// </summary>
        public int AtSpeed { get; set; }
        /// <summary>
        /// 复活时间(毫秒)
        /// </summary>
        public int Regen { get; set; }
        /// <summary>
        /// 冰抗
        /// </summary>
        public int ICE { get; set; }
        /// <summary>
        /// 毒抗
        /// </summary>
        public int POI { get; set; }
        /// <summary>
        /// 雷抗
        /// </summary>
        public int LIG { get; set; }
        /// <summary>
        /// 火抗
        /// </summary>
        public int FIR { get; set; }
        /// <summary>
        /// 特殊掉落组ID
        /// </summary>
        public int SpecialDrop { get; set; }
    }
    /// <summary>
    /// 配置数据:G怪物_点卡2.0.xlsx-怪物
    /// </summary>
    [ReadConfigAttribute(typeof(Enemy_InfoConfig), new AppType[] { AppType.Game,AppType.Map,AppType.Robot })]
    public class Enemy_InfoConfigJson : C_ConfigJson
    {
        /// <summary>
        /// 反序列化后的配置数据
        /// </summary>
        public readonly Dictionary<int, Enemy_InfoConfig> JsonDic = new Dictionary<int, Enemy_InfoConfig>();
        /// <summary>
        /// 初始化配置表 更新表的信息
        /// </summary>
        /// <param name="b_ReadStr">新的数据集合</param>
        public Enemy_InfoConfigJson(string b_ReadStr)
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
            List<Enemy_InfoConfig> jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Enemy_InfoConfig>>(b_ReadStr);
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
        public override Type ConfigType => typeof(Enemy_InfoConfigJson);
    }
}