using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CustomFrameWork;

namespace ETModel
{
    namespace EventType.NSMapItem
    {
        /// <summary>
        /// 通知 Hotfix 层，自己要离开Map
        /// </summary>
        public class Destory
        {
            public static readonly Destory Instance = new Destory();
            public ETModel.MapItem self;
        }
    }

    public enum EMapItemCreateType
    {
        /// <summary>其他</summary>
        Other = 0,
        /// <summary>玩家丢弃</summary>
        Discard = 1,
        /// <summary>怪物掉落</summary>
        MonsterDrop = 2,
        /// <summary>宝箱掉落</summary>
        TChestDrop = 3,
    }

    public class MapItem : MapEntity
    {
        public static readonly ETHotfix.G2C_ItemDrop_notice g2C_ItemDrop_notice = new ETHotfix.G2C_ItemDrop_notice();
        public static readonly ETHotfix.G2C_BattlePickUpDropItem_notice g2C_BattlePickUpDropItem_notice = new ETHotfix.G2C_BattlePickUpDropItem_notice();
        public ETHotfix.G2C_ItemDropData g2C_ItemDropData = new ETHotfix.G2C_ItemDropData();
        public static long MonsterDeathCount = 0;

        public long ItemUid { get; set; } = 0;
        public int ConfigId { get; set; } = 0;
        public int Level { get; set; } = 0;
        public long ProtectTick { get; set; }
        public int Count { get; set; } = 1;
        public int Quality { get; set; } = 0;
        public int SetId { get; set; } = 0;
        /// <summary> 追加 </summary>
        public int OptListId { get; set; } = 0;
        public int OptLevel { get; set; } = 0;
        public List<long> KillerId { get; private set; } = new List<long>();

        /// <summary> 物品所属玩家 </summary>
        public long OwnerUserId { get; set; } = 0;
        /// <summary> 物品所属角色 </summary>
        public long OwnerGameUserId { get; set; } = 0;

        public Item Item { get; private set; } = null;

        public EMapItemCreateType CreateType { get; set; } = EMapItemCreateType.Other;

        /// <summary> 掉落怪物id </summary>
        public int MonsterConfigId { get; set; } = 0;
        public void SetItem(Item b_Item)
        {
            Item = b_Item;
        }

        public override void Dispose()
        {
            if (IsDisposeable) return;

            // 方法都在Hotfix层，抛个事件让Hotfix去处理
            EventType.NSMapItem.Destory.Instance.self = this;
            Root.EventSystem.OnRun("NSMapItem.Destory", EventType.NSMapItem.Destory.Instance);

            base.Dispose();

            ItemUid = 0;
            ConfigId = 0;
            Level = 0;
            ProtectTick = 0;
            ConfigId = 0;
            Count = 1;
            Quality = 0;
            SetId = 0;
            CreateType = EMapItemCreateType.Other;
            MonsterConfigId = 0;
            OwnerUserId = 0;
            OwnerGameUserId = 0;
            if (KillerId.Count > 0) KillerId.Clear();

            if (Item != null)
            {
                Item.Dispose();
                Item = null;
            }
        }
    }
}