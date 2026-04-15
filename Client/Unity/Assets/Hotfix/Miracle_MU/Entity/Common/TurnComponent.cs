using UnityEngine;
using ETModel;


namespace ETHotfix
{
	[ObjectSystem]
	public class TurnComponentUpdateSystem : UpdateSystem<TurnComponent>
	{
		public override void Update(TurnComponent self)
		{
			self.Update();
		}
	}

	//[ObjectSystem]
	public class TurnComponentAwake : AwakeSystem<TurnComponent>
	{
		public override void Awake(TurnComponent self)
		{
			self.unitEntity = self.GetParent<UnitEntity>();
		}
	}
	/// <summary>
	/// 改编实体的朝向
	/// </summary>
	public class TurnComponent : Component
	{
		// turn
		public Quaternion To;
		public Quaternion From;
		public float t = float.MaxValue;
		public float TurnTime = 0.1f;

		public UnitEntity unitEntity;
		public void Update()
		{
			UpdateTurn();
		}

		private void UpdateTurn()
		{
			
			if (this.t > this.TurnTime)
			{
				return;
			}

			this.t += Time.deltaTime;

			Quaternion v = Quaternion.Slerp(this.From, this.To, this.t / this.TurnTime);
            if (this.GetParent<UnitEntity>() is RoleEntity roleEntity)
            {
                roleEntity.Rotation = v;
            }
            else
            {
                this.GetParent<UnitEntity>().Rotation = v;
            }
        }

		/// <summary>
		/// 改变UnitEntity的朝向
		/// </summary>
		public void Turn2D(Vector3 dir, float turnTime = 0.1f)
		{
			Vector3 nexpos = this.GetParent<UnitEntity>().Game_Object.transform.position + dir;
			Turn(nexpos, turnTime);
		}

		/// <summary>
		/// 改变UnitEntity的朝向
		/// </summary>
		public void Turn(Vector3 target, float turnTime = 0.1f)
		{
			if(this.GetParent<UnitEntity>()!=null){
				Quaternion quaternion = PositionHelper.GetVector3ToQuaternion(this.GetParent<UnitEntity>().Position, target);
				this.To = quaternion;
				this.From = this.GetParent<UnitEntity>().Rotation;
				this.t = 0;
				this.TurnTime = turnTime;
			}
		}
		public void Turn(AstarNode node)
		{
            if (this.GetParent<RoleEntity>() is RoleEntity roleEntity)
			{
                roleEntity.Rotation = PositionHelper.GetVector3ToQuaternion(roleEntity.CurrentNodePos, node);

            }
			else
			{
                this.GetParent<UnitEntity>().Rotation = PositionHelper.GetVector3ToQuaternion(this.GetParent<UnitEntity>().CurrentNodePos, node); ;
            }
        }
		/// <summary>
		/// 改变UnitEntity的朝向
		/// </summary>
		/// <param name="angle">与X轴正方向的夹角</param>
		public void Turn(float angle, float turnTime = 0.1f)
		{
			Quaternion quaternion = PositionHelper.GetAngleToQuaternion(angle);

			this.To = quaternion;
			this.From = this.GetParent<UnitEntity>().Rotation;
			this.t = 0;
			this.TurnTime = turnTime;
		}

		public void Turn(Quaternion quaternion, float turnTime = 0.1f)
		{
			this.To = quaternion;
			this.From = this.GetParent<UnitEntity>().Rotation;
			this.t = 0;
			this.TurnTime = turnTime;
		}

		public void TurnImmediately(Quaternion quaternion)
		{
			this.GetParent<UnitEntity>().Rotation = quaternion;
		}

		public void TurnImmediately(Vector3 target)
		{
			Vector3 nowPos = this.GetParent<UnitEntity>().Position;
			if (nowPos == target)
			{
				return;
			}

			Quaternion quaternion = PositionHelper.GetVector3ToQuaternion(this.GetParent<UnitEntity>().Position, target);
			this.GetParent<UnitEntity>().Rotation = quaternion;
		}

		public void TurnImmediately(float angle)
		{
			Quaternion quaternion = PositionHelper.GetAngleToQuaternion(angle);
			this.GetParent<UnitEntity>().Rotation = quaternion;
		}

		public override void Dispose()
		{
			if (this.IsDisposed)
			{
				return;
			}
			base.Dispose();
		}
	}
}