using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_GetCurrentBloodInfoVesselsHandler : AMActorRpcHandler<C2G_GetCurrentBloodInfoVessels, G2C_GetCurrentBloodInfoVessels>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_GetCurrentBloodInfoVessels b_Request, G2C_GetCurrentBloodInfoVessels b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }

        protected override async Task<bool> Run(C2G_GetCurrentBloodInfoVessels b_Request, G2C_GetCurrentBloodInfoVessels b_Response, Action<IMessage> b_Reply)
        {
            C_ServerArea mServerArea = Root.MainFactory.GetCustomComponent<ServerAreaManagerComponent>().GetGameArea((int)b_Request.AppendData);
            if (mServerArea == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(21006);
                b_Reply(b_Response);
                return true;
            }
            Player mPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(mServerArea.GameAreaId, b_Request.ActorId);
            if (mPlayer == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(200);
                b_Reply(b_Response);
                return false;
            }
            if (mPlayer.GameAreaId <= 0 || b_Request.BloodId == 0)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(201);
                b_Reply(b_Response);
                return false;
            }

            var mPlayerBloodAwakening = mPlayer.GetCustomComponent<PlayerBloodAwakeningComponent>();
            if (mPlayerBloodAwakening != null)
            {
                if (mPlayerBloodAwakening.BloodAwakeningInfo.TryGetValue(b_Request.BloodId, out var info))
                {
                    if (info.ActivateNeedTime <= Help_TimeHelper.GetNowSecond() && info.ActivateNeedTime != 0)
                    {
                        var BloodJson = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<BloodAwakening_InfoConfigJson>().JsonDic;
                        BloodJson.TryGetValue(b_Request.BloodId, out var Info);
                        info.ActivateNeedTime = 0;
                        var AttributeNodeJson = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<AttributeNode_InfoConfigJson>().JsonDic;
                        if (Info.AttributeNode.TryGetValue(info.UrrentRingNumber + 1, out var Attribute))
                        {
                            info.UrrentRingNumber++;
                            info.AttributeNode.Add(info.UrrentRingNumber, new List<int>());
                            foreach (var AttributeId in Attribute)
                            {
                                if (AttributeNodeJson.ContainsKey(AttributeId.Key))
                                {
                                    if (AttributeId.Value)
                                    {
                                        info.AttributeNode[info.UrrentRingNumber].Add(AttributeId.Key);
                                    }
                                }
                            }

                            info.AttributeNodeStr = Help_JsonSerializeHelper.Serialize(info.AttributeNode);
                            //属性应用
                            if (mPlayerBloodAwakening.UseBloodAwakeningId == b_Request.BloodId)
                            {
                                mPlayerBloodAwakening.EditAttribute(b_Request.BloodId);
                                mPlayerBloodAwakening.AttributeApplication(b_Request.BloodId);
                            }
                            mPlayerBloodAwakening.SetDB(b_Request.BloodId);
                        }
                    }
                }

                b_Response.BloodInfo = mPlayerBloodAwakening.GetBloodSendInfo(b_Request.BloodId);
                b_Reply(b_Response);
                return true;
            }
            b_Response.Error = 3803;
            b_Reply(b_Response);
            return true;
        }
    }
}