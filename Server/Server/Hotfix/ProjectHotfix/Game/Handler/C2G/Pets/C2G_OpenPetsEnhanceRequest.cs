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
    public class C2G_OpenPetsEnhanceRequestHandler : AMActorRpcHandler<C2G_OpenPetsEnhanceRequest, G2C_OpenPetsEnhanceResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_OpenPetsEnhanceRequest b_Request, G2C_OpenPetsEnhanceResponse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_OpenPetsEnhanceRequest b_Request, G2C_OpenPetsEnhanceResponse b_Response, Action<IMessage> b_Reply)
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

            var gameplayer = mPlayer.GetCustomComponent<GamePlayer>();
            var knapsack = mPlayer.GetCustomComponent<BackpackComponent>();
            if (knapsack == null || gameplayer == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1601);
                b_Reply(b_Response);
                return false;
            }

            Pets pets = null;
            pets = gameplayer.Pets;
            if (pets == null || (pets.dBPetsData.PetsId != b_Request.PetsID))
                gameplayer.PetsList.TryGetValue(b_Request.PetsID, out pets);

            if (pets != null)
            {
                if (pets.dBPetsData.EnhanceLv + 1 > 15)
                {
                    b_Response.Lv = 15;
                    b_Reply(b_Response);
                    return true;
                }
                var EnhanceMaterials = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<Pets_EnhanceMaterialsConfigJson>().JsonDic;
                var EMInfo = EnhanceMaterials[pets.dBPetsData.EnhanceLv + 1].Enhance;
                if (EMInfo != null)
                {
                    Dictionary<int, PetsItem> petsItem = new Dictionary<int, PetsItem>();
                    var List = EMInfo.Keys;

                    foreach (var Item in EMInfo)
                    {
                        if (petsItem.TryGetValue(Item.Key, out PetsItem petsItem2) == false)
                        {
                            petsItem2 = new PetsItem();
                            petsItem2.ItemConfingID = Item.Key;
                            petsItem2.ItemID = Item.Value;
                            petsItem2.ItemCnt = 0;
                            petsItem.Add(Item.Key, petsItem2);
                        }
                    }

                    foreach (var Item in knapsack.mItemDict)
                    {
                        if (List.Contains(Item.Value.ConfigID))
                        {
                            if (petsItem.TryGetValue(Item.Value.ConfigID, out PetsItem petsItem2))
                                petsItem2.ItemCnt += Item.Value.GetProp(EItemValue.Quantity);

                        }
                    }

                    b_Response.Lv = pets.dBPetsData.EnhanceLv;
                    b_Response.Rate = EnhanceMaterials[pets.dBPetsData.EnhanceLv + 1].SuccessRate;
                    b_Response.EnhanceMaterials.AddRange(petsItem.Values);
                    b_Reply(b_Response);
                    return true;
                }
            }
            b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1601);
            b_Reply(b_Response);
            return false;
        }
    }
}