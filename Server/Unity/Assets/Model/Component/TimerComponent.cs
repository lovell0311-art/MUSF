using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ETModel
{
    [ObjectSystem]
    public class TimerComponentUpdateSystem : UpdateSystem<ETTimerComponent>
    {
        public override void Update(ETTimerComponent self)
        {
            self.Update();
        }
    }

    public class ETTimerComponent : Component
    {
        public override void Dispose()
        {
            if (IsDisposed) return;

            mKeyValuePairs.Clear();
            base.Dispose();
        }


        public async Task<bool> WaitAsync(long b_TimeValue, CancellationToken cancellationToken, long b_TagID = 0)
        {
            return await WaitAsync(b_TimeValue, b_TagID);
        }
        public async Task<bool> WaitAsync(long b_TimeValue, long b_TagID = 0)
        {
            if (b_TimeValue <= 0) return false;
            long timeKey = TimeHelper.ClientNow() + b_TimeValue;
            List<TaskCompletionSource<bool>> taskList = default;
            //lock (objLock)
            {
                if (mKeyValuePairs.TryGetValue(b_TagID, out Dictionary<long, List<TaskCompletionSource<bool>>> timeDic) == false)
                    mKeyValuePairs[b_TagID] = timeDic = new Dictionary<long, List<TaskCompletionSource<bool>>>();
                if (timeDic.TryGetValue(timeKey, out taskList) == false)
                    timeDic[timeKey] = taskList = new List<TaskCompletionSource<bool>>();
            }
            if (mMinTimeValue > timeKey)
            {
                mMinTimeValue = timeKey;
            }
            TaskCompletionSource<bool> source = new TaskCompletionSource<bool>();
            taskList.Add(source);
            return await source.Task;
        }
        private readonly Dictionary<long, Dictionary<long, List<TaskCompletionSource<bool>>>> mKeyValuePairs = new Dictionary<long, Dictionary<long, List<TaskCompletionSource<bool>>>>();
        private long mMinTimeValue = long.MaxValue;



        private long GetTimerMinKey()
        {
            long result = long.MaxValue;
            List<Dictionary<long, List<TaskCompletionSource<bool>>>> IDValues = mKeyValuePairs.Values.ToList();
            for (int x = 0, len = IDValues.Count; x < len; x++)
            {
                List<long> TimeKeys = IDValues[x].Keys.ToList();
                for (int y = 0, ylen = TimeKeys.Count; y < ylen; y++)
                {
                    if (result > TimeKeys[y])
                    {
                        result = TimeKeys[y];
                    }
                }
            }
            return result;
        }
        private void RemoveTimer(long b_TimeValue)
        {
            //lock (objLock)
            {
                List<long> IDKeys = mKeyValuePairs.Keys.ToList();
                for (int x = 0, len = IDKeys.Count; x < len; x++)
                {
                    long IDKey = IDKeys[x];
                    List<long> TimeKeys = mKeyValuePairs[IDKey].Keys.ToList();
                    Dictionary<long, List<TaskCompletionSource<bool>>> TimeDics = mKeyValuePairs[IDKey];

                    for (int y = 0, ylen = TimeKeys.Count; y < ylen; y++)
                    {
                        long timeKey = TimeKeys[y];
                        if (b_TimeValue > timeKey)
                        {
                            if (TimeDics.TryGetValue(timeKey, out List<TaskCompletionSource<bool>> temp))
                            {
                                for (int z = 0, zlen = temp.Count; z < zlen; z++)
                                {
                                    temp[z].SetResult(true);
                                }
                                TimeDics.Remove(timeKey);
                            }
                        }
                    }
                    if (TimeDics.Count == 0)
                    {
                        mKeyValuePairs.Remove(IDKey);
                    }
                }
            }
        }

        public void Update()
        {
            if (mKeyValuePairs.Count == 0) return;

            long timeNow = TimeHelper.ClientNow();

            if (timeNow < mMinTimeValue) return;

            RemoveTimer(timeNow);
            mMinTimeValue = GetTimerMinKey();
        }
    }
}