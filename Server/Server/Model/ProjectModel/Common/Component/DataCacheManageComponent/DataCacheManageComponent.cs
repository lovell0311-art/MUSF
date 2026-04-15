using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using CustomFrameWork;
using CustomFrameWork.Baseic;

namespace ETModel
{
    /// <summary>
    /// Êý¾Ý¿â»º´æ
    /// </summary>
    public class DataCacheManageComponent : TCustomComponent<CustomComponent>
    {
        public readonly Dictionary<string, C_DataCache> DataCacheDic = new Dictionary<string, C_DataCache>();

        public override void Dispose()
        {
            if (IsDisposeable) return;

            RemoveAll();
            base.Dispose();
        }
        public void RemoveAll()
        {
            if (DataCacheDic != null && DataCacheDic.Count > 0)
            {
                var list = DataCacheDic.Values.ToList();
                for (int i = 0, len = list.Count; i < len; i++)
                {
                    var listChild = list[i];
                    listChild.Dispose();
                }
                DataCacheDic.Clear();
            }
        }
        public bool Remove<K>() where K : DBBase
        {
            string typeName = typeof(K).Name;
            C_DataCache<K> mResult = null;
            if (DataCacheDic.TryGetValue(typeName, out C_DataCache mDBBase))
                mResult = mDBBase as C_DataCache<K>;
            if (mResult != null)
            {
                DataCacheDic.Remove(typeName);
                mResult.Dispose();
                return true;
            }
            return false;
        }
    }
}