using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CustomFrameWork;
using CustomFrameWork.Component;

namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_OpenSkillGroupRequestHandler : AMActorRpcHandler<C2G_OpenSkillGroupRequest, G2C_OpenSkillGroupResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_OpenSkillGroupRequest b_Request, G2C_OpenSkillGroupResponse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }

        protected override async Task<bool> Run(C2G_OpenSkillGroupRequest b_Request, G2C_OpenSkillGroupResponse b_Response, Action<IMessage> b_Reply)
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

            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();

            DataCacheManageComponent mDataCacheManageComponent = mPlayer.AddCustomComponent<DataCacheManageComponent>();
            var mDataCache = mDataCacheManageComponent.Get<DBGameSkillData>();
            if (mDataCache == null)
            {
                var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, (int)mPlayer.GameAreaId);
                mDataCache = await HelpDb_DBGameSkillData.Init(mPlayer, mDataCacheManageComponent, dBProxy);
            }
            var mData = mDataCache.OnlyOne();
            if (mData == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1400);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("数据库异常!");
                b_Reply(b_Response);
                return false;
            }

            mPlayer.GetCustomComponent<GamePlayer>()?.EnsureMasterGrantedSkills(false);
            if (mData.SkillId == null)
            {
                mData.DeSerialize();
            }

            if (mData.SkillId.Count > 0)
            {
                b_Response.SkillIds.AddRange(mData.SkillId);
            }

            b_Reply(b_Response);
            return true;
        }
    }
}
