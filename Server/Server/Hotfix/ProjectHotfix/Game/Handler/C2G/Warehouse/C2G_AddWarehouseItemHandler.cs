using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CustomFrameWork;
using CustomFrameWork.Baseic;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_AddWarehouseItemHandler : AMActorRpcHandler<C2G_AddWarehouseItem, G2C_AddWarehouseItem>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_AddWarehouseItem b_Request, G2C_AddWarehouseItem b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_AddWarehouseItem b_Request, G2C_AddWarehouseItem b_Response, Action<IMessage> b_Reply)
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
            var backpackComponent = mPlayer.GetCustomComponent<BackpackComponent>();
            //从背包拿出物品
            var item = backpackComponent.GetItemByUID(b_Request.ItemUUID);
            if (item != null)
            {
                // TODO 物品状态限制 - 仓库
                if (item.GetProp(EItemValue.IsBind) == 2)
                {
                    // 绑定角色的物品无法移到仓库
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(3112);
                    b_Reply(b_Response);
                    return false;
                }
                if (item.GetProp(EItemValue.IsTask) != 0)
                {
                    // 任务物品无法移到仓库
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(3113);
                    b_Reply(b_Response);
                    return false;
                }
                if (item.GetProp(EItemValue.IsUsing) != 0)
                {
                    // 使用中的物品无法移到仓库
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(3114);
                    b_Reply(b_Response);
                    return false;
                }
                if (item.GetProp(EItemValue.IsLocking) != 0)
                {
                    // 锁定的物品无法移到仓库
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(3115);
                    b_Reply(b_Response);
                    return false;
                }

                var warehouseComponent = mPlayer.GetCustomComponent<WarehouseComponent>();
                if (warehouseComponent.CanAddItem(item, b_Request.PosInBackpackX, b_Request.PosInBackpackY))
                {
                    backpackComponent.RemoveItem(item, "移到仓库");
                    if (!warehouseComponent.AddItem(item, b_Request.PosInBackpackX, b_Request.PosInBackpackY,"背包移出"))
                    {
                        // 运行到这里，说明背包仓库代码已经失控了.
                        Log.Error($"移动物品错误（背包 -> 仓库）");
                        // 玩家反馈后，通过日志恢复物品
                        Log.PLog("Error", $"移动物品错误（背包 -> 仓库），mPlayer.UserId={mPlayer.UserId} mPlayer.GameUserId={mPlayer.GameUserId} item=({item.ToLogString()})");
                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(700);//指定位置装不下物品
                        b_Reply(b_Response);
                        return false;
                    }
                }
                else
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(700);//指定位置装不下物品
                    b_Reply(b_Response);
                    return false;
                }
                
            }
            else {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(807);
                b_Reply(b_Response);
                return false;
            }
            b_Reply(b_Response);
            return true;
        }
    }
}