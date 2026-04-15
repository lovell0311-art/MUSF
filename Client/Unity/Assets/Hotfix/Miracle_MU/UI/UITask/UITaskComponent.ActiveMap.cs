using ETModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    public enum FuBenname
    {
        EMoSquare = 0,//恶魔广场
        XueSeCastle//血色广场
    }
    public enum ActiveType
    {
        ActiveIntoTime,
        ActiveIntoLevel
    }
    public partial class UITaskComponent
    {
        ReferenceCollector activeMapCollector;
        public GameObject ActiveMapContent;
        public Button ActiveIntoLevelBtn;
        public Button ActiveIntoTimeBtn;
        public List<GameObject> ActiveMapList = new List<GameObject>();
        public void InitActiveMap()
        {
            activeMapCollector = ActiveMap.GetReferenceCollector();
            ActiveMapContent = activeMapCollector.GetImage("ActiveMapContent").gameObject;
            //活动地图进入等级限制
            activeMapCollector.GetButton("ActiveIntoLevelBtn").onClick.AddSingleListener(() =>
            {
                UIComponent.Instance.VisibleUI(UIType.UIActiveInfo);
                UIComponent.Instance.Get(UIType.UIActiveInfo).GetComponent<UIActiveInfoComponent>().SetActiveInfo(FuBenname.EMoSquare, "活动入场时间", ActiveType.ActiveIntoLevel);
            });
            //活动地图开始时间段
            activeMapCollector.GetButton("ActiveIntoTimeBtn").onClick.AddSingleListener(() =>
            {
                UIComponent.Instance.VisibleUI(UIType.UIActiveInfo);
                UIComponent.Instance.Get(UIType.UIActiveInfo).GetComponent<UIActiveInfoComponent>().SetActiveInfo(FuBenname.EMoSquare, "活动入场时间", ActiveType.ActiveIntoTime);
            });
        }

        //清理
        public void ClearActiveMapList()
        {
            for (int i = 0; i < ActiveMapList.Count; i++)
            {
                GameObject.Destroy(ActiveMapList[i]);
            }
            ActiveMapList.Clear();
        }

        public async ETTask GetBattleCopyInfoRequest()
        {
            ClearActiveMapList();

            G2C_GetBattleCopyInfoRequest g2C_GetBattleCopyInfo = (G2C_GetBattleCopyInfoRequest)await SessionComponent.Instance.Session.Call(new C2G_GetBattleCopyInfoRequest() { });
            if (g2C_GetBattleCopyInfo.Error != 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_GetBattleCopyInfo.Error.GetTipInfo());
            }
            else
            {
                ActiveMap.SetActive(true);

                for (int i = 0; i < g2C_GetBattleCopyInfo.BatteCopyStates.Count - 1; i++)
                {
                    if (i == 0)
                    {
                        ActiveMapContent.transform.Find("FuBenName").GetComponent<Text>().text = GetFuBenName(i);
                        ActiveMapContent.transform.Find("Image/IntoCountInfoTxt").GetComponent<Text>().text = $"可以{g2C_GetBattleCopyInfo.NumMaxs[i] - g2C_GetBattleCopyInfo.Numbers[i]}/{g2C_GetBattleCopyInfo.NumMaxs[i]} 入场";
                    }
                    else
                    {
                        GameObject activeMapContent = GameObject.Instantiate(ActiveMapContent);
                        activeMapContent.transform.position = ActiveMapContent.transform.position;
                        activeMapContent.transform.SetParent(ActiveMapContent.transform.parent);
                        activeMapContent.transform.localScale = Vector3.one;
                        activeMapContent.transform.Find("FuBenName").GetComponent<Text>().text = GetFuBenName(i);
                        activeMapContent.transform.Find("Image/IntoCountInfoTxt").GetComponent<Text>().text = $"可以{g2C_GetBattleCopyInfo.NumMaxs[i] - g2C_GetBattleCopyInfo.Numbers[i]}/{g2C_GetBattleCopyInfo.NumMaxs[i]} 入场";
                        ActiveMapList.Add(activeMapContent);
                    }

                }
             

            }
            string GetFuBenName(int index) => index switch
            {
                0 => "恶魔广场",
                1 => "血色城堡",
                _ => null
            };

        }
    }

}
