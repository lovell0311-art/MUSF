
using System;
using System.Collections.Generic;
using CustomFrameWork.Baseic;
using System.Text;
using System.Threading.Tasks;
using static CustomFrameWork.Component.AsyncLockComponent;

namespace CustomFrameWork.Component
{
    /// <summary>
    /// 流水线组件 流水线作业 不相同的同步作业 相同的等待前一个作业完成后
    /// </summary>
    public class PipeLineComponent : TCustomComponent<CustomComponent>
    {
        /// <summary>
        /// 动态获取父节点
        /// </summary>
        /// <typeparam name="K">父节点类型</typeparam>
        /// <returns>父节点</returns>
        public K GetParent<K>() where K : ACustomComponent
        {
            return Parent as K;
        }


        private int mPipeLineID = 1;
        private C_AsyncLock mLock;
        private long mWorkingPositionCopy;
        /// <summary>
        /// 初始化 Awake
        /// </summary>
        public override void Awake()
        {
            mPipeLineID = 1;
            ConfigInfoComponent c_component = Root.MainFactory.GetCustomComponent<ConfigInfoComponent>();
            string mTaskOverTimeStr = c_component.GetConfigInfo("TaskOverTime");
            int.TryParse(mTaskOverTimeStr, out int mTaskOverTime);
            AddCustomComponent<AsyncLockComponent, int>(mTaskOverTime);
        }
        /// <summary>
        /// 等待任务触发
        /// </summary>
        /// <returns></returns>
        public async Task<bool> WaitAsync()
        {
            if (IsDisposeable) return false;
            mLock = await GetCustomComponent<AsyncLockComponent>().AddTaskLock(mPipeLineID);
            mWorkingPositionCopy = mLock.WorkingPosition;
            if (IsDisposeable) return false;
            if (mLock.IsWorking == false) return false;
            return true;
        }
        /// <summary>
        /// 去出下一个任务
        /// </summary>
        public void Dequeue()
        {
            if (IsDisposeable) return;
            mLock?.Dequeue(mWorkingPositionCopy);
        }
        /// <summary>
        /// 清理 Dispose
        /// </summary>
        public override void Dispose()
        {
            if (IsDisposeable) return;

            mLock = null;
            mWorkingPositionCopy = 0;
            mPipeLineID = 0;
            base.Dispose();
        }
    }
}
