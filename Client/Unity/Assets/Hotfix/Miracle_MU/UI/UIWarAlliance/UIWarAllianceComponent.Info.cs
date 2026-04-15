using UnityEngine;
using ETModel;
using UnityEngine.UI;
using System.Text;

namespace ETHotfix
{

    /// <summary>
    /// 战盟 信息模块
    /// </summary>

    public partial class UIWarAllianceComponent
    {
        Text WarName_Info, WarMemberCoun, WarLeader;
        Transform badge_Info;
        ReferenceCollector collector_Info;
        InputField AnnoInfo;//战盟公告

        StringBuilder CurEditorAnnoInfoBuilder;//当前编辑的战盟公告

        bool isEditorAnno = false;//是否编辑过公告
        bool isLeader = false;//是否是盟主

        Button EditorAnnoBtn, DissolveBtn;
        public void Init_Info()
        {
            collector_Info = Info.GetReferenceCollector();
            WarName_Info = collector_Info.GetText("WarName");//战盟名称
            WarMemberCoun = collector_Info.GetText("WarCount");//战盟人数
            WarLeader = collector_Info.GetText("WarLeader");//战盟盟主
            badge_Info = collector_Info.GetGameObject("WarIcons").transform;//战盟徽章
            AnnoInfo = collector_Info.GetInputField("Anno_InputField");//战盟公告

            CurEditorAnnoInfoBuilder = new StringBuilder();
            //if (SystemUtil.IsInvaild(SendMessages))
            //修改战盟 公告
            AnnoInfo.onValueChanged.AddSingleListener((value) => 
            {
                if (SystemUtil.IsInvaild(value))
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "包含非法字符，请重新输入");
                    CurEditorAnnoInfoBuilder.Clear();
                    AnnoInfo.text = null;
                }
                isEditorAnno = true;
                CurEditorAnnoInfoBuilder.Clear();
                CurEditorAnnoInfoBuilder.Append(value);
            });
            //请求 修改 战盟公告
            EditorAnnoBtn = collector_Info.GetButton("EditorAnnoBtn");
            EditorAnnoBtn.onClick.AddSingleListener(() =>
            {
                if (isEditorAnno)
                {
                    ChangeAnnoAsync(CurEditorAnnoInfoBuilder.ToString()).Coroutine();
                }
            });
            //解散 战盟
            DissolveBtn = collector_Info.GetButton("DissolveBtn");
          
        }
        /// <summary>
        /// 初始化 战盟 信息
        /// </summary>
        /// <param name="warAllinceInfo">战盟信息 结构类</param>
        public void Init_Info(Struct_WarAllinceInfo warAllinceInfo,int count)
        {
            roleEntity.unionName = warAllinceInfo.WarAllianceName;//缓存战盟名
            roleEntity.unionPost = GetPos(warAllinceInfo.MemberPost);//缓存战盟名

            WarName_Info.text = warAllinceInfo.WarAllianceName;//战盟名称
            WarMemberCoun.text = $"人数：{count}";//战盟人数
            WarLeader.text = $"盟主：{warAllinceInfo.LeaderName}";//战盟盟主
            AnnoInfo.text = warAllinceInfo.WarAllianceNotice;//战盟公告
            Apply_isOn(warAllinceInfo.MemberPost);//根据职位是否显示申请列表
            isLeader = string.Equals(roleEntity.RoleName, warAllinceInfo.LeaderName);//是否是盟主
            if (!isLeader)//不是盟主
            {
                EditorAnnoBtn.gameObject.SetActive(false);
                AnnoInfo.interactable = false;
                DissolveBtn.transform.Find("Text").GetComponent<Text>().text = "退出";
                DissolveBtn.onClick.AddSingleListener(() =>
                {
                    UIConfirmComponent uIConfirm = UIConfirmComponentExtend.GetUIConfirmComponent();
                    uIConfirm.SetTipText("是否确定退出战盟？\n退出战盟后 2小时内不可加入战盟");
                    uIConfirm.AddActionEvent(() => { WarAllianceSignOut().Coroutine(); });

                });
            }
            else
            {
                DissolveBtn.transform.Find("Text").GetComponent<Text>().text = "解散";
                DissolveBtn.onClick.AddSingleListener(() =>
                {
                    UIConfirmComponent uIConfirm = UIConfirmComponentExtend.GetUIConfirmComponent();
                    uIConfirm.SetTipText("是否确定解散战盟？\n解除战盟后 24小时内不可建立战盟");
                    uIConfirm.AddActionEvent(() => { DisbandTheWarAsync().Coroutine(); });

                });
            }

            int chilidCount = badge_Info.childCount;
            Transform temp = badge_Info.GetChild(0);

            //初始化徽章
            for (int i = 0, length = warAllinceInfo.WarAllianceBadge.Count; i < length; i++)
            {
                Transform tr;
                if (i < chilidCount)
                {
                    tr = badge_Info.GetChild(i);
                }
                else
                {
                    tr = GameObject.Instantiate<Transform>(temp, badge_Info);
                }
                tr.GetComponent<Image>().enabled = warAllinceInfo.WarAllianceBadge[i] != 0;
                tr.GetComponent<Image>().sprite = GetSprite(warAllinceInfo.WarAllianceBadge[i]);

            }
            //显示战盟面板
            Show(E_WarType.WarMain);
        }
    }
}
