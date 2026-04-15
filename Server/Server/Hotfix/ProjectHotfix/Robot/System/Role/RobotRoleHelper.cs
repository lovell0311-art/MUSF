using System.Threading.Tasks;
using ETModel;
using ETModel.Robot;
using CustomFrameWork;


namespace ETHotfix.Robot
{
    public static class RobotRoleHelper
    {
        /// <summary>
        /// 请求玩家属性
        /// </summary>
        /// <param name="localUnit"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<bool> RequestPlayerProperty(Unit localUnit,ETCancellationToken cancellationToken = null)
        {
            Scene clientScene = localUnit.ClientScene();
            Session session = clientScene.GetComponent<SessionComponent>().session;

            IResponse res = await session.Call(new C2G_PlayerPropertyRequest()
            {
                SelectId = 0
            }, cancellationToken);
            if (cancellationToken != null && cancellationToken.IsCancel()) return false;
            if (res.Error != ErrorCode.ERR_Success)
            {
                Log.Warning($"[{clientScene.Name}] C2G_PlayerPropertyRequest:{res.Error}");
            }
            G2C_PlayerPropertyResponse response = res as G2C_PlayerPropertyResponse;

            NumericComponent numeric = localUnit.GetComponent<NumericComponent>();
            foreach (var kv in response.Info)
            {
                numeric.Set(kv.Key, kv.Value);
            }
            return true;
        }

        /// <summary>
        /// 添加属性
        /// </summary>
        /// <param name="localUnit"></param>
        /// <param name="propId"></param>
        /// <param name="addNumber"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<bool> AddPoint(Unit localUnit, E_GameProperty propId,int addNumber, ETCancellationToken cancellationToken = null)
        {
            Scene clientScene = localUnit.ClientScene();
            Session session = clientScene.GetComponent<SessionComponent>().session;
            IResponse res = await session.Call(new C2G_BattlePropertyAddPointRequest()
            {
                BattlePropertyId = (int)propId,
                AddPointNumber = addNumber,
            }, cancellationToken);
            if (cancellationToken != null && cancellationToken.IsCancel()) return false;
            if (res.Error != ErrorCode.ERR_Success) return false;
            G2C_BattlePropertyAddPointResponse response = res as G2C_BattlePropertyAddPointResponse;

            NumericComponent numeric = localUnit.GetComponent<NumericComponent>();
            foreach (var kv in response.Info)
            {
                numeric.Set(kv.Key, kv.Value);
            }
            numeric.Set((int)E_GameProperty.FreePoint, response.PropertyPoint);
            return true;
        }
    }
}
