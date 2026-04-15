
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using CustomFrameWork;
using CustomFrameWork.Component;
using Aop.Api.Domain;

namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_PurchaseOfTradeCardHandler : AMActorRpcHandler<C2G_PurchaseOfTradeCard,
        G2C_PurchaseOfTradeCard>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_PurchaseOfTradeCard b_Request, G2C_PurchaseOfTradeCard b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_PurchaseOfTradeCard b_Request, G2C_PurchaseOfTradeCard b_Response, Action<IMessage> b_Reply)
        {
            int mAreaId = (int)(b_Request.AppendData >> 16);
            Player mPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(mAreaId, b_Request.ActorId);
            if (mPlayer == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(200);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("不存在!");
                b_Reply(b_Response);
                return false;
            }
            if (mPlayer.GameAreaId <= 0)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(201);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("游戏区服不存在!");
                b_Reply(b_Response);
                return false;
            }

            var tasksComponent = mPlayer.GetCustomComponent<GameTasksComponent>();

            var taskConfigManager = Root.MainFactory.GetCustomComponent<GameTaskConfigManager>();
            if (!taskConfigManager.TryGetConfig(b_Request.PassId, out var taskConf))
            {
                // 没找到任务配置
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2000);
                b_Reply(b_Response);
                return true;
            }

            int Money = 500;
            if (Money > mPlayer.Data.YuanbaoCoin)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2205);
                b_Reply(b_Response);
                return false;
            }

            foreach (var info in tasksComponent.data.PassTasks)
            {
                if (info.Value.ConfigId/1000 == b_Request.PassId/1000)
                {
                    // 重复领取任务
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2002);
                    b_Reply(b_Response);
                    return true;
                }
                if (info.Value.TaskState == EGameTaskState.Doing)
                {
                    // 重复领取任务
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2002);
                    b_Reply(b_Response);
                    return true;
                }
            }
            var gameplayer = mPlayer.GetCustomComponent<GamePlayer>();
            gameplayer.UpdateCoin(E_GameProperty.YuanbaoCoin, -Money, "购买通行证");
            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, mPlayer.GameAreaId);
            var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get(mPlayer.GameAreaId);
            mWriteDataComponent.Save(gameplayer.Player.Data, dBProxy).Coroutine();

            G2C_ChangeValue_notice mChangeValueMessage = new G2C_ChangeValue_notice();
            G2C_BattleKVData mGoldCoinData = new G2C_BattleKVData();
            mGoldCoinData.Key = (int)E_GameProperty.YuanbaoCoin;
            mGoldCoinData.Value = gameplayer.Player.Data.YuanbaoCoin;
            mChangeValueMessage.Info.Add(mGoldCoinData);
            mPlayer.Send(mChangeValueMessage);

            if (!taskConf.CanReceive(mPlayer.GetCustomComponent<GamePlayer>()))
            {
                // 不满注释足领取条件
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2001);
                b_Reply(b_Response);
                return true;
            }
            if (tasksComponent.data.PassTasks.ContainsKey(b_Request.PassId))
            {
                // 重复领取任务
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2002);
                b_Reply(b_Response);
                return true;
            }
            if (!tasksComponent.BeforeTaskComplete(b_Request.PassId))
            {
                // 上一个任务还未完成
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2009);
                b_Reply(b_Response);
                return true;
            }

            var gameTask = GameTaskFactory.Create(taskConf);
            gameTask.TaskState = EGameTaskState.Doing;

            foreach (var beforeId in gameTask.Config.TaskBeforeId)
            {
                if (tasksComponent.data.PassTasks.ContainsKey(beforeId))
                {
                    tasksComponent.data.PassTasks.Remove(beforeId);
                    tasksComponent.SaveDB();
                }
            }

            GamePassTasksHelper.AddTask(tasksComponent, gameTask);
            b_Reply(b_Response);
            return true;
        }
    }
}