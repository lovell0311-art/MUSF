using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using TencentCloud.Mps.V20190612.Models;

namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_UseItemAddExpRequestHandler : AMActorRpcHandler<C2G_UseItemAddExpRequest, G2C_UseItemAddExpResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_UseItemAddExpRequest b_Request, G2C_UseItemAddExpResponse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_UseItemAddExpRequest b_Request, G2C_UseItemAddExpResponse b_Response, Action<IMessage> b_Reply)
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

            var mGamePlayer = mPlayer.GetCustomComponent<GamePlayer>();
            var knapsack = mPlayer.GetCustomComponent<BackpackComponent>();
            /*if (mGamePlayer != null && mGamePlayer.Data.Level < 50)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1600);
                b_Reply(b_Response);
                return false;
            }*/
            if (knapsack != null && mGamePlayer != null)
            {
                bool IsUseOK = false;
                var ItemInfo = knapsack.GetItemByUID(b_Request.ItemID);
                if (ItemInfo == null)
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1609);
                    b_Reply(b_Response);
                    return false;
                }
                int ItemCont = ItemInfo.GetProp(EItemValue.Quantity);
                if (ItemCont < 1)
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1602);
                    b_Reply(b_Response);
                    return false;
                }

                if (ItemInfo != null)
                {
                    if (mGamePlayer.Pets != null && mGamePlayer.Pets.dBPetsData.PetsId == b_Request.PetsID)
                    {
                        mGamePlayer.Pets.GamePlayer = mGamePlayer;
                        if (await mGamePlayer.Pets.AddExprience(ItemInfo.ConfigData.Value))
                        {
                            b_Response.Info = new PetsInfo();
                            b_Response.Info = mGamePlayer.Pets.GetPetsInfo(out bool SetDB);
                            IsUseOK = true;
                        }
                    }
                    else
                    {
                        if (mGamePlayer.PetsList.TryGetValue(b_Request.PetsID, out var PetsInfo) != false)
                        {
                            PetsInfo.GamePlayer = mGamePlayer;
                            if (await PetsInfo.AddExprience(ItemInfo.ConfigData.Value))
                            {
                                b_Response.Info = new PetsInfo();
                                b_Response.Info = PetsInfo.GetPetsInfo(out bool SetDB);
                                IsUseOK = true;
                            }
                        }
                    }
                }
                if (IsUseOK)
                {
                    if (ItemCont > 1)
                    {
                        //物品数量减少1，广播物品属性改变
                        knapsack.UseItem(ItemInfo, "宠物增加经验", 1);
                    }
                    else
                    {
                        //物品用完，删除物品
                        knapsack.DeleteItem(ItemInfo, "宠物增加经验");
                    }
                }
                else
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1602);
                    b_Reply(b_Response);
                    return false;
                }
            }

            b_Reply(b_Response);
            return true;
        }
    }
}