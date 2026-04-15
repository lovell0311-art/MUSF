using System;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork;
using CustomFrameWork.Baseic;
using Org.BouncyCastle.Asn1.Mozilla;

namespace ETModel
{
    /// <summary>
    /// 物品掉落类型
    /// </summary>
    public enum ItemDropType
    {
        /// <summary>
        /// 不掉落
        /// </summary>
        NoDrop,
        /// <summary>
        /// 普通白装
        /// </summary>
        Normal,
        /// <summary>
        /// 追加白装
        /// </summary>
        Append,
        /// <summary>
        /// 带有技能的普通白装
        /// </summary>
        Skill,
        /// <summary>
        /// 幸运装备
        /// </summary>
        Lucky,
        /// <summary>
        /// 卓越装备
        /// </summary>
        Excellent,
        /// <summary>
        /// 套装
        /// </summary>
        Set,
        /// <summary>
        /// 镶嵌装备
        /// </summary>
        Socket,
    }

    /// <summary>
    /// 物品掉落信息
    /// </summary>
    public struct ItemDropInfo
    {
        public int ItemConfigId;
        public ItemDropType DropType;
    }

    /// <summary>
    /// 物品默认掉落
    /// </summary>
    public struct ItemDefaultDrop
    {
        /// <summary>
        /// 装备掉落
        /// </summary>
        public RandomSelector<ItemDropInfo> Equip;
        /// <summary>
        /// 项链掉落
        /// </summary>
        public RandomSelector<ItemDropInfo> Necklace;
        /// <summary>
        /// 戒指掉落
        /// </summary>
        public RandomSelector<ItemDropInfo> Ring;
        /// <summary>
        /// 技能书掉落
        /// </summary>
        public RandomSelector<ItemDropInfo> SkillBook;
        /// <summary>
        /// 消耗品掉落
        /// </summary>
        public RandomSelector<ItemDropInfo> Consumables;

    }


    /// <summary>
    /// 物品默认掉落组件
    /// </summary>
    public class ItemDefaultDropComponent : TCustomComponent<MainFactory>
    {
        /// <summary>
        /// 支持的最大怪物等级
        /// </summary>
        public const int MAX_MONSTER_LEVEL = 300;

        public ItemDefaultDrop[] ItemDrop = new ItemDefaultDrop[MAX_MONSTER_LEVEL];

        public RandomSelector<int> DefaultDropStrengthenLevel = new RandomSelector<int>();

        public override void Dispose()
        {
            if (IsDisposeable) return;

            for(int i=0; i < MAX_MONSTER_LEVEL; i++)
            {
                ItemDrop[i].Equip?.Clear();
                ItemDrop[i].Necklace?.Clear();
                ItemDrop[i].Ring?.Clear();
                ItemDrop[i].SkillBook?.Clear();
                ItemDrop[i].Consumables?.Clear();
            }
            DefaultDropStrengthenLevel.Clear();

            base.Dispose();
        }
    }
}
