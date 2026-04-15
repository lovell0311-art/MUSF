using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using CustomFrameWork;
using CustomFrameWork.Baseic;

namespace ETModel
{
    /// <summary>
    /// 用来缓存数据
    /// </summary>
    public class DBComponentBase : CustomComponent
    {
        public MongoClient mongoClient { get; private set; }
        public IMongoDatabase database { get; private set; }

        public ulong taskCount { get; private set; }
        public List<DBTaskQueue> tasks { get; private set; }

        public void Init(string b_ConnectionString, string b_DBName, int b_taskCountMax)
        {
            //DBConfig config = StartConfigComponent.Instance.StartConfig.GetComponent<DBConfig>();
            string connectionString = b_ConnectionString;
            mongoClient = new MongoClient(connectionString);
            this.database = this.mongoClient.GetDatabase(b_DBName);

            taskCount = (ulong)b_taskCountMax;
            tasks = new List<DBTaskQueue>(b_taskCountMax);
            for (int i = 0; i < b_taskCountMax; ++i)
            {
                DBTaskQueue taskQueue = ComponentFactory.Create<DBTaskQueue>();
                this.tasks.Add(taskQueue);
            }
        }

        public IMongoCollection<ComponentWithId> GetCollection(string name)
        {
            return this.database.GetCollection<ComponentWithId>(name);
        }

        public IMongoCollection<BsonDocument> GetBsonCollection(string name)
        {
            return this.database.GetCollection<BsonDocument>(name);
        }

        public ComponentWithId DeserializeComponent(string collectionName, BsonDocument document)
        {
            if (document == null)
            {
                return null;
            }

            Type componentType = this.GetCollectionType(collectionName);
            return (ComponentWithId)BsonSerializer.Deserialize(document, componentType);
        }

        private Type GetCollectionType(string collectionName)
        {
            return Type.GetType($"ETModel.{collectionName}") ?? typeof(DBBase);
        }
    }
}
