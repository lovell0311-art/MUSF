
using System;
using System.Collections.Generic;
using System.Linq;

using CustomFrameWork;
using CustomFrameWork.Baseic;

namespace ETModel
{
    public partial class DBMongodbProxySaveManageComponent : TCustomComponent<MainFactory>
    {
        public Dictionary<int, C_MongodbProxySave> _Instance = new Dictionary<int, C_MongodbProxySave>();
        public override void Awake()
        {
            _RunIndex = 0;
            RemoveAll();
        }
        public int _RunIndex;
        public override void Dispose()
        {
            if (IsDisposeable) return;

            RemoveAll();
            base.Dispose();
        }
        private void RemoveAll()
        {
            if (_Instance.Count > 0)
            {
                var mGameTemplist = _Instance.Values.ToArray();
                for (int i = 0, len = mGameTemplist.Length; i < len; i++)
                {
                    mGameTemplist[i].Dispose();
                }
                _Instance.Clear();
            }
        }
    }
}
