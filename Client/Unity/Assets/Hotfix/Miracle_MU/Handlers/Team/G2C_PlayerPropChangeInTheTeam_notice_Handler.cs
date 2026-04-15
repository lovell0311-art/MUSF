using ETModel;
namespace ETHotfix
{
    /// <summary>
    /// 通知 队伍中实体的属性变动
    /// </summary>
    [MessageHandler]
    public class G2C_PlayerPropChangeInTheTeam_notice_Handler : AMHandler<G2C_PlayerPropChangeInTheTeam_notice>
    {
        protected override void Run(ETModel.Session session, G2C_PlayerPropChangeInTheTeam_notice message)
        {
            if (TeamDatas.OtherTeamMemberStatusList.Exists(r => r.GameUserId == message.ChangedPlayerGameUserId))
            {
                var palyer = TeamDatas.OtherTeamMemberStatusList.Find(r => r.GameUserId == message.ChangedPlayerGameUserId);
                TeamMateProperty teamMateProperty = new TeamMateProperty 
                {
                 HP=message.ChangedPlayerStatus.HP,
                 HPMax=message.ChangedPlayerStatus.HPMax,
                 MP=message.ChangedPlayerStatus.MP,
                 MPMax=message.ChangedPlayerStatus.MPMax,
                 Level=message.ChangedPlayerStatus.Level,
                };
                UIMainComponent.Instance.ChangeProperty(teamMateProperty, message.ChangedPlayerGameUserId);
            }
        }
    }
}