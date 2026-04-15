using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_ItemsSynthesisHandler : AMActorRpcHandler<C2G_ItemsSynthesis, G2C_ItemsSynthesis>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_ItemsSynthesis b_Request, G2C_ItemsSynthesis b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_ItemsSynthesis b_Request, G2C_ItemsSynthesis b_Response, Action<IMessage> b_Reply)
        {
            int mAreaId = (int)(b_Request.AppendData >> 16);
            Player mPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(mAreaId, b_Request.ActorId);
            if (mPlayer == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(200);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("账号不存在!");
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
            var npcShopConfig = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<Synthesis_InfoConfigJson>().JsonDic;
            if (!npcShopConfig.ContainsKey(b_Request.MethodConfigID))
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1721);
                b_Reply(b_Response);
                return false;
            }

            var mSynthesisRuleCreateBuilder = Root.MainFactory.GetCustomComponent<SynthesisRuleCreateBuilder>();
            if (mSynthesisRuleCreateBuilder.CacheDatas.TryGetValue(npcShopConfig[b_Request.MethodConfigID].Method, out var mSynthesisRuleType) == false)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1722);
                b_Reply(b_Response);
                return false;
            }
            var itemIDList = new List<long>(b_Request.AllItemUUID);
            SynthesisComponent synthesis = mPlayer.GetCustomComponent<SynthesisComponent>();
            if (synthesis == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1720);
                b_Reply(b_Response);
                return false;
            }
            var itemList = new List<Item>();
            for (int i = 0; i < itemIDList.Count; i++)
            {
                Item curItem = synthesis.GetItemByUID(itemIDList[i]);
                if (curItem != null)
                {
                    itemList.Add(curItem);
                }
                else {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1719);
                    b_Reply(b_Response);
                    return false;
                }
            }
            Log.Debug("进入合成执行方法");
            var mSynthesisRule = Root.CreateBuilder.GetInstance<C_SynthesisRule<SynthesisComponent, List<Item> , Synthesis_InfoConfig, G2C_ItemsSynthesis,int>>(mSynthesisRuleType);
            await mSynthesisRule.Run(synthesis, itemList, npcShopConfig[b_Request.MethodConfigID], b_Response, b_Request.ClientFinalR);
            mSynthesisRule.Dispose();

            if (b_Response.Error != 0)
            {
                b_Reply(b_Response);
                return false;
            }

            b_Reply(b_Response);

            await ETModel.ET.TimerComponent.Instance.WaitFrameAsync();
            synthesis.SendAllItemProp();
            return true;
        }
    }
}