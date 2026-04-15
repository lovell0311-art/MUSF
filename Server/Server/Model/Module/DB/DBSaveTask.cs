using System;
using MongoDB.Driver;
using System.Threading.Tasks;
using CustomFrameWork;

namespace ETModel
{

    [ObjectSystem]
    public class DbSaveTaskAwakeSystem : AwakeSystem<DBSaveTask, ComponentWithId, string, TaskCompletionSource<bool>>
    {
        public override void Awake(DBSaveTask self, ComponentWithId component, string collectionName, TaskCompletionSource<bool> tcs)
        {
            self.Component = component;
            self.CollectionName = collectionName;
            self.Tcs = tcs;
        }
    }

    public sealed class DBSaveTask : DBTask
    {
        public ComponentWithId Component;

        public string CollectionName { get; set; }

        public TaskCompletionSource<bool> Tcs;

        public override async Task Run()
        {
            DBComponent dbComponent = Root.MainFactory.GetCustomComponent<DBComponent>();

            try
            {
                // 执行保存数据库任务
                var mReplaceOneResult = await dbComponent.GetCollection(this.CollectionName).ReplaceOneAsync(s => s.Id == this.Component.Id, this.Component, new UpdateOptions { IsUpsert = true });
                var mResult = mReplaceOneResult.IsAcknowledged && mReplaceOneResult.IsModifiedCountAvailable;
                this.Tcs.SetResult(mResult);
            }
            catch (Exception e)
            {
                //this.Tcs.SetException(new Exception($"保存数据失败!  {CollectionName} {Id}", e));
                this.Tcs.SetResult(false);
            }
        }
    }
}