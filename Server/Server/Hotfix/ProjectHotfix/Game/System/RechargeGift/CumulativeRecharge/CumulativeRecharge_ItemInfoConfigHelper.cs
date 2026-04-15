using ETModel;

namespace ETHotfix
{
    public static partial class CumulativeRecharge_ItemInfoConfigHelper
    {
        public static ItemCreateAttr ToItemCreateAttr(this CumulativeRecharge_ItemInfoConfig self)
        {
            ItemCreateAttr info = new ItemCreateAttr();
            info.Level = self.Level;
            info.Quantity = self.Quantity;
            info.HaveSkill = self.HasSkill == 1;
            info.HaveLucky = self.HasLucky == 1;
            info.SetId = self.SetId;
            info.IsBind = self.IsBind;
            info.ValidTime = self.ItemExpirationTime;
            if (self.CustomAttrMathod != null)
            {
                info.CustomAttrMethod.AddRange(self.CustomAttrMathod.Split(","));
            }
            return info;
        }
    }
}
