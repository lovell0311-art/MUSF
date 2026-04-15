
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ETModel
{   
   
    public class TriggerEvents : SimpleSingleton<TriggerEvents>
    {
        public Action<int> ChangeSceneAction;
        public List<long> allUUId = new List<long>();
        public long UUId = 0;

        public Action<Transform> RoleActionEnter;
        public Action<Transform> RoleActionLevea;
        public Action<Transform> RoleActionStay;
        public void AddChangeSceneAction(Action<int> action)
        {
            ChangeSceneAction += action;
        }
        //进入触发器回调函数
        public Action<Transform,long> Enteraction;
        /// <summary>
        /// 离开触发器 回调函数
        /// </summary>
        public Action<Transform, long> Leaveaction;
        /// <summary>
        /// 呆在其中
        /// </summary>
        public Action<Transform, long> Stayaction;

        /// <summary>
        /// 攻城战时 占地盘使用
        /// 根据Transform来获取 当前玩家实体数据
        /// string 为当前建筑物的名称
        /// </summary>
        public Action<string, Transform> entityEnter;
        public Action<string, Transform> entityStay;
        public Action<string, Transform> entityExit;
        public void AddEnteraction(Action<Transform, long> action,long uuid)
        {
            UUId = uuid;
            Enteraction += action;
        }
        public void AddLeaveaction(Action<Transform, long> action, long uuid)
        {
            UUId = uuid;
            Leaveaction += action;
        }
        public void RemoveEnteraction(Action<Transform, long> action, long uuid)
        {
            UUId = uuid;
            Enteraction -= action;
        }
        public void RemoveLeaveaction(Action<Transform, long> action, long uuid)
        {
            UUId = uuid;
            Leaveaction -= action;
        }
        public void AddStayaction(Action<Transform, long> action)
        {
            Stayaction += action;
        }
    }
}
