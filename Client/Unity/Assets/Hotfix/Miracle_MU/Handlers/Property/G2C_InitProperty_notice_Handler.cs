using ETModel;

namespace ETHotfix
{
    /// <summary>
    /// 属性通知
    /// </summary>
    [MessageHandler]
    public class G2C_InitProperty_notice_Handler : AMHandler<G2C_InitProperty_notice>
    {
        protected override void Run(ETModel.Session session, G2C_InitProperty_notice message)
        {
            
            UnitEntityComponent.Instance.LocalRole.Property.ChangeProperValue(E_GameProperty.PROP_AG,message.AG);
            UnitEntityComponent.Instance.LocalRole.Property.ChangeProperValue(E_GameProperty.PROP_AG_MAX, message.AG);
            UnitEntityComponent.Instance.LocalRole.Property.ChangeProperValue(E_GameProperty.PROP_SD, message.SD);
            UnitEntityComponent.Instance.LocalRole.Property.ChangeProperValue(E_GameProperty.PROP_SD_MAX, message.SDMax);
            UnitEntityComponent.Instance.LocalRole.Property.ChangeProperValue(E_GameProperty.PROP_HP, message.Hp);
            UnitEntityComponent.Instance.LocalRole.Property.ChangeProperValue(E_GameProperty.PROP_HP_MAX, message.HpMax);
            UnitEntityComponent.Instance.LocalRole.Property.ChangeProperValue(E_GameProperty.PROP_MP, message.Mp);
            UnitEntityComponent.Instance.LocalRole.Property.ChangeProperValue(E_GameProperty.PROP_MP_MAX, message.MpMax);

            UIMainComponent.Instance.ChangeRoleHp();//改变玩家HP
            UIMainComponent.Instance.ChangeRoleMp();
            UIMainComponent.Instance.ChangeAG();
            UIMainComponent.Instance.ChangeSD();

        }

    }
}
