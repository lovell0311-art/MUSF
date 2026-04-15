using CustomFrameWork;
using ETModel;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETHotfix
{
    public static class DBItemDataHelper
    {
        public static MailItem ToMailItem(this DBItemData self)
        {
            MailItem mailItem = new MailItem();
            mailItem.ItemConfigID = self.ConfigID;
            mailItem.ItemID = self.Id;
            mailItem.AreaId = self.GameAreaId;
            if(!self.PropertyData.TryGetValue((int)EItemValue.Quantity,out int count))
            {
                count = 0;
            }
            mailItem.CreateAttr = new ItemCreateAttr()
            {
                Quantity = count,
            };
            return mailItem;
        }

        /// <summary>
        /// 现在，立刻保存
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task SaveDBNow(this DBItemData self)
        {
            try
            {
                DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
                var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, self.GameAreaId);
                if ((await dBProxy.Save(self)) == false)
                {
                    Log.Warning($"保存物品失败！item={self.ToJson()}");
                }
            }
            catch (Exception e)
            {
                string debugJson = "";
                try
                {
                    debugJson = self.ToJson();
                }
                catch (Exception e2)
                {
                    Log.Error(e2);
                }
                Log.Error(debugJson, e);
            }
        }
    }
}
