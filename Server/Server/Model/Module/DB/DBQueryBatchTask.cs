using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CustomFrameWork;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ETModel
{
    [ObjectSystem]
    public class DbQueryBatchTaskSystem : AwakeSystem<DBQueryBatchTask, List<long>, string, TaskCompletionSource<List<ComponentWithId>>>
    {
        public override void Awake(DBQueryBatchTask self, List<long> idList, string collectionName, TaskCompletionSource<List<ComponentWithId>> tcs)
        {
            self.IdList = idList;
            self.CollectionName = collectionName;
            self.Tcs = tcs;
        }
    }

    public sealed class DBQueryBatchTask : DBTask
    {
        public string CollectionName { get; set; }

        public List<long> IdList { get; set; }

        public TaskCompletionSource<List<ComponentWithId>> Tcs { get; set; }

        public override async Task Run()
        {
            DBComponent dbComponent = Root.MainFactory.GetCustomComponent<DBComponent>();
            List<ComponentWithId> result = new List<ComponentWithId>();

            try
            {
                FilterDefinition<BsonDocument> filterDefinition = Builders<BsonDocument>.Filter.In("Id", this.IdList);
                List<BsonDocument> documents = await dbComponent.GetBsonCollection(this.CollectionName).Find(filterDefinition).ToListAsync();

                foreach (BsonDocument document in documents)
                {
                    ComponentWithId component = dbComponent.DeserializeComponent(this.CollectionName, document);
                    if (component != null)
                    {
                        result.Add(component);
                    }
                }

                this.Tcs.SetResult(result);
            }
            catch (Exception)
            {
                this.Tcs.SetResult(null);
            }
        }
    }
}
