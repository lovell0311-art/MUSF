using ETModel;
using ETModel.Robot;


namespace ETHotfix.Robot
{
    public static class RobotSkillComponentSystem
    {
        // 获取下一个释放的技能
        public static RobotSkill GetNextAttackSkill(this RobotSkillComponent self)
        {
            if (self.SkillGroup.Count == 0) return null;
            while (self.NextUseSkillIdList.Count > 0)
            {
                int skillId = self.NextUseSkillIdList.Dequeue();

                if(self.SkillGroup.TryGetValue(skillId,out RobotSkill skill))
                {
                    if(skill.CanUse())
                    {
                        return skill;
                    }
                }
            }

            foreach (RobotSkill skill in self.SkillGroup.Values)
            {
                if(skill.SkillType == RobotSkillType.Attack)
                {
                    self.NextUseSkillIdList.Enqueue(skill.ConfigId);
                }
            }

            while (self.NextUseSkillIdList.Count > 0)
            {
                int skillId = self.NextUseSkillIdList.Dequeue();

                if (self.SkillGroup.TryGetValue(skillId, out RobotSkill skill))
                {
                    if (skill.CanUse())
                    {
                        return skill;
                    }
                }
            }

            return null;
        }
    }
}
