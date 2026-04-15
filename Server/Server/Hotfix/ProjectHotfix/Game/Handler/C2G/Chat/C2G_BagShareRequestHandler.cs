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
    public class C2G_BagShareRequestHandler : AMActorRpcHandler<C2G_BagShareRequest, G2C_BagShareResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_BagShareRequest b_Request, G2C_BagShareResponse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_BagShareRequest b_Request, G2C_BagShareResponse b_Response, Action<IMessage> b_Reply)
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
            var Bk = mPlayer.GetCustomComponent<BackpackComponent>();
            if (Bk == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(704);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("背包组件异常!");
                b_Reply(b_Response);
                return false;
            }

            var mGamePlayer = mPlayer.GetCustomComponent<GamePlayer>();
            if (mGamePlayer.Data.Level < 100)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(779);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("背包组件异常!");
                b_Reply(b_Response);
                return false;
            }


            var mItem = Bk.GetItemByUID(b_Request.ShareItemId);
            if (mItem == null)
            {
                //背包物品没有
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2211);
                b_Reply(b_Response);
                return false;
            }
            if (mItem.IsEquipment() == false)
            {  //背包物品没有
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2211);
                b_Reply(b_Response);
                return false;
            }

            int LineId = mPlayer.SourceGameAreaId & 0xffff;

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
            keyValues[mItem.ItemUID] = Help_JsonSerializeHelper.DeSerialize<Struct_ItemAllProperty>(Help_JsonSerializeHelper.Serialize(mItem.ToItemAllProperty()));

            if (mPlayer.GetCustomComponent<PlayerShopMallComponent>().GetPlayerShopState(DeviationType.MaxMonthlyCard))
            {
                G2C_BagSharenotice g2C_FullServiceHornnotice = new G2C_BagSharenotice();
                g2C_FullServiceHornnotice.SendGameUserId = mPlayer.GameUserId;
                g2C_FullServiceHornnotice.SendUserName = mPlayer.GetCustomComponent<GamePlayer>().Data.NickName;
                g2C_FullServiceHornnotice.SendTime = CustomFrameWork.TimeHelper.ClientNowSeconds();
                g2C_FullServiceHornnotice.MessageInfo = b_Request.MessageInfo;
                g2C_FullServiceHornnotice.LineId = LineId;
                g2C_FullServiceHornnotice.AllProperty = keyValues[mItem.ItemUID];
                g2C_FullServiceHornnotice.Color = b_Request.Color | 1 << 8;
                var mMatchConfigs = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfos(AppType.Game);
                foreach (var Server in mMatchConfigs)
                {
                    Dictionary<int, List<int>> keyValuePairs = Help_JsonSerializeHelper.DeSerialize<Dictionary<int, List<int>>>(Server.RunParameter);
                    int AreaId = 1;
                    foreach (var KeyValuePair in keyValuePairs)
                    {
                        AreaId = KeyValuePair.Key >> 16;
                        break;
                    }
                    if (mAreaId == AreaId)
                        Game.Scene.GetComponent<NetInnerComponent>().Get(Server.ServerInnerIP).Send(g2C_FullServiceHornnotice);
                }
            }
            else
            {
                var PlayerList = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().GetAllPlayers();
                if (PlayerList == null) return false;

                G2C_BagSharenotice g2C_FullServiceHornnotice = new G2C_BagSharenotice();
                g2C_FullServiceHornnotice.SendGameUserId = mPlayer.GameUserId;
                g2C_FullServiceHornnotice.SendUserName = mPlayer.GetCustomComponent<GamePlayer>().Data.NickName;
                g2C_FullServiceHornnotice.SendTime = CustomFrameWork.TimeHelper.ClientNowSeconds();
                g2C_FullServiceHornnotice.MessageInfo = b_Request.MessageInfo;
                g2C_FullServiceHornnotice.LineId = LineId;
                g2C_FullServiceHornnotice.Color = b_Request.Color;
                g2C_FullServiceHornnotice.ShareItemId = mItem.ItemUID;
                //g2C_FullServiceHornnotice.AllProperty = b_Request.AllProperty;
                foreach (var player in PlayerList)
                {
                    foreach (var player2 in player.Value)
                    {
                        player2.Value.Send(g2C_FullServiceHornnotice);
                    }
                }
            }
          
            b_Reply(b_Response);
            return true;
        }
    }
}