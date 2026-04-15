using CustomFrameWork.Baseic;
using CustomFrameWork.Component;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Mozilla;

namespace ETModel
{

    [BsonIgnoreExtraElements]
    public class Item : CustomComponent
    {
        public static readonly ETHotfix.G2C_ItemsAttrEntryChange_notice G2CItemAttrEntryChangeNotice = new ETHotfix.G2C_ItemsAttrEntryChange_notice();
        public static readonly ETHotfix.G2C_ItemsPropChange_notice G2CItemsPropChange_notice = new ETHotfix.G2C_ItemsPropChange_notice();
        public ETHotfix.Struct_ItemAllProperty StructItemAllProperty = null;
        /// <summary>唯一标志ID 对应 data.Id </summary>
        public long ItemUID { get; set; }
        /// <summary>配置ID</summary>
        public int ConfigID;
        /// <summary>类型</summary>
        public EItemType Type = EItemType.None;

        [BsonIgnore]
        public ItemConfig ConfigData;

        /// <summary>数据库数据</summary>
        public DBItemData data;

        /// <summary>属性更新处理方法</summary>
        [BsonIgnore]
        public IItemUpdatePropHandler __UpdatePropHandler;

        /// <summary>物品当前属性</summary>
        public Dictionary<EItemValue, int> __Property = new Dictionary<EItemValue, int>();

        /// <summary>没保存到数据库</summary>
        public bool NotSevedToDB;

        public override void Dispose()
        {
            if (IsDisposeable) return;
            base.Dispose();

            ItemUID = 0;
            ConfigID = 0;
            Type = EItemType.None;
            ConfigData = null;
            data = null;
            __UpdatePropHandler = null;
            __Property.Clear();
            StructItemAllProperty = null;
            NotSevedToDB = true;
        }
    }
}
