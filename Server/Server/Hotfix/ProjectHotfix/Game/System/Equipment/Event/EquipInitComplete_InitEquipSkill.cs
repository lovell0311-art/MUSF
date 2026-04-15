using System;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork;
using CustomFrameWork.Baseic;
using ETModel;

namespace ETHotfix
{
    /// <summary>
    /// 初始化装备技能
    /// </summary>
    [EventMethod("EquipInitComplete")]
    public class EquipInitComplete_InitEquipSkill : ITEventMethodOnRun<ETModel.EventType.EquipInitComplete>
    {
        public void OnRun(ETModel.EventType.EquipInitComplete args)
        {
            var equipment = args.unit.Player.GetCustomComponent<EquipmentComponent>();
            if (equipment == null)
            {
                return;
            }
            // 初始化装备技能
            foreach(var item in equipment.EquipPartItemDict.Values)
            {
                if (!item.HaveSkill()) continue;
                var studySkillId = item.GetEquipSkillId((E_GameOccupation)args.unit.Data.PlayerTypeId);
                if (args.unit.SkillGroup.ContainsKey(studySkillId) == false)
                {
                    var mSkillCreateBuilder = Root.MainFactory.GetCustomComponent<SkillCreateBuilder>();
                    var mSkillInstance = mSkillCreateBuilder.CreateHeroSKill(studySkillId);

                    args.unit.SkillGroup[mSkillInstance.Id] = mSkillInstance;

                    G2C_StudySkillSingle_notice mSkillSingle = new G2C_StudySkillSingle_notice();
                    mSkillSingle.SkillId = studySkillId;
                    args.unit.Player.Send(mSkillSingle);
                }
            }
        }
    }
}
