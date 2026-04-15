using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Threading.Tasks;
using TencentCloud.Tics.V20181115.Models;

namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_AddExchangeItemHandler : AMActorRpcHandler<C2G_AddExchangeItem, G2C_AddExchangeItem>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_AddExchangeItem b_Request, G2C_AddExchangeItem b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_AddExchangeItem b_Request, G2C_AddExchangeItem b_Response, Action<IMessage> b_Reply)
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

            ExchangeComponent exchangeComponent = mPlayer.GetCustomComponent<ExchangeComponent>();
            if (exchangeComponent == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(810);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("交易组件异常!");
                b_Reply(b_Response);
                return false;
            }

            //检测自己是否正在交易中
            if (exchangeComponent.ExchangeTargetGameUserId <= 0)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(809);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("不在交易中，无法调用接口!");
                b_Reply(b_Response);
                return false;
            }

            //检测对方是否在交易中，若不在交易或交易对象不是自己则终止交易
            Player targetPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(mAreaId, exchangeComponent.ExchangeTargetGameUserId);
            if (targetPlayer != null && targetPlayer.GetCustomComponent<ExchangeComponent>().ExchangeTargetGameUserId == mPlayer.GameUserId)
            {
                //确认锁定状态
                if (exchangeComponent.IsLock)
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(811);
                    //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("已锁定，无法操作!");
                    b_Reply(b_Response);
                    return true;
                }
                //拿到物品
                BackpackComponent backpackComponent = mPlayer.GetCustomComponent<BackpackComponent>();
                Item item = backpackComponent.GetItemByUID(b_Request.ItemUUID);
                if (item != null)
                {
                    // TODO 物品状态限制 - 交易
                    if (item.GetProp(EItemValue.IsBind) != 0)
                    {
                        // 绑定物品无法交易
                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(3104);
                        b_Reply(b_Response);
                        return false;
                    }
                    if (item.GetProp(EItemValue.IsTask) != 0)
                    {
                        // 任务物品无法交易
                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(3105);
                        b_Reply(b_Response);
                        return false;
                    }
                    if (item.GetProp(EItemValue.IsUsing) != 0)
                    {
                        // 使用中的物品无法交易
                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(3106);
                        b_Reply(b_Response);
                        return false;
                    }
                    if (item.GetProp(EItemValue.IsLocking) != 0)
                    {
                        // 锁定的物品无法交易
                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(3107);
                        b_Reply(b_Response);
                        return false;
                    }
                    if (item.GetProp(EItemValue.ValidTime) != 0)
                    {
                        // 时限物品无法交易
                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(3120);
                        b_Reply(b_Response);
                        return false;
                    }
                    //交易限制取消
                    //if (item.HaveExcellentOption() || item.HaveSetOption() || item.Type == EItemType.Pets || item.Type == EItemType.Wing || item.Type == EItemType.Mounts)
                    //{
                    //    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2522);
                    //    b_Reply(b_Response);
                    //    return false;
                    //}

                    //if (item.ConfigData.Sell == 1 && !item.IsArmor() && !item.IsWeapon() && !item.IsAccessory())
                    //{
                    //    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(3304);
                    //    b_Reply(b_Response);
                    //    return false;
                    //}
                    if (exchangeComponent.AddItem(item, b_Request.PosInBackpackX, b_Request.PosInBackpackY))
                    {
                        var notice = new G2C_AddExchangeItem_notice();
                        notice.ItemUUID = b_Request.ItemUUID;
                        notice.PlayerGameUserId = mPlayer.GameUserId;
                        notice.PosInBackpackX = b_Request.PosInBackpackX;
                        notice.PosInBackpackY = b_Request.PosInBackpackY;
                        notice.ItemConfigID = item.ConfigID;
                        notice.ItemLevel = item.GetProp(EItemValue.Level);
                        notice.ItemQuantity = item.GetProp(EItemValue.Quantity);

                        //通知双方物品进入消息
                        mPlayer.Send(notice);
                        item.SendAllPropertyData(mPlayer);
                        item.SendAllEntryAttr(mPlayer);
                        targetPlayer.Send(notice);
                        item.SendAllPropertyData(targetPlayer);
                        item.SendAllEntryAttr(targetPlayer);
                    }
                    else
                    {
                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(812);
                        //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("添加物品失败!");
                    }
                }
                else
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(813);
                    //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("背包找不到物品!");
                }
            }
            else
            {
                mPlayer.Send(new G2C_ExchangeResult_notice()
                {
                    State = false,
                    ErrorMessage = "对方不在交易中，交易终止!"
                });
                exchangeComponent.EndExchange();
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(814);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("对方不在交易中或交易对象不是自己，交易终止!");
            }
            b_Reply(b_Response);
            return true;
        }
    }
}