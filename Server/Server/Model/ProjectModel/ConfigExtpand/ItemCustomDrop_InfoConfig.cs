using System;
using System.Collections.Generic;
using System.Text;

namespace ETModel
{
    public partial class ItemCustomDrop_InfoConfig
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
}
