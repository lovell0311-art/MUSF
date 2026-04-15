using ETModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETHotfix
{
    public static partial class Lottery_ItemInfoConfigHelper
    {
        public static ItemCreateAttr ToItemCreateAttr(this Lottery_ItemInfoConfig self)
        {
            ItemCreateAttr info = new ItemCreateAttr();
            info.Level = self.Level;
            info.Quantity = self.Quantity;
            info.HaveSkill = self.HasSkill == 1;
            info.HaveLucky = self.HasLucky == 1;
            info.IsBind = self.IsBind;
            if (self.CustomAttrMathod != null)
            {
                info.CustomAttrMethod.AddRange(self.CustomAttrMathod.Split(","));
            }
            return info;
        }
    }
}
