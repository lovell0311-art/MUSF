using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_OpenMiracleActivitiesRequestHandler : AMActorRpcHandler<C2G_OpenMiracleActivitiesRequest, G2C_OpenMiracleActivitiesResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_OpenMiracleActivitiesRequest b_Request, G2C_OpenMiracleActivitiesResponse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_OpenMiracleActivitiesRequest b_Request, G2C_OpenMiracleActivitiesResponse b_Response, Action<IMessage> b_Reply)
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

            var Activities = mServerArea.GetCustomComponent<ActivitiesComponent>();
            if (!Activities.GetActivitState(b_Request.MiracleActivitiesID))
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2400);
                b_Reply(b_Response);
                return false;
            }

            var PlayerActivity = mPlayer.GetCustomComponent<PlayerActivitComponent>();
            if (PlayerActivity != null)
            {
                b_Response.Info = PlayerActivity.GetMiracleActivities(b_Request.MiracleActivitiesID);
                async Task CheckActivities13()
                {
                    var tasksComponent = mPlayer.GetCustomComponent<GameTasksComponent>();
                    if (tasksComponent.data.PassTasks != null && tasksComponent.data.PassTasks.Count >= 1)
                    {
                        var Info = tasksComponent.data.PassTasks.LastOrDefault();
                        if (Info.Key != 0)
                        {
                            if (Info.Value.TaskState == EGameTaskState.Received)
                            {
                                Log.PLog("通行证",$"玩家:{mPlayer.GameUserId}更新通行证任务状态--当前任务Id:{Info.Value.ConfigId}");
                                tasksComponent.data.PassTasks.Clear();
                                tasksComponent.SaveDB();
                            }
                        }

                    }
                    //if (Activities.GetActivitState(13))//通行证任务
                    //{
                    //    var ActivitieInfo = Activities.GetActivit(13);
                    //    if (ActivitieInfo != null && ActivitieInfo.IsOpen)
                    //    {
                    //        var palyerActivitie = PlayerActivity.GetMiracleActivities(13);
                    //        var tasksComponent = mPlayer.GetCustomComponent<GameTasksComponent>();
                    //        long PlyserTime = Help_TimeHelper.GetNowSecond();
                    //        if (palyerActivitie.Value64A == 0)
                    //        {
                    //            palyerActivitie.Value64A = ActivitieInfo.OpenTime;
                    //            palyerActivitie.Value64B = ActivitieInfo.EndTime;
                    //            tasksComponent.data.PassTasks.Clear();
                    //            tasksComponent.SaveDB();
                    //        }
                    //        else if (palyerActivitie.Value64B < PlyserTime)
                    //        {
                    //            palyerActivitie.Value64A = ActivitieInfo.OpenTime;
                    //            palyerActivitie.Value64B = ActivitieInfo.EndTime;
                    //            tasksComponent.data.PassTasks.Clear();
                    //            tasksComponent.SaveDB();
                    //        }
                    //        PlayerActivity.SetPlayerActivit(13, palyerActivitie);
                    //        PlayerActivity.DBPlayerActivit(13);
                    //    }
                    //}
                }
                async Task Check()
                {
                    switch (b_Request.MiracleActivitiesID)
                    {
                        case 7:
                            //CheckActivities7().Coroutine();
                            break;
                        case 4:
                            //CheckActivities4().Coroutine();
                            break;
                        case 13:
                            CheckActivities13().Coroutine();
                            break;
                    }
                }
                Check().Coroutine();
                b_Reply(b_Response);
                return true;
            }
            b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2401);
            b_Reply(b_Response);
            return false;
        }
    }
}