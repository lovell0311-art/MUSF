using System.Collections.Generic;
using System.Linq;

namespace ETModel.Robot
{
    [ObjectSystem]
    public class RobotSkillComponentAwakeSystem : AwakeSystem<RobotSkillComponent>
    {
        public override void Awake(RobotSkillComponent self)
        {
        }
    }

    [ObjectSystem]
    public class RobotSkillComponentDestroySystem : DestroySystem<RobotSkillComponent>
    {
        public override void Destroy(RobotSkillComponent self)
        {
            self.SkillGroup.Clear();
            while(self.NextUseSkillIdList.Count > 0)
            {
                self.NextUseSkillIdList.Dequeue();
            }
            foreach(RobotSkill skill in self.SkillGroup.Values.ToArray())
            {
                skill.Dispose();
            }
            self.SkillGroup.Clear();
        }
    }


    public class RobotSkillComponent : Entity
    {
        public Dictionary<int,RobotSkill> SkillGroup = new Dictionary<int,RobotSkill>();
        public Queue<int> NextUseSkillIdList = new Queue<int>();
    }
}
