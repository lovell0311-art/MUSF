using System;
using System.Collections.Generic;
using System.Text;

namespace ETModel
{
    /// <summary>
    /// 物品，需要保存到数据库中的属性
    /// </summary>
    public enum EDBItemValue
    {
        Durability = EItemValue.Durability,
        Level = EItemValue.Level,
        SkillId = EItemValue.SkillId,
        IsBind = EItemValue.IsBind,
        IsTask = EItemValue.IsTask,
        ValidTime = EItemValue.ValidTime,
        MountsLevel = EItemValue.MountsLevel,
        MountsExp = EItemValue.MountsExp,
        Quantity = EItemValue.Quantity,
        OptValue = EItemValue.OptValue,
        OptLevel = EItemValue.OptLevel,
        LuckyEquip = EItemValue.LuckyEquip,
        OrecycledID = EItemValue.OrecycledID,
        OrecycledLevel = EItemValue.OrecycledLevel,
        SetId = EItemValue.SetId,
        FluoreAttr = EItemValue.FluoreAttr,
        FluoreSlotCount = EItemValue.FluoreSlotCount,
        FluoreSlot1 = EItemValue.FluoreSlot1,
        FluoreSlot2 = EItemValue.FluoreSlot2,
        FluoreSlot3 = EItemValue.FluoreSlot3,
        FluoreSlot4 = EItemValue.FluoreSlot4,
        FluoreSlot5 = EItemValue.FluoreSlot5,
        TimeLimit = EItemValue.TimeLimit,
        TreasureMapId = EItemValue.TreasureMapId,
        TreasureKeyIdA = EItemValue.TreasureKeyIdA,
        TreasureKeyIdB = EItemValue.TreasureKeyIdB,
        TreasureZoneId = EItemValue.TreasureZoneId,
        TreasureNpcConfigId = EItemValue.TreasureNpcConfigId,
        Advanced = EItemValue.Advanced,
    }
}
