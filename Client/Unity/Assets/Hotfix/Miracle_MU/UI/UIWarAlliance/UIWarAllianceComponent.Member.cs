using System.Collections.Generic;
using UnityEngine;
using ETModel;

using UnityEngine.UI;


namespace ETHotfix
{

    /// <summary>
    /// 战盟成员 信息类
    /// </summary>
    public class WarMemberInfo
    {
        public string Name;//昵称
        public int Lev;//等级
        public int Post;//职位 0成员 1小队长 2副盟主 3盟主
        public int State;//状态 0 离线 1 在线
        public long UUID;//UUID


    }
    /// <summary>
    /// 战盟 成员模块
    /// </summary>
    public partial class UIWarAllianceComponent
    {

        List<WarMemberInfo> warMemberInfos = new List<WarMemberInfo>();
        public ScrollRect memberScrolView;
        Transform memberContent;

        ReferenceCollector collector_Member;
        public UICircularScrollView<WarMemberInfo> WarMemberInfoScrollView;

        public WarMemberInfo CurChooseWarMemberInfo = null;//当前选择的 成员信息
        public Transform CurChooseWarMemberTrs = null;//当前选择的 成员信息
        public List<Transform> CurChooseWarMemberTrsAll = new List<Transform>();//当前选择的 成员信息
        public Button AppointBtn, RemoveBtn, KichedOutBtn;
        public void Init_Member()
        {
            collector_Member = Members.GetReferenceCollector();
            memberScrolView = collector_Member.GetImage("MemberScrollView").GetComponent<ScrollRect>();
            memberContent = collector_Member.GetGameObject("Content").transform;

            AppointBtn = collector_Member.GetButton("AppointBtn");//任命
            RemoveBtn = collector_Member.GetButton("RemoveBtn");//撤职
            KichedOutBtn = collector_Member.GetButton("KichedOutBtn");//踢出
            ////任命
            AppointBtn.onClick.AddSingleListener(() =>
            {
                if (CurChooseWarMemberInfo == null)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "请您先选择一个成员");
                    return;
                }
                ShoePostSetPanel();
            });
            /////撤职
            RemoveBtn.onClick.AddSingleListener(() =>
            {
                if (CurChooseWarMemberInfo == null)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "请您先选择一个成员");
                }
                else if (CurChooseWarMemberInfo.Post == 3)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "不可撤销盟主的职务");
                    
                }
                else
                {
                    Isremoved = true;
                    UIConfirmComponent uIConfirmComponent = UIConfirmComponentExtend.GetUIConfirmComponent();
                    uIConfirmComponent.SetTipText($" 确定撤职：<color=red>[{CurChooseWarMemberInfo.Name}]</color>的[{GetPos(CurChooseWarMemberInfo.Post)}]职务？");
                    uIConfirmComponent.AddActionEvent(() =>
                    {
                        //放生
                        WarAllianceAppointmentAsync(CurChooseWarMemberInfo.UUID, 0).Coroutine();
                    });
                    
                }
            });
            ////踢出 战盟
            KichedOutBtn.onClick.AddSingleListener(() =>
            {
                if (CurChooseWarMemberInfo == null) return;
                if (CurChooseWarMemberInfo.Post == 3)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "不可踢出盟主");

                }
                else if (CurChooseWarMemberInfo.Post == 2)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "不可踢出副盟主");

                }
                else if (CurChooseWarMemberInfo.UUID == UnitEntityComponent.Instance.LocaRoleUUID)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "不可踢出自己");
                }
                else
                {
                    UIConfirmComponent uIConfirmComponent = UIConfirmComponentExtend.GetUIConfirmComponent();
                    uIConfirmComponent.SetTipText($" 确定将玩家[{CurChooseWarMemberInfo.Name}]踢出战盟？");
                    uIConfirmComponent.AddActionEvent(() =>
                    {
                        WarAlliancePropose().Coroutine();
                    });
                }
            });
            Init_WarMemberInfoScrollView();
        }

        public void Init_WarMemberInfoScrollView()
        {
            WarMemberInfoScrollView = ComponentFactory.Create<UICircularScrollView<WarMemberInfo>>();
            WarMemberInfoScrollView.ItemInfoCallBack = InitWarMemberCallBack;
            WarMemberInfoScrollView.InitInfo(E_Direction.Vertical, 1, 0, 10);
            WarMemberInfoScrollView.IninContent(memberContent.gameObject, memberScrolView);
        }

        private void InitWarMemberCallBack(GameObject go, WarMemberInfo memberInfo)
        {
            //在线 为白色 不在先为灰色
            string colorstr = memberInfo.State == 0 ? ColorTools.GetColorHtmlString(Color.white) : ColorTools.GetColorHtmlString(Color.gray);
            go.transform.Find("Lev").GetComponent<Text>().text = $"<color={colorstr}>{memberInfo.Lev}</color>";
            go.transform.Find("Name").GetComponent<Text>().text = $"<color={colorstr}>{memberInfo.Name}</color>"; ;
            go.transform.Find("Lev").GetComponent<Text>().text = $"<color={colorstr}>{memberInfo.Lev}</color>";
            go.transform.Find("Job").GetComponent<Text>().text = $"<color={colorstr}>{GetPos(memberInfo.Post)}</color>";
            go.transform.Find("State").GetComponent<Text>().text = memberInfo.State == 1 ? $"<color={colorstr}>离线</color>" : $"<color={colorstr}>在线</color>";
            Toggle toggle = go.transform.GetComponent<Toggle>();
            toggle.onValueChanged.AddSingleListener((value) =>
            {
                if (value)
                {
                    CurChooseWarMemberInfo = memberInfo;
                    CurChooseWarMemberTrs = go.transform;
                }
            });
            if (!CurChooseWarMemberTrsAll.Contains(go.transform))
            {
                CurChooseWarMemberTrsAll.Add(go.transform);
            }
            toggle.isOn = CurChooseWarMemberInfo != null && CurChooseWarMemberInfo.UUID == memberInfo.UUID;//当前选择的成员

            if (memberInfo.UUID == UnitEntityComponent.Instance.LocaRoleUUID)
            {
                //本地玩家
                AppointBtn.gameObject.SetActive(memberInfo.Post == 3);
                RemoveBtn.gameObject.SetActive(memberInfo.Post == 3);
                KichedOutBtn.gameObject.SetActive(memberInfo.Post == 3 || memberInfo.Post == 2);
            }
        }
        /// <summary>
        /// 根据 职务id 获取对应的职位名
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        public string GetPos(int post) => post switch
        {
            0 => "成员",
            1 => "小队长",
            2 => "副盟主",
            3 => "盟主",
            _ => "成员"
        };
        /// <summary>
        /// 根据职位 设置对应的权限
        /// </summary>
        /// <param name="Post">职位 编号 
        /// 0->成员
        /// 1->小队长
        /// 2->副盟主
        /// 3->盟主
        /// </param>
        private void SetPermissionsByPost(int Post)
        {
            AppointBtn.gameObject.SetActive(Post == 3);
            RemoveBtn.gameObject.SetActive(Post == 3);
            KichedOutBtn.gameObject.SetActive(Post == 3 || Post == 2);
            switch (Post)
            {
                case 0:
                   
                    break;
                default:
                    break;
            }
        }

    }
}
