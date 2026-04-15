using System;
using System.Collections.Generic;
using System.Text;

namespace ETModel
{
    /// <summary>
    /// 物品基础配置
    /// </summary>
    public partial class ItemConfig : ICloneable
    {
        /// <summary>
        /// 配置ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 名字
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 种类名
        /// </summary>
        public string KindName { get; set; }
        /// <summary>
        /// 种类名 对应的种类id。(每次重启服务器都会变)
        /// </summary>
        public int KindId { get; set; }
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
        /// 追加属性
        /// </summary>
        public List<int> AppendAttrId { get; set; } = new List<int>();
        /// <summary>
        /// 额外属性
        /// </summary>
        public List<int> ExtraAttrId { get; set; } = new List<int>();
        /// <summary>
        /// 额外属性
        /// </summary>
        public List<int> ExtraAttrId2 { get; set; } = new List<int>();
        /// <summary>
        /// 400
        /// </summary>
        public int Is400 { get; set; }
        /// <summary>
        /// 双手武器
        /// </summary>
        public int TwoHand { get; set; }
        /// <summary>
        /// 最小伤害
        /// </summary>
        public int DamageMin { get; set; }
        /// <summary>
        /// 最大伤害
        /// </summary>
        public int DamageMax { get; set; }
        /// <summary>
        /// 诅咒
        /// </summary>
        public int Curse { get; set; }
        /// <summary>
        /// 宠物提升
        /// </summary>
        public int UpPet { get; set; }
        /// <summary>
        /// 魔力百分比
        /// </summary>
        public int MagicPct { get; set; }
        /// <summary>
        /// 攻击速度
        /// </summary>
        public int AttackSpeed { get; set; }
        /// <summary>
        /// 移动速度
        /// </summary>
        public int WalkSpeed { get; set; }
        /// <summary>
        /// 防御
        /// </summary>
        public int Defense { get; set; }
        /// <summary>
        /// 防御率
        /// </summary>
        public int DefenseRate { get; set; }
        /// <summary>
        /// 耐久
        /// </summary>
        public int Durable { get; set; }
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
        public Dictionary<int, int> UseRole  = new Dictionary<int, int>();
        /// <summary>
        /// 更新属性方法
        /// </summary>
        public string UpdatePropMethod { get; set; }
        /// <summary>
        /// 检查是否可以上架藏宝阁
        /// </summary>
        public int Sell { get; set; }
        /// <summary>
        /// 宠物Id
        /// </summary>
        public int PetsId { get; set; }
        /// <summary>
        /// 浅拷贝
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return this.MemberwiseClone();
        }


        private static Dictionary<string, int> KindName2Id = new Dictionary<string, int>();
        private static int GenKindId = 0;

        /// <summary>
        /// 获取种类id
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static int GetKindId(string name)
        {
            if (string.IsNullOrEmpty(name)) return 0;
            int id;
            if(KindName2Id.TryGetValue(name ,out id))
            {
                return id;
            }
            else
            {
                id = (++GenKindId);
                KindName2Id.Add(name, id);
            }
            return id;
        }
    }
}
