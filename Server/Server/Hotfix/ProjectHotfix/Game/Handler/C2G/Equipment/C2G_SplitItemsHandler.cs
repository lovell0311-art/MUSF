using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_SplitItemsHandler : AMActorRpcHandler<C2G_SplitItems, G2C_SplitItems>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_SplitItems b_Request, G2C_SplitItems b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_SplitItems b_Request, G2C_SplitItems b_Response, Action<IMessage> b_Reply)
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

            if(b_Request.Count <= 0)
            {
                // 参数错误，拆分物品的数量不能为0
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(750);
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

            //检测背包是否有物品
            Item targetItem = backpackComponent.GetItemByUID(b_Request.ItemUUID);
            if (targetItem == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(705);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("获取背包物品失败!");
                b_Reply(b_Response);
                return true;
            }

            //检测物品是否能被拆分、分堆
            int quantity = targetItem.GetProp(EItemValue.Quantity);
            if (quantity <= 1 || targetItem.ConfigData.StackSize <= 1)
            {
                // 物品不可拆分、分堆!
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(751);
                b_Reply(b_Response);
                return true;
            }
            if (quantity <= b_Request.Count)
            {
                // 分堆数量超过物品本身数量
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(752);
                b_Reply(b_Response);
                return true;
            }
            Item newItem = ItemFactory.Create(targetItem.ConfigID, mPlayer.GameAreaId, b_Request.Count);
            newItem.SetProp(EItemValue.Level, targetItem.GetProp(EItemValue.Level));
            newItem.SetProp(EItemValue.IsBind, targetItem.GetProp(EItemValue.IsBind));
            newItem.UpdateProp();
            if (!backpackComponent.AddItem(newItem, "创建分堆物品",false))
            {
                //加入失败，销毁新建物品
                newItem.Dispose();
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(700);//背包空间不足
                b_Reply(b_Response);
                return false;
            }
            backpackComponent.SetItemQuantity(targetItem, quantity - b_Request.Count);

            b_Reply(b_Response);
            return true;
        }
    }
}