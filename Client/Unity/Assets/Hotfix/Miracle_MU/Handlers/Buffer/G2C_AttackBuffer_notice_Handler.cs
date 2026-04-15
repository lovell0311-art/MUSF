using ETModel;
using NUnit.Framework.Interfaces;
using UnityEngine;

namespace ETHotfix
{

    [MessageHandler]
    public class G2C_AttackBuffer_notice_Handler : AMHandler<G2C_AttackBuffer_notice>
    {
        protected override void Run(ETModel.Session session, G2C_AttackBuffer_notice message)
        {
            // Log.Debug("message.AttackTarget  " + message.AttackTarget + " 显示Buff " + message.BufferId);

            if (message.BufferId==15)
            {

            }
            else
            {
                if (UnitEntityComponent.Instance.Get<UnitEntity>(message.AttackTarget) is UnitEntity unitEntity)
                {

                    if (unitEntity.GetComponent<BufferComponent>() != null)
                    {
                        if ((int)(message.BufferId & 0xffff) == (int)E_BattleSkillStats.BingFeng)//减速
                        {
                            unitEntity.IsSlowDown = true;
                        }

                        unitEntity.GetComponent<BufferComponent>()?.ShowBuffer((int)(message.BufferId & 0xffff));

                    }
                    //   UIMainComponent.Instance.ReshUi();
                    //本地玩家 更新Buffer时间
                    if (message.Ticks > 0 && UIMainComponent.Instance != null && unitEntity.Id == UnitEntityComponent.Instance.LocaRoleUUID)
                    {
                        /*Buff_UnitConfig buff_UnitConfig = ConfigComponent.Instance.GetItem<Buff_UnitConfig>((int)(message.BufferId & 0xffff));
                        if (buff_UnitConfig != null)
                        {
                            Log.DebugRed($"{buff_UnitConfig.Name} message.Ticks:{message.Ticks}");
                        }*/
                        UIMainComponent.Instance.ChangeBufferCoolTime((int)(message.BufferId & 0xffff), (message.Ticks / 1000));

                    }

                    if ((int)(message.BufferId & 0xffff) == (int)E_BattleSkillStats.FangYuHuZhao)//护罩
                    {
                        if (unitEntity is PetEntity petEntity)
                        {
                            petEntity.Game_Object.transform.Find("Pet_ShuangTouLong_Skill_01").gameObject.SetActive(true);
                        }
                        else if (unitEntity is RoleEntity roleEntity)
                        {
                            if (roleEntity.Game_Object != null)
                            {
                                if (roleEntity.Game_Object.transform.Find("Pet_ShuangTouLong_Skill_01") == null)
                                {
                                    GameObject skillObj = ResourcesComponent.Instance.LoadGameObject("Pet_ShuangTouLong_Skill_01".StringToAB(), "Pet_ShuangTouLong_Skill_01");
                                    roleEntity.GetComponent<BufferComponent>().BufferDic[(int)(message.BufferId & 0xffff)] = skillObj;
                                    skillObj.transform.parent = roleEntity.Game_Object.transform;
                                    skillObj.transform.localPosition = Vector3.up;
                                }
                            }
                        }
                    }
                }
            }

        }
    }
}