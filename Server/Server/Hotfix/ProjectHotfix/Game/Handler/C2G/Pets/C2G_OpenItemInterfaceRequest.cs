using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_OpenItemInterfaceRequestHandler : AMActorRpcHandler<C2G_OpenItemInterfaceRequest, G2C_OpenItemInterfaceResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_OpenItemInterfaceRequest b_Request, G2C_OpenItemInterfaceResponse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_OpenItemInterfaceRequest b_Request, G2C_OpenItemInterfaceResponse b_Response, Action<IMessage> b_Reply)
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
            var knapsack = mPlayer.GetCustomComponent<BackpackComponent>();
            if (knapsack != null)
            {
                Dictionary<int, PetsItem> petsItem = new Dictionary<int, PetsItem>();
                foreach (var Item in knapsack.mItemDict)
                {
                    if (Item.Value.ConfigID == 310053 || Item.Value.ConfigID == 310054 || Item.Value.ConfigID == 310055)
                    {
                        if (petsItem.TryGetValue(Item.Value.ConfigID, out PetsItem petsItem2) == false)
                        {
                            petsItem2 = new PetsItem();
                            petsItem2.ItemConfingID = Item.Value.ConfigID;
                            petsItem2.ItemID = Item.Key;
                            petsItem2.ItemCnt = Item.Value.GetProp(EItemValue.Quantity);
                            petsItem.Add(Item.Value.ConfigID, petsItem2);
                        }
                        else
                            petsItem2.ItemCnt += Item.Value.GetProp(EItemValue.Quantity);

                    }
                }
                b_Response.List.AddRange(petsItem.Values);
            }
            b_Reply(b_Response);
            return true;
        }
    }
}