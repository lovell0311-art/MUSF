using DG.Tweening;
using ETModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace ETHotfix
{
	[ObjectSystem]
    public class UnitEntityMoveComponentUpdate : UpdateSystem<UnitEntityMoveComponent>
    {
        public override void Update(UnitEntityMoveComponent self)
        {
			self.Update();
		}
    }
	[ObjectSystem]
    public class UnitEntityMoveComponentAwake : AwakeSystem<UnitEntityMoveComponent>
    {
        public override void Awake(UnitEntityMoveComponent self)
        {
			self.Awake();
        }
    }
  
    /// <summary>
    /// 实体移动组件 
    /// </summary>
    public class UnitEntityMoveComponent : Component
    {
		public Vector3 Target;
		// 开启移动协程的时间
		public long StartTime;

        // 开启移动协程的Unit的位置
        public Vector3 StartPos;

        public long needTime;//毫秒

		public UnitEntity unitEntity;

		public Action Callback;
		/// <summary>正在移动</summary>
		public bool Moving => StartTime != 0;


        public void Awake() 
		{
			unitEntity = GetParent<UnitEntity>();
		}
		long timeNow;
		float amount;
        public void Update()
		{
			if (this.StartTime == 0)
			{
				return;
			}
			//long timeNow = TimeHelper.Now();
			timeNow = TimeHelper.Now();
            if (timeNow - this.StartTime >= this.needTime)
            {
                unitEntity.Position = this.Target;

				Action callback = Callback;
				Callback = null;

				callback?.Invoke();
                return;
            }
			//this.unitEntity.GetComponent<AnimatorComponent>().Walk();
            //	float amount = (timeNow - this.StartTime) * 1f / this.needTime;
            amount = (timeNow - this.StartTime) * 1f / this.needTime;
	    	unitEntity.Position = Vector3.Lerp(this.StartPos, this.Target, amount);
			
		
		}
		public ETTask MoveToAsync(Vector3 target, float speedValue, CancellationToken cancellationToken)
		{
			/*if(this.unitEntity is RoleEntity)
			Log.DebugRed($"{this.unitEntity.GetType()} : {this.unitEntity.Id} : {this.unitEntity.CurrentNodePos}: 请求移动到：{AstarComponent.Instance.GetNodeVector(target)}  curpos:{this.unitEntity.Game_Object.transform.transform.position} this.unitEntity.CurrentNodePos: {this.unitEntity.CurrentNodePos}");*/

			if ((target - this.Target).magnitude < 0.1f)
			{
				return ETTask.CompletedTask;
			}
			this.Target = target;
			if(this.unitEntity is RoleEntity role)
                this.StartPos = role.Position;
			else
            this.StartPos = unitEntity.Position;

			this.StartTime = TimeHelper.Now();

		

			float distance = (this.Target - this.StartPos).magnitude;
			if (Math.Abs(distance) < 0.1f)
			{
				return ETTask.CompletedTask;
			}
			
			this.needTime = (long)(distance / speedValue * 1000);

            if (this.unitEntity is RoleEntity localRole && localRole.Id == UnitEntityComponent.Instance.LocaRoleUUID)
            {
                Log.DebugGreen($"MoveToAsync start from={this.StartPos.x:F1},{this.StartPos.y:F1},{this.StartPos.z:F1} to={this.Target.x:F1},{this.Target.y:F1},{this.Target.z:F1} distance={distance:F2} speed={speedValue:F2} needTime={this.needTime}");
            }

			var tcs = new ETTaskCompletionSource();
			this.Callback = () =>
			{
				//Debug.Log("Result");
                this.StartTime = 0;
                if (this.unitEntity is RoleEntity localRole && localRole.Id == UnitEntityComponent.Instance.LocaRoleUUID)
                {
                    Log.DebugGreen($"MoveToAsync complete at={this.unitEntity.Position.x:F1},{this.unitEntity.Position.y:F1},{this.unitEntity.Position.z:F1}");
                }
				tcs.SetResult();
			};
			cancellationToken.Register(() => {
				//Debug.Log("取消移动");
				this.Stop(); 
			});

            return tcs.Task;
		}

		public void Stop()
		{
			StartTime = 0;
            if (this.unitEntity is RoleEntity localRole && localRole.Id == UnitEntityComponent.Instance.LocaRoleUUID)
            {
                Log.DebugGreen($"MoveToAsync stop at={this.unitEntity.Position.x:F1},{this.unitEntity.Position.y:F1},{this.unitEntity.Position.z:F1}");
            }
			Action cb = Callback;
			Callback = null;
			cb?.Invoke();
        }

        public override void Dispose()
        {
			if (this.IsDisposed)
				return;

            base.Dispose();
            unitEntity.Position = this.Target;
			Stop();
        }

    }
}
