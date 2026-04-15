using CustomFrameWork;
using CustomFrameWork.Component;

namespace ETModel.Robot
{
    [ObjectSystem]
    public class TransferPointManagerAwakeSystem : AwakeSystem<TransferPointManager>
    {
        public override void Awake(TransferPointManager self)
        {
            TransferPointManager.Instance = self;
            self.Load();



        }
    }

    [ObjectSystem]
    public class TransferPointManagerDestroySystem : DestroySystem<TransferPointManager>
    {
        public override void Destroy(TransferPointManager self)
        {
            self.MapId2TransferPoint.Clear();
            self.MapId2FreeTransferPoint.Clear();
            TransferPointManager.Instance = null;
        }
    }


    public class TransferPointManager : Entity
    {
        public static TransferPointManager Instance;

        public readonly MultiMap<int, Map_TransferPointConfig> MapId2TransferPoint = new MultiMap<int, Map_TransferPointConfig>();
        public readonly MultiMap<int, Map_TransferPointConfig> MapId2FreeTransferPoint = new MultiMap<int, Map_TransferPointConfig>();

        public void Load()
        {
            MapId2TransferPoint.Clear();
            MapId2FreeTransferPoint.Clear();

            ReadConfigComponent readConfig = Root.MainFactory.GetCustomComponent<ReadConfigComponent>();
            var jsonDic = readConfig.GetJson<Map_TransferPointConfigJson>().JsonDic;
            foreach(Map_TransferPointConfig config in jsonDic.Values)
            {
                if (config.MapCostGold == 0)
                {
                    // 免费的传送点
                    MapId2FreeTransferPoint.Add(config.MapId, config);
                }
                else
                {
                    // 收费的
                    MapId2TransferPoint.Add(config.MapId, config);
                }
            }
        }
    }
}
