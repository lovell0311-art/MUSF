using System;
using System.Collections.Generic;
using System.Text;

namespace ETModel.Robot
{
    public static class AI_ReplaceEquipItem
    {
        public static readonly HashSet<EItemType> EquipItemTypeList = new HashSet<EItemType>(){
            EItemType.Swords,
            EItemType.Axes,
            EItemType.Maces,
            EItemType.Bows,
            EItemType.Crossbows,
            EItemType.Arrow,
            EItemType.Spears,
            EItemType.Staffs,
            EItemType.MagicBook,
            EItemType.Scepter,
            EItemType.RuneWand,
            EItemType.FistBlade,
            EItemType.MagicSword,
            EItemType.ShortSword,
            EItemType.MagicGun,
            EItemType.Shields,
            EItemType.Helms,
            EItemType.Armors,
            EItemType.Pants,
            EItemType.Gloves,
            EItemType.Boots,
            EItemType.Wing,
            EItemType.Necklace,
            EItemType.Rings,
        };
    }
}
