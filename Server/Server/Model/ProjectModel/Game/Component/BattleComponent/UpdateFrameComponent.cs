using CustomFrameWork.Baseic;

namespace ETModel
{
    [PrivateObject]
    public class UpdateFrameComponent : TCustomComponent<CombatSource>
    {
        public BattleComponent battleComponent;
        public long timerId;
    }
}
