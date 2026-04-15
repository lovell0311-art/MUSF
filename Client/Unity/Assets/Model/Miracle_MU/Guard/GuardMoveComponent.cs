

using System.Collections.Generic;
using UnityEngine;

namespace ETModel
{
    /// <summary>
    /// 守护移动组件
    /// </summary>
    public class GuardMoveComponent : MonoBehaviour
    {
        /// <summary>
        /// 移动跟随目标
        /// </summary>
        public GameObject followTarget;
        /// <summary>
        /// 偏移量
        /// </summary>
        public Vector3 offset;
        /// <summary>
        /// 向右的距离
        /// </summary>
        public float rightDistance = 1;
        //高度
        public float topDistance = 1;


        public float speed = 1;//移动速度

        public bool IsMove = false;

        Animator animator;

        public float idleTime = 0;
        public float idleUpdateTime = 20;//每隔8秒更新飞一下
        public HashSet<string> Parameter = new HashSet<string>();
        private void Awake()
        {
            animator = transform.Find("World").GetComponent<Animator>();
            if (animator == null) return;
            foreach (AnimatorControllerParameter animatorControllerParameter in animator.parameters)
            {
                this.Parameter.Add(animatorControllerParameter.name);
            }
        }
        private void Start()
        {
            transform.SetPositionAndRotation(followTarget.transform.position, followTarget.transform.rotation);
        }
        public bool HasParameter(string parameter)
        {
            return this.Parameter.Contains(parameter);
        }
        public void SetFollowGameObj(GameObject obj, float speed = 1)
        {
            followTarget = obj;
            this.speed = speed;
        }

        private void LateUpdate()
        {
            //设置偏移量 
            offset = followTarget.transform.right * rightDistance + followTarget.transform.up * topDistance;

            if (IsMove)
            {
                transform.SetPositionAndRotation(Vector3.Lerp(transform.position, followTarget.transform.position + offset, Time.deltaTime * speed), followTarget.transform.rotation);
            }

            //使用插值 让守护有一个平滑的移动
            transform.SetPositionAndRotation(Vector3.Lerp(transform.position, followTarget.transform.position + offset, Time.deltaTime * speed), followTarget.transform.rotation);
            IsMove = Vector3.Distance(followTarget.transform.position + offset, transform.position) > 3f;
        }

        private void Update()
        {

            if (IsMove) return;

            if (!this.HasParameter("fly"))
            {
                return;
            }
            transform.rotation = followTarget.transform.rotation;
            //每隔8秒 播放一次 飞行动画
            idleTime += Time.deltaTime;
            if (idleTime >= idleUpdateTime)
            {
                idleTime = 0;
                animator.SetTrigger("fly");
            }

        }
    }
}