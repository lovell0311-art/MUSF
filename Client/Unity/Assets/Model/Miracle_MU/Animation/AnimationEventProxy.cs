using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ETModel
{

    /// <summary>
    /// 动画事件 管理
    /// </summary>
    public class AnimationEventProxy : MonoBehaviour
    {
        /// <summary>
        /// 技能特效
        /// </summary>
        public Action Effect_action;
        /// <summary>
        /// 普攻特效
        /// </summary>
        public Action Attack_action;
        /// <summary>
        /// 走了音效
        /// </summary>
        public Action Sound_action;

        public Action<String> Effect_Action_Str;
        public Action<String> Monster_DeadEffect_action;
        public Action<GameObject> Pet_Effect_action;
        public void Effect() 
        {

            Effect_action?.Invoke();
           
        }
        public void Effect_Obj(GameObject obj)
        {
            Pet_Effect_action?.Invoke(obj);
        }
        public void Effect_Str(string effectName)
        {
         
            Effect_Action_Str?.Invoke(effectName);
           
        }
        
        public void DeadEffect_Str(string effectName)
        {
          
            Monster_DeadEffect_action?.Invoke(effectName);
            //Log.DebugWhtie($"怪物动画事件 :{effectName}");
        }
        public void AtkEffect()
        {

            Attack_action?.Invoke();
        }
        public void Sound()
        {
            Sound_action?.Invoke();
        }
        public void ZouLuSound()
        {
            Sound_action?.Invoke();
            
        }
    }
}