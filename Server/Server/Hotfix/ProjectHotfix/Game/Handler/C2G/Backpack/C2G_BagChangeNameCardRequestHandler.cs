
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using CustomFrameWork;
using CustomFrameWork.Component;
using System.Text.RegularExpressions;

namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_BagChangeNameCardRequestHandler : AMActorRpcHandler<C2G_BagChangeNameCardRequest,
        G2C_BagChangeNameCardResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_BagChangeNameCardRequest b_Request, G2C_BagChangeNameCardResponse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_BagChangeNameCardRequest b_Request, G2C_BagChangeNameCardResponse b_Response, Action<IMessage> b_Reply)
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
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("玩家不存在!");
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

            string mRenameStr = b_Request.Name.Trim();
            if (mRenameStr.Equals(""))
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(99);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("更改的名字不能为空!!");
                b_Reply(b_Response);
                return true;
            }
            if (RegexName(mRenameStr) == false)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(314);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("更改的名字不能为空!!");
                b_Reply(b_Response);
                return true;
            }
            if (mRenameStr.Length > 6)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(315);
                b_Reply(b_Response);
                return true;
            }
            bool RegexName(string str)
            {
                return Regex.IsMatch(str, @"^[\u4e00-\u9fa5_a-zA-Z0-9]+$");
            }

            var mNameComponent = Root.MainFactory.GetCustomComponent<NameComponent>();
            if (mNameComponent.Namelist.Contains(mRenameStr))
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(99);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("更改的名字不能为空!!");
                b_Reply(b_Response);
                return true;
            }
            G2M_NameLockRecordRequest g2M_NameLockRecordRequest = new G2M_NameLockRecordRequest();
            g2M_NameLockRecordRequest.Name = mRenameStr;
            var NameLockRecordRequest = await mPlayer.GetSessionMGMT().Call(g2M_NameLockRecordRequest);
            if (NameLockRecordRequest == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(303);
                b_Reply(b_Response);
                return true;
            }
            else if(NameLockRecordRequest.Error != 0)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(304);
                b_Reply(b_Response);
                return true;
            }
            
            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            var mDBProxy = mDBProxyManager.GetZoneDB(DBType.Core, (int)mPlayer.GameAreaId);

            var mQueryNamelist = await mDBProxy.Query<DBGamePlayerData>(p => p.NickName == mRenameStr);
            if (mQueryNamelist != null && mQueryNamelist.Count > 0)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(304);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("昵称重复!");
                b_Reply(b_Response);
                return true;
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
            if (curItem.GetProp(EItemValue.IsLocking) != 0)
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

            var itemConfig = curItem.ConfigData;

            if (itemConfig.Id != 310102)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(705);
                b_Reply(b_Response);
                return false;
            }

            var mCurrentTemp = mPlayer.GetCustomComponent<GamePlayer>();
            mCurrentTemp.Data.NickName = mRenameStr;

            var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, (int)mPlayer.GameAreaId);
            dBProxy2.Save(mCurrentTemp.Data).Coroutine();

            backpack.UseItem(curItem.ItemUID, "使用物品1", 1);
            M2G_NameLockRecord m2G_NameLockRecord1 = new M2G_NameLockRecord();
            m2G_NameLockRecord1.Name = mRenameStr;
            mPlayer.GetSessionMGMT().Send(m2G_NameLockRecord1);
            b_Reply(b_Response);
            return true;
        }
    }
}