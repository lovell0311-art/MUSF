
namespace CustomFrameWork.Baseic
{
    public abstract class TCustomComponent<T> : CustomComponent where T : ACustomComponent
    {
        /// <summary>
        /// 父级对象
        /// </summary>
        public virtual T Parent { get; private set; }
        /// <summary>
        /// 设置父物体
        /// </summary>
        /// <param name="b_Parent">父级对象</param>
        public sealed override void Awake(ACustomComponent b_Parent)
        {
            this.Parent = b_Parent as T;
            base.Awake(b_Parent);
            Awake();
        }
        /// <summary>
        /// Awake()
        /// </summary>
        public virtual void Awake() { }
        /// <summary>
        /// Dispose()
        /// </summary>
        public override void Dispose()
        {
            if (IsDisposeable) return;

            Parent = null;
            base.Dispose();
        }
    }
}