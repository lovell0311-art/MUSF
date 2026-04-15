
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using CustomFrameWork;
using CustomFrameWork.Component;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_MoveWarehouseItemHandler : AMActorRpcHandler<C2G_MoveWarehouseItem,
        G2C_MoveWarehouseItem>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_MoveWarehouseItem b_Request, G2C_MoveWarehouseItem b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_MoveWarehouseItem b_Request, G2C_MoveWarehouseItem b_Response, Action<IMessage> b_Reply)
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

            var warehouseComponent = mPlayer.GetCustomComponent<WarehouseComponent>();
            if (warehouseComponent == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1900);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("仓库组件异常!");
                b_Reply(b_Response);
                return false;
            }

            //检测是否有该物品
            //var delItems = dataCache.DataQuery(p => p.ItemData.ItemUID == b_Request.ItemUUID && 
            //p.IsDispose == 0);
            //if (delItems.Count > 0)
            if (warehouseComponent.mItemDict.TryGetValue(b_Request.ItemUUID, out Item itemData) == false)
            {
                //b_Response.PropId = b_Request.ItemUUID;
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(703);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("背包找不到要删除的物品!ID:" + b_Request.ItemUUID);
                b_Reply(b_Response);
                return false;
            }
            if (warehouseComponent.MoveItem(itemData, b_Request.PosInBackpackX, b_Request.PosInBackpackY) == false)
            {
                //b_Response.PropId = b_Request.ItemUUID;
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(708);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("移动物品失败!ID:" + b_Request.ItemUUID);
            }

            b_Reply(b_Response);
            return true;
        }
    }
}