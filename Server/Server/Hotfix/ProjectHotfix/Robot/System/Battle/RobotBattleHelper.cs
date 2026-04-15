using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using ETModel;
using ETModel.Robot;
using ETModel.Robot.WaitType;
using ETModel.ET;
using CustomFrameWork;


namespace ETHotfix.Robot
{
    public static class RobotBattleHelper
    {
        /// <summary>
        /// 攻击单位
        /// </summary>
        /// <param name="localUnit"></param>
        /// <param name="targetUnit"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<bool> AttackUnit(Unit localUnit,Unit targetUnit,ETCancellationToken cancellationToken)
        {
            Scene clientScene = localUnit.ClientScene();
            long instanceId = targetUnit.InstanceId;
            AttackComponent attackComponent = localUnit.GetComponent<AttackComponent>();
            while (true)
            {
                if (instanceId != targetUnit.InstanceId) return true;

                bool EnterAttackDistance()
                {
                    if ((targetUnit.Position - localUnit.Position).sqrMagnitude <= (attackComponent.AttackDistance * attackComponent.AttackDistance))
                    {
                        // 进入攻击范围
                        return true;
                    }
                    return false;
                }
                bool ret;
                if (!EnterAttackDistance())
                {
                    // 移动到攻击距离
                    ret = await localUnit.MoveToUnitAsync(targetUnit, EnterAttackDistance, cancellationToken);
                    if (ret == false) return false;
                    if (instanceId != targetUnit.InstanceId) return true;
                }
                // 目标单位进入攻击范围

                ret = await StartAttack(localUnit, targetUnit, cancellationToken);
                if(ret == false) return false;
            }
        }

        private static async Task<bool> StartAttack(Unit localUnit,Unit targetUnit,ETCancellationToken cancellationToken)
        {
            AttackComponent attackComponent = localUnit.GetComponent<AttackComponent>();
            RobotSkillComponent robotSkillComponent = localUnit.GetComponent<RobotSkillComponent>();
            long instanceId = targetUnit.InstanceId;
            RobotSkill skill = null;
            while (true)
            {
                bool ret = await TimerComponent.Instance.WaitTillAsync(attackComponent.NextAttackTime, cancellationToken);
                if (ret == false) return false;
                if (instanceId != targetUnit.InstanceId) return true;
                if (attackComponent.NextAttackType != 0)
                {
                    if(!robotSkillComponent.SkillGroup.TryGetValue(attackComponent.NextAttackType, out skill))
                    {
                        skill = null;
                        UpdateNextAttackType(localUnit);
                        return true;
                    }
                }
                else
                {
                    skill = null;
                }

                bool EnterAttackDistance()
                {
                    if ((targetUnit.Position - localUnit.Position).sqrMagnitude <= (attackComponent.AttackDistance * attackComponent.AttackDistance))
                    {
                        // 进入攻击范围
                        return true;
                    }
                    return false;
                }
                if (!EnterAttackDistance()) return true;    // 目标单位离开攻击距离

                if (skill != null)
                {
                    ret = await UseSkill(skill,targetUnit, cancellationToken);
                    if (ret == false) return false;
                }
                else
                {
                    ret = await NormalAttack(targetUnit, cancellationToken);
                    if (ret == false) return false;
                }
                

                ret = await TimerComponent.Instance.WaitTillAsync(attackComponent.NextAttackTime, cancellationToken);
                if (ret == false) return false;


                UpdateNextAttackType(localUnit);
            }
        }

        public static async Task<bool> NormalAttack(Unit targetUnit, ETCancellationToken cancellationToken)
        {
            Scene clientScene = targetUnit.ClientScene();
            Session session = clientScene.GetComponent<SessionComponent>().session;

            ETCancellationToken cancelWaitToken = new ETCancellationToken();
            async Task<bool> C2G_AttackRequestCoroutine()
            {

                IResponse res = await session.Call(new C2G_AttackRequest()
                {
                    GameUserId = targetUnit.Id,
                    AttackType = 0,
                    PosX = targetUnit.Position.x,
                    PosY = targetUnit.Position.y,
                    Tick = Help_TimeHelper.GetNow(),
                }, cancellationToken);
                if(cancellationToken.IsCancel())return false;
                if (res.Error != ErrorCode.ERR_Success)
                {
                    Log.Warning($"[{clientScene.Name}] C2G_AttackRequest:{res.Error}");
                    cancelWaitToken.Cancel();
                    return false;
                }
                return true;
            }

            async Task<bool> Wait_AttackStartNoticeCoroutine()
            {
                await clientScene.GetComponent<ObjectWait>().Wait<Wait_AttackStartNotice>(cancelWaitToken);
                if (cancelWaitToken.IsCancel()) return false;
                return true;
            }

            void CancelWaitToken()
            {
                cancelWaitToken.Cancel();
            }
          
            try
            {
                cancellationToken?.Add(CancelWaitToken);

                using ListComponent<Task<bool>> tasks = ListComponent<Task<bool>>.Create();

                tasks.Add(C2G_AttackRequestCoroutine());
                tasks.Add(Wait_AttackStartNoticeCoroutine());

                List<bool> rets = await TaskHelper.WaitAll(tasks);
                if (cancellationToken.IsCancel()) return false;
                foreach (bool ret in rets) if (ret == false) return false;
            }
            finally
            {
                cancellationToken?.Remove(CancelWaitToken);
            }

            return true;
        }

        public static async Task<bool> UseSkill(RobotSkill skill, Unit targetUnit, ETCancellationToken cancellationToken)
        {
            Scene clientScene = skill.ClientScene();
            Session session = clientScene.GetComponent<SessionComponent>().session;
            skill.LastUseTime = Help_TimeHelper.GetNow();

            ETCancellationToken cancelWaitToken = new ETCancellationToken();
            async Task<bool> C2G_AttackRequestCoroutine()
            {

                IResponse res = await session.Call(new C2G_AttackRequest()
                {
                    GameUserId = targetUnit.Id,
                    AttackType = skill.ConfigId,
                    PosX = targetUnit.Position.x,
                    PosY = targetUnit.Position.y,
                    Tick = Help_TimeHelper.GetNow(),
                }, cancellationToken);
                if (cancellationToken.IsCancel())return false;
                if (res.Error != ErrorCode.ERR_Success)
                {
                    Log.Warning($"[{clientScene.Name}] C2G_AttackRequest:{res.Error}");
                    cancelWaitToken.Cancel();
                    return false;
                }
                return true;
            }

            async Task<bool> Wait_AttackStartNoticeCoroutine()
            {
                await clientScene.GetComponent<ObjectWait>().Wait<Wait_AttackStartNotice>(cancelWaitToken);
                if (cancelWaitToken.IsCancel()) return false;
                return true;
            }


            void CancelWaitToken()
            {
                cancelWaitToken.Cancel();
            }


            try
            {
                cancellationToken?.Add(CancelWaitToken);
                using ListComponent<Task<bool>> tasks = ListComponent<Task<bool>>.Create();

                tasks.Add(C2G_AttackRequestCoroutine());
                tasks.Add(Wait_AttackStartNoticeCoroutine());

                List<bool> rets = await TaskHelper.WaitAll(tasks);
                if (cancellationToken.IsCancel()) return false;
                foreach (bool ret in rets) if (ret == false) return false;
            }
            finally
            {
                cancellationToken?.Remove(CancelWaitToken);
            }


            return true;
        }


        public static void UpdateNextAttackType(Unit localUnit)
        {
            AttackComponent attackComponent = localUnit.GetComponent<AttackComponent>();
            RobotSkillComponent robotSkillComponent = localUnit.GetComponent<RobotSkillComponent>();
            // TODO 取下次攻击类型
            RobotSkill skill = robotSkillComponent.GetNextAttackSkill();
            if (skill != null)
            {
                attackComponent.NextAttackType = skill.ConfigId;
                attackComponent.AttackDistance = skill.Distance;
            }
            else
            {
                attackComponent.NextAttackType = 0;
                attackComponent.AttackDistance = attackComponent.NormalAttackDistance;
            }
        }

    }
}
