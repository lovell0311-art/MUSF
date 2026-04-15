using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.Match)]
    public class G2M_EnterGameGetAreaLineInfoMessageHandler : AMRpcHandler<G2M_EnterGameGetAreaLineInfoMessage, M2G_EnterGameGetAreaLineInfoMessage>
    {
        protected override async Task<bool> CodeAsync(Session session, G2M_EnterGameGetAreaLineInfoMessage b_Request, M2G_EnterGameGetAreaLineInfoMessage b_Response, Action<IMessage> b_Reply)
        {
            List<C_GameAreaInfo> mResult = Root.MainFactory.GetCustomComponent<GameAreaComponent>().ServerQueryResult;
            if (mResult.Count <= 0)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(21000);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("游戏区服未开放!");
                b_Reply(b_Response);
                return true;
            }

            int GameAreaID = b_Request.AreaId;
            int GameAreaLineID = b_Request.AreaLineId;

            for (int i = 0, len = mResult.Count; i < len; i++)
            {
                C_GameAreaInfo mDBGameAreaInfo = mResult[i];

                if (mDBGameAreaInfo.GameAreaId == GameAreaID
                    && mDBGameAreaInfo.RealLine == GameAreaLineID)
                {
                    Dictionary<int, int> mGameServerIdDic = mDBGameAreaInfo.GameServerInfo;
                    int mServerId = 0;
                    if (mGameServerIdDic != null)
                    {
                        int mMinValue = 1000;
                        int[] mKeyArray = mGameServerIdDic.Keys.ToArray();
                        for (int j = 0, jlen = mKeyArray.Length; j < jlen; j++)
                        {
                            int mKey = mKeyArray[j];
                            int mValue = mGameServerIdDic[mKey];
                            if (mMinValue > mValue)
                            {
                                mServerId = mKey;
                                mMinValue = mValue;
                            }
                        }
                    }
                    if (mServerId == 0)
                    {
                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(21010);
                        //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("游戏区服未开放!");
                        b_Reply(b_Response);
                        return true;
                    }
                    b_Response.GameServerId = mServerId;
                    break;
                }
            }
            if (b_Response.GameServerId == 0)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(21002);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("目标服务器没有开放!");
            }

            b_Reply(b_Response);
            return true;
        }

    }
}