using ETModel;
using System.Reflection;
using UnityEngine;


namespace ETHotfix
{
    public static class AnimatorCondition
    {
        public static bool IsSwing = true;
    }
    /// <summary>
    /// 角色动画控制组件
    /// </summary>
    public partial class AnimatorComponent
    {
        /// <summary>
        /// 走路
        /// </summary>
        public void Walk() 
        {
            if (SceneComponent.Instance.CurrentSceneName == "YaTeLanDiSi" && !GetBoolParameterValue(MotionType.IsMount.ToString()))
            {
                if (unitEntity is RoleEntity roleEntity)
                {
                    if (roleEntity.IsSafetyZone == false)
                    {
                        AnimatorCondition.IsSwing = true;
                        UnitEntityComponent.Instance.LocalRole.GetComponent<RoleEquipmentComponent>().EnterSafeArea(true);
                        Swim();
                    }
                    else
                    {
                        if (GetBoolParameterValue(MotionType.IsSwim.ToString()))
                        {
                            SetBoolValue(MotionType.IsSwim.ToString(), false);
                        }
                        if (!GetBoolParameterValue(MotionType.IsMove.ToString()))///播放 走路 
                            SetBoolValue(MotionType.IsMove.ToString(), true);
                    }
                    return;
                }

            }
            //拥有坐骑
           /* if (GetBoolParameterValue(MotionType.IsMount.ToString()) && this.unitEntity.Game_Object.transform.localPosition.z == .5)
            {
                this.unitEntity.Game_Object.transform.localPosition += Vector3.forward * -.75f;
            }*/
            if (GetBoolParameterValue(MotionType.Stand.ToString()))///关闭 靠墙
            {
                SetBoolValue(MotionType.Stand.ToString(), false);
            } 

            if (GetBoolParameterValue(MotionType.IsRun.ToString()))///关闭 奔跑
            {
                SetBoolValue(MotionType.IsRun.ToString(), false);
            }
            if (GetBoolParameterValue(MotionType.IsSwim.ToString()))///播放 奔跑
            {
                SetBoolValue(MotionType.IsSwim.ToString(), false);
            }
            if (!GetBoolParameterValue(MotionType.IsMove.ToString()))///播放 走路 
            {
                SetBoolValue(MotionType.IsMove.ToString(), true);
            }
         
        }
        /// <summary>
        /// 奔跑
        /// </summary>
        public void Run()
        {
            
            if (SceneComponent.Instance.CurrentSceneName == SceneName.YaTeLanDiSi.ToString() && !GetBoolParameterValue(MotionType.IsMount.ToString()))
            {
                if (unitEntity is RoleEntity roleEntity)
                {
                    if (roleEntity.IsSafetyZone == false)
                    {
                        AnimatorCondition.IsSwing = true;
                        UnitEntityComponent.Instance.LocalRole.GetComponent<RoleEquipmentComponent>().EnterSafeArea(true);
                        Swim();
                    }
                }
                return;
            }
            //拥有坐骑
           /* if (GetBoolParameterValue(MotionType.IsMount.ToString())&& this.unitEntity.Game_Object.transform.localPosition.z!=0.5f)
            {
                this.unitEntity.Game_Object.transform.localPosition += Vector3.forward * .75f;
            }*/

            if (GetBoolParameterValue(MotionType.Stand.ToString()))///关闭 靠墙
            {
                SetBoolValue(MotionType.Stand.ToString(), false);
            }

            if (GetBoolParameterValue(MotionType.IsMove.ToString()))///关闭 走路
            {
                SetBoolValue(MotionType.IsMove.ToString(), false);
            }
            if (GetBoolParameterValue(MotionType.IsSwim.ToString()))///播放 奔跑
            {
                SetBoolValue(MotionType.IsSwim.ToString(), false);
            }
            if (!GetBoolParameterValue(MotionType.IsRun.ToString()))///播放 奔跑
            {
                SetBoolValue(MotionType.IsRun.ToString(), true);
                SetBoolValue(MotionType.IsMove.ToString(), false);
            }
        }
      
        /// <summary>
        /// 待机
        /// </summary>
        public void Idle()
        {
            if (SceneComponent.Instance.CurrentSceneName == "YaTeLanDiSi" && !GetBoolParameterValue("IsMount") && AnimatorCondition.IsSwing)
            {
                if (unitEntity is RoleEntity roleEntity)
                {
                    if (roleEntity.IsSafetyZone == false)
                    {
                        UnitEntityComponent.Instance.LocalRole.GetComponent<RoleEquipmentComponent>().EnterSafeArea(true);
                        Fly();
                       
                        return;
                    }
                }

            }

          
            //拥有坐骑
           /* if (GetBoolParameterValue(MotionType.IsMount.ToString()) && this.unitEntity.Game_Object.transform.localPosition.z == .5)
            {
                this.unitEntity.Game_Object.transform.localPosition += -Vector3.forward * .75f;

            }*/
            if (GetBoolParameterValue(MotionType.IsSwim.ToString()))
            {
                SetBoolValue(MotionType.IsSwim.ToString(), false);
            }
            if (GetBoolParameterValue(MotionType.IsMove.ToString()))///关闭 走路
            {
                SetBoolValue(MotionType.IsMove.ToString(), false);
            }
            if (GetBoolParameterValue(MotionType.IsRun.ToString()))///关闭 奔跑
            { 
                SetBoolValue(MotionType.IsRun.ToString(), false); 
            }
            if (GetBoolParameterValue(MotionType.IsSwimIdle.ToString()))///关闭 水底待机
            {
                SetBoolValue(MotionType.IsSwimIdle.ToString(), false);
            }

        }

        public void ChangeAnimationLayerWeight(float weight)
        {
            this.Animator.SetLayerWeight(3,weight);
            this.Animator.SetLayerWeight(4,weight);
        }

        public void Swim()
        {

            if (GetBoolParameterValue(MotionType.IsRun.ToString()))///关闭 奔跑
            {
                SetBoolValue(MotionType.IsRun.ToString(), false);
            }
            if (GetBoolParameterValue(MotionType.IsMove.ToString()))///播放 奔跑
            {
                SetBoolValue(MotionType.IsMove.ToString(), false);
            }
            if (!GetBoolParameterValue(MotionType.IsSwim.ToString()))///播放 奔跑
            {
                SetBoolValue(MotionType.IsSwim.ToString(), true);
                SetBoolValue(MotionType.IsSwimIdle.ToString(), false);
            }
        }


        public void Fly()
        {
            if (GetBoolParameterValue(MotionType.IsRun))///关闭 奔跑
            {
                SetBoolValue(MotionType.IsRun, false);
            }
            if (GetBoolParameterValue(MotionType.IsMove.ToString()))///播放 奔跑
            {
                SetBoolValue(MotionType.IsMove.ToString(), false);
            }
            if (!GetBoolParameterValue(MotionType.IsSwimIdle.ToString()))///播放 奔跑
            {
                SetBoolValue(MotionType.IsSwimIdle.ToString(), true);
            }
        }

    }
}
