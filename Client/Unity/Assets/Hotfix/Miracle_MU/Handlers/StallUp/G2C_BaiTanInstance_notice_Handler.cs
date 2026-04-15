using ETModel;

namespace ETHotfix
{
    /// <summary>
    /// ¸½½ü°ÚÌ¯Êý¾Ý
    /// </summary>
    [MessageHandler]
    public class G2C_BaiTanInstance_notice_Handler : AMHandler<G2C_BaiTanInstance_notice>
    {
        protected override void Run(ETModel.Session session, G2C_BaiTanInstance_notice message)
        {
         
            for (int i = 0, length = message.Prop.Count; i < length; i++)
            {
               
                var stallUpInfo=message.Prop[i];
                
                if (UnitEntityComponent.Instance.Get<RoleEntity>(stallUpInfo.BaiTanInstanceId)  is UnitEntity role)
                {
                  
                    if (role == UnitEntityComponent.Instance.LocalRole)
                    {
                       
                        role.GetComponent<RoleStallUpComponent>().curStallUpName = stallUpInfo.BaiTanName;
                    }
                    else
                    {
                     
                        role.GetComponent<RoleStallUpComponent>().StartStallUp(stallUpInfo.BaiTanName);
                   
                    }
                }
            }
        }
    }
}
