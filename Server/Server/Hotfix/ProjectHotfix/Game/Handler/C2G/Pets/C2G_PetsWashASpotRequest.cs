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
    public class C2G_PetsWashASpotRequestHandler : AMActorRpcHandler<C2G_PetsWashASpotRequest, C2G_PetsWashASpotResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_PetsWashASpotRequest b_Request, C2G_PetsWashASpotResponse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_PetsWashASpotRequest b_Request, C2G_PetsWashASpotResponse b_Response, Action<IMessage> b_Reply)
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
                    if (Item.Value.ConfigID == 310057 && Item.Value.GetProp(EItemValue.Quantity) >= 1)
                        Iteminfo = Item.Value;
                }
                if (Iteminfo != null)
                {
                    bool IsOk = false;
                    if (mGamePlayer.PetsList.TryGetValue(b_Request.PetsID, out Pets petsInfo))
                    {
                        var mJsonDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<Pets_InfoConfigJson>().JsonDic;
                        int ConfigId = petsInfo.dBPetsData.ConfigID;

                        //mGamePlayer.PetsDleExcellent(petsInfo);

                        petsInfo.dBPetsData.PetsSTR = mJsonDic[ConfigId].Strength;
                        petsInfo.dBPetsData.PetsPSTR = mJsonDic[ConfigId].BoneGas;
                        petsInfo.dBPetsData.PetsPINT = mJsonDic[ConfigId].Willpower;
                        petsInfo.dBPetsData.PetsDEX = mJsonDic[ConfigId].Agility;
                        petsInfo.DataUpdateProperty();
                        petsInfo.UpdataExcellentValue();
                        petsInfo.dBPetsData.AttributePoint = (petsInfo.dBPetsData.PetsLevel - 1) * mJsonDic[ConfigId].AppendLevel;

                        DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
                        var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, (int)mPlayer.GameAreaId);
                        var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get((int)mPlayer.GameAreaId);
                        mWriteDataComponent.Save(petsInfo.dBPetsData, dBProxy).Coroutine();
                        IsOk = true;

                        b_Response.Info = petsInfo.GetPetsInfo(out bool SetDB);

                    }
                    else if (mGamePlayer.Pets.dBPetsData.PetsId == b_Request.PetsID)
                    {
                        var mJsonDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<Pets_InfoConfigJson>().JsonDic;
                        int ConfigId = mGamePlayer.Pets.dBPetsData.ConfigID;

                        //mGamePlayer.PetsDleExcellent(petsInfo);

                        mGamePlayer.Pets.dBPetsData.PetsSTR = mJsonDic[ConfigId].Strength;
                        mGamePlayer.Pets.dBPetsData.PetsPSTR = mJsonDic[ConfigId].BoneGas;
                        mGamePlayer.Pets.dBPetsData.PetsPINT = mJsonDic[ConfigId].Willpower;
                        mGamePlayer.Pets.dBPetsData.PetsDEX = mJsonDic[ConfigId].Agility;
                        mGamePlayer.Pets.DataUpdateProperty();
                        mGamePlayer.Pets.UpdataExcellentValue();
                        mGamePlayer.Pets.dBPetsData.AttributePoint = (mGamePlayer.Pets.dBPetsData.PetsLevel - 1) * mJsonDic[ConfigId].AppendLevel;

                        DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
                        var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, (int)mPlayer.GameAreaId);
                        var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get((int)mPlayer.GameAreaId);
                        mWriteDataComponent.Save(mGamePlayer.Pets.dBPetsData, dBProxy).Coroutine();
                        IsOk = true;

                        b_Response.Info = mGamePlayer.Pets.GetPetsInfo(out bool SetDB);
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
                else
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1609);
                    b_Reply(b_Response);
                    return false;
                }
            }
            b_Reply(b_Response);
            return false;
        }
    }
}