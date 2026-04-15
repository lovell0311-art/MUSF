using CustomFrameWork.Baseic;

namespace ETModel
{
    /// <summary>
    /// 在天空之城，定时检查翅膀有没有脱掉
    /// </summary>
    [PrivateObject]
    public class CheckWingComponent : TCustomComponent<GamePlayer>
    {
        public long timerId;
        public EquipmentComponent equipmentCom;
    }
}
