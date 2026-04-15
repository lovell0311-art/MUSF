using System;
using System.Threading.Tasks;
using ETModel;
using CustomFrameWork;
using CustomFrameWork.Component;
using System.Collections.Generic;
using global::Google.Protobuf.Collections;

namespace ETHotfix
{
    public static class PlayerSystem
    {
        private static bool ShouldTraceRoleFlowMessage(IActorMessage actorMessage)
        {
            return actorMessage is G2C_MovePos_notice || actorMessage is G2C_LoadingComplete;
        }

        private static string DescribeRoleFlowMessage(IActorMessage actorMessage)
        {
            if (actorMessage is G2C_MovePos_notice movePos)
            {
                return $"{actorMessage.GetType().Name} actor={actorMessage.ActorId} gameUserId={movePos.GameUserId} map={movePos.MapId} pos={movePos.X},{movePos.Y} view={movePos.ViewId} needMove={movePos.IsNeedMove}";
            }

            return $"{actorMessage.GetType().Name} actor={actorMessage.ActorId}";
        }

        public static void SetData(this Player b_Player, DBAccountZoneData b_UnitData)
        {
            b_Player.Data = b_UnitData;
        }
        public static void Send(this Player b_Player, IActorMessage b_ActorMessage)
        {
            b_ActorMessage.ActorId = b_Player.GameUserId;
            if (b_Player.GateServerId == -1)
            {
                if (ShouldTraceRoleFlowMessage(b_ActorMessage))
                {
                    Log.Warning($"#RoleFlow# Player.Send skipped gate-missing user={b_Player.UserId} role={b_Player.GameUserId} msg={DescribeRoleFlowMessage(b_ActorMessage)}");
                }
                return;
            }

            var mStartUpInfo = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfo(b_Player.GateServerId);
            Session targetSession = Game.Scene.GetComponent<NetInnerComponent>().Get(mStartUpInfo.ServerInnerIP);
            if (targetSession == null || targetSession.IsDisposed)
            {
                Log.Error($"玩家的链接失效 {b_Player.GameUserId} :{b_Player.GateServerId} {mStartUpInfo.ServerInnerIP}");
                if (ShouldTraceRoleFlowMessage(b_ActorMessage))
                {
                    Log.Error($"#RoleFlow# Player.Send target invalid user={b_Player.UserId} role={b_Player.GameUserId} msg={DescribeRoleFlowMessage(b_ActorMessage)}");
                }
                return;
            }

            if (ShouldTraceRoleFlowMessage(b_ActorMessage))
            {
                Log.Info($"#RoleFlow# Player.Send user={b_Player.UserId} role={b_Player.GameUserId} gate={b_Player.GateServerId} session={targetSession.Id} msg={DescribeRoleFlowMessage(b_ActorMessage)}");
            }
            targetSession.Send(b_ActorMessage);
        }

        public static void Send(this IEnumerable<Player> b_PlayerList, IActorListMessage b_ActorMessage)
        {
            using (DictionaryComponent<int, RepeatedFieldComponent<long>> GateServerId2ActorIdList = DictionaryComponent<int, RepeatedFieldComponent<long>>.Create())
            {
                foreach (Player player in b_PlayerList)
                {
                    if (player != null)
                    {
                        if (player.GateServerId != -1)
                        {
                            if (GateServerId2ActorIdList.TryGetValue(player.GateServerId, out RepeatedFieldComponent<long> list))
                            {
                                list.Add(player.GameUserId);
                            }
                            else
                            {
                                RepeatedFieldComponent<long> newList = RepeatedFieldComponent<long>.Create();
                                newList.Add(player.GameUserId);
                                GateServerId2ActorIdList.Add(player.GateServerId, newList);
                            }
                        }
                    }
                }
                foreach (var kv in GateServerId2ActorIdList)
                {
                    var mStartUpInfo = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfo(kv.Key);

                    Session targetSession = Game.Scene.GetComponent<NetInnerComponent>().Get(mStartUpInfo.ServerInnerIP);
                    if (targetSession == null || targetSession.IsDisposed)
                    {
                        Log.Error($"Server连接异常 {kv.Key} {mStartUpInfo.ServerInnerIP}");
                        return;
                    }
                    b_ActorMessage.ActorIdList = kv.Value;
                    targetSession.Send(b_ActorMessage);
                    kv.Value.Dispose();
                }
            }
        }

        /// <summary>
        /// 将当前协议广播给周围的玩家和自己
        /// </summary>
        /// <param name="b_Component"></param>
        /// <param name="b_ActorMessage"></param>
        public static void NotifyAroundPlayer(this Player self, IActorMessage b_ActorMessage)
        {
            //获得角色当前所在地图和坐标
            var mGamePlayer = self.GetCustomComponent<GamePlayer>();
            if (mGamePlayer == null) return;

            int mapIndex = mGamePlayer.UnitData.Index;
            int x = mGamePlayer.UnitData.X;
            int y = mGamePlayer.UnitData.Y;

            C_ServerArea mServerArea = Root.MainFactory.GetCustomComponent<ServerAreaManagerComponent>().GetGameArea(mGamePlayer.Player.SourceGameAreaId);
            if (mServerArea == null)
            {
                Log.Error($"找不到区服");
                return;
            }
            MapComponent mapComponent = Help_MapHelper.GetMapByMapId(mServerArea, mapIndex, mGamePlayer.Player.GameUserId);
            if (mapComponent == null) return;

            C_FindTheWay2D b_Source = mapComponent.GetFindTheWay2D(x, y);
            mapComponent.SendNotice(b_Source, b_ActorMessage);
            //获得当前地图组件
            //if (mServerArea.GetCustomComponent<MapManageComponent>().keyValuePairs.TryGetValue(mapIndex, out var mapComponent))
            //{
            //    C_FindTheWay2D b_Source = mapComponent.GetFindTheWay2D(x, y);
            //    mapComponent.SendNotice(b_Source, b_ActorMessage);
            //}
        }

        /// <summary>
        /// 获得自己当前附近的所有玩家GamePlayer
        /// </summary>
        public static bool TryGetAroundPlayer(this Player self, ref List<GamePlayer> values)
        {
            //获得角色当前所在地图和坐标
            var mGamePlayer = self.GetCustomComponent<GamePlayer>();
            if (mGamePlayer == null) return false;

            int mapIndex = mGamePlayer.UnitData.Index;

            C_ServerArea mServerArea = Root.MainFactory.GetCustomComponent<ServerAreaManagerComponent>().GetGameArea(mGamePlayer.Player.SourceGameAreaId);
            if (mServerArea == null)
            {
                Log.Error($"找不到区服");
                return false;
            }

            //获得当前地图组件
            if (mServerArea.GetCustomComponent<MapManageComponent>().keyValuePairs.TryGetValue(mapIndex, out var mapComponent))
            {
                MapCellAreaComponent mSourceCellField = mapComponent.GetMapCellField(mGamePlayer);
                if (mSourceCellField != null)
                {
                    for (int i = 0, len = mSourceCellField.AroundField.Count; i < len; i++)
                    {
                        var mAroundFieldIndex = mSourceCellField.AroundField[i];

                        if (mSourceCellField.AroundFieldDic.TryGetValue(mAroundFieldIndex, out var mCellField))
                        {
                            foreach (GamePlayer gamePlayer in mCellField.FieldPlayerDic.Values)
                            {
                                values.Add(gamePlayer);
                            }
                        }
                    }
                }
                return true;
            }
            return false;
        }

    }
}
