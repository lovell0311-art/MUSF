using ETModel;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    public partial class UITreasureHouseComponent
    {
        public ReferenceCollector itemListCollector;
        public UICircularScrollView<TreasureHouseItemInfo> uICircular_Item;
        public ScrollRect itemScrollView;
        public GameObject itemContent;
        public GameObject lastClickItem = null;
        public TreasureHouseItemInfo lastClickItemInfo = null;
        public GameObject CangBaoGeItem_Icon;

        public static readonly int UILayer = LayerMask.NameToLayer(LayerNames.UI);
        public List<GameObject> itemsList = new List<GameObject>();

        public void ItelList()
        {
            itemListCollector = collector.GetImage("ItemListPanel").gameObject.GetReferenceCollector();
            itemScrollView = itemListCollector.GetImage("ItemScrollView").GetComponent<ScrollRect>();
            itemContent = itemListCollector.GetGameObject("Content");
            CangBaoGeItem_Icon = ResourcesComponent.Instance.LoadGameObject("CangBaoGeItem_Icon".StringToAB(), "CangBaoGeItem_Icon");
            UICircularChatInit();
        }

        /// <summary>
        /// 初始化物品滑动框
        /// </summary>
        public void UICircularChatInit()
        {
            uICircular_Item = ComponentFactory.Create<UICircularScrollView<TreasureHouseItemInfo>>();
            uICircular_Item.InitInfo(E_Direction.Vertical, 1, 0, 0);
            uICircular_Item.ItemInfoCallBack = InitItemList;
            uICircular_Item.ItemClickCallBack = ClickItemList;
            uICircular_Item.IninContent(itemContent, itemScrollView);
        }

        private void ClickItemList(GameObject item, TreasureHouseItemInfo itemInfo)
        {
           // Log.DebugGreen(itemInfo.Uid.ToString() + itemInfo.Name);
            if (lastClickItemInfo != null && lastClickItemInfo.Uid == itemInfo.Uid) return;
            lastClickItemInfo = itemInfo;
            GetTreasureHouseItemInfo(itemInfo).Coroutine();
            lastClickItem?.transform.Find("Select").gameObject.SetActive(false);
            lastClickItem = item;
            item.transform.Find("Select").gameObject.SetActive(true);
        }

        private void InitItemList(GameObject item, TreasureHouseItemInfo itemInfo)
        {
            Sprite initSprite;
           // Log.DebugBrown("InitItemList" + itemInfo.ConfigID);
            if (pageType == 0)
            {
                initSprite = CangBaoGeItem_Icon.GetReferenceCollector().GetSprite(ItemType.GetItenOneType(filtrateData.Page));
            }
            else
            {
                initSprite = CangBaoGeItem_Icon.GetReferenceCollector().GetSprite(ItemType.GetItenOneType(ItemType.GetItenType(itemInfo.ConfigID / 10000)));

                item.transform.Find("Time").GetComponent<Text>().text = TimerComponent.Instance.ScendToYMDs(itemInfo.EndTime);
            }
              ((long)itemInfo.ConfigID).GetItemInfo_Out(out Item_infoConfig item_Info);
            if (item_Info != null)
            {
                GameObject obj = ResourcesComponent.Instance.LoadGameObject(item_Info.ResName.StringToAB(), item_Info.ResName);//???????????????
                if (obj != null)
                {
                    obj.SetUI(itemInfo.Enhance);
                    Vector3 vector = item.transform.Find("Image").transform.position;
                    RectTransform rect = item.transform.Find("Image").GetComponent<RectTransform>();
                    MeshSize.Result result = MeshSize.GetMeshSize(obj.transform, UILayer);
                    float scale = result.GetScreenScaleFactor(new Vector2(rect.rect.width, rect.rect.height) * 1.2f); // ???????ui????????
                    if (scale > 1f) scale = 1f; // ???С???????
                    vector.z = 99.5f;
                    obj.transform.position = vector;
                    obj.transform.localScale *= scale;
                    itemsList.Add(obj);
                }
                else
                {
                    item.transform.Find("Image").GetComponent<Image>().sprite = initSprite;
                }
            }
            else
            {

                item.transform.Find("Image").GetComponent<Image>().sprite = initSprite;
            }
            item.transform.Find("Image/Text").GetComponent<Text>().text = itemInfo.Cnt.ToString();
            item.transform.Find("Name").GetComponent<Text>().text = itemInfo.Name;
            item.transform.Find("Class").GetComponent<Text>().text = itemInfo.Class;
            item.transform.Find("ZhuoYueTxt").GetComponent<Text>().text = $"+{itemInfo.Excellent}";
            item.transform.Find("QiangHuaTxt").GetComponent<Text>().text = $"+{itemInfo.Enhance}";
            item.transform.Find("yigoumai").gameObject.SetActive(false);
            item.transform.Find("yixiajia").gameObject.SetActive(false);
            string Readdition = string.Empty;
            if(itemInfo.Readdition == 1)
            {
                Readdition = "套装";
            }
            else if(itemInfo.Readdition == 2)
            {
                Readdition = "镶嵌装";
            }else if (itemInfo.Readdition == 0)
            {
                Readdition = "";
            }
            item.transform.Find("TaoZhuang").GetComponent<Text>().text = Readdition;
            item.transform.Find("MoJing").GetComponent<Text>().text = itemInfo.Price.ToString();

            if(lastClickItemInfo != null && lastClickItemInfo.Uid == itemInfo.Uid)
            {
                item.transform.Find("Select").gameObject.SetActive(true);
                lastClickItem = item;
            }
            else
            {
                item.transform.Find("Select").gameObject.SetActive(false);
            }

        }

        public void ModelHide()
        {
            for (int i = 0; i < itemsList.Count; i++)
            {
                ResourcesComponent.Instance.DestoryGameObjectImmediate(itemsList[i], itemsList[i].name.StringToAB());
            }
            itemsList.Clear();
        }
    }

}
