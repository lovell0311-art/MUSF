using CustomFrameWork.Baseic;
using CustomFrameWork;
using ETModel;
using System;
using System.Threading.Tasks;
using CustomFrameWork.Component;
using DnsClient;
using MongoDB.Driver.Core.Servers;
using System.Collections.Generic;
using System.Linq;

namespace ETHotfix
{
    [Timer(TimerType.OnlineStatistics)]
    public class OnlineStatisticsTimer : ATimer<OnlineStatisticsComponent>
    {
        public override void Run(OnlineStatisticsComponent self)
        {
            self.SaveOnlineCount().Coroutine();
        }
    }

    [FriendOf(typeof(OnlineStatisticsComponent))]
    [EventMethod(typeof(OnlineStatisticsComponent), EventSystemType.INIT)]
    public class OnlineStatisticsComponentEventOnInit : ITEventMethodOnInit<OnlineStatisticsComponent>
    {
        public void OnInit(OnlineStatisticsComponent self)
        {
            // 每分钟统计一次
            self.timerId = ETModel.ET.TimerComponent.Instance.NewRepeatedTimer(1000 * 60, TimerType.OnlineStatistics, self);
        }
    }

    [FriendOf(typeof(OnlineStatisticsComponent))]
    [EventMethod(typeof(OnlineStatisticsComponent), EventSystemType.DISPOSE)]
    public class OnlineStatisticsComponentEventOnDispose : ITEventMethodOnDispose<OnlineStatisticsComponent>
    {
        public override void OnDispose(OnlineStatisticsComponent self)
        {
            ETModel.ET.TimerComponent.Instance.Remove(ref self.timerId);
        }
    }

    public static class OnlineStatisticsComponentSystem
    {
        /// <summary>
        /// 统计在线人数
        /// </summary>
        /// <returns></returns>
        public static async Task SaveOnlineCount(this OnlineStatisticsComponent self)
        {
            // 去所有的GameServer上获取在线人数
            async Task<int> GetOnlineCount(int gameServerId)
            {
                try
                {
                    var gameServer = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfo(AppType.Game, gameServerId);
                    if (gameServer == null)
                    {
                        return 0;
                    }

                    Session gameServerSession = Game.Scene.GetComponent<NetInnerComponent>().Get(gameServer.ServerInnerIP);
                    Game2GM_GetGameServerStatus game2GM_GetGameServerStatus = await gameServerSession.Call(new GM2Game_GetGameServerStatus() { }) as Game2GM_GetGameServerStatus;
                    if (game2GM_GetGameServerStatus == null)
                    {
                        return 0;
                    }
                    return game2GM_GetGameServerStatus.OnlineCount;
                }
                catch(Exception e)
                {
                    // 服务器没开启
                }
                return 0;
            }


            var allGameServerInfo = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfos(AppType.Game);
            using ListComponent<Task<int>> tasks = ListComponent<Task<int>>.Create();
            foreach(var info in allGameServerInfo)
            {
                tasks.Add(GetOnlineCount(info.AppId));
            }
            List<int> results = await TaskHelper.WaitAll(tasks);


            DBOnlineCount dbOnlineCount = new DBOnlineCount()
            {
                CreateTime = TimeHelper.Now(),
                Count = results.Sum()
            };

            DBLogHelper.Write(dbOnlineCount);
        }

    }
}
