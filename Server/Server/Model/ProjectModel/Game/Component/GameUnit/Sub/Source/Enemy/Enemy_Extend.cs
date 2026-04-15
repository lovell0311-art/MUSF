using System;
using System.Collections.Generic;
namespace ETModel
{
    public sealed partial class Enemy
    {
        public override E_PKModel PKModel()
        {
            return E_PKModel.AllBoddy;
        }
        public override long GetPlayerInstance()
        {
            return InstanceId;
        }
    }
}