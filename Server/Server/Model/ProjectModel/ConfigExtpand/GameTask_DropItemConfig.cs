using System;
using System.Collections.Generic;
using System.Text;

namespace ETModel
{
    public partial class GameTask_DropItemConfig
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
            info.IsBind = IsBind;
            info.IsTask = IsTask == 1 ? true : false;
            info.OptionExcellent = OptionExcellent;
            if(CustomAttrMathod != null)
            {
                info.CustomAttrMethod.AddRange(CustomAttrMathod.Split(","));
            }
            return info;
        }

    }
}
