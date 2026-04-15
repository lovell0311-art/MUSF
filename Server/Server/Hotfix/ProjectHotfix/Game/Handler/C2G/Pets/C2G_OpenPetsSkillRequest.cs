using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_OpenPetsSkillRequestHandler : AMActorRpcHandler<C2G_OpenPetsSkillRequest, G2C_OpenPetsSkillResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_OpenPetsSkillRequest b_Request, G2C_OpenPetsSkillResponse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_OpenPetsSkillRequest b_Request, G2C_OpenPetsSkillResponse b_Response, Action<IMessage> b_Reply)
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
            var bknapsack = mPlayer.GetCustomComponent<BackpackComponent>();
            if (bknapsack == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1601);
                b_Reply(b_Response);
                return false;
            }
            void GetSkillBook(int PetsConfigID)
            {
                var Json2 = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<Item_SkillBooksConfigJson>().JsonDic;
                foreach (var Item in bknapsack.mItemDict)
                {
                    if (Json2.TryGetValue(Item.Value.ConfigID, out Item_SkillBooksConfig SkillBook) != false)
                    {
                        if (SkillBook.ValueDic.TryGetValue(PetsConfigID, out int SkillID) != false)
                        {
                            PetsItem petsItem = new PetsItem();
                            petsItem.ItemConfingID = SkillID;
                            petsItem.ItemID = Item.Key;
                            petsItem.ItemCnt = Item.Value.GetProp(EItemValue.Quantity);
                            b_Response.LesrnSkillList.Add(petsItem);
                        }
                    }
                }
            }
            var mGamePlayer = mPlayer.GetCustomComponent<GamePlayer>();
            if (mGamePlayer != null)
            {
                var SkillJsonDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<Pets_SkillConfigJson>().JsonDic;
                if (mGamePlayer.Pets != null && mGamePlayer.Pets.InstanceId == b_Request.PetsID)
                {
                    b_Response.PetsID = mGamePlayer.Pets.InstanceId;
                    b_Response.SkillList.AddRange(mGamePlayer.Pets.dBPetsData.SkillId);

                    GetSkillBook(mGamePlayer.Pets.dBPetsData.ConfigID);
                }
                else if (mGamePlayer.PetsList.TryGetValue(b_Request.PetsID, out var PetsInfo) != false)
                {
                    b_Response.PetsID = PetsInfo.InstanceId;
                    b_Response.SkillList.AddRange(PetsInfo.dBPetsData.SkillId);
                    GetSkillBook(PetsInfo.dBPetsData.ConfigID);
                }
                else
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1604);
                    b_Reply(b_Response);
                    return false;
                }
            }
            b_Reply(b_Response);
            return true;
        }
    }
}