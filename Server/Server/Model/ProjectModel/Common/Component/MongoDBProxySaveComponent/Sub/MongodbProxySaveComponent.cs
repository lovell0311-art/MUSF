
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;


using MongoDB.Driver;
using CustomFrameWork;
using CustomFrameWork.Baseic;
namespace ETModel
{
    public partial class C_MongodbProxySave : ADataContext<int>
    {
        public class C_DBSaveGroup : ADataContext
        {
            public Dictionary<long, DBBase> UpdateDataDic { get; private set; }

            public override void ContextAwake()
            {
                UpdateDataDic = new Dictionary<long, DBBase>();
            }
            public override void Dispose()
            {
                if (IsDisposeable) return;

                if (UpdateDataDic != null)
                {
                    UpdateDataDic.Clear();
                    UpdateDataDic = null;
                }

                base.Dispose();
            }
        }
        /// <summary>
        /// 用于读取
        /// </summary>
        public Dictionary<string, Dictionary<long, DBBase>> _ReverseDataDic { get; private set; }
        /// <summary>
        /// 按时间保存
        /// </summary>
        public Dictionary<long, C_DBSaveGroup> _UpdateDataDic { get; private set; }

        public long _IntervalTimeTick = 300000000;
        
        public int GameAreaId { get; private set; } = -1;
        public override void ContextAwake(int b_GameAreaId)
        {
            GameAreaId = b_GameAreaId;
            _UpdateDataDic = new Dictionary<long, C_DBSaveGroup>();
            _ReverseDataDic = new Dictionary<string, Dictionary<long, DBBase>>();
        }
        public override void Dispose()
        {
            if (IsDisposeable) return;

            GameAreaId = -1;
            if (_ReverseDataDic != null)
            {
                if (_ReverseDataDic.Count > 0)
                {
                    _ReverseDataDic.Clear();
                }
                _ReverseDataDic = null;
            }
            if (_UpdateDataDic != null)
            {
                var mTempList = _UpdateDataDic.Values.ToList();
                for (int i = 0, len = mTempList.Count; i < len; i++)
                {
                    mTempList[i].Dispose();
                }
                _UpdateDataDic.Clear();
                _UpdateDataDic = null;
            }

            base.Dispose();
        }
    }
}
