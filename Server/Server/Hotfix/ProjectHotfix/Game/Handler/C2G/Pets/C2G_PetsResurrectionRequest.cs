using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using TencentCloud.Hcm.V20181106.Models;

namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_PetsResurrectionRequestHandler : AMActorRpcHandler<C2G_PetsResurrectionRequest, G2C_PetsResurrectionResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_PetsResurrectionRequest b_Request, G2C_PetsResurrectionResponse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_PetsResurrectionRequest b_Request, G2C_PetsResurrectionResponse b_Response, Action<IMessage> b_Reply)
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

            var mGamePlayer = mPlayer.GetCustomComponent<GamePlayer>();
            var knapsack = mPlayer.GetCustomComponent<BackpackComponent>();

            /*if (mGamePlayer != null && mGamePlayer.Data.Level < 50)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1600);
                b_Reply(b_Response);
                return false;
            }*/

            if (mGamePlayer != null && knapsack != null)
            {
                Item Iteminfo = null;
                foreach (var Item in knapsack.mItemDict)
                {
                    if (Item.Value.ConfigID == 310056 && Item.Value.GetProp(EItemValue.Quantity) >= 1)
                        Iteminfo = Item.Value;
                }
                if (Iteminfo != null)
                {
                    bool IsOk = false;
                    Pets petsInfo = null;
                    if (mGamePlayer.PetsList.TryGetValue(b_Request.PetsID, out petsInfo))
                    {

                    }
                    else if (mGamePlayer.Pets.dBPetsData.PetsId == b_Request.PetsID)
                    {
                        petsInfo = mGamePlayer.Pets;
                    }

                    if (petsInfo != null)
                    {
                        // 复活宠物
                        RebirthHelper.Rebirth(petsInfo);

                        DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
                        var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, (int)mPlayer.GameAreaId);
                        var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get((int)mPlayer.GameAreaId);
                        mWriteDataComponent.Save(petsInfo.dBPetsData, dBProxy).Coroutine();
                        IsOk = true;
                        b_Response.Info = petsInfo.GetPetsInfo(out bool SetDB);
                    }


                    if (IsOk)
                    {
                        if (Iteminfo.GetProp(EItemValue.Quantity) > 1)
                        {
                            //物品数量减少1，广播物品属性改变
                            knapsack.UseItem(Iteminfo, "宠物使用", 1);
                        }
                        else
                        {
                            //物品用完，删除物品
                            knapsack.DeleteItem(Iteminfo, $"宠物使用");
                        }
                        b_Reply(b_Response);
                        return true;
                    }
                }
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1609);
                b_Reply(b_Response);
                return false;
            }
            b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(200);
            b_Reply(b_Response);
            return false;
        }
    }
}