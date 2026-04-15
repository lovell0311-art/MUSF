using ETModel;
using System.Threading.Tasks;
namespace ETHotfix
{
    [ObjectSystem]
    public class LocationProxyComponentSystem : AwakeSystem<LocationProxyComponent>
    {
        public override void Awake(LocationProxyComponent self)
        {
            self.Awake();
        }
    }

    public static class LocationProxyComponentEx
    {
        public static void Awake(this LocationProxyComponent self)
        {
            StartConfigComponent startConfigComponent = StartConfigComponent.Instance;
#if NOLOCATION

#else
            StartConfig startConfig = startConfigComponent.LocationConfig;
            self.LocationAddress = startConfig.GetComponent<InnerConfig>().IPEndPoint;
#endif
        }

        public static async Task Add(this LocationProxyComponent self, long key, long instanceId)
        {
#if NOLOCATION
            self.KeyValuePairs[key] = instanceId;
#else
            Session session = Game.Scene.GetComponent<NetInnerComponent>().Get(self.LocationAddress);
            await session.Call(new ObjectAddRequest() { Key = key, InstanceId = instanceId });
#endif
        }

        public static async Task Lock(this LocationProxyComponent self, long key, long instanceId, int time = 1000)
        {
            Session session = Game.Scene.GetComponent<NetInnerComponent>().Get(self.LocationAddress);
            await session.Call(new ObjectLockRequest() { Key = key, InstanceId = instanceId, Time = time });
        }

        public static async Task UnLock(this LocationProxyComponent self, long key, long oldInstanceId, long instanceId)
        {
            Session session = Game.Scene.GetComponent<NetInnerComponent>().Get(self.LocationAddress);
            await session.Call(new ObjectUnLockRequest() { Key = key, OldInstanceId = oldInstanceId, InstanceId = instanceId });
        }

        public static async Task Remove(this LocationProxyComponent self, long key)
        {
#if NOLOCATION
            if (self.KeyValuePairs.ContainsKey(key))
            {
                self.KeyValuePairs.Remove(key);
            }
#else
            Session session = Game.Scene.GetComponent<NetInnerComponent>().Get(self.LocationAddress);
            await session.Call(new ObjectRemoveRequest() { Key = key });
#endif
        }

        public static async Task<long> Get(this LocationProxyComponent self, long key)
        {
#if NOLOCATION
            if (IdGenerater.GetAppId(key) != Game.Scene.GetComponent<OptionComponent>().Options.AppId)
            {
                return key;
            }
            if (self.KeyValuePairs.TryGetValue(key, out long mResult))
            {
                return mResult;
            }
            return -1;
#else
            Session session = Game.Scene.GetComponent<NetInnerComponent>().Get(self.LocationAddress);
            ObjectGetResponse response = (ObjectGetResponse)await session.Call(new ObjectGetRequest() { Key = key });
            return response.InstanceId;
#endif
        }
    }
}