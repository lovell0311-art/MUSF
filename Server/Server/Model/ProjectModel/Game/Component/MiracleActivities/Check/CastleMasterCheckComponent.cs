using CustomFrameWork.Baseic;

namespace ETModel
{
    /// <summary>
    /// 冰封城主过期检查
    /// </summary>
    public class CastleMasterCheckComponent : TCustomComponent<Player>
    {
        public long TimerId;
    }
    /// <summary>
    /// 掉落限制更新检查
    /// </summary>
    public class CheckDropItemTimeComponent : TCustomComponent<Player>
    {
        public long TimerId;
    }
    /// <summary>
    /// 试炼塔定时检测
    /// </summary>
    public class TrialTowerCheckComponent : TCustomComponent<GamePlayer>
    {
        public long TimerId;
    }
}
