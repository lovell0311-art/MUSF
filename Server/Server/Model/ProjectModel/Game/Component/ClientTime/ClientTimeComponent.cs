using System;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork;
using CustomFrameWork.Baseic;

namespace ETModel
{
    public class ClientTimeComponent : TCustomComponent<Player>
    {
        private long _ServerStartTime = 0;
        private long _ClientStartTime = 0;
        private long _LastServerTime = 0;
        private long _LastClientTime = 0;

        /// <summary>
        /// 客户端时间
        /// </summary>
        public long ClientTime { 
            get { return _LastClientTime + (Help_TimeHelper.GetNow() - _LastServerTime); }
            set
            {
                if (_ServerStartTime == 0)
                {
                    _ServerStartTime = Help_TimeHelper.GetNow();
                    _ClientStartTime = value;
                }
                _LastServerTime = Help_TimeHelper.GetNow();
                _LastClientTime = value;
            }
        }

        public long ServerDiffTime { get { return _LastServerTime - _ServerStartTime; } }
        public long ClientDiffTime { get { return _LastClientTime - _ClientStartTime; } }

        public override void Dispose()
        {
            if (IsDisposeable) return;
            base.Dispose();

            _ServerStartTime = 0;
            _ClientStartTime = 0;
            _LastServerTime = 0;
            _LastClientTime = 0;
        }
    }
}
