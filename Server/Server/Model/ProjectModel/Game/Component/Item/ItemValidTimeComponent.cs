using CustomFrameWork;
using CustomFrameWork.Baseic;

namespace ETModel
{
    /// <summary>
    /// 物品有效时间组件
    /// <para>修改物品有效时间后，需要将这个组件移除后重新添加</para>
    /// </summary>
    public class ItemValidTimeComponent : TCustomComponent<Item>
    {
        public long TimerId;
    }
    /// <summary>
    /// 定时物品删除，如藏宝图
    /// </summary>
    public class ItemTimeLimitComponent : TCustomComponent<Item>
    {
        public long TimerId;
    }
}
