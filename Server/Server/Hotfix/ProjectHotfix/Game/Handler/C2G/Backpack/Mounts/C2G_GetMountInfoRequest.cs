using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using CustomFrameWork;
using CustomFrameWork.Component;


namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_GetMountInfoRequestHandler : AMActorRpcHandler<C2G_GetMountInfoRequest, G2C_GetMountInfoResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_GetMountInfoRequest b_Request, G2C_GetMountInfoResponse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }

        protected override async Task<bool> Run(C2G_GetMountInfoRequest b_Request, G2C_GetMountInfoResponse b_Response, Action<IMessage> b_Reply)
        {
            int mAreaId = (int)(b_Request.AppendData >> 16);
            C_ServerArea mServerArea = Root.MainFactory.GetCustomComponent<ServerAreaManagerComponent>().GetGameArea((int)b_Request.AppendData);
            if (mServerArea == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(21006);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("当前游戏服务器非目标服务器!");
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
            if (mPlayer.GameAreaId <= 0)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(201);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("游戏区服不存在!");
                b_Reply(b_Response);
                return false;
            }

            var backpackComponent = mPlayer.GetCustomComponent<BackpackComponent>();
            if (backpackComponent == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(704);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("背包组件异常!");
                b_Reply(b_Response);
                return false;
            }
            var Mount = backpackComponent.GetMountInfo(b_Request.MountID);
            if (Mount == null)
            {
                b_Response.Error = 704;
                b_Reply(b_Response);
                return true;
            }

            b_Response.MountID = Mount.ItemUID;
            b_Response.ConfigId = Mount.ConfigID;
            b_Response.Fortifiedlevel = Mount.GetProp(EItemValue.Level);
            b_Response.AdvancedLevel = Mount.GetProp(EItemValue.Advanced);
            b_Response.IsUsing = Mount.GetProp(EItemValue.IsUsing);
            b_Reply(b_Response);
            return true;
        }
    }
}