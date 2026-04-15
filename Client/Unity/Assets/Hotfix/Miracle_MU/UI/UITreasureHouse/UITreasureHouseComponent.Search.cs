using ETModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    public partial class UITreasureHouseComponent
    {
        public ReferenceCollector searchCollider;
        public GameObject Togs;
        public InputField SearchInput;
        public Button SearchBtn;
        public GameObject ItemTypePanelActive;
        public GameObject ItemInfoActive;
        public GameObject TradingPanelActive;
        public GameObject ItemBG;
        public GameObject JiShouItemBG;
        public string SearchContent = string.Empty;
        public string LastInputContent = string.Empty;
        public int pageType = 0;
        public RectTransform CenterRect;
        public void Search()
        {
            searchCollider = collector.GetGameObject("Top").GetReferenceCollector();
            Togs = searchCollider.GetGameObject("Togs");

            ItemTypePanelActive = searchCollider.GetImage("ItemTypePanel").gameObject;
            CenterRect = searchCollider.GetGameObject("Center").GetComponent<RectTransform>();
            ItemInfoActive = searchCollider.GetGameObject("ItemInfo");
            TradingPanelActive = searchCollider.GetGameObject("TradingPanel");
            ItemBG = searchCollider.GetImage("ItemBG").gameObject;
            ItemBG.SetActive(true);
            JiShouItemBG = searchCollider.GetImage("JiShouItemBG").gameObject;
            JiShouItemBG.SetActive(false);
            SearchInput = searchCollider.GetInputField("SearchInput");
            SearchBtn = searchCollider.GetButton("SearchBtn");
            SearchInput.onValueChanged.AddSingleListener((value) =>
            {
                SearchContent = value.ToString();
                filtrateData.Name = SearchContent;
            });
            SearchBtn.onClick.AddSingleListener(() =>
            {
                SearchClick();
            });
            InitSearch();
        }
        public void InitSearch()
        {
            for (int i = 0,length = Togs.transform.childCount; i < length; i++)
            {
                Toggle toggle = Togs.transform.GetChild(i).GetComponent<Toggle>();
                int e = i;
                toggle.onValueChanged.AddSingleListener((isOn) =>
                {
                    TogClick(isOn,e);
                });
            }
        }
        public void TogClick(bool IsOn,int e)
        {
            if (!IsOn) return;
            pageType = e;
            JiShouItemBG.SetActive(e == 0);
            ItemBG.SetActive(e == 1);
            switch (e)
            {
                case 0:
                    CenterRect.anchorMin = new Vector2(0.5f, 0.5f);
                    CenterRect.anchorMax = new Vector2(0.5f, 0.5f);
                    CenterRect.sizeDelta = new Vector2(890f, 824f);
                    CenterRect.anchoredPosition = new Vector2(-91f, -31f);
                    ItemTypePanelActive.SetActive(true);
                    ItemInfoActive.SetActive(true);
                    TradingPanelActive.SetActive(false);

                    if(selectIndexOne != null)
                    {
                        Transform tempTranone = selectIndexOne.Find("OneGrade/Background/Select");
                        tempTranone.gameObject.SetActive(false);
                        selectIndexOne = null;
                    }

                    if(selectIndexTwo != null)
                    {
                        Transform tempTran = selectIndexTwo.Find("OneGrade/Background/Select");
                        tempTran.gameObject.SetActive(false);
                        selectIndexTwo = null;
                    }

                    LastLickId = 0;//当前选择的二级标题
                    ButtonOneClick(itemOne, itemOrderOne);
                    break;
                case 1:
                    CenterRect.anchorMin = new Vector2(0.5f, 0.5f);
                    CenterRect.anchorMax = new Vector2(0.5f, 0.5f);
                    CenterRect.sizeDelta = new Vector2(1107f, 824f);
                    CenterRect.anchoredPosition = new Vector2(-200f, -31f);

                    ItemTypePanelActive.SetActive(false);
                    ItemInfoActive.SetActive(false);
                    TradingPanelActive.SetActive(true);
                    OpenConsign().Coroutine();
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// 搜索
        /// </summary>
        public void SearchClick()
        {
            if (string.IsNullOrEmpty(SearchContent))
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint,"请输入搜索内容");
                return;
            }
            if (LastInputContent == SearchContent)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint,"请输入不同的搜索内容");
                return;
            }
            LastInputContent = SearchContent;
            SearchForItems(filtrateData).Coroutine();
        }
    }

}
