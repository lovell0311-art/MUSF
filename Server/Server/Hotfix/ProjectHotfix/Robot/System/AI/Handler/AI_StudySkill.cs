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
    /// 学习技能
    /// </summary>
    [AIHandler]
    public class AI_StudySkill : AAIHandler
    {

        public override int Check(AIComponent aiComponent, AI_Config aiConfig)
        {
            Scene clientScene = aiComponent.ClientScene();
            Unit localUnit = UnitHelper.GetLocalUnitFromClientScene(clientScene);
            RobotBackpackComponent backpack = localUnit.GetComponent<RobotBackpackComponent>();
            if (backpack.WhereFromItemType(EItemType.SkillBooks, item => CanUseSkillBook(localUnit, item)).Count > 0) return 0;
            return 1;
        }
        public override async Task Execute(AIComponent aiComponent, AI_Config aiConfig, ETCancellationToken cancellationToken)
        {
            // 开始换装备
            Scene clientScene = aiComponent.ClientScene();
            Unit localUnit = UnitHelper.GetLocalUnitFromClientScene(clientScene);
            if (localUnit == null) return;
            RobotEquipmentComponent equipment = localUnit.GetComponent<RobotEquipmentComponent>();
            RobotBackpackComponent backpack = localUnit.GetComponent<RobotBackpackComponent>();
            List<RobotItem> allSkillBooks = backpack.WhereFromItemType(EItemType.SkillBooks, item => CanUseSkillBook(localUnit, item));
            if (allSkillBooks.Count == 0) return;
            bool ret = await RobotBackpackHelper.UseItem(localUnit, allSkillBooks.First(), cancellationToken);
            if (cancellationToken.IsCancel()) return;
            if (!ret) return;
        }

        /// <summary>
        /// 可以使用技能书
        /// </summary>
        /// <param name="localUnit"></param>
        /// <param name="skillBook"></param>
        /// <returns></returns>
        public static bool CanUseSkillBook(Unit localUnit,RobotItem skillBook)
        {
            if (!skillBook.CanUse(localUnit)) return false;
            RobotSkillComponent skillCom = localUnit.GetComponent<RobotSkillComponent>();
            RobotRoleComponent role = localUnit.GetComponent<RobotRoleComponent>();
            if (!skillBook.Config.GameOccupation2SkillId.TryGetValue(role.RoleType, out int skillId)) return false;
            if(skillId == 0)
            {
                Log.Warning($"#物品# 技能书无法使用: {skillBook.Config.Id}");
                return false;
            }
            return !skillCom.SkillGroup.ContainsKey(skillId);
        }
    }
}
