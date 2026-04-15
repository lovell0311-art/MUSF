using System;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork;
using CustomFrameWork.Baseic;
using ETModel;

namespace ETHotfix
{
    /// <summary>
    /// 学习技能
    /// </summary>
    [EventMethod("EquipItem")]
    public class EquipItemEvent_StudySkill : ITEventMethodOnRun<ETModel.EventType.EquipItem>
    {
        public void OnRun(ETModel.EventType.EquipItem args)
        {
            if (!args.item.HaveSkill()) return;

            // 学习技能，不保存
            var studySkillId = args.item.GetEquipSkillId((E_GameOccupation)args.unit.Data.PlayerTypeId);
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
