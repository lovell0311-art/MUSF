using System;
using System.Collections.Generic;
using System.Text;
using ETModel;
using CustomFrameWork;
using CustomFrameWork.Component;

namespace ETHotfix
{
    public static class ItemSetFactory
    {
        public static ItemSet Create(int configId)
        {
            ItemSet itemSet = Root.CreateBuilder.GetInstance<ItemSet>();
            SetItem_TypeConfig conf;
            if(!Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<SetItem_TypeConfigJson>().JsonDic.TryGetValue(configId, out conf))
            {
                return null;
            }
            itemSet.ConfigId = configId;
            itemSet.Config = conf;
            return itemSet;
        }
    }
}
