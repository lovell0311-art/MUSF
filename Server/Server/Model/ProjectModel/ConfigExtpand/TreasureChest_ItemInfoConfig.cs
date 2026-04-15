using System;
using System.Collections.Generic;
using System.Text;

namespace ETModel
{
    public partial class TreasureChest_ItemInfoConfig
    {

        public ItemCreateAttr ToItemCreateAttr()
        {
            ItemCreateAttr info = new ItemCreateAttr();
            info.Level = Level;
            info.Quantity = Quantity;
            info.OptListId = OptListId;
            info.OptLevel = OptLevel;
            info.HaveSkill = HasSkill == 1 ? true : false;
            info.SetId = SetId;
            //info.IsBind = 
            info.OptionExcellent = OptionExcellent;
            if (CustomAttrMathod != null)
            {
                info.CustomAttrMethod.AddRange(CustomAttrMathod.Split(","));
            }
            return info;
        }
    }

    public partial class DropItem_SpecialConfig
    {

        public ItemCreateAttr ToItemCreateAttr()
        {
            ItemCreateAttr info = new ItemCreateAttr();
            info.Level = Level;
            info.Quantity = Quantity;
            info.OptListId = OptListId;
            info.OptLevel = OptLevel;
            info.HaveSkill = HasSkill == 1 ? true : false;
            info.SetId = SetId;
            info.IsBind = IsTaskItem == 1 ? 2: 0 ;
            info.OptionExcellent = OptionExcellent;
            if (CustomAttrMathod != null)
            {
                info.CustomAttrMethod.AddRange(CustomAttrMathod.Split(","));
            }
            return info;
        }
    }
    public partial class BoosEnemy_DropConfig
    {
        public RandomSelector<int> CountDrop = new RandomSelector<int>();
        public override void InitExpand()
        {
            if (CountRate != null)
            {
                foreach (var C in CountRate)
                {
                    CountDrop.Add(C.Key, C.Value);
                }
            }

        }
        public int GetCountDrop()
        {
            int count = 0;
            if (!CountDrop.TryGetValue(out count))
            {
                return 0;
            }
            return count;
        }
    }
    public partial class TreasureChest_TypeConfig
    {
        public RandomSelector<int> CountDrop = new RandomSelector<int>();
        public override void InitExpand()
        {
            if (CountRate != null)
            {
                foreach (var C in CountRate)
                {
                    CountDrop.Add(C.Key, C.Value);
                }
            }

        }
        public int GetCountDrop()
        {
            int count = 0;
            if (!CountDrop.TryGetValue(out count))
            {
                return 0;
            }
            return count;
        }
    }
}
