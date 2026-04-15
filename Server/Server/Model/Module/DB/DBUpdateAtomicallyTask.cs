using System;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace ETModel
{

    [ObjectSystem]
    public class DBUpdateAtomicallyTaskAwakeSystem : AwakeSystem<DBUpdateAtomicallyTask, string, string, UpdateDefinition<ComponentWithId>, TaskCompletionSource<ComponentWithId>, DBComponent>
    {
        public override void Awake(DBUpdateAtomicallyTask self, string collectionName, string json, UpdateDefinition<ComponentWithId> updateDefinition, TaskCompletionSource<ComponentWithId> tcs, DBComponent DbComponent)
        {
            self.CollectionName = collectionName;
            self.Json = json;
            self.UpdateDefinition = updateDefinition;
            self.Tcs = tcs;
            self.DbComponent = DbComponent;
        }

    }

    /// <summary>
    /// 原子操作更新表
    /// </summary>
    public sealed class DBUpdateAtomicallyTask : DBTask
    {
        public DBComponent DbComponent { get; set; }
        public ComponentWithId Component { get; set; }

        public string CollectionName { get; set; }
        public string Json { get; set; }

        public UpdateDefinition<ComponentWithId> UpdateDefinition { get; set; }

        public TaskCompletionSource<ComponentWithId> Tcs;

        public override async Task Run()
        {
            DBComponent db = DbComponent;

            try
            {
                FilterDefinition<ComponentWithId> filterDefinition = new JsonFilterDefinition<ComponentWithId>(this.Json);
                var updateResult = await db.GetCollection(this.CollectionName).FindOneAndUpdateAsync(filterDefinition, UpdateDefinition,
                    new FindOneAndUpdateOptions<ComponentWithId> { ReturnDocument = ReturnDocument.After });

                this.Tcs.SetResult(updateResult);
            }
            catch (Exception e)
            {
                //this.Tcs.SetException(new Exception($"Update Atomically Failed!  {CollectionName} {Id}", e));
                this.Tcs.SetResult(null);
            }
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}