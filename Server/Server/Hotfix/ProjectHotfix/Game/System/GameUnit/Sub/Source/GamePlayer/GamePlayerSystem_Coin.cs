using ETModel;
using System.Collections.Generic;
using CustomFrameWork.Component;
using CustomFrameWork;

namespace ETHotfix
{
    [FriendOf(typeof(DBAccountZoneData))]
    public static class GamePlayerSystem_Coin
    {
        public static long GetGoldCoin(this GamePlayer b_GamePlayer)
        {
            return b_GamePlayer.Data.GoldCoin;
        }
        public static void UpdateCoin(this GamePlayer b_GamePlayer, E_GameProperty b_GameProperty, int b_ChangeValue, string b_Str)
        {
            switch (b_GameProperty)
            {
                case E_GameProperty.GoldCoin:
                    {
                        var mSourceValue = b_GamePlayer.Data.GoldCoin;
                        if (b_GamePlayer.Data.GoldCoin + b_ChangeValue >= long.MaxValue)
                        {
                            b_GamePlayer.Data.GoldCoin = long.MaxValue;
                        }
                        else
                        {
                            b_GamePlayer.Data.GoldCoin += b_ChangeValue;
                        }
                        Log.PLog($"账号id {b_GamePlayer.Player.UserId}  角色id:{b_GamePlayer.Player.GameUserId} {b_Str} - {(b_ChangeValue > 0 ? "金币收入" : "金币支出")}: {mSourceValue} - {b_ChangeValue} - {b_GamePlayer.Data.GoldCoin}");
                    }
                    break;
                case E_GameProperty.MiracleCoin:
                    {
                        var mSourceValue = b_GamePlayer.Data.MiracleCoin;
                        b_GamePlayer.Data.MiracleCoin += b_ChangeValue;
                        Log.PLog($"账号id {b_GamePlayer.Player.UserId}  角色id:{b_GamePlayer.Player.GameUserId} {b_Str} - {(b_ChangeValue > 0 ? "奇迹币收入" : "奇迹币支出")}: {mSourceValue} - {b_ChangeValue} - {b_GamePlayer.Data.MiracleCoin}");
                    }
                    break;
                case E_GameProperty.YuanbaoCoin:
                    {
                        var mSourceValue = b_GamePlayer.Player.Data.YuanbaoCoin;
                        b_GamePlayer.Player.Data.yuanbaoCoin += b_ChangeValue;

                        DbChangeCoinLog dbChangeCoinLog = new DbChangeCoinLog()
                        {
                            UserId = b_GamePlayer.Player.UserId,
                            GameUserId = b_GamePlayer.Player.GameUserId,
                            GameAreaId = b_GamePlayer.Player.SourceGameAreaId,
                            GameServerId = OptionComponent.Options.AppId,
                            Str = b_Str,
                            CoinType = (int)b_GameProperty,
                            SourceValue = mSourceValue,
                            ChangeValue = b_ChangeValue,
                            CoinValue = b_GamePlayer.Player.Data.YuanbaoCoin,
                            CreateTime = TimeHelper.Now()
                        };

                        DBLogHelper.Write(dbChangeCoinLog, b_GamePlayer.Player.GameAreaId);

                        Log.PLog($"LogId:{dbChangeCoinLog.Id}  账号id {b_GamePlayer.Player.UserId}  角色id:{b_GamePlayer.Player.GameUserId} {b_Str} - {(b_ChangeValue > 0 ? "元宝收入" : "元宝支出")}: {mSourceValue} - {b_ChangeValue} - {b_GamePlayer.Player.Data.YuanbaoCoin}");
                    }
                    break;
                default:
                    break;
            }

        }
    }
}