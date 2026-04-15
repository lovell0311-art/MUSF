using System;
using System.Collections.Generic;
using System.Linq;
using ETModel;
using ETModel.Robot;
using CustomFrameWork;
using System.Threading.Tasks;

namespace ETHotfix.Robot
{
    /// <summary>
    /// 随机切换地图
    /// </summary>
    [AIHandler]
    public class AI_RangeMap : AAIHandler
    {

        public override int Check(AIComponent aiComponent, AI_Config aiConfig)
        {
            Scene clientScene = aiComponent.ClientScene();
            Unit localUnit = UnitHelper.GetLocalUnitFromClientScene(clientScene);

            MoveComponent move = localUnit.GetComponent<MoveComponent>();

            long mNextDeliveryTime = Help_TimeHelper.GetNow();
            if (move.NextDeliveryTime > mNextDeliveryTime) return 1;

            return 0;
        }
        public override async Task Execute(AIComponent aiComponent, AI_Config aiConfig, ETCancellationToken cancellationToken)
        {
            Scene clientScene = aiComponent.ClientScene();
            Unit localUnit = UnitHelper.GetLocalUnitFromClientScene(clientScene);
            if (localUnit == null) return;
            Log.Console($"[{clientScene.Name}] 随机地图");

            MoveComponent move = localUnit.GetComponent<MoveComponent>();

            long mNextDeliveryTime = Help_TimeHelper.GetNow();
            move.NextDeliveryTime = mNextDeliveryTime + RandomHelper.RandomNumber(60, 60 * 10) * 1000;

            var mReadConfigComponent = Root.MainFactory.GetCustomComponent<CustomFrameWork.Component.ReadConfigComponent>();
            var mConfigInfo = mReadConfigComponent.GetJson<Map_TransferPointConfigJson>();

            var mConfigValues = new List<int>()
            {
                100,200,300,304,305,400,405,406,407,500,503,508,600,604,605,700,708,707,709,703,704,710,800,804,900,902,1000,1100,1200,1203,1300,1400,1500,1600,10200
            };
            var mConfigKeylist = new List<Map_TransferPointConfig>();

            for (int i = 0; i < mConfigValues.Count; i++)
            {
                var mConfigId = mConfigValues[i];

                if (mConfigInfo.JsonDic.TryGetValue(mConfigId, out var mConfigValue))
                {
                    mConfigKeylist.Add(mConfigValue);
                }
            }
            var mRandomKey = RandomHelper.RandomNumber(0, mConfigKeylist.Count);
            var mRandomValue = mConfigKeylist[mRandomKey];

            var mLevel = localUnit.GetComponent<NumericComponent>().GetAsInt((int)E_GameProperty.Level);
            if (mLevel < mRandomValue.MapMinLevel)
            {
                return;
            }
            var mGoldCoin = localUnit.GetComponent<NumericComponent>().GetAsInt((int)E_GameProperty.GoldCoin);
            var mTargetGoldCoin = mRandomValue.MapCostGold + 100;
            if (mGoldCoin < mTargetGoldCoin)
            {
                return;
            }

            C2G_MapDeliveryRequest msg = new C2G_MapDeliveryRequest()
            {
                MapId = mRandomValue.Id
            };

            IResponse res = await clientScene.GetComponent<SessionComponent>().session.Call(msg, cancellationToken);
            if (cancellationToken.IsCancel()) return;

            Log.Console($"[{clientScene.Name}] 随机地图 结束");
        }
    }
}
