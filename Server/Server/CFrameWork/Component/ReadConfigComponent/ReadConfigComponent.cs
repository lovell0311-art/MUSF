using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;

using CustomFrameWork.Baseic;

namespace CustomFrameWork.Component
{
    /// <summary>
    /// json数据读取并缓存
    /// </summary>
    public partial class ReadConfigComponent: TCustomComponent<MainFactory>
    {
        private readonly Dictionary<string, C_ConfigJson> mJsonDateDic = new Dictionary<string, C_ConfigJson>();

        public async override void Awake()
        {
            if (mJsonDateDic.Count > 0)
            {
                mJsonDateDic.Clear();
            }
        }

        public override void Clear()
        {
            mJsonDateDic.Clear();
        }

        public override void Dispose()
        {
            if (IsDisposeable) return;
            if (mJsonDateDic.Count > 0)
            {
                mJsonDateDic.Clear();
            }
            base.Dispose();
        }

        public void AddJson(C_ConfigJson b_Value)
        {
            string mKey = b_Value.ConfigType.Name;
            if (mJsonDateDic.ContainsKey(mKey))
            {
                LogToolComponent.Warning($"Config中有重复,key: {mKey}");
            }
            mJsonDateDic[mKey] = b_Value;
        }
        public void AddJson(string b_Key, C_ConfigJson b_Value)
        {
            if (mJsonDateDic.ContainsKey(b_Key))
            {
                LogToolComponent.Error($"Config中有重复,key: {b_Key}");
            }
            mJsonDateDic[b_Key] = b_Value;
        }
        public T GetJson<T>() where T : C_ConfigJson
        {
            string mKey = typeof(T).Name;
            if (mJsonDateDic.TryGetValue(mKey, out C_ConfigJson mResult))
            {
                return mResult as T;
            }
            return null;
        }
        public T GetJson<T>(string b_Key) where T : C_ConfigJson
        {
            if (mJsonDateDic.TryGetValue(b_Key, out C_ConfigJson mResult))
            {
                return mResult as T;
            }
            return null;
        }
    }
    public abstract class C_ConfigInfo
    {
        public virtual void InitExpand() { }
    }
    public abstract class C_ConfigJson
    {
        public abstract void ReadData(string b_ReadStr);
        public abstract Type ConfigType { get; }
    }
}
