
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using CustomFrameWork;
using CustomFrameWork.Component;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_PlayerUseItemInTheBackpackHandler : AMActorRpcHandler<C2G_PlayerUseItemInTheBackpack,
        G2C_PlayerUseItemInTheBackpack>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_PlayerUseItemInTheBackpack b_Request, G2C_PlayerUseItemInTheBackpack b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_PlayerUseItemInTheBackpack b_Request, G2C_PlayerUseItemInTheBackpack b_Response, Action<IMessage> b_Reply)
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

            //var ErrorCode = UseItemSystem.UseBackPackItem(b_Request.ItemUUID, mPlayer);
            //if (ErrorCode != ErrorCodeHotfix.ERR_Success)
            //{
            //    b_Response.Error = ErrorCode;
            //    b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("使用物品错误，详见错误码注释");
            //}



            BackpackComponent backpack = mPlayer.GetCustomComponent<BackpackComponent>();
            if (backpack == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(702);
                b_Reply(b_Response);
                return true;
            }
            //找到物品
            if (backpack.mItemDict.TryGetValue(b_Request.ItemUUID, out Item curItem) == false)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(705);
                b_Reply(b_Response);
                return true;
            }

            // TODO 物品状态限制 - 使用
            if(curItem.GetProp(EItemValue.IsLocking) != 0)
            {
                // 锁定的物品无法使用
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(3119);
                b_Reply(b_Response);
                return false;
            }

            if (curItem.GetProp(EItemValue.Quantity) <= 0)
            {
                // 无效物品，请销毁
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(754);
                b_Reply(b_Response);
                return false;
            }

            switch (curItem.Type)
            {
                case EItemType.SkillBooks:
                    {
                        var mJsonDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<Item_SkillBooksConfigJson>().JsonDic;

                        if (mJsonDic.TryGetValue(curItem.ConfigID, out var mConfig) == false)
                        {
                            b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(701);
                            b_Reply(b_Response);
                            return true;
                        }

                        if (!curItem.CanUse(mPlayer.GetCustomComponent<GamePlayer>()))
                        {
                            b_Response.Error = 725;
                            b_Reply(b_Response);
                            return true;
                        }

                        var mItemUseRuleCreateBuilder = Root.MainFactory.GetCustomComponent<ItemUseRuleCreateBuilder>();
                        if (mItemUseRuleCreateBuilder.CacheDatas.TryGetValue(mConfig.UseMethod, out var mItemUseRuleType) == false)
                        {
                            b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(709);
                            b_Reply(b_Response);
                            return true;
                        }

                        try
                        {
                            var mItemUseRule = Root.CreateBuilder.GetInstance<C_ItemUseRule<Player, Item, IResponse>>(mItemUseRuleType);
                            await mItemUseRule.Run(mPlayer, curItem, b_Response);
                            mItemUseRule.Dispose();
                        }
                        catch (Exception e)
                        {

                            Log.Error($"{mConfig.Name} 物品使用异常",e);
                        }
                    }
                    break;
                case EItemType.Consumables:
                case EItemType.Mounts:
                    {
                        var itemConfig = curItem.ConfigData;

                        if (!curItem.CanUse(mPlayer.GetCustomComponent<GamePlayer>()))
                        {
                            b_Response.Error = 725;
                            b_Reply(b_Response);
                            return true;
                        }

                        var mItemUseRuleCreateBuilder = Root.MainFactory.GetCustomComponent<ItemUseRuleCreateBuilder>();
                        if (mItemUseRuleCreateBuilder.CacheDatas.TryGetValue(itemConfig.UseMethod, out var mItemUseRuleType) == false)
                        {
                            b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(710);
                            b_Response.Message = "物品不存在";
                            b_Reply(b_Response);
                            return true;
                        }

                        try
                        {
                            var mItemUseRule = Root.CreateBuilder.GetInstance<C_ItemUseRule<Player, Item, IResponse>>(mItemUseRuleType);
                            await mItemUseRule.Run(mPlayer, curItem, b_Response);
                            mItemUseRule.Dispose();
                        }
                        catch (Exception e)
                        {
                            Log.Error($"{itemConfig.Name} 物品使用异常",e);
                        }
                    }
                    break;
                case EItemType.Pets:
                    {
                        var itemConfig = curItem.ConfigData;

                        if (!curItem.CanUse(mPlayer.GetCustomComponent<GamePlayer>()))
                        {
                            b_Response.Error = 725;
                            b_Reply(b_Response);
                            return true;
                        }

                        var mItemUseRuleCreateBuilder = Root.MainFactory.GetCustomComponent<ItemUseRuleCreateBuilder>();
                        if (mItemUseRuleCreateBuilder.CacheDatas.TryGetValue(itemConfig.UseMethod, out var mItemUseRuleType) == false)
                        {
                            b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(710);
                            b_Response.Message = "物品不存在";
                            b_Reply(b_Response);
                            return true;
                        }

                        try
                        {
                            var mItemUseRule = Root.CreateBuilder.GetInstance<C_ItemUseRule<Player, Item, IResponse>>(mItemUseRuleType);
                            await mItemUseRule.Run(mPlayer, curItem, b_Response);
                            mItemUseRule.Dispose();
                        }
                        catch (Exception e)
                        {
                            Log.Error($"{itemConfig.Name} 物品使用异常", e);
                        }
                    }
                    break;
                default:
                    {
                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(709);
                        b_Reply(b_Response);
                        return true;
                    }
                    break;
            }

            if (b_Response.Error == 0 && curItem.Type != EItemType.Mounts)
            {
                switch (curItem.ConfigID)
                {
                    case 310073:
                    case 310075:
                    case 310076:
                    case 310086:
                    case 310087:

                        break;
                    default:
                        backpack.UseItem(curItem.ItemUID, "使用物品1", 1);
                        break;
                }
            }

            b_Reply(b_Response);
            return true;
        }
    }
}