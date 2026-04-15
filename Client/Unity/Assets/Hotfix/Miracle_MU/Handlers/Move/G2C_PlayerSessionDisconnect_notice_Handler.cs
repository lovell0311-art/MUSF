using ETModel;
namespace ETHotfix
{
    /// <summary>
    /// 玩家退出通知
    /// </summary>
    [MessageHandler]
    public class G2C_PlayerSessionDisconnect_notice_Handler : AMHandler<G2C_PlayerSessionDisconnect_notice>
    {
        protected override void Run(ETModel.Session session, G2C_PlayerSessionDisconnect_notice message)
        {
            if (UnitEntityComponent.Instance.Get<RoleEntity>(message.InstanceId) is RoleEntity roleEntity)
            {
                if (UIMainComponent.Instance.IsChangeLine && message.InstanceId == UnitEntityComponent.Instance.LocaRoleUUID)
                {
                 //   Log.DebugBrown($"本地玩家切换线路");
                    return;
                }
               
              //  Log.DebugBrown($"玩家 {message.InstanceId}  {roleEntity.RoleName} 已经下线");
                roleEntity.Dispose();
            }
               
            if (UnitEntityComponent.Instance.Get<UnitEntity>(message.PetsID) is PetEntity petEntity)
            {
               // Log.DebugBrown($"宠物 {message.InstanceId}  {petEntity.name} 已经下线");
                petEntity.Dispose();
            }
        }
    }
}
