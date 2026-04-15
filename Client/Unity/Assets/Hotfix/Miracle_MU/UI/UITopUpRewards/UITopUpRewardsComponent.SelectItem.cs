
using ETModel;
using log4net.Core;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    public partial class UITopUpRewardsComponent
    {
        public ReferenceCollector selectItemCollector;
        public Dictionary<int, GameObject> selectItems = new Dictionary<int, GameObject>();//缓存物品模型，关闭销毁
        public Dictionary<int, GameObject> selectToggle = new Dictionary<int, GameObject>();//缓存Toggle点击信息
        public GameObject SelectItems;
        public GameObject Items;
        public Text SelectTxt;
        public Text GudingTxt;
        public Button TopUpBtn;
        public List<int> getItemConfigID = new List<int>();
        public int selectId;
        public int Money = 0;
        public int TypeId;
        public E_PlayerShopQuotaType quotaType = E_PlayerShopQuotaType.LevelTopUpI;
        public static readonly int UILayer = LayerMask.NameToLayer(LayerNames.UI);
        public void InitSelectItem()
        {
            selectItemCollector = collector.GetImage("levelBg").gameObject.GetReferenceCollector();
            selectItemCollector.GetButton("CloseBtn").onClick.AddSingleListener(() =>
            {
                SetSelectIsShow(false);
            });
            SelectItems = selectItemCollector.GetImage("SelectItems").gameObject;
            Items = selectItemCollector.GetImage("Items").gameObject;
            SelectTxt = selectItemCollector.GetText("SelectTxt");
            GudingTxt = selectItemCollector.GetText("GudingTxt");
            TopUpBtn = selectItemCollector.GetButton("TopUpBtn");
            TopUpBtn.onClick.AddSingleListener(() =>
            {
                if (selectId != 0)
                {
                    getItemConfigID.Add(selectId);
                }
                ReceiveRechargeGiftPack().Coroutine();
            });
            Toggle[] toggles = SelectItems.transform.GetComponentsInChildren<Toggle>();
            foreach (var item in toggles)
            {
                Toggle toggle = item;
                toggle.onValueChanged.AddSingleListener((bool isOn) =>
                {
                    if (isOn)
                    {
                        foreach (var item1 in selectToggle)
                        {
                            if (item1.Value.name == item.name)
                            {
                                selectId = item1.Key;
                                twoId = item1.Key;
                                Log.DebugGreen($"选择的自选物品Id->{selectId}");
                                break;
                            }
                        }
                    }

                });
            }
        }
        public int DangCi = 0;
        public void LoadModle(List<CumulativeRecharge_ItemInfoConfig> topUpItems)
        {
            DisItems();
            selectToggle.Clear();
            //显示模型
            Log.DebugGreen("显示模型"+ topUpItems.Count);
            for (int i = 0, length = SelectItems.transform.childCount; i < length; i++)
            {
                SelectItems.transform.GetChild(i).gameObject.SetActive(false);
            }
            for (int i = 0, length = Items.transform.childCount; i < length; i++)
            {
                Items.transform.GetChild(i).gameObject.SetActive(false);
            }
            if (topUpItems.Count == 0) return;
            var recharge_Type = ConfigComponent.Instance.GetItem<CumulativeRecharge_TypeConfig>(topUpItems.First().TypeId);
            TopUpBtn.gameObject.SetActive(TopUpRewardsGlob.TotalAmount >= recharge_Type.Money && !TopUpRewardsGlob.Configids.Contains((int)recharge_Type.Id));
            //TopUpBtn.gameObject.SetActive(!TopUpRewardsGlob.Configids.Contains((int)recharge_Type.Id));
            int index = 0;
            int selectCount = 0;
            int selectCountone = 0;
            TypeId = topUpItems.First().TypeId;
            twoId = topUpItems.First().Id2;
            foreach (var item in topUpItems)
            {
                if (item.IsSelectable == 1)//固定
                {
                    Items.transform.GetChild(index).gameObject.SetActive(true);
                    Items.transform.GetChild(index).Find("Count").GetComponent<Text>().text = "X" + item.Quantity;
                    Items.transform.GetChild(index).Find("Name").GetComponent<Text>().text = item.ItemName;
                    getItemConfigID.Add(item.ItemId);
                    selectCountone++;
                    selectId = item.ItemId;
                    LoadModle(item, Items);
                }
                else//可选
                {
                    SelectItems.transform.GetChild(index).gameObject.SetActive(true);
                    SelectItems.transform.GetChild(index).Find("Count").GetComponent<Text>().text = "X" + item.Quantity;
                    SelectItems.transform.GetChild(index).Find("Name").GetComponent<Text>().text = item.ItemName;
                    selectCount++;
                    selectToggle[item.Id2] = SelectItems.transform.GetChild(index).gameObject;
                    LoadModle(item, SelectItems);

                }
                //Items.transform.GetChild(index).gameObject.SetActive(false);
                index++;
            }
            // Log.DebugGreen($"自选物品数量->{selectToggle.Count}");
            if (selectCount > 0)
            {
                //有自选物品
                selectId = selectToggle.First().Key;
                SelectTxt.text = $"自选物品 {selectCount}选1";
            }
            SelectTxt.gameObject.SetActive(selectCount > 0);
            SelectItems.gameObject.SetActive(selectCount > 0);
            GudingTxt.gameObject.SetActive(selectCountone > 0);
            Items.gameObject.SetActive(selectCountone > 0);

            TimerComponent.Instance.RegisterTimeCallBack(100, () =>
            {
                RefreshContentSizeFitter();
            });
            void LoadModle(CumulativeRecharge_ItemInfoConfig item, GameObject items)
            {
                ((long)item.ItemId).GetItemInfo_Out(out Item_infoConfig item_Info);
                //显示物品模型
                GameObject obj = ResourcesComponent.Instance.LoadGameObject(item_Info.ResName.StringToAB(), item_Info.ResName);
                Transform target = items.transform.GetChild(index).Find("Image");
                if (obj != null)
                {
                    obj.SetUI(item.Level);
                    obj.transform.SetParent(target, false);
                    Vector3 vector = Vector3.zero;
                    RectTransform rect = target.GetComponent<RectTransform>();
                    MeshSize.Result result = MeshSize.GetMeshSize(obj.transform, UILayer);
                    float scale = result.GetScreenScaleFactor(new Vector2(rect.rect.width, rect.rect.height) * 60);

                    if (scale > 80) scale = 80;
                    vector.z = -10;
                    obj.transform.localPosition = vector;


                    obj.transform.localScale = Vector3.one * scale;
                    selectItems[item.ItemId] = obj;
                }
            }
            SetSelectIsShow(true);
        }

        public void SetSelectIsShow(bool isShow)
        {
            selectItemCollector.gameObject.SetActive(isShow);
        }

        public void DisItems()
        {
            foreach (var item in selectItems)
            {
                if (item.Value != null)
                {
                    ResourcesComponent.Instance.DestoryGameObjectImmediate(item.Value, item.Value.name.StringToAB());
                }
            }
            selectItems.Clear();
        }
        public void RefreshContentSizeFitter()
        {
            ContentSizeFitter[] csfs = selectItemCollector.GetComponentsInChildren<ContentSizeFitter>();
            foreach (var item in csfs)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(item.GetComponent<RectTransform>());
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(selectItemCollector.GetComponent<RectTransform>());
        }
    }
}

