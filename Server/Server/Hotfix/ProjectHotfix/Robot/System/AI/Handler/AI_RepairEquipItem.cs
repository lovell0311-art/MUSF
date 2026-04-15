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
    /// 维修装备
    /// </summary>
    [AIHandler]
    public class AI_RepairEquipItem : AAIHandler
    {
        public override int Check(AIComponent aiComponent, AI_Config aiConfig)
        {
            Scene clientScene = aiComponent.ClientScene();
            Unit localUnit = UnitHelper.GetLocalUnitFromClientScene(clientScene);
            if (DurLower80Pct(localUnit) == false) return 1;
            NumericComponent numeric = localUnit.GetComponent<NumericComponent>();
            if (RepairMoney(localUnit) > numeric.GetAsInt((int)E_GameProperty.GoldCoin)) return 1;
            return 0;
        }
        public override async Task Execute(AIComponent aiComponent, AI_Config aiConfig, ETCancellationToken cancellationToken)
        {
            Scene clientScene = aiComponent.ClientScene();
            Unit localUnit = UnitHelper.GetLocalUnitFromClientScene(clientScene);
            if (localUnit == null) return;
            Log.Console($"[{clientScene.Name}] 去维修装备");
            // 前往npc
            Unit targetUnit = await RobotNpcHelper.GoToNpc(localUnit, aiConfig.NodeParams, cancellationToken);
            if (cancellationToken.IsCancel()) return;
            if (targetUnit == null) return;

            RobotEquipmentComponent equipment = localUnit.GetComponent<RobotEquipmentComponent>();
            using ListComponent<EquipPosition> repairEquipList = ListComponent<EquipPosition>.Create();
            
            foreach (var kv in equipment.EquipPartItemDict)
            {
                NumericComponent numeric = kv.Value.GetComponent<NumericComponent>();
                if(numeric.GetAsInt((int)EItemValue.RepairMoney) > 0)
                {
                    repairEquipList.Add(kv.Key);
                }
            }

            bool ret = await RobotEquipmentHelper.RepairEquipItem(localUnit, targetUnit, repairEquipList, cancellationToken);
            if(cancellationToken.IsCancel()) return;
            if (ret == false) return;
        }

        /// <summary>
        /// 耐久低于80%
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static bool DurLower80Pct(Unit localUnit)
        {
            RobotEquipmentComponent equipment = localUnit.GetComponent<RobotEquipmentComponent>();
            return equipment.EquipPartItemDict.Values.Where(item =>
            {
                NumericComponent numeric = item.GetComponent<NumericComponent>();
                if (numeric.GetAsInt((int)EItemValue.RepairMoney) <= 0) return false;
                int dur = numeric.GetAsInt((int)EItemValue.Durability);
                int durMax = numeric.GetAsInt((int)EItemValue.DurabilityMax);
                if (durMax <= 0) return false;
                if ((dur / (float)durMax) > 0.8f) return false;
                return true;
            }).Count() != 0;
        }

        /// <summary>
        /// 维修价格
        /// </summary>
        /// <param name="localUnit"></param>
        /// <returns></returns>
        public static int RepairMoney(Unit localUnit)
        {
            int repairMoney = 0;
            RobotEquipmentComponent equipment = localUnit.GetComponent<RobotEquipmentComponent>();
            foreach(RobotItem item in equipment.EquipPartItemDict.Values)
            {
                NumericComponent numeric = item.GetComponent<NumericComponent>();
                repairMoney += numeric.GetAsInt((int)EItemValue.RepairMoney);
            }
            return repairMoney;
        }
    }
}
