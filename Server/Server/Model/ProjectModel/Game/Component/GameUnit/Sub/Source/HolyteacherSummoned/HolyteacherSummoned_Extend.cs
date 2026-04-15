using System;
using System.Collections.Generic;
namespace ETModel
{
    public sealed partial class HolyteacherSummoned
    {
        public override E_PKModel PKModel()
        {
            E_PKModel mCurrnetPKModel;
            if (GamePlayer != null && GamePlayer.IsDisposeable == false)
            {
                mCurrnetPKModel = GamePlayer.PKModel();
            }
            else
            {
                mCurrnetPKModel = E_PKModel.Peace;
            }
            return mCurrnetPKModel;
        }
        public override long GetPlayerInstance()
        {
            if (GamePlayer != null && GamePlayer.IsDisposeable == false)
            {
                return GamePlayer.Player.GameUserId;
            }
            else
            {
                return 0;
            }
        }
        public override Dictionary<long, long> GetFanJiIdlist()
        {
            if (GamePlayer != null && GamePlayer.IsDisposeable == false)
            {
                return GamePlayer.FanJiIdlist;
            }
            else
            {
                return null;
            }
        }
    }
}