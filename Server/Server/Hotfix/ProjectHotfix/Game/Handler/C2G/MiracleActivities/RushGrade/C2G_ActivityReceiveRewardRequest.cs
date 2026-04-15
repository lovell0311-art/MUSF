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
    public class C2G_RushGradeActivityReceiveRequestHandler : AMActorRpcHandler<C2G_RushGradeActivityReceiveRequest, G2C_RushGradeActivityReceiveResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_RushGradeActivityReceiveRequest b_Request, G2C_RushGradeActivityReceiveResponse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_RushGradeActivityReceiveRequest b_Request, G2C_RushGradeActivityReceiveResponse b_Response, Action<IMessage> b_Reply)
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
            if (b_Request.RewardID.count == 0 || b_Request.MiracleActivitiesID == 0)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2400);
                b_Reply(b_Response);
                return false;
            }
            var PlayerActivity = mPlayer.GetCustomComponent<PlayerActivitComponent>();
            if (PlayerActivity == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2400);
                b_Reply(b_Response);
                return false;
            }
            int ActivityID = b_Request.MiracleActivitiesID;
            var Activities = mServerArea.GetCustomComponent<ActivitiesComponent>();
            if (!Activities.GetActivitState(ActivityID))
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2400);
                b_Reply(b_Response);
                return false;
            }
            //int Type = b_Request.RewardID;
            List<int> ints = new List<int>();
            foreach (var RID in b_Request.RewardID)
            {
                ints.Add(RID);
                if (RID > 15)
                {
                    if (mPlayer.GetCustomComponent<PlayerTitle>().CheckTitle(60012))
                        ints.Remove(RID);
                }
            }
            foreach (var Id in ints)
            {
                int ErrorID = 2403;
                {
                    long Type = 1L << (Id - 1);

                    if (PlayerActivity.GetPlayerActivitRewardState(ActivityID, Type))
                    {
                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2402);
                        b_Reply(b_Response);
                        return false;
                    }
                    var RushGradeActivity = mServerArea.GetCustomComponent<RushGradeActivity>();
                    if (RushGradeActivity != null && RushGradeActivity.ActivitID == ActivityID)
                    {
                        if (RushGradeActivity.PlayerReceiveActivityRewards(Id, mPlayer, out ErrorID))
                        {
                            PlayerActivity.SetPlayerActivitRewardState(ActivityID, Type);
                            PlayerActivity.DBPlayerActivit(ActivityID);
                        }
                    }
                }
            }
            b_Response.Info = PlayerActivity.GetMiracleActivities(ActivityID);
            b_Reply(b_Response);
            return true;
        }
    }
}