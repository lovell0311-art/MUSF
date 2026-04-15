using System;
using System.Threading.Tasks;
using CustomFrameWork;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ETModel
{
    [ObjectSystem]
    public class DBQueryTaskSystem : AwakeSystem<DBQueryTask, string, TaskCompletionSource<ComponentWithId>>
    {
        public override void Awake(DBQueryTask self, string collectionName, TaskCompletionSource<ComponentWithId> tcs)
        {
            self.CollectionName = collectionName;
            self.Tcs = tcs;
        }
    }

    public sealed class DBQueryTask : DBTask
    {
        public string CollectionName { get; set; }

        public TaskCompletionSource<ComponentWithId> Tcs { get; set; }

        public override async Task Run()
        {
            DBComponent dbComponent = Root.MainFactory.GetCustomComponent<DBComponent>();
            try
            {
                FilterDefinition<BsonDocument> filterDefinition = Builders<BsonDocument>.Filter.Eq("Id", this.Id);
                BsonDocument document = await dbComponent.GetBsonCollection(this.CollectionName).Find(filterDefinition).FirstOrDefaultAsync();
                this.Tcs.SetResult(dbComponent.DeserializeComponent(this.CollectionName, document));
            }
            catch (Exception)
            {
                this.Tcs.SetResult(null);
            }
        }
    }
}
