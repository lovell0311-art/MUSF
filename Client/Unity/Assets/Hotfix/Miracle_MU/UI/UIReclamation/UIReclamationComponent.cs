using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using UnityEngine.UI;
using UnityEngine.U2D;
using System;
using System.Linq;
using static NPOI.HSSF.Util.HSSFColor;
using static UnityEditor.Progress;
using static UnityEditor.ShaderData;


namespace ETHotfix
{

    [ObjectSystem]
    public class UIReclamationComponentAwake : AwakeSystem<UIReclamationComponent>
    {
        public override void Awake(UIReclamationComponent self)
        {

            self.rc = self.GetParent<UI>().GameObject.GetReferenceCollector();
            self.rc.GetButton("CloseBtn").onClick.AddSingleListener(() =>
            {
                UIComponent.Instance.Remove(UIType.UIReclamation);
                UIComponent.Instance.Remove(UIType.UIIntroduction);
            });
            self.leftContent = self.rc.GetGameObject("LeftContent");
            self.rightContent = self.rc.GetGameObject("RightContent");
            self.knapsackDataItem = ComponentFactory.Create<KnapsackDataItem>();
            //获取配置表
            IConfig[] alltypes = ConfigComponent.Instance.GetAll<SelectionGiftBundle_TypeConfig>();
            IConfig[] allitems = ConfigComponent.Instance.GetAll<SelectionGiftBundle_ItemInfoConfig>();
            foreach (var items in allitems.Cast<SelectionGiftBundle_ItemInfoConfig>())
            {
                if (!self.dic.ContainsKey(items.RoleType))
                {
                    self.dic.Add(items.RoleType, 0);
                }
            }
            //    for (int i = 0; i < alltypes.Length; i++)
            //{
            //    SelectionGiftBundle_TypeConfig typeConfig = alltypes[i] as SelectionGiftBundle_TypeConfig;
            //    List<IConfig> items = allitems.FindAll(p => ((SelectionGiftBundle_ItemInfoConfig)p).TypeId == typeConfig.Id);
            //    for (int j = 0; j < items.Count; j++)
            //    {
            //        SelectionGiftBundle_ItemInfoConfig itemInfoConfig = items[j] as SelectionGiftBundle_ItemInfoConfig;
            //        if (typeConfig.Id == 100)
            //        {

            //            if (!self.dic.ContainsKey(itemInfoConfig.RoleType))
            //            {
            //                self.dic.Add(itemInfoConfig.RoleType, 0);
            //            }
            //        }
            //    }
            //}

            if (self.dic.Count > 0)//展示职业的数量
            {
                IConfig[] pack = ConfigComponent.Instance.GetAll<SelectionGiftBundle_ItemInfoConfig>();
                int index = 0;
                int itemindex = 0;
                foreach (var item in self.dic)
                {
                    itemindex = 0;
                    self.leftContent.transform.GetChild(index).gameObject.SetActive(true);
                    E_RoleType e_RoleType = (E_RoleType)item.Key;
                    self.leftContent.transform.GetChild(index).transform.GetChild(0).GetChild(0).GetComponent<Text>().text = e_RoleType.GetRoleName(0);
                    foreach (var items in pack.Cast<SelectionGiftBundle_ItemInfoConfig>())
                    {

                        if (items.RoleType == item.Key)
                        {
                            self.leftContent.transform.GetChild(index).GetChild(2).GetChild(itemindex).gameObject.SetActive(true);
                            self.leftContent.transform.GetChild(index).GetChild(2).GetChild(itemindex).GetComponent<Button>().onClick.AddSingleListener(() =>
                            {
                                self.OnNewItemClick(items, self.leftContent.transform.GetChild(index).gameObject, true);
                            });


                            self.leftContent.transform.GetChild(index).GetChild(1).GetComponent<Button>().onClick.AddSingleListener(async () =>
                            {
                                var response = (G2C_ClearingGiftKit)await SessionComponent.Instance.Session.Call(new C2G_ClearingGiftKit
                                {
                                    MaxType = (int)100,
                                    MinType = items.RoleType,
                                });
                                Log.DebugBrown("开挂礼包的错误码" + response.Error);
                                if (response.Error != ErrorCode.ERR_Success)
                                {
                                    UIComponent.Instance.VisibleUI(UIType.UIHint, response.Error.GetTipInfo());
                                }
                                else
                                {
                                    UIComponent.Instance.VisibleUI(UIType.UIHint, "购买成功！");
                                }
                            });


                            GameObject f = ResourcesComponent.Instance.LoadGameObject(items.ResourceName.StringToAB(), items.ResourceName);
                            if (f != null)
                            {
                                f.transform.parent = self.leftContent.transform.GetChild(index).GetChild(2).GetChild(itemindex);
                                f.SetUI();
                                f.transform.localPosition = Vector3.zero;
                                f.transform.localScale = new Vector3(50, 50, 50);
                            }
                            itemindex++;
                        }
                    }
                    index++;
                }

            }
            // self.OnInit();
        }
    }

    [ObjectSystem]

    public partial class UIReclamationComponent : Component
    {



        public ReferenceCollector rc;

        public GameObject leftContent;
        public GameObject rightContent;
        public GameObject itemFab;
        public GameObject reclamationItemFab;

        public Dictionary<int, GameObject> leftParent = new Dictionary<int, GameObject>();
        public Dictionary<int, GameObject> rightParent = new Dictionary<int, GameObject>();

        //public UICircularScrollView<UIReclamationModels> uICircular_Left;
        //public UICircularScrollView<UIReclamationModels> uICircular_Right;


        public UIIntroductionComponent uIIntroduction;

        public KnapsackDataItem knapsackDataItem;
        public List<String> diratr = new List<String>();
        public Dictionary<int, int> dic = new Dictionary<int, int>();

        public void SetknapsackDataItemAtr(SelectionGiftBundle_ItemInfoConfig itemInfoConfig, long configId)
        {
            knapsackDataItem.ClearProper();
            diratr.Clear();
            knapsackDataItem.ConfigId = configId;

            diratr.Add($"<color={ColorTools.NormalItemNameColor}>{itemInfoConfig.ItemName}</color>");
            diratr.Add($"<color={ColorTools.ExcellenceItemColor}>强化等级 +{itemInfoConfig.Level}</color>");
            // self.diratr.Add($"<color={ColorTools.OriginItemColor}>锻造等级 +{itemInfoConfig.SmithLevel}</color>");
        }
        public void OnNewItemClick(SelectionGiftBundle_ItemInfoConfig itemInfoConfig, GameObject item, bool model)
        {
            if (uIIntroduction == null)
            {
                uIIntroduction = UIComponent.Instance.VisibleUI(UIType.UIIntroduction).GetComponent<UIIntroductionComponent>();
            }
            SetknapsackDataItemAtr(itemInfoConfig, itemInfoConfig.ItemId);

            uIIntroduction.GetAllAtrs(knapsackDataItem);
            uIIntroduction.AddDiyAtr(diratr);
            uIIntroduction.ShowAtrs();
            uIIntroduction.Introcontent.transform.GetChild(1).gameObject.SetActive(false);
            var pos = item.transform.position;
            if (model)
            {
                uIIntroduction.SetPos(pos + new Vector3(3, 0, 0), 0.5f);
            }
            else
            {
                uIIntroduction.SetPos(pos += Vector3.left, 0.5f);
            }
        }
        internal void OnInit()
        {

        }
    }
}
