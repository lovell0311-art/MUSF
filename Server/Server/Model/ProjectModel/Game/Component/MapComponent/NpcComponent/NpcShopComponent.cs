using CustomFrameWork;
using CustomFrameWork.Baseic;
using CustomFrameWork.Component;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ETModel
{
    public class NpcShopComponent
    {

        public const int I_WarehouseWidth = 8;
        public const int I_WarehouseHigh = 12;
        public GameNpc Parent;
        public ItemsBoxStatus itemBox;
        public Dictionary<long, Item> mItemDict = new Dictionary<long, Item>();
        public Dictionary<long, Npc_Shop_InfoConfig> mShopConfigDict = new Dictionary<long, Npc_Shop_InfoConfig>();

        public void Dispose()
        {
            Parent = null;
            //清理数据
            if (mItemDict != null && mItemDict.Count > 0)
            {
                var mTemp = mItemDict.Values.ToList();
                for (int i = 0, len = mTemp.Count; i < len; i++)
                {
                    mTemp[i].Dispose();
                }
                mItemDict.Clear();
            }
            mShopConfigDict.Clear();
            itemBox = null;
        }
    }
}
