using System.Threading.Tasks;
using ETModel;
using ETModel.Robot;
using ETModel.Robot.WaitType;
using CustomFrameWork;
using CustomFrameWork.Component;
using MongoDB.Bson;

namespace ETHotfix.Robot
{
    [MessageHandler(AppType.Robot)]
    public class G2C_StudySkillSingle_noticeHandler : AMHandler<G2C_StudySkillSingle_notice>
    {
        protected async override Task<bool> BeforeCodeAsync(Session session, G2C_StudySkillSingle_notice message)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, session.InstanceId))
            {
                return await NextCodeAsync(session, message);
            }
        }
        protected async override Task<bool> CodeAsync(Session session, G2C_StudySkillSingle_notice message)
        {
            Scene clientScene = session.ClientScene();
            Unit localUnit = UnitHelper.GetLocalUnitFromClientScene(clientScene);
            if(localUnit == null)
            {
                Log.Warning($"[{clientScene.Name}] localUnit == null");
                return false;
            }

            RobotSkill skill = RobotSkillFactory.Create(clientScene,message.SkillId);
            if(skill == null)
            {
                Log.Warning($"[{clientScene.Name}] 技能配置不存在，skillId={message.SkillId}");
                return false;
            }
            RobotSkillComponent robotSkillComponent = localUnit.GetComponent<RobotSkillComponent>();
            robotSkillComponent.SkillGroup.TryAdd(skill.ConfigId, skill);

            return false;
        }
    }
}
