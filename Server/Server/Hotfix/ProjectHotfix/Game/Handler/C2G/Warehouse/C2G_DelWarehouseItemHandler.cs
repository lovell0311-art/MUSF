using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using CustomFrameWork;
using CustomFrameWork.Component;

namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_DelWarehouseItemHandler : AMActorRpcHandler<C2G_DelWarehouseItem, G2C_DelWarehouseItem>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_DelWarehouseItem b_Request, G2C_DelWarehouseItem b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_DelWarehouseItem b_Request, G2C_DelWarehouseItem b_Response, Action<IMessage> b_Reply)
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
            if (warehouseComponent.mItemDict.TryGetValue(b_Request.ItemUUID, out Item itemData))
            {
                BackpackComponent backpack = mPlayer.GetCustomComponent<BackpackComponent>();
                if (backpack.CanAddItem(itemData, b_Request.PosInBackpackX, b_Request.PosInBackpackY))
                {
                    warehouseComponent.RemoveItem(itemData,"移出到背包");
                    if(!backpack.AddItem(itemData, b_Request.PosInBackpackX, b_Request.PosInBackpackY, "仓库移出"))
                    {
                        // 运行到这里，说明背包仓库代码已经失控了.
                        Log.Error($"移动物品错误（仓库 -> 背包）");
                        // 玩家反馈后，通过日志恢复物品
                        Log.PLog("Error", $"移动物品错误（仓库 -> 背包），mPlayer.UserId={mPlayer.UserId} mPlayer.GameUserId={mPlayer.GameUserId} item=({itemData.ToLogString()})");
                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(700);
                        //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("指定位置装不下物品");
                        b_Reply(b_Response);
                        return false;
                    }
                }
                else
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(700);
                    //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("指定位置装不下物品");
                    b_Reply(b_Response);
                    return false;
                }
            }
            else
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1901); //仓库找不到要移动的物品
            }

            b_Reply(b_Response);
            return true;
        }
    }
}