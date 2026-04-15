using ETModel;
using System.Collections;
using System.Collections.Generic;

namespace ETHotfix
{
    [MessageHandler]
    public class G2C_WarAllianceAppointmentNotice_Handler : AMHandler<G2C_WarAllianceAppointmentNotice>
    {
        protected override void Run(ETModel.Session session, G2C_WarAllianceAppointmentNotice message)
        {
           
            UIComponent.Instance.Get(UIType.UIWarAlliance)?.GetComponent<UIWarAllianceComponent>()?.WarBeAppointed(message.ClassType,message.CharName);

            UIComponent.Instance.VisibleUI(UIType.UIHint, $"[{message.CharName}]已被任命为[{GetPos(message.ClassType)}]");
            string GetPos(int post) => post switch
            {
                0 => "成员",
                1 => "小队长",
                2 => "副盟主",
                3 => "盟主",
                _ => "成员"
            };
        }
    }
}

