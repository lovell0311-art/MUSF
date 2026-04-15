using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TencentCloud.Kms.V20190118.Models;

namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_BagShareGetInfoRequestHandler : AMActorRpcHandler<C2G_BagShareGetInfoRequest, G2C_BagShareGetInfoResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_BagShareGetInfoRequest b_Request, G2C_BagShareGetInfoResponse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_BagShareGetInfoRequest b_Request, G2C_BagShareGetInfoResponse b_Response, Action<IMessage> b_Reply)
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
            //检查道具
            //var Bk = mPlayer.GetCustomComponent<BackpackComponent>();
            //if (Bk == null)
            //{
            //    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(704);
            //    //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("背包组件异常!");
            //    b_Reply(b_Response);
            //    return false;
            //}
            //检查月卡
            //if (!mPlayer.GetCustomComponent<PlayerShopMallComponent>().GetPlayerShopState(DeviationType.MaxMonthlyCard)
            //    && !mPlayer.GetCustomComponent<PlayerShopMallComponent>().GetPlayerShopState(DeviationType.MinMonthlyCard))
            //{
            //    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2211);
            //    b_Reply(b_Response);
            //    return false;
            //}

            var mChatManageComponent = Root.MainFactory.GetCustomComponent<ChatManageComponent>();
            var mkey = DateTime.Now.Date.Ticks;
            if (mChatManageComponent.ShareItemDic.TryGetValue(mkey, out var keyValues) == false)
            {
                keyValues = mChatManageComponent.ShareItemDic[mkey] = new Dictionary<long, Struct_ItemAllProperty>();
                #region clear
                int mMaxCapacity = 3;
                if (mChatManageComponent.ShareItemDic.Count > mMaxCapacity)
                {
                    List<long> mkeylist = new List<long>();
                    for (int i = 0; i < mMaxCapacity; i++)
                    {
                        mkeylist.Add(DateTime.Now.Date.AddDays(i * -1).Ticks);
                    }

                    var mDisposelist = mChatManageComponent.ShareItemDic.Keys.Except(mkeylist).ToList();
                    for (int i = 0, len = mDisposelist.Count; i < len; i++)
                    {
                        var mDispose = mDisposelist[i];

                        if (mChatManageComponent.ShareItemDic.TryGetValue(mDispose, out var mTempdic))
                        {
                            mChatManageComponent.ShareItemDic.Remove(mDispose);

                            mTempdic.Clear();
                        }
                    }
                }
                #endregion
            }

            if (keyValues.TryGetValue(b_Request.ShareItemId, out var value) == false)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2211);
                b_Reply(b_Response);
                return false;
            } 
            
            b_Response.AllProperty = value;

            b_Reply(b_Response);
            return true;
        }
    }
}