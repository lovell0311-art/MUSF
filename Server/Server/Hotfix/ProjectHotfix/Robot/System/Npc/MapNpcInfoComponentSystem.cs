using ETModel;
using ETModel.Robot;

namespace ETHotfix.Robot
{
    [ObjectSystem]
    public class MapNpcInfoComponentDestroySystem : DestroySystem<MapNpcInfoComponent>
    {
        public override void Destroy(MapNpcInfoComponent self)
        {
            self.AllNpcSpawnPoint.Clear();
        }
    }

    public static partial class MapNpcInfoComponentSystem
    {
    }
}
