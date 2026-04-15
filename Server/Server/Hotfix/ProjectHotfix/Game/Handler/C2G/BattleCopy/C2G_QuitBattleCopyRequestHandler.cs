using ETModel;
using System;
using System.Threading.Tasks;
using CustomFrameWork;
using CustomFrameWork.Component;

namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_QuitBattleCopyRequestHandler : AMActorRpcHandler<C2G_QuitBattleCopyRequest, G2C_QuitBattleCopyResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_QuitBattleCopyRequest b_Request, G2C_QuitBattleCopyResponse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }

        protected override async Task<bool> Run(C2G_QuitBattleCopyRequest b_Request, G2C_QuitBattleCopyResponse b_Response, Action<IMessage> b_Reply)
        {
            try
            {
                int type = b_Request.Type;
                if (type < 1 && type > 2)
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2605);
                    //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("副本类型参数错误");
                    b_Reply(b_Response);
                    return false;
                }
                int level = b_Request.Level;
                if (level < 1 || level > 7)
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2606);
                    //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("副本等级错误");
                    b_Reply(b_Response);
                    return false;
                }

                int mAreaId = (int)(b_Request.AppendData >> 16);
                Player mPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(mAreaId, b_Request.ActorId);
                if (mPlayer == null)
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(200);
                    //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("玩家没有找到!");
                    b_Reply(b_Response);
                    return false;
                }

                C_ServerArea mServerArea = Root.MainFactory.GetCustomComponent<ServerAreaManagerComponent>().GetGameArea((int)b_Request.AppendData);
                if (mServerArea == null)
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(21006);
                    //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("当前游戏服务器非目标服务器!");
                    b_Reply(b_Response);
                    return true;
                }
                if (!BatteCopyManagerComponent.BattleCopyMapIDList.Contains(mPlayer.GetCustomComponent<GamePlayer>().UnitData.Index))
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2600);
                    b_Reply(b_Response);
                    return false;
                }

                BatteCopyManagerComponent batteCopyManagerCpt = mServerArea.GetCustomComponent<BatteCopyManagerComponent>();
                if (batteCopyManagerCpt == null)
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2600);
                    //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("没有找到副本管理组件");
                    b_Reply(b_Response);
                    return false;
                }
                BattleCopyComponent battleCopyCpt = batteCopyManagerCpt.Get(type);
                if (battleCopyCpt == null)
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2610);
                    //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("没有找到恶魔广场组件");
                    b_Reply(b_Response);
                    return false;
                }

                long userId = b_Request.ActorId;
      
                DBProxyManagerComponent mDBProxyManagerComponent = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
                battleCopyCpt.QuitRoom(userId);//先退出副本房间
                if (!battleCopyCpt.QuitMap(userId, mDBProxyManagerComponent, mServerArea))//在退出地图
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2612);
                    //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("退出副本地图失败");
                    b_Reply(b_Response);
                    return false;
                }
                battleCopyCpt.RemoveUser(userId);
            }
            catch (Exception e)
            {
                Log.Error($"退出恶魔广场地图时，发生错误，错误日志 : {e}"); 
            }

            b_Reply(b_Response);
            return true;
        }
    }
}
