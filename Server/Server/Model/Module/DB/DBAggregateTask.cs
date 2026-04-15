using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;
namespace ETModel
{

    [ObjectSystem]
    public class DBAggregateTaskAwakeSystem : AwakeSystem<DBAggregateTask, string, PipelineDefinition<ComponentWithId, BsonDocument>, TaskCompletionSource<List<BsonDocument>>, DBComponent>
    {
        public override void Awake(DBAggregateTask self, string collectionName, PipelineDefinition<ComponentWithId, BsonDocument> pipelineDefinition, TaskCompletionSource<List<BsonDocument>> tcs, DBComponent DbComponent)
        {
            self.CollectionName = collectionName;
            self.PipelineDefinition = pipelineDefinition;
            self.Tcs = tcs;
            self.DbComponent = DbComponent;
        }

    }

    /// <summary>
    /// 原子操作更新表
    /// </summary>
    public sealed class DBAggregateTask : DBTask
    {
        public DBComponent DbComponent { get; set; }
        public ComponentWithId Component { get; set; }

        public string CollectionName { get; set; }
        public PipelineDefinition<ComponentWithId, BsonDocument> PipelineDefinition { get; set; }

        public TaskCompletionSource<List<BsonDocument>> Tcs;

        public override async Task Run()
        {
            DBComponent db = DbComponent;

            try
            {
                var result = await db.GetCollection(this.CollectionName).AggregateAsync(PipelineDefinition);
                List<BsonDocument> docs = await result.ToListAsync();
                this.Tcs.SetResult(docs);
            }
            catch (Exception e)
            {
                this.Tcs.SetException(new Exception($"Aggregate Failed!  {CollectionName} {Id}", e));
            }
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}