using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Bson.Serialization.Attributes;

namespace ETModel
{
    [BsonIgnoreExtraElements]
    public class DBWarehouseItem : DBBase
    {
        /// <summary>
        /// 所属玩家，对应DBGamePlayer的UserId，Player的UserId
        /// </summary>
        [DBMongodb(1)]
        public long UserId { get; set; }
        //public DBItemData ItemData { get; set; }
        //public BackpackComponent backpackComponent;
        public int PageHeight;
        public int Width;
        public int Capacity;
        /// <summary>
        /// 金币（仓库）
        /// </summary>
        [Obsolete("禁止直接调用，使用 'WerahouseComponent' 中提供的接口访问。(cmpt.Coin,cmpt.InceraseCoin,cmpt.DeductCoin)",false)]
        public long Coin { get; set; } = 0;
        /// <summary>
        /// 所在区服
        /// </summary>
        public int GameAreaId { get; set; } = 0;

        public int IsDispose{ get; set; }   //1代表已删除
}
}
