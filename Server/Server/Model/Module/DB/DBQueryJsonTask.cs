using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CustomFrameWork;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ETModel
{
    [ObjectSystem]
    public class DBQueryJsonTaskAwakeSystem : AwakeSystem<DBQueryJsonTask, string, string, TaskCompletionSource<List<ComponentWithId>>>
    {
        public override void Awake(DBQueryJsonTask self, string collectionName, string json, TaskCompletionSource<List<ComponentWithId>> tcs)
        {
            self.CollectionName = collectionName;
            self.Json = json;
            self.Tcs = tcs;
        }
    }

    public sealed class DBQueryJsonTask : DBTask
    {
        public string CollectionName { get; set; }

        public string Json { get; set; }

        public TaskCompletionSource<List<ComponentWithId>> Tcs { get; set; }

        public int Count { get; set; } = 0;

        public override async Task Run()
        {
            DBComponent dbComponent = Root.MainFactory.GetCustomComponent<DBComponent>();
            try
            {
                FilterDefinition<BsonDocument> filterDefinition = new JsonFilterDefinition<BsonDocument>(this.Json);
                IMongoCollection<BsonDocument> collection = dbComponent.GetBsonCollection(this.CollectionName);
                List<BsonDocument> documents;

                if (this.Count == 0)
                {
                    documents = await collection.Find(filterDefinition).ToListAsync();
                }
                else
                {
                    documents = await collection.Find(filterDefinition).Limit(this.Count).ToListAsync();
                }

                List<ComponentWithId> components = new List<ComponentWithId>(documents.Count);
                foreach (BsonDocument document in documents)
                {
                    components.Add(dbComponent.DeserializeComponent(this.CollectionName, document));
                }

                this.Tcs.SetResult(components);
            }
            catch (Exception e)
            {
                this.Tcs.SetException(new Exception($"查询数据库异常! {this.CollectionName} {this.Json}", e));
            }
        }
    }
}
