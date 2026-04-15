using CustomFrameWork.Baseic;
namespace CustomFrameWork.Baseic
{
    /// <summary>
    /// 有参组件
    /// </summary>
    /// <typeparam name="K">父组件类型</typeparam>
    /// <typeparam name="T">参数类型</typeparam>
    public abstract class TCustomComponent<K, T> : CustomComponent, IAwakeComponent<T> where K : ACustomComponent
    {
        /// <summary>
        /// 父级对象
        /// </summary>
        public virtual K Parent { get; private set; }
        /// <summary>
        /// 初始化 Awake Init之后执行
        /// </summary>
        /// <param name="b_Parent">父组件</param>
        /// <param name="b_Args">参数</param>
        public void Awake(ACustomComponent b_Parent, T b_Args)
        {
            this.Parent = b_Parent as K;
            base.Awake(b_Parent);
            Awake(b_Args);
        }
        /// <summary>
        /// Awake
        /// </summary>
        /// <param name="b_Args"></param>
        public abstract void Awake(T b_Args);

        /// <summary>
        /// 清理
        /// </summary>
        public override void Dispose()
        {
            if (IsDisposeable) return;

            Parent = null;
            base.Dispose();
        }
    }
    /// <summary>
    /// 有参组件 提供两个参数
    /// </summary>
    /// <typeparam name="K">父组件类型</typeparam>
    /// <typeparam name="T1">参数1类型</typeparam>
    /// <typeparam name="T2">参数2类型</typeparam>
    public abstract class TCustomComponent<K, T1, T2> : CustomComponent, IAwakeComponent<T1, T2> where K : ACustomComponent
    {
        /// <summary>
        /// 父级对象
        /// </summary>
        public virtual K Parent { get; private set; }
        /// <summary>
        /// 格式初始化
        /// </summary>
        /// <param name="b_Parent"></param>
        /// <param name="b_Args1"></param>
        /// <param name="b_Args2"></param>
        public void Awake(ACustomComponent b_Parent, T1 b_Args1, T2 b_Args2)
        {
            this.Parent = b_Parent as K;
            base.Awake(b_Parent);
            Awake(b_Args1, b_Args2);
        }
        /// <summary>
        /// 自定义初始化
        /// </summary>
        /// <param name="b_Args1"></param>
        /// <param name="b_Args2"></param>
        public abstract void Awake(T1 b_Args1, T2 b_Args2);
        /// <summary>
        /// 清理Dispose
        /// </summary>
        public override void Dispose()
        {
            if (IsDisposeable) return;

            Parent = null;
            base.Dispose();
        }
    }
    /// <summary>
    /// 有参组件 提供三个参数
    /// </summary>
    /// <typeparam name="K">父组件类型</typeparam>
    /// <typeparam name="T1">参数1类型</typeparam>
    /// <typeparam name="T2">参数2类型</typeparam>
    /// <typeparam name="T3">参数3类型</typeparam>
    public abstract class TCustomComponent<K, T1, T2, T3> : CustomComponent, IAwakeComponent<T1, T2, T3> where K : ACustomComponent
    {
        /// <summary>
        /// 父级对象
        /// </summary>
        public virtual K Parent { get; private set; }
        /// <summary>
        /// 格式初始化
        /// </summary>
        /// <param name="b_Parent"></param>
        /// <param name="b_Args1"></param>
        /// <param name="b_Args2"></param>
        /// <param name="b_Args3"></param>
        public void Awake(ACustomComponent b_Parent, T1 b_Args1, T2 b_Args2, T3 b_Args3)
        {
            this.Parent = b_Parent as K;
            base.Awake(b_Parent);
            Awake(b_Args1, b_Args2, b_Args3);
        }
        /// <summary>
        /// 自定义初始化
        /// </summary>
        /// <param name="b_Args1"></param>
        /// <param name="b_Args2"></param>
        /// <param name="b_Args3"></param>
        public abstract void Awake(T1 b_Args1, T2 b_Args2, T3 b_Args3);
        /// <summary>
        /// 清理Dispose
        /// </summary>
        public override void Dispose()
        {
            if (IsDisposeable) return;

            Parent = null;
            base.Dispose();
        }
    }
    /// <summary>
    /// 有参组件 提供四个参数
    /// </summary>
    /// <typeparam name="K">父组件类型</typeparam>
    /// <typeparam name="T1">参数1类型</typeparam>
    /// <typeparam name="T2">参数2类型</typeparam>
    /// <typeparam name="T3">参数3类型</typeparam>
    /// <typeparam name="T4">参数4类型</typeparam>
    public abstract class TCustomComponent<K, T1, T2, T3, T4> : CustomComponent, IAwakeComponent<T1, T2, T3, T4> where K : ACustomComponent
    {
        /// <summary>
        /// 父级对象
        /// </summary>
        public virtual K Parent { get; private set; }
        /// <summary>
        /// 格式初始化
        /// </summary>
        /// <param name="b_Parent"></param>
        /// <param name="b_Args1"></param>
        /// <param name="b_Args2"></param>
        /// <param name="b_Args3"></param>
        /// <param name="b_Args4"></param>
        public void Awake(ACustomComponent b_Parent, T1 b_Args1, T2 b_Args2, T3 b_Args3, T4 b_Args4)
        {
            this.Parent = b_Parent as K;
            base.Awake(b_Parent);
            Awake(b_Args1, b_Args2, b_Args3, b_Args4);
        }
        /// <summary>
        /// 自定义初始化
        /// </summary>
        /// <param name="b_Args1"></param>
        /// <param name="b_Args2"></param>
        /// <param name="b_Args3"></param>
        /// <param name="b_Args4"></param>
        public abstract void Awake(T1 b_Args1, T2 b_Args2, T3 b_Args3, T4 b_Args4);
        /// <summary>
        /// 清理Dispose
        /// </summary>
        public override void Dispose()
        {
            if (IsDisposeable) return;

            Parent = null;
            base.Dispose();
        }
    }
    /// <summary>
    /// 有参组件 提供五个参数
    /// </summary>
    /// <typeparam name="K">父组件类型</typeparam>
    /// <typeparam name="T1">参数1类型</typeparam>
    /// <typeparam name="T2">参数2类型</typeparam>
    /// <typeparam name="T3">参数3类型</typeparam>
    /// <typeparam name="T4">参数4类型</typeparam>
    /// <typeparam name="T5">参数5类型</typeparam>
    public abstract class TCustomComponent<K, T1, T2, T3, T4, T5> : CustomComponent, IAwakeComponent<T1, T2, T3, T4, T5> where K : ACustomComponent
    {
        /// <summary>
        /// 父级对象
        /// </summary>
        public virtual K Parent { get; private set; }
        /// <summary>
        /// 格式初始化
        /// </summary>
        /// <param name="b_Parent"></param>
        /// <param name="b_Args1"></param>
        /// <param name="b_Args2"></param>
        /// <param name="b_Args3"></param>
        /// <param name="b_Args4"></param>
        /// <param name="b_Args5"></param>
        public void Awake(ACustomComponent b_Parent, T1 b_Args1, T2 b_Args2, T3 b_Args3, T4 b_Args4, T5 b_Args5)
        {
            this.Parent = b_Parent as K;
            base.Awake(b_Parent);
            Awake(b_Args1, b_Args2, b_Args3, b_Args4, b_Args5);
        }
        /// <summary>
        /// 自定义初始化
        /// </summary>
        /// <param name="b_Args1"></param>
        /// <param name="b_Args2"></param>
        /// <param name="b_Args3"></param>
        /// <param name="b_Args4"></param>
        /// <param name="b_Args5"></param>
        public abstract void Awake(T1 b_Args1, T2 b_Args2, T3 b_Args3, T4 b_Args4, T5 b_Args5);
        /// <summary>
        /// 清理Dispose
        /// </summary>
        public override void Dispose()
        {
            if (IsDisposeable) return;

            Parent = null;
            base.Dispose();
        }
    }
}