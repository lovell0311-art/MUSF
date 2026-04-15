using System;
using System.Collections.Generic;
namespace ETModel
{
    public partial class CombatSource
    {
        public virtual E_PKModel PKModel() { return E_PKModel.Peace; }
        public virtual long GetPlayerInstance() { return 0; }
        public virtual Dictionary<long, long> GetFanJiIdlist() { return null; }


    }
}