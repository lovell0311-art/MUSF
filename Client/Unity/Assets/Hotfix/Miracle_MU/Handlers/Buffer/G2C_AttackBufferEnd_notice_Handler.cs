using ETModel;
using System.Security.Principal;
using UnityEngine;

namespace ETHotfix
{

    /// <summary>
    /// buffer 结束
    /// </summary>
    [MessageHandler]
    public class G2C_AttackBufferEnd_notice_Handler : AMHandler<G2C_AttackBufferEnd_notice>
    {
        protected override void Run(ETModel.Session session, G2C_AttackBufferEnd_notice message)
        {
            if (UnitEntityComponent.Instance.Get<UnitEntity>(message.AttackTarget) is UnitEntity unitEntity)
            {
                if (unitEntity.GetComponent<BufferComponent>() != null)
                {
                    unitEntity.GetComponent<BufferComponent>()?.HideBuffer((int)(message.BufferId & 0xffff));
                }
                if ((int)(message.BufferId & 0xffff) ==(int)E_BattleSkillStats.BingFeng)//减速
                {
                    unitEntity.IsSlowDown = false;
                }



                if ((int)(message.BufferId & 0xffff) == (int)E_BattleSkillStats.FangYuHuZhao)//护罩
                {
                    if (unitEntity is PetEntity petEntity)
                    {
                        petEntity.Game_Object.transform.Find("Pet_ShuangTouLong_Skill_01").gameObject.SetActive(false);
                    }
                    else if (unitEntity is RoleEntity roleEntity)
                    {
                        if (roleEntity.Game_Object != null)
                        {
                            GameObject skillObj = roleEntity.Game_Object?.transform.Find("Pet_ShuangTouLong_Skill_01")?.gameObject;
                            if (skillObj != null)
                            {
                                if (roleEntity.GetComponent<BufferComponent>() is BufferComponent bufferComponent)
                                {
                                    if (bufferComponent.BufferDic.TryGetValue((int)(message.BufferId & 0xffff),out GameObject buffer))
                                    {
                                        bufferComponent.BufferDic.Remove((int)(message.BufferId & 0xffff));
                                    }
                                }
                                ResourcesComponent.Instance.DestoryGameObjectImmediate(skillObj, skillObj.name.StringToAB());
                            }
                        }
                    }
                }
            }
           
        }
    }
}