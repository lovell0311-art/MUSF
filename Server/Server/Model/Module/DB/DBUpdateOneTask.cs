using System;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;

namespace ETModel
{

    [ObjectSystem]
    public class DBUpdateOneTaskAwakeSystem : AwakeSystem<DBUpdateOneTask, string, FilterDefinition<ComponentWithId>, UpdateDefinition<ComponentWithId>, TaskCompletionSource<bool>, DBComponent>
    {
        public override void Awake(DBUpdateOneTask self, string collectionName, FilterDefinition<ComponentWithId> filterDefinition, UpdateDefinition<ComponentWithId> updateDefinition, TaskCompletionSource<bool> tcs, DBComponent DbComponent)
        {
            self.CollectionName = collectionName;
            self.FilterDefinition = filterDefinition;
            self.UpdateDefinition = updateDefinition;
            self.Tcs = tcs;
            self.DbComponent = DbComponent;
        }

    }

    /// <summary>
    /// 原子操作更新表
    /// </summary>
    public sealed class DBUpdateOneTask : DBTask
    {
        public DBComponent DbComponent { get; set; }
        public ComponentWithId Component { get; set; }

        public string CollectionName { get; set; }
        public FilterDefinition<ComponentWithId> FilterDefinition { get; set; }

        public UpdateDefinition<ComponentWithId> UpdateDefinition { get; set; }

        public TaskCompletionSource<bool> Tcs;

        public override async Task Run()
        {
            DBComponent db = DbComponent;

            try
            {
                var updateResult = await db.GetCollection(this.CollectionName).UpdateOneAsync(FilterDefinition, UpdateDefinition,
                    new UpdateOptions { IsUpsert = true });
                var mResult = updateResult.IsAcknowledged && updateResult.IsModifiedCountAvailable;

                this.Tcs.SetResult(mResult);
            }
            catch (Exception e)
            {
                this.Tcs.SetException(new Exception($"Update One Failed!  {CollectionName} {Id}", e));
            }
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}