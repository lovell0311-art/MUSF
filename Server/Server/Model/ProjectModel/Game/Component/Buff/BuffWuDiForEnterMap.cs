using CustomFrameWork;
using CustomFrameWork.Baseic;

namespace ETModel
{
    /// <summary>
    /// 进入场景时会添加，当收到客户端发来LoadSceneComplete消息时会移除
    /// </summary>
    [PrivateObject]
    public class BuffWuDiForEnterMap : TCustomComponent<CombatSource>
    {
        public long timerId;

        public CombatSource combatSource;

        public BattleComponent battleComponent;
    }
}
