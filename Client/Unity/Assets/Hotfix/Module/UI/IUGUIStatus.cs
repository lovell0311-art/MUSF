namespace ETHotfix
{
    /// <summary>
    /// UI面板发生变化的事件
    /// </summary>
    public interface IUGUIStatus 
    {
        /// <summary>
        ///  显示时调用
        /// </summary>
        /// <param name="data">参数（没有时 可不填）</param>
        void OnVisible(object[] data);

        void OnVisible();
        /// <summary>
        /// 隐藏时调用
        /// </summary>
        void OnInVisibility();
    }
}