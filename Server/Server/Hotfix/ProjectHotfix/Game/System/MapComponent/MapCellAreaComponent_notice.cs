using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CustomFrameWork;
using CustomFrameWork.Baseic;
using CustomFrameWork.Component;
using ETModel;

namespace ETHotfix
{
    /// <summary>
    /// 对地图进行分域
    /// </summary>
    public static partial class MapCellAreaComponentSystem
    {
        /// <summary>
        /// 域广播
        /// </summary>
        /// <param name="b_Component"></param>
        public static void RadioBroadcast(this MapCellAreaComponent b_Component, IActorMessage b_ActorMessage)
        {
            if (b_Component.FieldPlayerDic.Count > 0)
            {
                var mGamePlayers = b_Component.FieldPlayerDic.Values.ToArray();
                for (int i = 0, len = mGamePlayers.Length; i < len; i++)
                {
                    var mGamePlayer = mGamePlayers[i];

                    if (mGamePlayer.IsDisposeable) continue;

                    mGamePlayer.Player.Send(b_ActorMessage);
                }
            }
        }

        public static void RadioBroadcast(this MapCellAreaComponent b_Component, IActorListMessage b_ActorListMessage)
        {
            if (b_Component.FieldPlayerDic.Count > 0)
            {
                using ListComponent<Player> playerList = ListComponent<Player>.Create();
                foreach(GamePlayer gamePlayer in b_Component.FieldPlayerDic.Values) playerList.Add(gamePlayer.Player);
                if (playerList.Count != 0)
                {
                    playerList.Send(b_ActorListMessage);
                }
            }
        }

        public static void RadioBroadcast(this IEnumerable<MapCellAreaComponent> b_ComponentList, IActorListMessage b_ActorListMessage)
        {
            using ListComponent<Player> playerList = ListComponent<Player>.Create();
            foreach (MapCellAreaComponent b_Component in b_ComponentList)
            {
                if (b_Component.FieldPlayerDic.Count > 0)
                {
                    foreach (GamePlayer gamePlayer in b_Component.FieldPlayerDic.Values) playerList.Add(gamePlayer.Player);
                }
            }
            if (playerList.Count != 0)
            {
                playerList.Send(b_ActorListMessage);
            }
        }

        /// <summary>
        /// 域广播
        /// </summary>
        /// <param name="b_Component"></param>
        public static void RadioBroadcast(this MapCellAreaComponent b_Component, IActorMessage b_ActorMessage, GamePlayer b_NoSendGamePlayer)
        {
            if (b_Component.FieldPlayerDic.Count > 0)
            {
                foreach(GamePlayer gamePlayer in b_Component.FieldPlayerDic.Values)
                {
                    if (gamePlayer.IsDisposeable) continue;

                    if (b_NoSendGamePlayer.InstanceId == gamePlayer.InstanceId)
                    {
                        continue;
                    }

                    gamePlayer.Player.Send(b_ActorMessage);
                }
            }
        }

        public static void RadioBroadcast(this MapCellAreaComponent b_Component, IActorListMessage b_ActorListMessage, GamePlayer b_NoSendGamePlayer)
        {
            if (b_Component.FieldPlayerDic.Count > 0)
            {
                using ListComponent<Player> playerList = ListComponent<Player>.Create();
                foreach (GamePlayer gamePlayer in b_Component.FieldPlayerDic.Values)
                {
                    if (b_NoSendGamePlayer.InstanceId == gamePlayer.InstanceId) continue;
                    playerList.Add(gamePlayer.Player);
                }
                if (playerList.Count != 0)
                {
                    playerList.Send(b_ActorListMessage);
                }
            }
        }

        public static void RadioBroadcast(this IEnumerable<MapCellAreaComponent> b_ComponentList, IActorListMessage b_ActorListMessage, GamePlayer b_NoSendGamePlayer)
        {
            using ListComponent<Player> playerList = ListComponent<Player>.Create();
            foreach (MapCellAreaComponent b_Component in b_ComponentList)
            {
                if (b_Component.FieldPlayerDic.Count > 0)
                {
                    foreach (GamePlayer gamePlayer in b_Component.FieldPlayerDic.Values)
                    {
                        if (b_NoSendGamePlayer.InstanceId == gamePlayer.InstanceId) continue;
                        playerList.Add(gamePlayer.Player);
                    }
                }
            }
            if(playerList.Count != 0)
            {
                playerList.Send(b_ActorListMessage);
            }
        }

        public static void RadioBroadcastByAroundCellField(this MapCellAreaComponent b_Component, IActorMessage b_ActorMessage)
        {
            for (int i = 0, len = b_Component.AroundField.Count; i < len; i++)
            {
                var mAroundFieldIndex = b_Component.AroundField[i];

                if (b_Component.AroundFieldDic.TryGetValue(mAroundFieldIndex, out var mCellField))
                {
                    mCellField.RadioBroadcast(b_ActorMessage);
                }
            }
        }
        public static void RadioBroadcastByAroundArea(this MapCellAreaComponent b_Component, IActorMessage b_ActorMessage, GamePlayer b_NoSendGamePlayer)
        {
            for (int i = 0, len = b_Component.AroundField.Count; i < len; i++)
            {
                var mAroundFieldIndex = b_Component.AroundField[i];

                if (b_Component.AroundFieldDic.TryGetValue(mAroundFieldIndex, out var mCellField))
                {
                    mCellField.RadioBroadcast(b_ActorMessage, b_NoSendGamePlayer);
                }
            }
        }
    }
}