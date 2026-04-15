using CustomFrameWork.Baseic;

namespace ETModel
{
    /// <summary>
    /// 进入古战场后，定时检查
    /// <para>玩家进入古战场后，添加当前组件，离开后移除</para>
    /// </summary>
    public class AncientBattlefieldCheckComponent : TCustomComponent<Player>
    {
        public long TimerId;
    }
}
