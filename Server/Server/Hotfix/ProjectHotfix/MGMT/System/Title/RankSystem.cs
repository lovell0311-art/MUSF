using System.Collections.Generic;
using System.Threading.Tasks;
using CustomFrameWork;
using CustomFrameWork.Component;
using CustomFrameWork.Baseic;
using ETModel;
using System;
using System.Linq;

namespace ETHotfix
{
    [EventMethod(typeof(RankComponent), EventSystemType.INIT)]
    public class RankComponentEventOnInit : ITEventMethodOnInit<RankComponent>
    {
        /// <summary>
        /// 读取排行数据
        /// </summary>
        /// <param name="b_Component"></param>
        public void OnInit(RankComponent b_Component)
        {
            try
            {
                TimerComponent mTimerComponent = Root.MainFactory.GetCustomComponent<TimerComponent>();
                mTimerComponent.WaitAsync(10000).Coroutine();
                b_Component.OnInit().Coroutine();
            }
            catch (Exception e)
            {
                Log.Fatal("排行初始化失败", e);
            }
        }
    }
    public static class RankComponentSystem
    {
        public async static Task OnInit(this RankComponent b_component)
        {
            b_component.rankInfo = new Dictionary<RankType, RankInfo>();
            b_component.AllRankList = new Dictionary<RankType, List<RankStructure>>();
            var startConfig = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfo(OptionComponent.Options.AppId);

            var mDBProxy = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>().GetZoneDB(DBType.Core, startConfig.ZoneId);
            if (mDBProxy == null) return;

            var RankInfoList = await mDBProxy.Query<RankInfo>(p => p.DataValidity == true);
            if (RankInfoList == null) return;

            if (RankInfoList.Count > 0)
            {
                foreach (var Info in RankInfoList)
                {
                    List<RankStructure> dBRankData = Help_JsonSerializeHelper.DeSerialize<List<RankStructure>>((Info as RankInfo).RankListInfo);

                    b_component.AllRankList.Add(dBRankData[0].GetRankType(), dBRankData);
                    b_component.rankInfo.Add((RankType)(Info as RankInfo).Type,(Info as RankInfo));
                }
            }
        }
        public async static Task SetPlayerTitle(this RankComponent b_component, long UserID, int RankType, int TitleId, int mAreaId, long EndTime)
        {
            if (RankType < 0 && RankType > 3) return;

            long Time = TitleId == 60003 ? EndTime + 604799 : 0;

            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, mAreaId);
            if (dBProxy2 != null)
            {
                var Title = await dBProxy2.Query<DBPlayerTitle>(P => P.TitleID == TitleId && P.UserId == UserID);
                if (Title != null && Title.Count > 0)
                {
                    DBPlayerTitle dBPlayerTitle1 = Title[0] as DBPlayerTitle;
                    if (Time != 0)
                        dBPlayerTitle1.BingTime = Help_TimeHelper.GetNowSecond();
                    else
                        dBPlayerTitle1.BingTime = 0;

                    dBPlayerTitle1.EndTime = Time;

                    dBPlayerTitle1.IsDisabled = 0;
                    await dBProxy2.Save(dBPlayerTitle1);
                }
                else
                {
                    DBPlayerTitle dBPlayerTitle1 = new DBPlayerTitle();
                    dBPlayerTitle1.TitleID = TitleId;
                    dBPlayerTitle1.Id = IdGeneraterNew.Instance.GenerateUnitId(mAreaId);
                    dBPlayerTitle1.UserId = UserID;
                    if (Time != 0)
                        dBPlayerTitle1.BingTime = Help_TimeHelper.GetNowSecond();
                    else
                        dBPlayerTitle1.BingTime = 0;
                    dBPlayerTitle1.EndTime = Time;
                    dBPlayerTitle1.Type = 0;
                    dBPlayerTitle1.IsDisabled = 0;
                    await dBProxy2.Save(dBPlayerTitle1);
                }
            }
        }
        public static bool SendMessage(this RankComponent b_component, IMessage G_Mmessage)
        {
            var mMatchConfigs = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfos(AppType.Game);
            foreach (var Info in mMatchConfigs)
            {
                if (Info.ZoneId == OptionComponent.Options.ZoneId)
                    Game.Scene.GetComponent<NetInnerComponent>().Get(Info.ServerInnerIP).Send(G_Mmessage);
            }
            return true;
        }
        public static async Task<bool> SetDB(this RankComponent b_component, RankType rankType, bool isDelete = false)
        {
            var startConfig = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfo(OptionComponent.Options.AppId);
            var mDBProxy = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>().GetZoneDB(DBType.Core, startConfig.ZoneId);
            if (mDBProxy == null) return false;

            RankInfo rankInfo = new RankInfo();
            rankInfo.Id = IdGeneraterNew.Instance.GenerateUnitId(startConfig.ZoneId);
            
            if (b_component.AllRankList.TryGetValue(rankType, out var rankStructures))
                rankInfo.RankListInfo = Help_JsonSerializeHelper.Serialize(rankStructures);

            rankInfo.DataValidity = !isDelete;
            rankInfo.Type = (int)rankType;
            if (!b_component.rankInfo.ContainsKey(rankType))
            {
                b_component.rankInfo.Add(rankType, rankInfo);
            }
           
            if (isDelete)
            {
                b_component.rankInfo.Remove(rankType);
                b_component.AllRankList.Remove(rankType);
            }
            
            var RankTypeInfo = await mDBProxy.Query<RankInfo>(p => p.Type == (int)rankType);
            if (RankTypeInfo != null && RankTypeInfo.Count == 1)
            {
                rankInfo.Id = (RankTypeInfo[0] as RankInfo).Id;
                await mDBProxy.Save(rankInfo);
            }
            else
            {
                await mDBProxy.Save(rankInfo);
            }
            return true;

        }
    }
}
