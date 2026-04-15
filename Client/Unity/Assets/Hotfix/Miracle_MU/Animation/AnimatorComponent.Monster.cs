using System.Collections.Generic;
using UnityEngine;
using ETModel;
using ILRuntime.Runtime;
using Google.Protobuf.Collections;
using Newtonsoft.Json;
using System.Threading;
using UnityEngine.Profiling;
using DG.Tweening;
using NPOI.SS.Formula.Functions;

namespace ETHotfix
{
    /// <summary>
    /// 怪物· 动画 组件
    /// </summary>
    public partial class AnimatorComponent
    {
        public MonsterEntity monsterEntity;
      
        GameSetInfo gameSetInfo;
        int spaceTime;//待机音效间隔时间
        /// <summary>圆形提示</summary>
        private const string yuanPrefab = "BossOSkillTip";

        /// <summary> 扇形提示</summary>
        private const string fangPrefab = "BossSSkillTip";

        AnimationEventProxy Proxy;
        public UnitEntity hitunitEntity;//攻击目标实体
        public void Init_Monster(MonsterEntity monsterEntity)
        {

            this.monsterEntity = monsterEntity;
          
            gameSetInfo = LocalDataJsonComponent.Instance.gameSetInfo;
            spaceTime = RandomHelper.RandomNumber(3, 8);
            if (monsterEntity.Game_Object.GetComponent<Animator>() != null)
            {
                Proxy = monsterEntity.Game_Object.GetComponent<AnimationEventProxy>()
                        ?? monsterEntity.Game_Object.AddComponent<AnimationEventProxy>();
            }
            else
            {
                Proxy = monsterEntity.Game_Object.GetComponentInChildren<AnimationEventProxy>();
            }
            Proxy.Effect_Action_Str = ShowEffece;

            if (monsterEntity.monsterConfigId == 547)//NPC警卫队长摩仑
            {
                subAnimators.AddRange(monsterEntity.Game_Object.transform.GetComponentsInChildren<Animator>());
            }

        }
        public void Init_Summon(SummonEntity summonEntity)
        {

            this.monsterEntity = summonEntity;

            gameSetInfo = LocalDataJsonComponent.Instance.gameSetInfo;
            spaceTime = RandomHelper.RandomNumber(3, 8);
            Proxy = monsterEntity.Game_Object.GetComponent<AnimationEventProxy>()
                    ?? monsterEntity.Game_Object.AddComponent<AnimationEventProxy>();
            Proxy.Effect_Action_Str = ShowEffece;

        }
        public void ShowEffece(string effectResName)
        {
            if (string.IsNullOrEmpty(effectResName)) return;


            //加载技能特效
            GameObject effect = ResourcesComponent.Instance.LoadEffectObject(effectResName.StringToAB(), effectResName);
            if (effect == null) return;
           /* if (effect.GetComponent<ResourcesRecycle>() is null)
            {
                effect.AddComponent<ResourcesRecycle>();
            }*/
            //技能特效是否 带LineRenderer组件
            if (!effect.TryGetComponent<LineRenderer>(out var lineRenderer) && effect.GetComponentInChildren<LineRenderer>() == false)
            {
                if (monsterEntity.IsDead || monsterEntity.Game_Object == null)
                {
                    return;
                }
                //特效位置就 在玩家原地

                Transform effectPos = monsterEntity.Game_Object.transform.Find("effectPos");
                if (effectPos != null)
                {
                    effect.transform.localPosition = effectPos.position;
                }
                else
                {
                    effect.transform.localPosition = monsterEntity.Game_Object.transform.position;
                }
                //effect.transform.localPosition = monsterEntity.Game_Object.transform.position;//+Vector3.up*1.5f
                effect.transform.localRotation = monsterEntity.Game_Object.transform.rotation;

            }
            else
            {

                if (lineRenderer == null)
                {
                    LineRenderer[] lineRendererlist = effect.GetComponentsInChildren<LineRenderer>();
                  //  Log.DebugGreen($"显示怪物特效：{effectResName} {hitunitEntity != null && hitunitEntity.Game_Object != null} ");
                    if (hitunitEntity != null && hitunitEntity.Game_Object != null)
                    {
                        effect.transform.localRotation = monsterEntity.Game_Object.transform.rotation;
                        effect.transform.localPosition = monsterEntity.Game_Object.transform.position;//+Vector3.up*1.5f

                        for (int i = 0, length = lineRendererlist.Length; i < length; i++)
                        {
                            lineRendererlist[i].SetPosition(0, monsterEntity.Position + Vector3.up * 4f);//起始点
                            lineRendererlist[i].SetPosition(1, hitunitEntity.CurrentPos + Vector3.up * 2.5f);//起始点
                        }

                    }
                    else
                    {
                        effect.transform.localRotation = monsterEntity.Game_Object.transform.rotation;
                        effect.transform.localPosition = monsterEntity.Game_Object.transform.position;
                        for (int i = 0, length = lineRendererlist.Length; i < length; i++)
                        {
                            lineRendererlist[i].SetPosition(0, monsterEntity.Position + Vector3.up * 4f);//起始点
                            lineRendererlist[i].SetPosition(1, monsterEntity.Game_Object.transform.position + Vector3.up * 2.5f);//起始点
                        }
                    }
                }
                else
                {
                    if (hitunitEntity != null)
                    {
                        lineRenderer.SetPosition(0, monsterEntity.Position + Vector3.up * 2.5f);//起始点
                        lineRenderer.SetPosition(1, hitunitEntity.CurrentPos + Vector3.up * 2f);//目标点 为被击实体
                    }
                    else
                    {
                        lineRenderer.SetPosition(0, monsterEntity.Position + Vector3.up * 2.5f);//起始点
                        lineRenderer.SetPosition(1, monsterEntity.Game_Object.transform.position + Vector3.up * 2f);//目标点 为被击实体
                    }
                }
            }


        }

        /// <summary>
        /// 显示 被击动画
        /// </summary>
        public void HitEffect()
        {
            SetTrigger(MotionType.Hit);
        }

        /// <summary>
        /// 播放 怪物 攻击音效
        /// </summary>
        public void PlayAttackSound()
        {
            if (monsterEntity.Sound_Attack.Length==0) return;
           // Profiler.BeginSample("PlayAttackSound");
            SoundComponent.Instance.PlayClip(monsterEntity.Sound_Attack, monsterEntity.CurrentPos);//monsterEntity.CurrentPos
           // Profiler.EndSample();
        }

        Vector3[] path1 = new Vector3[4];

        /// <summary>
        /// 怪物 使用技能
        /// </summary>
        /// <param name="UnitUUID">目标 实体的UUID</param>
        public void UseSkill(long UnitUUID,int skillid=0)
        {
            hitunitEntity = UnitEntityComponent.Instance.Get<UnitEntity>(UnitUUID);
            if (monsterEntity == null) return;
            monsterEntity?.Game_Object?.transform.LookAt(hitunitEntity?.Game_Object?.transform);

            Quaternion monsterRotation = monsterEntity.Rotation;// 特效 旋转 与怪物的旋转一样
            if (hitunitEntity == null) return;
            Setparameter(hitunitEntity);
          //  Log.DebugBrown("天鹰monsterEntity" + monsterEntity+":::"+ monsterEntity.monsterConfigId);
            if (monsterEntity.monsterConfigId == 546)//天鹰
            {

                //Log.DebugBrown("天鹰" + "天鹰播放动画");
                if (skillid == 10023)
                {
                    if (monsterEntity.IsDead == false)
                    {
                        SetTrigger("10023");//播放 攻击动画
                    }
                    GameObject effect_ = ResourcesComponent.Instance.LoadEffectObject("Tianying_gongji_effect".StringToAB(), "Tianying_gongji_effect");
                    if (effect_ == null) return;
                    /*if (hitunitEntity is RoleEntity role_)
                        effect_.transform.position = role_.Position;//特效位置 为玩家的位置
                    else
                        effect_.transform.position = hitunitEntity.Position;//特效位置 为玩家的位置

                    effect_.transform.rotation = monsterRotation;*/

                    path1[0] = monsterEntity.Game_Object.transform.position;
                    path1[1] = monsterEntity.Game_Object.transform.position + (Vector3.up + Vector3.forward) * 3;
                    path1[2] = hitunitEntity.Game_Object.transform.position;
                    path1[3] = monsterEntity.Game_Object.transform.position;
                    //Tianying_gongji_effect
                    monsterEntity.Game_Object.transform.DOPath(path1, 1f).OnComplete(() =>
                    {
                        if (monsterEntity.IsDead == false)
                        {
                            SetTrigger("10023");//播放 攻击动画
                        }
                        GameObject effect = ResourcesComponent.Instance.LoadEffectObject("Tianying_gongji_effect".StringToAB(), "Tianying_gongji_effect");
                        if (effect == null) return;
                        if (hitunitEntity is RoleEntity role)
                            effect.transform.position = role.Position;//特效位置 为玩家的位置
                        else
                            effect.transform.position = hitunitEntity.Position;//特效位置 为玩家的位置

                        effect.transform.rotation = monsterEntity.Rotation;// 特效 旋转 与怪物的旋转一样
                    });
                }
            }


            if (monsterEntity.AttackEffect.Length==0) return;

           

            //显示 玩家被怪物技能击中的特效 
            GameObject effect = ResourcesComponent.Instance.LoadEffectObject(monsterEntity.AttackEffect.StringToAB(), monsterEntity.AttackEffect);
            if (effect == null) return;
           /* if (effect.GetComponent<ResourcesRecycle>() == null)
            {
                effect.AddComponent<ResourcesRecycle>().RecycleTime = 1;
            }*/
            if (hitunitEntity is RoleEntity role)
                effect.transform.position = role.Position;//特效位置 为玩家的位置
            else
                effect.transform.position = hitunitEntity.Position;//特效位置 为玩家的位置

            effect.transform.rotation = monsterRotation;// 特效 旋转 与怪物的旋转一样
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="UnitUUID">目标 实体的UUID</param>
        /// <param name="delayTime">技能释放延迟时间</param>
        public void BossUseSkill(long UnitUUID, long AttackType, long timetick, RepeatedField<long> target)
        {
            ShowBossSkillTips(AttackType, UnitUUID);
            UnitEntity entity = UnitEntityComponent.Instance.Get<UnitEntity>(UnitUUID);
            Setparameter(entity);
        
            if (AttackType == 0)
            {
                if (monsterEntity.AttackEffect.Length>0)
                {
                    GameObject effect = ResourcesComponent.Instance.LoadEffectObject(monsterEntity.AttackEffect.StringToAB(), monsterEntity.AttackEffect);
                    if (string.IsNullOrEmpty(monsterEntity.AttackEffect)) return;
                    effect.transform.position = monsterEntity.Position;//特效位置 为玩家的位置
                    effect.transform.rotation = monsterEntity.Rotation;// 特效 旋转 与怪物的旋转一样
                }
            }
            else
            {
                Skill_monsterConfig skillinfo = ConfigComponent.Instance.GetItem<Skill_monsterConfig>(AttackType.ToInt32());//获取怪物 技能配置表
                if (skillinfo == null)
                {
                    //Log.DebugRed($"skillid:{AttackType} 技能配置不存在");
                    return;
                }
              
                if (skillinfo.Id == 10021 || skillinfo.Id == 10017)//昆顿火焰术，下落随机点
                {
                    for (int i = 0, len = target.count; i < len; i++)
                    {
                        long id = target[i];

                        long time = (int)(id >> 32) + timetick;
                        int posx = (int)((id >> 16) & 0xffff);
                        int posy = (int)(id & 0xffff);
                      //  Log.DebugGreen($"测试副本---技能下落点X->{posx}---技能下落点Y->{posy}---时间->{TimeHelper.ClientNow()}--{time}--几秒后释放->{(time - TimeHelper.ClientNow())/1000}");
                        TimerComponent.Instance.RegisterTimeCallBack(time - TimeHelper.ClientNow(), () =>
                        {
                            GameObject attackEffect = ResourcesComponent.Instance.LoadEffectObject(skillinfo.AttackEffect.StringToAB(), skillinfo.AttackEffect);
                            attackEffect.transform.position = AstarComponent.Instance.GetVectory3(posx, posy);
                          //  Log.DebugGreen($"测试Boss开始生成特效{attackEffect.transform.position}");
                        });
                    }
                }
                else
                {
                    int time = 0;
                    var values = JsonConvert.DeserializeObject<Dictionary<int, int>>(skillinfo.OtherData);//附加数值
                    if (values.ContainsKey(5))
                    {
                        time = values[5];
                    }
                    
                   // Log.DebugGreen($"技能生成类型-->{skillinfo.skillType} 技能时间-->{time} 特效：{skillinfo.AttackEffect}");
                    TimerComponent.Instance.RegisterTimeCallBack(time, () =>
                    {
                     
                        if (string.IsNullOrEmpty(skillinfo.AttackEffect)) return;
                        
                        GameObject attackEffect = ResourcesComponent.Instance.LoadEffectObject(skillinfo.AttackEffect.StringToAB(), skillinfo.AttackEffect);
                        if (attackEffect == null) return;
                        Vector3 pos = Vector3.zero;
                        switch (skillinfo.skillType)
                        {
                            case 0:
                                break;
                            case 1:
                                pos = monsterEntity.CurrentPos;
                                break;
                            case 2:
                                pos = entity.CurrentPos;
                                break;
                            default:
                                break;
                        }
                        attackEffect.transform.position = pos.GroundPos();
                    });
                }
            }

        }

        //显示Boss 技能提示特效
        public void ShowBossSkillTips(long skillId, long UnitUUID)
        {
            UnitEntity entity = UnitEntityComponent.Instance.Get<UnitEntity>(UnitUUID);
            Skill_monsterConfig skillinfo = ConfigComponent.Instance.GetItem<Skill_monsterConfig>(skillId.ToInt32());//获取怪物 技能配置表
            if (skillinfo == null)
            {
                //Log.DebugRed($"skillid:{skillId} 技能配置不存在");
                return;
            }
            int time = 0;
            int distance = 0;
            var values = JsonConvert.DeserializeObject<Dictionary<int, int>>(skillinfo.OtherData);//附加数值
            if (values.ContainsKey(5))
            {
                time = values[5];
            }
            if (values.ContainsKey(2))
            {
                distance = values[2];
            }
            switch (skillinfo.TipsType)
            {
                case 0://圆形
                    GameObject circle = ResourcesComponent.Instance.LoadEffectObject(yuanPrefab.StringToAB(), yuanPrefab,1000);
                    Vector3 pos = Vector3.zero;
                    switch (skillinfo.skillType)
                    {
                        case 0:
                            break;
                        case 1:
                            pos = monsterEntity.CurrentPos;
                            break;
                        case 2:
                            pos = entity.CurrentPos;
                            break;
                        default:
                            break;
                    }
                    circle.transform.position = pos.GroundPos();
                    //   Log.DebugGreen($"��������{skillinfo.skillType} ���ܴ�С{distance}  ԭ����С{circle.transform.Find("AnimPos/x").GetComponent<RectTransform>().sizeDelta}");
                   // Log.DebugGreen($"========{circle.transform.Find("AnimPos/x").GetComponent<RectTransform>().sizeDelta}");
                    circle.transform.Find("AnimPos/x").GetComponent<RectTransform>().sizeDelta = new Vector2(4.0f, 4.0f) * distance;
                    circle.transform.Find("AnimPos/s").GetComponent<RectTransform>().sizeDelta = new Vector2(4.0f, 4.0f) * distance;
                    //设置销毁
                    /*TimerComponent.Instance.RegisterTimeCallBack(time + 1000, () =>
                    {
                        ResourcesComponent.Instance.DestoryGameObjectImmediate(circle, yuanPrefab.StringToAB());
                    });*/
                    break;
                case 1://矩形
                    //GameObject rectangle = ResourcesComponent.Instance.LoadGameObject(fangPrefab.StringToAB(), fangPrefab);
                    break;
                case 2://射线
                    //GameObject ray = ResourcesComponent.Instance.LoadGameObject(fangPrefab.StringToAB(), fangPrefab);
                    break;
                case 3://扇形
                    // GameObject fan = ResourcesComponent.Instance.LoadEffectObject(fangPrefab.StringToAB(), fangPrefab);
                    break;
                default:
                    break;
            }
            //var roleEntityPosY = UnitEntityComponent.Instance.LocalRole.Game_Object.transform.position.y;
            //if (skillinfo.TipsType == 0)
            //{
            //    //圈提示
            //    GameObject yuanGo = ResourcesComponent.Instance.LoadGameObject(yuanPrefab.StringToAB(), yuanPrefab);
            //    Vector3 pos = AstarComponent.Instance.GetVectory3(unitEntity.CurrentNodePos.x, unitEntity.CurrentNodePos.z);
            //    pos.y = roleEntityPosY + 0.3f;
            //    yuanGo.transform.position = pos;
            //    yuanGo.transform.localScale = Vector3.one * skillinfo.Distance;
            //    //设置动画
            //    Animator anim = yuanGo.GetComponentInChildren<Animator>();
            //    anim.SetFloat("speed", 1f / (Convert.ToInt32(skillinfo.TimeLeft * 0.001f)));//延迟时间
            //    yuanGo.SetActive(true);
            //    //设置销毁
            //    TimerComponent.Instance.RegisterTimeCallBack(skillinfo.TimeLeft + 1000, () =>
            //    {
            //        ResourcesComponent.Instance.RecycleGameObject(yuanGo);
            //    });
            //}
            //else if (skillinfo.TipsType == 1)
            //{
            //    //矩形提示  (起始点与结束点)
            //    GameObject yuanGo = ResourcesComponent.Instance.LoadGameObject(fangPrefab.StringToAB(), fangPrefab);
            //    Vector3 pos = AstarComponent.Instance.GetVectory3((unitEntity.CurrentNodePos.x + 2) / 2, unitEntity.CurrentNodePos.z);
            //    pos.y = roleEntityPosY + 0.3f;
            //    yuanGo.transform.position = pos;
            //    yuanGo.transform.localScale = Vector3.one;
            //    yuanGo.transform.localEulerAngles = Vector3.zero;
            //    UnityEngine.UI.Image[] images = yuanGo.GetComponentsInChildren<UnityEngine.UI.Image>();
            //    for (int i = 0; i < images.Length; i++)
            //    {
            //        Transform curTrans = images[i].transform;
            //        // curTrans.localScale = new Vector3(Mathf.Abs(item.BeginPosX - item.EndPosX), Mathf.Abs(item.BeginPosY - item.EndPosY), 1);
            //    }
            //    yuanGo.SetActive(true);
            //    //设置动画
            //    //Animator anim = yuanGo.GetComponentInChildren<Animator>();
            //    //anim.SetFloat("speed", 1f / (Convert.ToInt32(message.TimeLeft * 0.001f)));
            //    //设置销毁
            //    TimerComponent.Instance.RegisterTimeCallBack(skillinfo.TimeLeft + 1000, () =>
            //    {
            //        ResourcesComponent.Instance.RecycleGameObject(yuanGo);
            //    });
            //}
            //else if (skillinfo.skillType == 2)
            //{
            //    //射线提示

            //    GameObject rayGo = ResourcesComponent.Instance.LoadGameObject(fangPrefab.StringToAB(), fangPrefab);
            //    Vector3 pos = AstarComponent.Instance.GetVectory3(unitEntity.CurrentNodePos.x, unitEntity.CurrentNodePos.z);
            //    Vector3 endPos = AstarComponent.Instance.GetVectory3(unitEntity.CurrentNodePos.x, unitEntity.CurrentNodePos.z);
            //    pos.y = roleEntityPosY + 0.3f;
            //    endPos.y = roleEntityPosY + 0.3f;
            //    rayGo.transform.position = pos;
            //    rayGo.transform.localScale = Vector3.one;
            //    //生成射线大小
            //    UnityEngine.UI.Image[] images = rayGo.GetComponentsInChildren<UnityEngine.UI.Image>();
            //    for (int i = 0; i < images.Length; i++)
            //    {
            //        Transform curTrans = images[i].transform;
            //        //   curTrans.localScale = new Vector3(item.Width, Vector3.Distance(pos, endPos), 1);
            //    }
            //    rayGo.transform.LookAt(endPos);
            //    rayGo.SetActive(true);
            //    //设置动画
            //    //Animator anim = yuanGo.GetComponentInChildren<Animator>();
            //    //anim.SetFloat("speed", 1f / (Convert.ToInt32(message.TimeLeft * 0.001f)));
            //    //设置销毁
            //    TimerComponent.Instance.RegisterTimeCallBack(skillinfo.TimeLeft + 1000, () =>
            //    {
            //        ResourcesComponent.Instance.RecycleGameObject(rayGo);
            //    });
            //}
        }
        public void Setparameter(UnitEntity entity)
        {
            if (entity != null)
            {
                if (entity is RoleEntity roleEntity)
                {
                    //this.monsterEntity.Rotation = PositionHelper.GetVector3ToQuaternion(this.monsterEntity.Position, roleEntity.Position);

                   // var pos = roleEntity.Game_Object.transform.parent.position;
                    var pos = roleEntity.roleTrs.position;
                    if (this.monsterEntity.Game_Object != null)
                    {
                        pos.y = this.monsterEntity.Game_Object.transform.position.y;
                        this.monsterEntity.Game_Object.transform.LookAt(pos);
                    }
                }
                else
                {
                    var pos = entity.Game_Object.transform.position;
                    //Log.DebugBrown($"{this.monsterEntity.MonsterName}");
                    if (this.monsterEntity.Game_Object != null)
                    {
                        pos.y = this.monsterEntity.Game_Object.transform.position.y;
                        this.monsterEntity.Game_Object.transform.LookAt(pos);
                    }
                }
            }
            if (monsterEntity.IsDead == false)
            {
                SetTrigger(MotionType.Attack);//播放 攻击动画
            }
           /* if(!monsterEntity.GetComponent<AnimatorComponent>().GetBoolParameterValue(MotionType.Dead))
                SetTrigger(MotionType.Attack);//播放 攻击动画*/

          //  PlayAttackSound();//播放攻击 音效
        }

        public void Dead()
        {
            
            if (!string.IsNullOrEmpty(monsterEntity.Sound_Dead))
                SoundComponent.Instance.PlayClip(monsterEntity.Sound_Dead, monsterEntity.Position);//播放死亡音效

            //SetTrigger(MotionType.Dead);//播放死亡动画 
            if(!GetBoolParameterValue(MotionType.Dead))
                SetBoolValue(MotionType.Dead,true);//播放死亡动画 

        }

        /// <summary>
        /// 播放待机音效
        /// </summary>
        public void UpdateIdleSound()
        {
            if (gameSetInfo != null && gameSetInfo.CloseSound) return; ///关闭音效
            if (Time.deltaTime % spaceTime == 0) //间隔 spaceTime 播放一次 待机音效
            {
                SoundComponent.Instance.PlayClip(monsterEntity.Sound_Idle, monsterEntity.Position);
            }
        }

    }
}