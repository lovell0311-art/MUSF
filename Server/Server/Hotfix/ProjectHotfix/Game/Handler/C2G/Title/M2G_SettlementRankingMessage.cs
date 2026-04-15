using CustomFrameWork.Component;
using CustomFrameWork;

using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TencentCloud.Bri.V20190328.Models;
using TencentCloud.Hcm.V20181106.Models;

namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class M2G_SettlementRankingMessageHandler : AMHandler<M2G_SettlementRankingMessage>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, M2G_SettlementRankingMessage b_Request)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request);
            }
        }
        protected override async Task<bool> Run(M2G_SettlementRankingMessage b_Request)
        {
            var playerlist = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().GetAllByZone(OptionComponent.Options.ZoneId);
            if (playerlist == null) { return false; }
            switch ((RankType)b_Request.RankType)
            { 
                case RankType.LevelRank:
                    { 
                        
                    }
                    break;
                case RankType.SiegeWarfare:
                    {
                        G2C_SendFullServiceMessage g2C_SendFullServiceMessage = new G2C_SendFullServiceMessage();
                        g2C_SendFullServiceMessage.TitleID = b_Request.Value32A;
                        g2C_SendFullServiceMessage.MessageId = 2709;
                        g2C_SendFullServiceMessage.PlayerName = b_Request.StrA;
                        if (b_Request.Value32B == 1)
                        {
                            foreach (var player in playerlist)
                            {
                                player.Value.Send(g2C_SendFullServiceMessage);
                            }
                        }

                        if (playerlist.TryGetValue(b_Request.Value64A, out var mPlayer))
                        {
                            if (b_Request.Value32B == 0)
                            {
                                mPlayer.Send(g2C_SendFullServiceMessage);
                            }
                            var Info = mPlayer.GetCustomComponent<PlayerTitle>();
                            if (Info != null)
                            {
                                await Info.LoadNewTitle(b_Request.Value32A, 0);
                            }
                            G2C_ServerSendTitleMessage g2C_ServerSendTitleMessage = new G2C_ServerSendTitleMessage();
                            g2C_ServerSendTitleMessage.UseTitle = Info.UseTitle;
                            foreach (var item in Info.ListString)
                            {
                                Title_Status title_Status = new Title_Status();
                                title_Status.TitleID = item.TitleID;
                                title_Status.BingTime = item.BingTime;
                                title_Status.EndTime = item.EndTime;
                                g2C_ServerSendTitleMessage.TitleList.Add(title_Status);
                            }
                            mPlayer.Send(g2C_ServerSendTitleMessage);
                            mPlayer.AddCustomComponent<CastleMasterCheckComponent>();

                            var maill = mPlayer.GetCustomComponent<PlayerMailComponent>();
                            if (maill != null)
                            {
                                maill.mailInfos.Clear();
                                maill.ServerMail.Clear();
                                DataCacheManageComponent mDataCacheComponent = mPlayer.GetCustomComponent<DataCacheManageComponent>();
                                if (mDataCacheComponent.Remove<DBMailData>())
                                    await maill.PlayerLoadMail(OptionComponent.Options.ZoneId);
                            }

                            var WarAlliance = mPlayer.GetCustomComponent<PlayerWarAllianceComponent>();
                            if (WarAlliance != null && WarAlliance.WarAllianceID != 0)
                            {
                                WarAlliance.AllianceScore += 1000;
                                WarAlliance.UpDateWarAlliancePlayerInfo();
                                G2C_ChangeValue_notice mChangeValueMessage = new G2C_ChangeValue_notice();
                                G2C_BattleKVData mGoldCoinData = new G2C_BattleKVData();
                                mGoldCoinData.Key = (int)E_GameProperty.AllianceScoreChange;
                                mGoldCoinData.Value = WarAlliance.AllianceScore;
                                mChangeValueMessage.Info.Add(mGoldCoinData);
                                mPlayer.Send(mChangeValueMessage);
                            }
                        }
                    }
                    break;
                default:
                    Log.Warning("排行结束类型异常");
                        break;
            }
            return true;
        }
    }
}