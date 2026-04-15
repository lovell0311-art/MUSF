using System;
using System.Threading.Tasks;
using MongoDB.Driver;
using CustomFrameWork;
using CustomFrameWork.Baseic;

namespace ETModel
{

    [ObjectSystem]
    public class DBGetCountTaskAwakeSystem : AwakeSystem<DBGetCountTask, string, string, TaskCompletionSource<long>>
    {
        public override void Awake(DBGetCountTask self, string collectionName, string json, TaskCompletionSource<long> tcs)
        {
            self.CollectionName = collectionName;
            self.Json = json;
            self.Tcs = tcs;
        }

    }

    /// <summary>
    /// 获取表数据量
    /// </summary>
    public sealed class DBGetCountTask : DBTask
    {
        public ComponentWithId Component;

        public string CollectionName { get; set; }
        public string Json { get; set; }

        public TaskCompletionSource<long> Tcs;


        public override async Task Run()
        {
            DBComponent db = Root.MainFactory.GetCustomComponent<DBComponent>();

            try
            {
                FilterDefinition<ComponentWithId> filterDefinition = new JsonFilterDefinition<ComponentWithId>(this.Json);
                long count = await db.GetCollection(this.CollectionName).CountDocumentsAsync(filterDefinition);
                this.Tcs.SetResult(count);
            }
            catch (Exception e)
            {
                //this.Tcs.SetException(new Exception($"获取数据库数量失败!  {CollectionName} {Id}", e));
                this.Tcs.SetResult(-1);
            }
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}