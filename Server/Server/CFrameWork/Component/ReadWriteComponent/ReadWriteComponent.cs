
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Diagnostics;
using CustomFrameWork.Baseic;
using static CustomFrameWork.Component.AsyncLockComponent;

namespace CustomFrameWork.Component
{
    /// <summary>
    /// 每个文本加一个枚举对象
    /// </summary>
    public enum E_ReadWriteLock
    {// 每个文本加一个枚举对象 一个对象一个共同的锁
        NONE = 0,
        LOG = 1,
        LOGWARNING,
        LOGERROR,
        CONFIG

    }
    /// <summary>
    /// 读写工具 全局唯一
    /// </summary>
    public class ReadWriteComponent : TCustomComponent<MainFactory>
    {
        private AsyncLockComponent mAsyncLockComponent;
        /// <summary>
        /// 初始化
        /// </summary>
        public override void Awake()
        {
            mAsyncLockComponent = AddCustomComponent<AsyncLockComponent, int>(0);
        }

        private string ChangePath(string b_Path)
        {
            return b_Path.Replace("\\", "/");
        }


        /// <summary>
        /// 添加一个写入任务 对于相同任务锁ID
        /// </summary>
        /// <param name="b_RWLock">任务锁ID</param>
        /// <param name="b_Path">写入路径</param>
        /// <param name="b_DataInfo">写入信息</param>
        public async Task AddWriteAsync(E_ReadWriteLock b_RWLock, string b_Path, string b_DataInfo)
        {
            if (IsDisposeable) return;
            C_AsyncLock mLock = await mAsyncLockComponent.AddTaskLock((int)b_RWLock);
            long mWorkingPositionCopy = mLock.WorkingPosition;
            if (IsDisposeable) return;
            if (mLock.IsWorking == false) return;
            //await Task.Run(async () => await File.WriteAllTextAsync(b_Path, b_DataInfo));
            string mPath = ChangePath(b_Path);
            if (File.Exists(mPath) == false)
            {
                var obj = File.Create(mPath);
                obj.Close();
                obj.Dispose();
            }
            await File.WriteAllTextAsync(mPath, b_DataInfo);
            // File.WriteAllText(b_Path, b_DataInfo);
            if (IsDisposeable) return;
            mLock.Dequeue(mWorkingPositionCopy);
        }
        public async Task AddWriteAppendAsync(E_ReadWriteLock b_RWLock, string b_Path, string b_DataInfo)
        {
            if (IsDisposeable) return;
            C_AsyncLock mLock = await mAsyncLockComponent.AddTaskLock((int)b_RWLock);
            long mWorkingPositionCopy = mLock.WorkingPosition;
            if (IsDisposeable) return;
            if (mLock.IsWorking == false) return;
            string mPath = ChangePath(b_Path);
            if (File.Exists(mPath) == false)
            {
                var obj = File.Create(mPath);
                obj.Close();
                obj.Dispose();
            }
            //File.AppendAllText(b_Path, b_DataInfo);
            await File.AppendAllTextAsync(mPath, b_DataInfo);
            if (IsDisposeable) return;
            mLock.Dequeue(mWorkingPositionCopy);
        }
        public async Task AddLogWriteAppendAsync(E_ReadWriteLock b_RWLock, string b_Path, string b_DataInfo)
        {
            if (IsDisposeable) return;
            C_AsyncLock mLock = await mAsyncLockComponent.AddTaskLock((int)b_RWLock);
            long mWorkingPositionCopy = mLock.WorkingPosition;
            if (IsDisposeable) return;
            if (mLock.IsWorking == false) return;
            string mPath = ChangePath(b_Path);
            if (File.Exists(mPath) == false)
            {
                var obj = File.Create(mPath);
                obj.Close();
                obj.Dispose();
            }
            File.AppendAllText(mPath, b_DataInfo);
            // await File.AppendAllTextAsync(b_Path, b_DataInfo);
            if (IsDisposeable) return;
            mLock.Dequeue(mWorkingPositionCopy);
        }
        /// <summary>
        /// 添加一个写入任务 对于相同任务锁ID
        /// </summary>
        /// <param name="b_RWLock">任务锁ID</param>
        /// <param name="b_Path">写入路径</param>
        /// <param name="b_DataInfoBuffer">写入信息</param>
        public async Task AddWriteAsync(E_ReadWriteLock b_RWLock, string b_Path, byte[] b_DataInfoBuffer)
        {
            if (IsDisposeable) return;
            C_AsyncLock mLock = await mAsyncLockComponent.AddTaskLock((int)b_RWLock);
            long mWorkingPositionCopy = mLock.WorkingPosition;
            if (IsDisposeable) return;
            if (mLock.IsWorking == false) return;
            string mPath = ChangePath(b_Path);
            if (File.Exists(mPath) == false)
            {
                var obj = File.Create(mPath);
                obj.Close();
                obj.Dispose();
            }
            await File.WriteAllBytesAsync(mPath, b_DataInfoBuffer);
            // File.WriteAllBytes(b_Path, b_DataInfoBuffer);

            if (IsDisposeable) return;
            mLock.Dequeue(mWorkingPositionCopy);
        }
        /// <summary>
        /// 同步读写
        /// </summary>
        /// <param name="b_Path"></param>
        /// <returns></returns>
        public string AddRead(string b_Path)
        {
            if (IsDisposeable) return null;
            string mPath = ChangePath(b_Path);
            if (File.Exists(mPath) == false)
            {
                LogToolComponent.Error($"传入的文件路径无效！！{mPath}", false);
                return default;
            }
            string mReader = File.ReadAllText(mPath);
            if (IsDisposeable) return null;
            return mReader;
        }
        /// <summary>
        /// 添加一个读取任务 对于相同任务锁ID
        /// </summary>
        /// <param name="b_RWLock">任务锁ID</param>
        /// <param name="b_Path">读取路径</param>
        /// <returns>读取信息</returns>
        public async Task<string> AddReadAsync(E_ReadWriteLock b_RWLock, string b_Path)
        {
            if (IsDisposeable) return null;
            C_AsyncLock mLock = await mAsyncLockComponent.AddTaskLock((int)b_RWLock);
            long mWorkingPositionCopy = mLock.WorkingPosition;
            if (IsDisposeable) return null;
            if (mLock.IsWorking == false) return null;
            string mPath = ChangePath(b_Path);
            if (File.Exists(mPath) == false)
            {
                LogToolComponent.Error($"传入的文件路径无效！！{mPath}", false);
                return default;
            }
            //string mReader = await Task.Run(async () => await File.ReadAllTextAsync(b_Path));
            string mReader = await File.ReadAllTextAsync(mPath);
            if (IsDisposeable) return null;
            mLock.Dequeue(mWorkingPositionCopy);
            return mReader;
        }
        public async Task<byte[]> AddReadByteAsync(E_ReadWriteLock b_RWLock, string b_Path)
        {
            if (IsDisposeable) return null;
            C_AsyncLock mLock = await mAsyncLockComponent.AddTaskLock((int)b_RWLock);
            long mWorkingPositionCopy = mLock.WorkingPosition;
            if (IsDisposeable) return null;
            if (mLock.IsWorking == false) return null;
            string mPath = ChangePath(b_Path);
            if (File.Exists(mPath) == false)
            {
                LogToolComponent.Error($"传入的文件路径无效！！{mPath}", false);
                return default;
            }
            byte[] mReader = await File.ReadAllBytesAsync(mPath);
            if (IsDisposeable) return null;
            mLock.Dequeue(mWorkingPositionCopy);
            return mReader;
        }
        /// <summary>
        /// 添加一个读取任务 无锁
        /// </summary>
        /// <param name="b_Path">读取路径</param>
        /// <returns>读取信息</returns>
        public async Task<string> AddReadAsync(string b_Path)
        {
            if (IsDisposeable) return null;
            string mPath = ChangePath(b_Path);
            if (File.Exists(mPath) == false)
            {
                LogToolComponent.Error($"传入的文件路径无效！！{mPath}", false);
                return default;
            }
            string mReader = await File.ReadAllTextAsync(mPath);
            if (IsDisposeable) return null;
            return mReader;
        }
        /// <summary>
        /// 清理
        /// </summary>
        public override void Dispose()
        {
            if (IsDisposeable) return;

            mAsyncLockComponent = null;
            base.Dispose();
        }
    }
}
