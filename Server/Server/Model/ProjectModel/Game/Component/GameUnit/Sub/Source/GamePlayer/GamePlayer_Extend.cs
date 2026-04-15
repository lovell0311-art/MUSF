using System;
using System.Collections.Generic;
namespace ETModel
{
    public sealed partial class GamePlayer
    {
        public override E_PKModel PKModel()
        {
            E_PKModel mCurrnetPKModel;
            if (Data != null && Data.Level <= 50)
            {
                mCurrnetPKModel = E_PKModel.Peace;
            }
            else
            {
                mCurrnetPKModel = _PKModel;
            }
            return mCurrnetPKModel;
        }
        public override long GetPlayerInstance()
        {
            if (Player != null && Player.IsDisposeable == false)
            {
                return Player.GameUserId;
            }
            else
            {
                return 0;
            }
        }
        public override Dictionary<long, long> GetFanJiIdlist()
        {
            if (IsDisposeable == false)
            {
                return FanJiIdlist;
            }
            else
            {
                return null;
            }
        }
    }
}