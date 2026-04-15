using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CustomFrameWork;
using CustomFrameWork.Baseic;
namespace ETHotfix
{
#if DEVELOP
    [MessageHandler(AppType.Game)]
    public class C2G_AddBackpackItemRequestHandler : AMActorRpcHandler<C2G_AddBackpackItemRequest, G2C_AddBackpackItemResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_AddBackpackItemRequest b_Request, G2C_AddBackpackItemResponse b_Response, Action<IMessage> b_Reply)
        {
            using(await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }

        protected override async Task<bool> Run(C2G_AddBackpackItemRequest b_Request, G2C_AddBackpackItemResponse b_Response, Action<IMessage> b_Reply)
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

            //生成物品

            var backpackComponent = mPlayer.GetCustomComponent<BackpackComponent>();

            if (backpackComponent == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(704);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("背包组件异常!");
                b_Reply(b_Response);
                return false;
            }

            ItemConfig mIConfig = Root.MainFactory.GetCustomComponent<ItemConfigManagerComponent>().Get((int)b_Request.ItemConfigID);
            if (mIConfig == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(701);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage($"物品配置不存在!  ItemConfigID:{b_Request.ItemConfigID}");
                b_Reply(b_Response);
                return false;
            }

            if (backpackComponent.mItemBox.CheckStatus(mIConfig.X, mIConfig.Y, b_Request.PosInBackpackX, b_Request.PosInBackpackY))
            {
                var itemCreateAttr = new ItemCreateAttr();
                itemCreateAttr.Level = b_Request.Level;
                itemCreateAttr.Quantity = b_Request.Quantity;
                itemCreateAttr.OptListId = b_Request.OptListId;
                itemCreateAttr.OptLevel = b_Request.OptLevel;
                itemCreateAttr.HaveSkill = (b_Request.HasSkill == 1) ? true : false;
                itemCreateAttr.SetId = b_Request.SetId;
                itemCreateAttr.OptionExcellent.AddRange(b_Request.OptionExcellent.array);
                itemCreateAttr.CustomAttrMethod.AddRange(b_Request.CustomAttrMethod.array);
                itemCreateAttr.FluoreAttr = b_Request.FluoreAttr;
                itemCreateAttr.FluoreSlotCount = b_Request.FluoreSlotCount;
                itemCreateAttr.FluoreSlot.AddRange(b_Request.FluoreSlot.array);

                Item newItem = ItemFactory.Create(mIConfig, mPlayer.GameAreaId, itemCreateAttr);
                //添加物品到背包
                newItem.data.GameUserId = mPlayer.GameUserId;
                //newItem.data.Width = b_Request.Width;
                //newItem.data.Height = b_Request.Height;
                backpackComponent.AddItem(newItem, b_Request.PosInBackpackX, b_Request.PosInBackpackY,"GM添加");
            }
            else
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(700);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("背包当前位置装不下物品!");
            }
            Log.Debug("=============================收到添加物品信息:" + b_Request.ItemConfigID);

            b_Reply(b_Response);
            return true;
        }
    }
#endif
}