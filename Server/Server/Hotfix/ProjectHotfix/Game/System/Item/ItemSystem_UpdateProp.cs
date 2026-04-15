using System;
using System.Collections.Generic;
using System.Text;
using ETModel;
using CustomFrameWork;

namespace ETHotfix
{
    public static partial class ItemSystem
    {

        public static IItemUpdatePropHandler GetUpdatePropHandler(this Item self)
        {
            if (self.__UpdatePropHandler == null)
            {
                if (String.IsNullOrEmpty(self.ConfigData.UpdatePropMethod))
                {
                    // TODO 使用默认更新方法
                    self.__UpdatePropHandler = Root.MainFactory.GetCustomComponent<ItemUpdatePropManagerComponent>().DefaultHandler;
                }
                else
                {
                    self.__UpdatePropHandler = Root.MainFactory.GetCustomComponent<ItemUpdatePropManagerComponent>().GetHandler(self.ConfigData.UpdatePropMethod);
                }
                if (self.__UpdatePropHandler == null)
                {
                    throw new Exception($"Item update prop handler '{self.ConfigData.UpdatePropMethod}' not found,item uuid={self.ItemUID} id={self.ConfigID} name={self.ConfigData.Name}");
                }
            }
            return self.__UpdatePropHandler;
        }

        /// <summary>
        /// 更新属性
        /// </summary>
        /// <exception cref="Exception"></exception>
        public static void UpdateProp(this Item self)
        {
            self.GetUpdatePropHandler().UpdateProp(self);
        }

        /// <summary>
        /// 只更新耐久
        /// </summary>
        /// <param name="self"></param>
        public static void OnlyUpdateDurability(this Item self)
        {
            var updateProp = self.GetUpdatePropHandler() as ItemUpdateProp.Default;
            if (updateProp == null)
            {
                return;
            }
            updateProp.StartUpdate(self);
            updateProp.UpdateDurability(self);
            updateProp.EndUpdate(self);
        }

        /// <summary>
        /// 只更新价格，物品数量变动，耐久变动时更新
        /// </summary>
        /// <param name="self"></param>
        public static void OnlyUpdateMoney(this Item self)
        {
            var updateProp = self.GetUpdatePropHandler() as ItemUpdateProp.Default;
            if (updateProp == null)
            {
                return;
            }
            updateProp.StartUpdate(self);
            updateProp.UpdateBuyMoney(self);
            updateProp.UpdateSellMoney(self);
            updateProp.UpdateRepairMoney(self);
            updateProp.EndUpdate(self);
        }
    }
}
