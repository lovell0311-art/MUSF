using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using CustomFrameWork;
using CustomFrameWork.Component;

using UnityEngine;
using TencentCloud.Hcm.V20181106.Models;

namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_OpenTheChestHandler : AMActorRpcHandler<C2G_OpenTheSpecialTreasureChestRequest, G2C_OpenTheSpecialTreasureChestResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_OpenTheSpecialTreasureChestRequest b_Request, G2C_OpenTheSpecialTreasureChestResponse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }

        protected override async Task<bool> Run(C2G_OpenTheSpecialTreasureChestRequest b_Request, G2C_OpenTheSpecialTreasureChestResponse b_Response, Action<IMessage> b_Reply)
        {
            int mAreaId = (int)(b_Request.AppendData >> 16);
            C_ServerArea mServerArea = Root.MainFactory.GetCustomComponent<ServerAreaManagerComponent>().GetGameArea((int)b_Request.AppendData);
            if (mServerArea == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(21006);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("当前游戏服务器非目标服务器!");
                b_Reply(b_Response);
                return true;
            }
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
            if (backpackComponent == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(704);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("背包组件异常!");
                b_Reply(b_Response);
                return false;
            }
            int Ax = 0;
            int Ay = 0;
            if (!backpackComponent.mItemBox.CheckStatus(4, 6, ref Ax, ref Ay))
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(700);
                b_Reply(b_Response);
                return false;
            }
            Dictionary<int,int> keyValuePairs = new Dictionary<int, int>() 
            { {320407,320433 },{ 320408,320434},{ 320409,320435} };
            Item TreasureChest = backpackComponent.GetItemByUID(b_Request.TreasureChestId);
            if (TreasureChest != null)
            {
                Item Chestkey = backpackComponent.GetItemByUID(b_Request.ChestkeyId);
                if (keyValuePairs.TryGetValue(TreasureChest.ConfigID, out var KeyId))
                {
                    if (Chestkey.ConfigID != KeyId)
                    {
                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(782);
                        b_Reply(b_Response);
                        return false;
                    }
                }

                if (Chestkey != null)
                {
                    var treasureChest = Root.MainFactory.GetCustomComponent<TreasureChestManager>();
                    var readConfig = Root.MainFactory.GetCustomComponent<ReadConfigComponent>();
                    if (readConfig.GetJson<TreasureChest_TypeConfigJson>().JsonDic.TryGetValue(TreasureChest.ConfigID, out var dropConfig))
                    {
                        if (treasureChest.ItemSelector.TryGetValue(TreasureChest.ConfigID, out var selector))
                        {
                            var itemInfoJson = readConfig.GetJson<TreasureChest_ItemInfoConfigJson>().JsonDic;

                            if (dropConfig.NotRepeat == 1)
                            {
                                selector = new RandomSelector<int>(selector);
                            }
                            int count = dropConfig.GetCountDrop();
                            using ListComponent<Item> itemList = ListComponent<Item>.Create();
                            if (dropConfig.MustFall.Count >= 1)
                            {
                                foreach (var Info in dropConfig.MustFall)
                                {
                                    itemInfoJson.TryGetValue(Info.Key, out var itemInfoConfig);
                                    for (int j = 0; j < Info.Value; j++)
                                    {
                                        var item = ItemFactory.TryCreate(itemInfoConfig.ItemId, mPlayer.GameAreaId, itemInfoConfig.ToItemCreateAttr());
                                        if (item == null)
                                        {
                                            Log.Error($"配置的物品id不存在{item.ConfigID},itemInfoConfig.ItemId={itemInfoConfig.ItemId}");
                                            continue;
                                        }
                                        itemList.Add(item);
                                    }
                                }
                            }

                            for (int i = 0; i < count; i++)
                            {
                                int itemInfoId = 0;
                                if (dropConfig.NotRepeat == 1)
                                {
                                    if (!selector.TryGetValueAndRemove(out itemInfoId))
                                    {
                                    }
                                }
                                else
                                {
                                    if (!selector.TryGetValue(out itemInfoId))
                                    {
                                    }
                                }

                                itemInfoJson.TryGetValue(itemInfoId, out var itemInfoConfig);
                                try
                                {
                                    var item = ItemFactory.Create(itemInfoConfig.ItemId, mPlayer.GameAreaId, itemInfoConfig.ToItemCreateAttr());
                                    itemList.Add(item);
                                }
                                catch (ItemNotSupportAttrException e)
                                {
                                    Log.Warning($"宝箱配置的物品无法添加指定属性. args.mapItem.InstanceId={TreasureChest.ConfigID},itemInfoConfig.ItemId={itemInfoConfig.ItemId}({e.ToString()})");
                                }
                                catch (ItemConfigNotExistException e)
                                {
                                    Log.Error($"宝箱配置的物品id不存在. args.mapItem.InstanceId={TreasureChest.ConfigID},itemInfoConfig.ItemId={itemInfoConfig.ItemId}({e.ToString()})");
                                }
                            }
                            if (itemList.Count == 0)
                            {
                                Log.Error($"宝箱没有配置可以掉落的物品. args.mapItem.InstanceId={TreasureChest.ConfigID}");
                            }

                            int posX = 0;
                            int posY = 0;
                            List<(Item item, int x, int y)> DropItemList = new List<(Item, int, int)>();
                            // 锁格子用的，类似 lock_guard
                            using var backpackLockList = ItemsBoxStatus.LockList.Create();
                            foreach (var Item in itemList)
                            {
                                if (!backpackComponent.mItemBox.CheckStatus(Item.ConfigData.X, Item.ConfigData.Y, ref posX, ref posY))
                                {
                                    var ItemDelList = DropItemList.ToArray();
                                    foreach (var v in ItemDelList)
                                    {
                                        v.item.Dispose();
                                    }
                                    Item.Dispose();

                                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(700);
                                    b_Reply(b_Response);
                                    return false;
                                }
                                backpackLockList.Add(backpackComponent.mItemBox.LockGrid(Item.ConfigData.X, Item.ConfigData.Y, posX, posY));
                                DropItemList.Add((Item, posX, posY));
                            }
                            // 手动释放锁
                            backpackLockList.Dispose();

                            backpackComponent.UseItem(TreasureChest, "宝箱开启");
                            backpackComponent.UseItem(Chestkey, "宝箱开启");
                            foreach (var item in itemList) 
                            {
                                mPlayer.GetCustomComponent<GamePlayer>().SendItem(5, item).Coroutine();
                                if (!backpackComponent.AddItem(item, "宝箱开启"))
                                {
                                    Log.Error($"代码异常-PlayerId:{mPlayer.GameUserId} 宝箱Id:{TreasureChest.ItemUID} 道具Id:{item.ConfigID}");
                                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(700);
                                    b_Reply(b_Response);
                                    return false;
                                }
                            }
                        }
                    }
                }
            }
            b_Reply(b_Response);
            return false;
        }
    }
}