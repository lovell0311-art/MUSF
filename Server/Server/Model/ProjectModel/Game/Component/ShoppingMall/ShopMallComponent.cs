using CustomFrameWork;
using CustomFrameWork.Baseic;
using CustomFrameWork.Component;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using TencentCloud.Cat.V20180409.Models;

namespace ETModel
{
    public class ShopMallComponent : TCustomComponent<MainFactory>
    {
        public Dictionary<ShopMallType, Dictionary<ItemSortType, Dictionary<int, ShopItem>>> ShopItemDic;
        public override void Dispose()
        {
            ShopItemDic.Clear();
        }
    }

   
}