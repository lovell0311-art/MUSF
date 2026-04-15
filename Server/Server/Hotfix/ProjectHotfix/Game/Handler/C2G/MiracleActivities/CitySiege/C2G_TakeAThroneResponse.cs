using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class G2C_TakeAThroneResponseHandler : AMActorRpcHandler<C2G_TakeAThroneRequest, G2C_TakeAThroneResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_TakeAThroneRequest b_Request, G2C_TakeAThroneResponse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_TakeAThroneRequest b_Request, G2C_TakeAThroneResponse b_Response, Action<IMessage> b_Reply)
        {
            int mAreaId = (int)(b_Request.AppendData >> 16);
            C_ServerArea mServerArea = Root.MainFactory.GetCustomComponent<ServerAreaManagerComponent>().GetGameArea((int)b_Request.AppendData);
            if (mServerArea == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(21006);
                b_Reply(b_Response);
                return true;
            }
            Player mPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(mAreaId, b_Request.ActorId);
            if (mPlayer == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(200);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("不存在!");
                b_Reply(b_Response);
                return false;
            }
            var wal = mPlayer.GetCustomComponent<PlayerWarAllianceComponent>();
            if (wal != null && wal.MemberPost != 3)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(200);
                b_Reply(b_Response);
                return false;
            }
            var Activities = mServerArea.GetCustomComponent<CitySiegeActivities>();
            if (Activities != null && Activities.HaveInHand)
            {
                if (Activities.SupremeThrone != 0)
                {
                    if (Activities.SupremeThrone == mPlayer.GameUserId)
                    {
                        if (Help_TimeHelper.GetNowSecond() - Activities.LeaveTiem >= 30)
                        {
                            Activities.SupremeThroneTiem = Help_TimeHelper.GetNowSecond();
                        }
                    }
                    else
                    {
                        if (Activities.LeaveTiem == 0)
                        {
                            b_Reply(b_Response);
                            return false;
                        }

                        Activities.SupremeThrone = mPlayer.GameUserId;
                        Activities.SupremeThroneName = mPlayer.GetCustomComponent<GamePlayer>().Data.NickName;
                        Activities.SupremeThroneTiem = Help_TimeHelper.GetNowSecond();
                        Activities.WarAlliance = mPlayer.GetCustomComponent<PlayerWarAllianceComponent>().WarAllianceName;
                        //MapComponent mapComponent = mServerArea.GetCustomComponent<MapManageComponent>().GetMapByMapIndex(4);
                        //if (mapComponent != null)
                        //{
                        G2C_SendPointOutMessage g2C_SendPointOutMessage = new G2C_SendPointOutMessage();
                        g2C_SendPointOutMessage.Status = 1;
                        g2C_SendPointOutMessage.Pointout = 2708;
                        g2C_SendPointOutMessage.PlayerName = Activities.SupremeThroneName;
                        g2C_SendPointOutMessage.WarName = Activities.WarAlliance;
                        g2C_SendPointOutMessage.Time= 0;
                        g2C_SendPointOutMessage.TitleName = 60001;
                        g2C_SendPointOutMessage.PlayerId = 0;
                        Activities.SendMessage(g2C_SendPointOutMessage);
                        //mapComponent.SendNoticeByServer(g2C_SendPointOutMessage);
                        //}
                    }
                    Activities.LeaveTiem = 0;
                }
                else
                {
                    Activities.SupremeThrone = mPlayer.GameUserId;
                    Activities.SupremeThroneName = mPlayer.GetCustomComponent<GamePlayer>().Data.NickName;
                    Activities.SupremeThroneTiem = Help_TimeHelper.GetNowSecond();
                    Activities.WarAlliance = mPlayer.GetCustomComponent<PlayerWarAllianceComponent>().WarAllianceName;
                    Activities.LeaveTiem = 0;

                    //MapComponent mapComponent = mServerArea.GetCustomComponent<MapManageComponent>().GetMapByMapIndex(4);
                    //if (mapComponent != null)
                    //{
                    G2C_SendPointOutMessage g2C_SendPointOutMessage = new G2C_SendPointOutMessage();
                    g2C_SendPointOutMessage.Status = 1;
                    g2C_SendPointOutMessage.Pointout = 2708;
                    g2C_SendPointOutMessage.PlayerName = Activities.SupremeThroneName;
                    g2C_SendPointOutMessage.WarName = Activities.WarAlliance;
                    g2C_SendPointOutMessage.Time = 0;
                    g2C_SendPointOutMessage.TitleName = 60001;
                    g2C_SendPointOutMessage.PlayerId =0;
                    Activities.SendMessage(g2C_SendPointOutMessage);
                    //mapComponent.SendNoticeByServer(g2C_SendPointOutMessage);
                    //}
                }

                b_Reply(b_Response);
                return true;
            }
            b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(21006);
            b_Reply(b_Response);
            return false;
        }
    }
}