using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ETModel
{
    /// <summary>
    /// ShowId
    /// </summary>
    public class DBShowId : DBBase
    {
        public long ShowId { get; set; }
    }
}