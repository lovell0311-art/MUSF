using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_FluoreStoneComposeHandler : AMActorRpcHandler<C2G_FluoreStoneCompose, G2C_FluoreStoneCompose>
    {
        
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_FluoreStoneCompose b_Request, G2C_FluoreStoneCompose b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_FluoreStoneCompose b_Request, G2C_FluoreStoneCompose b_Response, Action<IMessage> b_Reply)
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
                b_Reply(b_Response);
                return false;
            }
            BackpackComponent backpack = mPlayer.GetCustomComponent<BackpackComponent>();
            Item add4Set = backpack.GetItemByUID(b_Request.Add4SetEquipItemUID);
            Item add4Exc = backpack.GetItemByUID(b_Request.Add4ExcEquipItemUID);
            Item createGems = backpack.GetItemByUID(b_Request.CreateGemsItemUID);
            Item mayaGems = backpack.GetItemByUID(b_Request.MayaGemsItemUID);
            Item recycledGems = backpack.GetItemByUID(b_Request.RecycledGemsItemUID);

            //检查材料
            if (add4Set == null || add4Exc == null || createGems == null || mayaGems == null || recycledGems == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1702);
                b_Reply(b_Response);
                return false;
            }

            //检查金钱
            if (mPlayer.GetCustomComponent<GamePlayer>().Data.GoldCoin < 1000000)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1704);//金币不足
                b_Reply(b_Response);
                return false;
            }

            //检查材料性质
            if (!add4Set.IsEquipment() || !add4Set.IsEnoughLevel(4) ||!add4Set.IsSetEquip())
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1714);//请放入正确的套装装备
                b_Reply(b_Response);
                return false;
            }

            if (!add4Exc.IsEquipment() || !add4Exc.IsEnoughLevel(4) || !add4Exc.IsExcellentEquip())
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1715);//请放入正确的卓越装备
                b_Reply(b_Response);
                return false;
            }

            if (createGems.ConfigID != (int)EItemStrengthen.CREATE_GEMS || mayaGems.ConfigID != (int)EItemStrengthen.MAYA_GEMS || recycledGems.ConfigID != (int)EItemStrengthen.RECYCLED_GEMS)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1701);//请放入正确的宝石
                b_Reply(b_Response);
                return false;
            }

            //装备消耗掉以后必然有1x1空间，所以不判断空间是否足够，直接成功
            backpack.UseItem(add4Set.ItemUID,"荧之石合成", 1);
            backpack.UseItem(add4Exc.ItemUID, "荧之石合成", 1);
            backpack.UseItem(createGems.ItemUID, "荧之石合成", 1);
            backpack.UseItem(mayaGems.ItemUID, "荧之石合成", 1);
            backpack.UseItem(recycledGems.ItemUID, "荧之石合成", 1);

            EItemFluoreGem[] randomList = {
                EItemFluoreGem.FluoreStone_Fire,
                EItemFluoreGem.FluoreStone_Ice,
                EItemFluoreGem.FluoreStone_Light,
                EItemFluoreGem.FluoreStone_Soil,
                EItemFluoreGem.FluoreStone_Water,
                EItemFluoreGem.FluoreStone_Wind
            };

            //合成随机荧之石
            Item fluoreStone = ItemFactory.Create((int)randomList[RandomHelper.RandomNumber(0, randomList.Length)], mPlayer.GameAreaId);
            backpack.AddItem(fluoreStone, "荧之石合成");

            b_Reply(b_Response);
            return true;
        }
    }
}