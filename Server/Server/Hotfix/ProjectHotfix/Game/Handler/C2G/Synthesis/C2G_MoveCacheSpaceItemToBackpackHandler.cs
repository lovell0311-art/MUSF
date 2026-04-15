using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_MoveCacheSpaceItemToBackpackHandler : AMActorRpcHandler<C2G_MoveCacheSpaceItemToBackpack, G2C_MoveCacheSpaceItemToBackpack>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_MoveCacheSpaceItemToBackpack b_Request, G2C_MoveCacheSpaceItemToBackpack b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_MoveCacheSpaceItemToBackpack b_Request, G2C_MoveCacheSpaceItemToBackpack b_Response, Action<IMessage> b_Reply)
        {
            int mAreaId = (int)(b_Request.AppendData >> 16);
            Player mPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(mAreaId, b_Request.ActorId);
            if (mPlayer == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(200);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("账号不存在!");
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
            BackpackComponent backpackComponent = mPlayer.GetCustomComponent<BackpackComponent>();
            if (backpackComponent == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(704);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("背包组件异常!");
                b_Reply(b_Response);
                return false;
            }
            SynthesisComponent synthesisComponent = mPlayer.GetCustomComponent<SynthesisComponent>();
            if (synthesisComponent == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1720);
                b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("合成组件异常!");
                b_Reply(b_Response);
                return false;
            }

            Item curItem = synthesisComponent.DeleteItem(b_Request.MovedItemUUID);
            if (curItem == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(705);
                b_Reply(b_Response);
                return false;
            }
            if (!backpackComponent.AddItem(curItem, b_Request.PosInBackpackX, b_Request.PosInBackpackY,"合成临时空间移出"))
            {
                synthesisComponent.AddItem(curItem);
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(700);
                b_Reply(b_Response);
                return false;
            }

            b_Reply(b_Response);
            return true;
        }
    }
}