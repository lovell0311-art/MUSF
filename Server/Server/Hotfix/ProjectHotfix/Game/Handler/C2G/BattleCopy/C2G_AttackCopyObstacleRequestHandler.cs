using System;
using ETModel;
using CustomFrameWork;
using System.Threading.Tasks;
using CustomFrameWork.Component;

namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_AttackCopyObstacleRequestHandler : AMActorRpcHandler<C2G_AttackCopyObstacleRequest, G2C_AttackCopyObstacleRequest>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_AttackCopyObstacleRequest b_Request, G2C_AttackCopyObstacleRequest b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_AttackCopyObstacleRequest b_Request, G2C_AttackCopyObstacleRequest b_Response, Action<IMessage> b_Reply)
        {
            await Task.Delay(0);

            C_ServerArea mServerArea = Root.MainFactory.GetCustomComponent<ServerAreaManagerComponent>().GetGameArea((int)b_Request.AppendData);
            if (mServerArea == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(21006);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("当前游戏服务器非目标服务器!");
                b_Reply(b_Response);
                return true;
            }
      
            BatteCopyManagerComponent batteCopyManagerCpt = mServerArea.GetCustomComponent<BatteCopyManagerComponent>();
            if (batteCopyManagerCpt == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2600);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("没有找到副本管理组件");
                b_Reply(b_Response);
                return false;
            }

            BattleCopyComponent battleCopyCpt = batteCopyManagerCpt.Get((int)CopyType.RedCastle);
            if (battleCopyCpt == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2601);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("没有找到副本组件");
                b_Reply(b_Response);
                return false;
            }

            long userId = b_Request.ActorId;
            if (!battleCopyCpt.copyRankDataDic.ContainsKey(userId))
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2602);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("攻击副本障碍时,组件中找不到玩家");
                b_Reply(b_Response);
                return false;
            }

            CopyRankData copyRankData = battleCopyCpt.copyRankDataDic[userId];
            long level = copyRankData.Level;
            int index = copyRankData.Index;
            BattleCopyRoom battleCopyRoom = battleCopyCpt.battleCopyRoomDic[level][index];
            if (battleCopyRoom == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2603);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("攻击副本障碍时，没有找到副本房间");
                b_Reply(b_Response);
                return false;
            }

            b_Response.Number = battleCopyRoom.AttackCopyObstacle(userId);

            b_Reply(b_Response);
            return true;
        }

    }
}
