#if !SERVER
using Object = UnityEngine.Object;
#else
using CustomFrameWork.CustomInterface;
using System;
using Object = System.Object;
#endif
namespace CustomFrameWork.Baseic
{
    /// <summary>
    /// 组件 生命周期
    /// </summary>
    public abstract class CustomComponent : ACustomComponent
    {
        /// <summary>
        /// 是否运行update
        /// </summary>
        public bool IsRunUpdate { get; private set; }
        /// <summary>
        /// 初始化 立刻执行 不运行Update
        /// </summary>
        public virtual void Awake(ACustomComponent b_Parent)
        {
            OnlyStopUpdate();
            base.DataContextSourceAwake();
        }

        /// <summary>
        /// 初始化 延后到下帧执行 不运行Update 
        /// </summary>
        public virtual void Start() { }
        /// <summary>
        /// 开始 运行
        /// </summary>
        public virtual void OnEnable() { OnlyRunUpdate(); }
        /// <summary>
        /// Update 逻辑更新
        /// </summary>
        public virtual void Update() { }
        /// <summary>
        /// 游戏清理
        /// </summary>
        public virtual void Clear() { OnlyStopUpdate(); }

        /// <summary>
        /// 仅仅打开update
        /// </summary>
        public void OnlyRunUpdate() { IsRunUpdate = true; }
        /// <summary>
        /// 仅仅关闭update
        /// </summary>
        public void OnlyStopUpdate() { IsRunUpdate = false; }

        /// <summary>
        /// 清理
        /// </summary>
        public override void Dispose()
        {
            Clear();
            base.Dispose();
        }
    }
}
