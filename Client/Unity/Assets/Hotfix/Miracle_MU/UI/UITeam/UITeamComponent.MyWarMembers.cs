using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
namespace ETHotfix
{

    /// <summary>
    /// 我的战盟成员
    /// </summary>
    public partial class UITeamComponent
    {

        /// <summary>
        /// 初始化 战盟 成员
        /// </summary>
        public void Init_MyWarMembers() 
        {
            if (string.IsNullOrEmpty(UnitEntityComponent.Instance.LocalRole.unionName))
            {
                NearPlayersScrollView.Items = null;
                UIComponent.Instance.VisibleUI(UIType.UIHint,"你还未加入战盟");
                return;
            }

            List<OtherPlayerInfo> MyWarMembersList = new List<OtherPlayerInfo>();
            GetMyWarMembers().Coroutine();



            ///获取专门 成员
            async ETVoid GetMyWarMembers() 
            {
                G2C_OpenMemberListResponse g2C_OpenMember = (G2C_OpenMemberListResponse)await SessionComponent.Instance.Session.Call(new C2G_OpenMemberListRequest 
                {
                 Type=0
                });
                if (g2C_OpenMember.Error != 0)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_OpenMember.Error.GetTipInfo());
                }
                else
                {
                    MyWarMembersList.Clear();
                    foreach (var warmember in g2C_OpenMember.MemberList)
                    {
                        MyWarMembersList.Add(new OtherPlayerInfo 
                        {
                            roleUUId = warmember.GameUserID,
                            roleName = warmember.MemberName,
                            roleLev = warmember.MemberLevel,
                            roleType = warmember.MemberClassType,
                            warName ="无",
                            isInvite = false
                        });
                    }

                    MyWarMembersList.Sort((m1, m2) =>
                    {
                        return m2.roleLev.CompareTo(m1.roleLev);
                    });
                    NearPlayersScrollView.Items = MyWarMembersList;
                }
            }
        }
    }
}
