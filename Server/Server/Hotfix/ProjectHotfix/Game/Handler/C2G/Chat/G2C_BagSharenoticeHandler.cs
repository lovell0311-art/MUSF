using CustomFrameWork.Component;
using CustomFrameWork;

using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class G2C_BagSharenoticeHandler : AMHandler<G2C_BagSharenotice>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, G2C_BagSharenotice b_Request)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request);
            }
        }
        protected override async Task<bool> Run(G2C_BagSharenotice b_Request)
        {
            var PlayerList = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().GetAllPlayers();
            if (PlayerList == null) return false;

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
            keyValues[b_Request.AllProperty.ItemUUID] = Help_JsonSerializeHelper.DeSerialize<Struct_ItemAllProperty>(Help_JsonSerializeHelper.Serialize(b_Request.AllProperty));


            G2C_BagSharenotice g2C_FullServiceHornnotice = new G2C_BagSharenotice();
            g2C_FullServiceHornnotice.SendGameUserId = b_Request.SendGameUserId;
            g2C_FullServiceHornnotice.SendUserName = b_Request.SendUserName;
            g2C_FullServiceHornnotice.SendTime = CustomFrameWork.TimeHelper.ClientNowSeconds();
            g2C_FullServiceHornnotice.MessageInfo = b_Request.MessageInfo;
            g2C_FullServiceHornnotice.LineId = b_Request.LineId;
            g2C_FullServiceHornnotice.Color = b_Request.Color;
            g2C_FullServiceHornnotice.ShareItemId = b_Request.AllProperty.ItemUUID;
            //g2C_FullServiceHornnotice.AllProperty = b_Request.AllProperty;
            foreach (var player in PlayerList)
            {
                foreach (var player2 in player.Value)
                {
                    player2.Value.Send(g2C_FullServiceHornnotice);
                }
            }
            return true;
        }
    }
}