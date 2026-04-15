using CustomFrameWork.Baseic;
using CustomFrameWork.Component;
using CustomFrameWork;
using ETModel;
using System.Collections.Generic;
using ETHotfix;
using System;
using System.Threading.Tasks;



[EventMethod(typeof(CitySiegeActivities), EventSystemType.INIT)]
public class CitySiegeActivitiesEventOnInit : ITEventMethodOnInit<CitySiegeActivities>
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="b_Component"></param>
    public void OnInit(CitySiegeActivities b_Component)
    {
        b_Component.OnInit().Coroutine();
    }
}
public static partial class FullServiceNotification
{

    public static void SendMessage(this CitySiegeActivities b_Component, IActorMessage b_ActorMessage)
    {
        var mMatchConfigs = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfos(AppType.Game);
        foreach (var Server in mMatchConfigs)
        {
            Dictionary<int, List<int>> keyValuePairs = Help_JsonSerializeHelper.DeSerialize<Dictionary<int, List<int>>>(Server.RunParameter);
            int AreaId = 1;
            foreach (var KeyValuePair in keyValuePairs)
            {
                AreaId = KeyValuePair.Key >> 16;
                break;
            }
            if (b_Component.Parent.GameAreaId == AreaId)
                Game.Scene.GetComponent<NetInnerComponent>().Get(Server.ServerInnerIP).Send(b_ActorMessage);
        }
    }
}
public static partial class CitySiegeActivitiesComponentSystem
{
    public static async Task OnInit(this CitySiegeActivities b_Component)
    {
        b_Component.SupremeThrone = 0;
        b_Component.SupremeThroneTiem = 0;
        b_Component.SupremeThroneName = "";
        b_Component.LeaveTiem = 0;
        b_Component.WarAlliance = "";
        b_Component.HaveInHand = false;
        //b_Component.Parent.GetCustomComponent<ActivitiesComponent>().AddCitySiegeActivities();
        try
        {
            if (b_Component.Parent.GameAreaRouteId != 1) return;

            TimerComponent mTimerComponent = Root.MainFactory.GetCustomComponent<TimerComponent>();
            await mTimerComponent.WaitAsync(10000);
            b_Component.OnlyRunUpdate();

            while (b_Component.IsDisposeable == false && b_Component.IsRunUpdate)
            {
                long Time = Help_TimeHelper.GetNowSecond();
                await mTimerComponent.WaitAsync(1000);

                b_Component.CheckTiem();
                b_Component.TestThrone();
            }
        }
        catch (Exception e)
        {
            Log.Error(e);
        }

    }
    public static bool GetSate(this CitySiegeActivities b_Component)
    {
        return b_Component.HaveInHand;
    }
    public static void TestThrone(this CitySiegeActivities b_Component)
    {
        if (b_Component.HaveInHand)
        {
            //if (b_Component.SupremeThrone != 0)
            {
                if (b_Component.LeaveTiem != 0)
                {
                    long TiemS = 30 - (Help_TimeHelper.GetNowSecond() - b_Component.LeaveTiem);
                    if (TiemS <= 0)
                    {
                        G2C_SendPointOutMessage g2C_SendPointOutMessage = new G2C_SendPointOutMessage();
                        g2C_SendPointOutMessage.Status = 1;
                        g2C_SendPointOutMessage.Pointout = 2704;
                        g2C_SendPointOutMessage.PlayerName = "";
                        g2C_SendPointOutMessage.WarName = "";
                        g2C_SendPointOutMessage.Time = 0;
                        g2C_SendPointOutMessage.TitleName = 0;
                        g2C_SendPointOutMessage.PlayerId = 0;
                        b_Component.SendMessage(g2C_SendPointOutMessage);


                        b_Component.SupremeThrone = 0;
                        b_Component.SupremeThroneName = "";
                        b_Component.SupremeThroneTiem = 0;
                        b_Component.LeaveTiem = 0;
                        return;
                    }
                    else
                    {
                        G2C_SendPointOutMessage g2C_SendPointOutMessage = new G2C_SendPointOutMessage();

                        g2C_SendPointOutMessage.Status = 1;
                        g2C_SendPointOutMessage.Pointout = 2703;
                        g2C_SendPointOutMessage.PlayerName = b_Component.SupremeThroneName;
                        g2C_SendPointOutMessage.WarName = "";
                        g2C_SendPointOutMessage.Time = (int)TiemS;
                        g2C_SendPointOutMessage.TitleName = 0;
                        g2C_SendPointOutMessage.PlayerId = b_Component.SupremeThrone;
                        b_Component.SendMessage(g2C_SendPointOutMessage);

                        return;
                    }
                }
                else
                {
                    if (b_Component.SupremeThrone == 0) return;

                    long TiemS = 240 - (Help_TimeHelper.GetNowSecond() - b_Component.SupremeThroneTiem);
                    if (TiemS <= 0)
                    {
                        G2C_SendPointOutMessage g2C_SendPointOutMessage = new G2C_SendPointOutMessage();

                        g2C_SendPointOutMessage.Status = 0;
                        g2C_SendPointOutMessage.Pointout = 2702;
                        g2C_SendPointOutMessage.PlayerName = b_Component.SupremeThroneName;
                        g2C_SendPointOutMessage.WarName = b_Component.WarAlliance;
                        g2C_SendPointOutMessage.Time = 0;
                        g2C_SendPointOutMessage.TitleName = 0;
                        b_Component.SendMessage(g2C_SendPointOutMessage);
                        b_Component.HaveInHand = false;

                        int zoid = b_Component.Parent.GameAreaId;
                        var Player = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(zoid, b_Component.SupremeThrone);
                        if (Player != null)
                        {
                            G2M_SendTitleWarAllinceMember g2M_SendTitleWarAllinceMember = new G2M_SendTitleWarAllinceMember();
                            g2M_SendTitleWarAllinceMember.AppendData = b_Component.Parent.GameAreaId;
                            g2M_SendTitleWarAllinceMember.WarAllianceID = Player.GetCustomComponent<PlayerWarAllianceComponent>().WarAllianceID;
                            Player.GetSessionMGMT().Send(g2M_SendTitleWarAllinceMember);
                        }
                        //{
                        //    var PlData = Player.GetCustomComponent<GamePlayer>().Data;
                        //    var Title = Player.GetCustomComponent<PlayerTitle>();
                        //    if (Title == null) Title = Player.AddCustomComponent<PlayerTitle>();

                        //    if (PlData != null)
                        //    {
                        //        DateTime time = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 19, 30, 0);
                        //        long Crt = Help_TimeHelper.DateConversionTime(time) + 604800;
                        //        Title.AddTitle(60001, 0, Crt);
                        //        DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
                        //        var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, Player.GameAreaId);
                        //        var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get(Player.GameAreaId);
                        //        mWriteDataComponent.Save(PlData, dBProxy).Coroutine();

                        //        G2C_ServerSendTitleMessage g2C_ServerSendTitleMessage = new G2C_ServerSendTitleMessage();
                        //        g2C_ServerSendTitleMessage.UseTitle = Title.UseTitle;
                        //        foreach (var item in Title.ListString)
                        //        {
                        //            Title_Status title_Status = new Title_Status();
                        //            title_Status.TitleID = item.TitleID;
                        //            title_Status.BingTime = item.BingTime;
                        //            title_Status.EndTime = item.EndTime;
                        //            g2C_ServerSendTitleMessage.TitleList.Add(title_Status);
                        //        }
                        //        Player.Send(g2C_ServerSendTitleMessage);
                        //        Player.AddCustomComponent<CastleMasterCheckComponent>();
                        //    }
                        //}
                        return;
                    }
                    else
                    {
                        int zoid = b_Component.Parent.GameAreaId;
                        var Player = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(zoid, b_Component.SupremeThrone);
                        if (Player != null && Player.GetCustomComponent<GamePlayer>().IsDeath)
                        {
                            b_Component.LeaveTiem = Help_TimeHelper.GetNowSecond();
                            G2C_SendPointOutMessage g2C_SendPointOutMessage = new G2C_SendPointOutMessage();
                            g2C_SendPointOutMessage.Status = 1;
                            g2C_SendPointOutMessage.Pointout = 2703;
                            g2C_SendPointOutMessage.PlayerName = b_Component.SupremeThroneName;
                            g2C_SendPointOutMessage.WarName = b_Component.WarAlliance;
                            g2C_SendPointOutMessage.Time = (int)TiemS;
                            g2C_SendPointOutMessage.TitleName = 0;
                            g2C_SendPointOutMessage.PlayerId = b_Component.SupremeThrone;
                            b_Component.SendMessage(g2C_SendPointOutMessage);
                        }
                        else
                        {
                            G2C_SendPointOutMessage g2C_SendPointOutMessage = new G2C_SendPointOutMessage();
                            g2C_SendPointOutMessage.Status = 1;
                            g2C_SendPointOutMessage.Pointout = 2701;
                            g2C_SendPointOutMessage.PlayerName = b_Component.SupremeThroneName;
                            g2C_SendPointOutMessage.WarName = b_Component.WarAlliance;
                            g2C_SendPointOutMessage.Time = (int)TiemS;
                            g2C_SendPointOutMessage.TitleName = 0;
                            g2C_SendPointOutMessage.PlayerId = b_Component.SupremeThrone;
                            b_Component.SendMessage(g2C_SendPointOutMessage);
                        }
                        return;

                    }

                }
            }
        }
    }
    public static void CheckTiem(this CitySiegeActivities b_Component)
    {
        if (b_Component == null)
            return;
        int OpenWk = 6;
        int OpenH = 19;
        int OpenM = 30;
        int OpenS = 0;
        int EndH = 21;
        int EndM = 0;
        int EndS = 0;
        int wk = Convert.ToInt32(DateTime.Now.DayOfWeek);
        //{//攻城测试数据
        //    OpenWk = wk;
        //    OpenH = DateTime.Now.Hour;
        //    OpenM = 5;
        //    EndH = OpenH;
        //    EndM = 55;
        //}
        int hour = DateTime.Now.Hour;
        int minute = DateTime.Now.Minute;
        int second = DateTime.Now.Second;
        if (b_Component.HaveInHand)
        {
            if (EndH == hour && EndM == minute && EndS == second)
            {
                b_Component.HaveInHand = false;
                G2C_SendPointOutMessage g2C_SendPointOutMessage = new G2C_SendPointOutMessage();
                g2C_SendPointOutMessage.Status = 0;
                g2C_SendPointOutMessage.Pointout = 2705;
                b_Component.SendMessage(g2C_SendPointOutMessage);
            }
        }
        else
        {
            if (OpenWk == wk && OpenH == hour && OpenM == (minute + 1) && OpenS == second)
            {
                G2C_SendPointOutMessage g2C_SendPointOutMessage = new G2C_SendPointOutMessage();
                g2C_SendPointOutMessage.Status = 0;
                g2C_SendPointOutMessage.Pointout = 2706;
                b_Component.SendMessage(g2C_SendPointOutMessage);
                b_Component.SupremeThrone = 0;
                b_Component.SupremeThroneName = "";
                b_Component.SupremeThroneTiem = 0;
                b_Component.WarAlliance = "";
                b_Component.LeaveTiem = 0;
            }

            if (OpenWk == wk && OpenH == hour && OpenM == minute && OpenS == second)
            {
                b_Component.HaveInHand = true;
                G2C_SendPointOutMessage g2C_SendPointOutMessage = new G2C_SendPointOutMessage();
                g2C_SendPointOutMessage.Status = 1;
                g2C_SendPointOutMessage.Pointout = 2707;
                b_Component.SendMessage(g2C_SendPointOutMessage);
            }
        }
    }
}