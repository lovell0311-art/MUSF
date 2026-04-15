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
    public class G2M_GetGameAreaInfoMessageHandler : AMRpcHandler<G2M_GetGameAreaInfoMessage, M2G_GetGameAreaInfoMessage>
    {
        protected override async Task<bool> CodeAsync(Session session, G2M_GetGameAreaInfoMessage b_Request, M2G_GetGameAreaInfoMessage b_Response, Action<IMessage> b_Reply)
        {
            if (b_Request.GetGameAreaPage == 0)
            {
                DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
                var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, DBProxyComponent.CommonDBId);

                if (dBProxy == null)
                {
                    Log.Error($"dBProxy:{dBProxy == null}");

                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(21001);
                    //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("服务异常!");
                    b_Reply(b_Response);
                    return false;
                }
                else
                {
                    List<ComponentWithId> mResult = await dBProxy.Query<DBGameAreaLoginData>(p => p.UserId == b_Request.UserId);
                    // 有没有区服登录日志
                    //List<DBGameAreaLoginInfoData> mResult = await mDBProxyComponent.DataQuery<DBGameAreaLoginInfoData>(
                    //   , sortP => sortP.LastLoginTime == -1);
                    if (mResult == null)
                    {
                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(21001);
                        //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("服务异常!");
                        b_Reply(b_Response);
                        return false;
                    }
                    int GameAreaId = 0;
                    if (mResult.Count > 0)
                    {
                        List<DBGameAreaLoginData> mResultTarget = mResult.Select(p => p as DBGameAreaLoginData).ToList();
                        mResultTarget.Sort(
                                (left, right) =>
                                {
                                    if (left.LastLoginTime < right.LastLoginTime)
                                    {
                                        return 1;
                                    }
                                    else if (left.LastLoginTime == right.LastLoginTime)
                                    {
                                        return 0;
                                    }
                                    else
                                    {
                                        return -1;
                                    }
                                }
                            );

                        GameAreaId = mResultTarget[0].GameAreaId;
                    }
                    List<C_GameAreaInfo> mResultData = Root.MainFactory.GetCustomComponent<GameAreaComponent>().ServerQueryResult;
                    if (mResultData.Count <= 0)
                    {
                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(21000);
                        //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("游戏区服未开放!");
                        b_Reply(b_Response);
                        return true;
                    }

                    //b_Response.GameAreaInfos = new List<G2C_GameAreaInfoMessage>();

                    C_GameAreaInfo mDBGameAreaInfo = null;
                    if (GameAreaId > 0)
                    {
                        for (int i = 0, len = mResultData.Count; i < len; i++)
                        {
                            C_GameAreaInfo mData = mResultData[i];
                            if (mData.RealLine != 0) continue;

                            if (mData.GameAreaId == GameAreaId)
                            {
                                mDBGameAreaInfo = mData;
                                break;
                            }
                        }
                    }
                    // 没有 给最新服
                    if (mDBGameAreaInfo == null)
                    {
                        for (int i = 0, len = mResultData.Count; i < len; i++)
                        {
                            C_GameAreaInfo mData = mResultData[i];
                            if (mData.RealLine != 0) continue;

                            mDBGameAreaInfo = mData;
                            break;
                        }
                    }
                    G2C_GameAreaInfoMessage2 mGameAreaInfoMessage = GetMessage(mDBGameAreaInfo);
                    b_Response.GameAreaInfos.Add(mGameAreaInfoMessage);
                }
            }
            else
            {
                List<C_GameAreaInfo> mResult = Root.MainFactory.GetCustomComponent<GameAreaComponent>().ServerQueryResult;
                if (mResult.Count > 0)
                {
                    //b_Response.GameAreaInfos = new List<G2C_GameAreaInfoMessage>();

                    if (b_Request.GetGameAreaPage < 0) b_Request.GetGameAreaPage = 0;
                    int len = b_Request.GetGameAreaPage * 20;
                    if (len > mResult.Count) len = mResult.Count;
                    int i = len - 20;
                    if (i < 0) i = 0;
                    for (; i < len; i++)
                    {
                        C_GameAreaInfo mDBGameAreaInfo = mResult[i];
                        if (mDBGameAreaInfo.RealLine == 0) continue;
                        G2C_GameAreaInfoMessage2 mGameAreaInfoMessage = GetMessage(mDBGameAreaInfo);
                        b_Response.GameAreaInfos.Add(mGameAreaInfoMessage);
                    }
                }
                else
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(21000);
                }
            }
            Log.Debug(Newtonsoft.Json.JsonConvert.SerializeObject(b_Response));
            b_Reply(b_Response);
            return true;
        }
        private G2C_GameAreaInfoMessage2 GetMessage(C_GameAreaInfo b_DBGameAreaInfo)
        {
            G2C_GameAreaInfoMessage2 mResult = new G2C_GameAreaInfoMessage2();
            mResult.GameAreaId = b_DBGameAreaInfo.GameAreaId;
            mResult.GameAreaNickName = b_DBGameAreaInfo.NickName;
            mResult.IsGameAreaState = b_DBGameAreaInfo.State;
            //游戏区服状态  0正常 1爆满 2新服
            if (DateTime.UtcNow.Ticks - b_DBGameAreaInfo.CreateTime < 2591999999997)
            {
                mResult.GameAreaType = 2;
            }
            else if (b_DBGameAreaInfo.PlayerCount > 500)
            {
                mResult.GameAreaType = 1;
            }

            return mResult;
        }
    }
}