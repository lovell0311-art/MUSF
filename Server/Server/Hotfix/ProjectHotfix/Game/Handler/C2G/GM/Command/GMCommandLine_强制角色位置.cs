
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CustomFrameWork;
using CustomFrameWork.Baseic;
using CustomFrameWork.Component;
using ETModel;

namespace ETHotfix
{
    [GameMasterCommandLine("强制角色位置")]
    public class GMCommandLine_强制角色位置 : C_GameMasterCommandLine<Player, IResponse>
    {
        public override async Task Run(Player b_Player, List<int> b_Parameter, IResponse b_Response)
        {
            if (b_Parameter.Count < 3)
            {
                b_Response.Error = 99;
                b_Response.Message = "参数不对";
                return;
            }

            int mMapId = b_Parameter[0];
            int mMapPosX = b_Parameter[1];
            int mMapPosY = b_Parameter[2];

            C_ServerArea mServerArea = Root.MainFactory.GetCustomComponent<ServerAreaManagerComponent>().GetGameArea(b_Player.SourceGameAreaId);
            if (mServerArea == null)
            {
                b_Response.Error = 99;
                b_Response.Message = "参数不对";
                return;
            }
            /*if (mServerArea.GetCustomComponent<MapManageComponent>().keyValuePairs.TryGetValue(mMapId, out var mapComponent) == false)
            {
                b_Response.Error = 99;
                b_Response.Message = "地图数据异常!";
                return;
            }*/

            MapComponent mapComponent = Help_MapHelper.GetMapByMapId(mServerArea, mMapId, b_Player.GameUserId);
            if (mapComponent == null)
            {
                b_Response.Error = 416;
                b_Response.Message = "地图数据异常!";
                return;
            }
            if (mapComponent.TryGetPosX(mMapPosX) == false)
            {
                b_Response.Error = 99;
                b_Response.Message = "请求位置数据异常x,不可行走!";
                return;
            }
            var mMapCellTarget = mapComponent.GetFindTheWay2D(mMapPosX, mMapPosY);
            if (mMapCellTarget == null)
            {
                b_Response.Message = "请求位置数据异常y,不可行走!";
                return;
            }
            if (mMapCellTarget.IsObstacle)
            {
                b_Response.Message = "请求位置数据异常,是障碍物!";
                return;
            }

            var mGamePlayer = b_Player.GetCustomComponent<GamePlayer>();

            mapComponent.MoveSendNotice(mapComponent.GetFindTheWay2D(mGamePlayer), mMapCellTarget, mGamePlayer, false);

            DBProxyManagerComponent mDBProxyManagerComponent = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            var dBProxy2 = mDBProxyManagerComponent.GetZoneDB(DBType.Core, (int)b_Player.GameAreaId);
            var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get(b_Player.GameAreaId);
            mWriteDataComponent.Save(mGamePlayer.UnitData, dBProxy2).Coroutine();
        }
    }
}