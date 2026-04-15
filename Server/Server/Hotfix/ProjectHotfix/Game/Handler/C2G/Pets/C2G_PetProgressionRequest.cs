using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using TencentCloud.Ame.V20190916.Models;
using TencentCloud.Cfs.V20190719.Models;
using TencentCloud.Mps.V20190612.Models;

namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_PetProgressionRequestHandler : AMActorRpcHandler<C2G_PetProgressionRequest, G2C_PetProgressionResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_PetProgressionRequest b_Request, G2C_PetProgressionResponse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_PetProgressionRequest b_Request, G2C_PetProgressionResponse b_Response, Action<IMessage> b_Reply)
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
            if (mGamePlayer == null || knapsack == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1600);
                b_Reply(b_Response);
                return false;
            }

            Pets pets = null;
            pets = mGamePlayer.Pets;
            if (pets == null || (pets.dBPetsData.PetsId != b_Request.PetsID))
                mGamePlayer.PetsList.TryGetValue(b_Request.PetsID, out pets);

            if (pets != null && pets.dBPetsData.PetsId == b_Request.PetsID)
            {
                var EnhanceMaterials = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<Pets_EnhanceMaterialsConfigJson>().JsonDic;
                Dictionary<int,int> EMInfo = new Dictionary<int, int>() { { 280003, 1 }, { 280004, 1 }, { 320469, 1 } };
                if (EMInfo != null)
                {
                    Dictionary<int, PetsItem> petsItem = new Dictionary<int, PetsItem>();
                    var List = EMInfo.Keys;
                    //整理背包里所需要的道具
                    foreach (var Item in knapsack.mItemDict)
                    {
                        if (List.Contains(Item.Value.ConfigID))
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
                    if (petsItem.Count < EMInfo.Count)
                    {
                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1613);
                        b_Reply(b_Response);
                        return false;
                    }
                    //检查数量
                    foreach (var item in petsItem)
                    {
                        if (EMInfo.TryGetValue(item.Value.ItemConfingID, out int Cnt))
                        {
                            if (Cnt > item.Value.ItemCnt)
                            {
                                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1613);
                                b_Reply(b_Response);
                                return false;
                            }
                        }
                    }
                    //扣除道具
                    foreach (var item in petsItem)
                    {
                        var ItemList = knapsack.GetAllItemByConfigID(item.Value.ItemConfingID);
                        if (ItemList != null)
                        {
                            int Cnt = EMInfo[item.Value.ItemConfingID];

                            foreach (var item2 in ItemList)
                            {
                                if (item2.Value.GetProp(EItemValue.Quantity) >= Cnt)
                                {
                                    if (!knapsack.UseItem(item2.Value, "宠物进阶", Cnt))
                                    {
                                        //代码到这里表示代码出了异常
                                        Log.Error($"宠物进阶，使用道具失败GamePlayer:{mPlayer.GameUserId}");
                                    }
                                    Cnt = 0;
                                    break;
                                }
                                else
                                {
                                    Cnt -= item2.Value.GetProp(EItemValue.Quantity);
                                    if (!knapsack.UseItem(item2.Value, "宠物进阶", item2.Value.GetProp(EItemValue.Quantity)))
                                    {
                                        //代码到这里表示代码出了异常
                                        Log.Error($"宠物进阶，使用道具失败GamePlayer:{mPlayer.GameUserId}");
                                    }

                                }
                            }
                        }
                        else
                        {
                            b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1611);
                            b_Reply(b_Response);
                            return false;
                        }
                    }
                    pets.dBPetsData.AdvancedLevel++;
                    if (pets.dBPetsData.PetsId == mGamePlayer.Pets?.dBPetsData.PetsId)
                    {
                        var equipComponent = mPlayer.GetCustomComponent<EquipmentComponent>();
                        if (equipComponent != null)
                        {
                            equipComponent.ApplyEquipProp();
                        }
                    }

                    DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
                    var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, (int)mPlayer.GameAreaId);
                    var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get((int)mPlayer.GameAreaId);
                    mWriteDataComponent.Save(pets.dBPetsData, dBProxy).Coroutine();
                    b_Reply(b_Response);
                    return true;
                }
            }

            b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1600);
            b_Reply(b_Response);
            return false;
        }
    }
}