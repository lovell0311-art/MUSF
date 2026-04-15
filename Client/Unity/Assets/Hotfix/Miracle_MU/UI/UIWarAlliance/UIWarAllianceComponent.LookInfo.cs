using Codice.CM.Client.Differences;
using ETModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    public partial class UIWarAllianceComponent
    {
        Text WarName_LookInfo, WarMemberCounLook, WarLeaderLook;
        Transform badge_LookInfo;
        InputField AnnoInfoLook;//战盟公告
        GameObject LookInfo;
        ReferenceCollector collector_LookInfo;
        public void LookWarInfo()
        {
            LookInfo = collector_Look.GetReferenceCollector().GetGameObject("Info");
            collector_LookInfo = LookInfo.GetReferenceCollector();
            WarName_LookInfo = collector_LookInfo.GetText("WarName");//战盟名称
            WarMemberCounLook = collector_LookInfo.GetText("WarCount");//战盟人数
            WarLeaderLook = collector_LookInfo.GetText("WarLeader");//战盟盟主
            badge_LookInfo = collector_LookInfo.GetGameObject("WarIcons").transform;//战盟徽章
            AnnoInfoLook = collector_LookInfo.GetInputField("Anno_InputField");//战盟公告
            AnnoInfoLook.GetComponent<Image>().raycastTarget = false;
            collector_Look.GetReferenceCollector().GetButton("CloseBtn").onClick.AddSingleListener(() =>
            {
                collector_Look.gameObject.SetActive(false);
            });
            collector_LookInfo.GetButton("ApplyBtn").onClick.AddSingleListener(() =>
            {
                RequestAddWarAsync(warId).Coroutine();

                async ETVoid RequestAddWarAsync(long warUUID)
                {
                    G2C_AddWarAllianceResponse g2C_AddWar = (G2C_AddWarAllianceResponse)await SessionComponent.Instance.Session.Call(new C2G_AddWarAllianceRequest
                    {
                        WarAllianceID = warUUID
                    });
                    if (g2C_AddWar.Error != 0)
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_AddWar.Error.GetTipInfo());
                    }
                    else
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, "已申请");
                        collector_LookInfo.GetButton("ApplyBtn").transform.Find("Text").GetComponent<Text>().text = "审核中";
                        collector_LookInfo.GetButton("ApplyBtn").interactable = false;
                        if(curInfo != null)
                        {
                            curInfo.State = 1;
                        }
                    }
                }
            });
        }
        long warId = 0;
        WarInfo curInfo;
        public void Init_LookInfo(WarInfo info)
        {
            curInfo = info;
            int count = info.struct_WarAllince.Currentquantity;
            Struct_WarAllinceInfo warAllinceInfo = info.struct_WarAllince;
            warId = warAllinceInfo.WarAllianceID;
            WarName_LookInfo.text = $"战盟名称：{warAllinceInfo.WarAllianceName}";//战盟名称
            WarMemberCounLook.text = $"人数：{count}";//战盟人数
            WarLeaderLook.text = $"盟主：{warAllinceInfo.LeaderName}";//战盟盟主
            AnnoInfoLook.text = warAllinceInfo.WarAllianceNotice;//战盟公告

            int chilidCount = badge_LookInfo.childCount;
            Transform temp = badge_LookInfo.GetChild(0);

            collector_LookInfo.GetButton("ApplyBtn").transform.Find("Text").GetComponent<Text>().text = info.State == 1 ? "审核中" : "申请";
            collector_LookInfo.GetButton("ApplyBtn").interactable = info.State == 1 ? false : true;

            //初始化徽章
            for (int i = 0, length = warAllinceInfo.WarAllianceBadge.Count; i < length; i++)
            {
                Transform tr;
                if (i < chilidCount)
                {
                    tr = badge_LookInfo.GetChild(i);
                }
                else
                {
                    tr = GameObject.Instantiate<Transform>(temp, badge_LookInfo);
                }
                tr.GetComponent<Image>().enabled = warAllinceInfo.WarAllianceBadge[i] != 0;
                tr.GetComponent<Image>().sprite = GetSprite(warAllinceInfo.WarAllianceBadge[i]);

            }
            collector_Look.gameObject.SetActive(true);
        }
    }

}
