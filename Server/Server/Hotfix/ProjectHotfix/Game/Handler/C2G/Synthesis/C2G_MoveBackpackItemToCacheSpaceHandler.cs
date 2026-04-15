using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_MoveBackpackItemToCacheSpaceHandler : AMActorRpcHandler<C2G_MoveBackpackItemToCacheSpace, G2C_MoveBackpackItemToCacheSpace>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_MoveBackpackItemToCacheSpace b_Request, G2C_MoveBackpackItemToCacheSpace b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_MoveBackpackItemToCacheSpace b_Request, G2C_MoveBackpackItemToCacheSpace b_Response, Action<IMessage> b_Reply)
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
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(99);
                b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("合成组件异常!");
                b_Reply(b_Response);
                return false;
            }
            Item curItem = backpackComponent.GetItemByUID(b_Request.MovedItemUUID);
            if (curItem == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(705);
                b_Reply(b_Response);
                return false;
            }
            //添加背包物品，不进入mItemBox里，只在ItemDict保存，
            //合成逻辑有问题部分道具消耗存在异常，增加一个存储位置类型来区分
            var Iteminfp = backpackComponent.RemoveItem(curItem, "移动到合成空间");
            Iteminfp.data.InComponent = EItemInComponent.TemporarySpace;
            await Iteminfp.SaveDBNow();
            synthesisComponent.AddItem(Iteminfp);

            b_Reply(b_Response);
            return true;
        }
    }
}