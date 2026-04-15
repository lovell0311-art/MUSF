using Codice.CM.Common;
using ETModel;
using ILRuntime.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Experimental.GraphView.GraphView;

namespace ETHotfix
{
    public static class levelTopUp
    {
        public static List<LevelTopUp_TypeConfig> levelTopUp_Types = new List<LevelTopUp_TypeConfig>();
    }
    [ObjectSystem]
    public class UILevelTopUpComponentAwake : AwakeSystem<UILevelTopUpComponent>
    {
        public override void Awake(UILevelTopUpComponent self)
        {
            self.Awake();
        }
    }
    public class UILevelTopUpComponent : Component, IUGUIStatus
    {
        public static readonly int UILayer = LayerMask.NameToLayer(LayerNames.UI);
        RoleEntity roleEntity_ => UnitEntityComponent.Instance.LocalRole;
        public Text Tips,TopUpAmount;
        public Transform Items;
        public int Level;
        public string saveContent;
        public Image pohuai, xianglian, shengjian, shengling, fuhuo, yuangu;
        public List<GameObject> itemsList = new List<GameObject>();

        public bool IsTopUp = false;

        public E_PlayerShopQuotaType quotaType = E_PlayerShopQuotaType.LevelTopUpI;
        internal void Awake()
        {
            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            Tips = rc.GetText("Tips");
            pohuai = rc.GetImage("pohuai");
            xianglian = rc.GetImage("xianglian");
            shengjian = rc.GetImage("shengjian");
            shengling = rc.GetImage("shengling");
            fuhuo = rc.GetImage("fuhuo");
            yuangu = rc.GetImage("yuangu");
            TopUpAmount = rc.GetText("TopUpAmount");
            Items = rc.GetImage("Items").transform;
            IsTopUp = false;
            rc.GetButton("CloseBtn").onClick.AddSingleListener(() =>
            {
                if (IsTopUp)
                {
                    if (PlayerPrefs.GetInt(saveContent) == 0)
                    {
                        PlayerPrefs.SetInt(saveContent, 1);
                        PlayerPrefs.Save();
                    }
                    UIMainComponent.Instance.SetLevelTopUpShow(Level, false);
                }
                else
                {
                    //设置主界面UI Level
                    UIMainComponent.Instance.SetLevelTopUpShow(Level,true);
                }
                UIComponent.Instance.Remove(UIType.UILevelTopUp);
            });
            rc.GetButton("TopUpBtn").onClick.AddSingleListener(() =>
            {
                TopUpComponent.Instance.TopUp((int)quotaType).Coroutine();
                IsTopUp = true;
            });
            rc.GetButton("NoTips").onClick.AddSingleListener(() =>
            {
                if (PlayerPrefs.GetInt(saveContent) == 0)
                {
                    PlayerPrefs.SetInt(saveContent, 1);
                    PlayerPrefs.Save();
                }
                UIMainComponent.Instance.SetLevelTopUpShow(Level,false);
                UIComponent.Instance.Remove(UIType.UILevelTopUp);
            });
        }
        public void LoadLevelTopUp_Type()
        {
            if (levelTopUp.levelTopUp_Types.Count == 0)
            {
                var levelTopUp_Type = ConfigComponent.Instance.GetAll<LevelTopUp_TypeConfig>();
                foreach (var item in levelTopUp_Type.Cast<LevelTopUp_TypeConfig>())
                {
                    levelTopUp.levelTopUp_Types.Add(item);
                }
            }
        }

        public void SetLevelTopUpValue()
        {
            int index = 0; int Amount = 0;
            ModelHide();
            for (int i = 0,length = Items.childCount - 1; i < length; i++)
            {
                Items.transform.GetChild(i).gameObject.SetActive(false);
            }
            Tips.text = $"恭喜达到{Level}级，充值固定金额赠送以下物品";
            foreach (var item in levelTopUp.levelTopUp_Types)
            {
                if(item.Level == Level && ((int)roleEntity_.RoleType == item.RoleType|| item.RoleType == 0))
                {
                    Amount = item.RechargeAmount;
                   ((long)item.ConfigId).GetItemInfo_Out(out Item_infoConfig item_Info);
                    Items.transform.GetChild(index).gameObject.SetActive(true);
                    Items.transform.GetChild(index).Find("Name").GetComponent<Text>().text = $"{item_Info.Name}";
                    Items.transform.GetChild(index).Find("Count").GetComponent<Text>().text = $"X{item.Quantity}";
                    Items.transform.GetChild(index).gameObject.GetComponent<Button>().onClick.AddSingleListener(() =>
                    {
                        if (item.ConfigId == 230012)//项链
                        {
                            if(xianglian.gameObject.activeSelf)
                                xianglian.gameObject.SetActive(false);
                            else
                                xianglian.gameObject.SetActive(true);
                        }
                        //shengjian, shengling, fuhuo, yuangu
                        if (item.ConfigId == 10016)//破坏之剑
                        {
                            if (pohuai.gameObject.activeSelf)
                                pohuai.gameObject.SetActive(false);
                            else
                                pohuai.gameObject.SetActive(true);
                        }
                        if (item.ConfigId == 80006)//复活之杖
                        {
                            if (fuhuo.gameObject.activeSelf)
                                fuhuo.gameObject.SetActive(false);
                            else
                                fuhuo.gameObject.SetActive(true);
                        }
                        if (item.ConfigId == 40007)//圣灵之弓
                        {
                            if (shengjian.gameObject.activeSelf)
                                shengjian.gameObject.SetActive(false);
                            else
                                shengjian.gameObject.SetActive(true);
                        }
                        if (item.ConfigId == 80029)//远古之杖
                        {
                            if (yuangu.gameObject.activeSelf)
                                yuangu.gameObject.SetActive(false);
                            else
                                yuangu.gameObject.SetActive(true);
                        }
                        if (item.ConfigId == 100002)//圣剑权杖
                        {
                            if (shengjian.gameObject.activeSelf)
                                shengjian.gameObject.SetActive(false);
                            else
                                shengjian.gameObject.SetActive(true);
                        }
                    });
                    GameObject obj = ResourcesComponent.Instance.LoadGameObject(item_Info.ResName.StringToAB(), item_Info.ResName);//显示当前附件的模型
                    if (obj == null) return;

                    RectTransform rect = Items.transform.GetChild(index).Find("Image").GetComponent<RectTransform>();
                    MeshSize.Result result = MeshSize.GetMeshSize(obj.transform, UILayer);
                    float scale = result.GetScreenScaleFactor(new Vector2(rect.rect.width, rect.rect.height) * 1.8f); // 让装备比ui卡槽大一点
                    if (scale > 1f) scale = 1f; // 只缩小，不放大

                    obj.transform.parent = Items.transform.GetChild(index).Find("Image").transform;
                    /*obj.transform.localPosition = new Vector3(0, 0, -130f);
                    obj.transform.localScale = Vector3.one * 50f;*/

                    obj.transform.localPosition = Vector3.zero;
                    obj.transform.localScale *= scale;

                    index++;
                    itemsList.Add(obj);
                }
            }
            Items.transform.Find("mojing/Count").GetComponent<Text>().text = $"X{Amount}";
            TopUpAmount.text = $"充值{Amount}";
        }
        public void ModelHide()
        {
            for (int i = 0; i < itemsList.Count; i++)
            {
                ResourcesComponent.Instance.DestoryGameObjectImmediate(itemsList[i], itemsList[i].name.StringToAB());
            }
            itemsList.Clear();
        }
        public void OnInVisibility()
        {

        }
        public void OnVisible(object[] data)
        {
            Level = data[0].ToInt32();
            if (Level == 220)
            {
                quotaType = E_PlayerShopQuotaType.LevelTopUpI;
            }
            else if (Level == 250)
            {
                quotaType = E_PlayerShopQuotaType.LevelTopUpII;
            }
            else if (Level == 280)
            {
                quotaType = E_PlayerShopQuotaType.LevelTopUpIII;
            }
            else if (Level == 300)
            { 
             quotaType= E_PlayerShopQuotaType.LevelTopUpIV;
            }
            else if (Level == 320)
            { 
             quotaType= E_PlayerShopQuotaType.LevelTopUpV;
            }
            else if (Level == 350)
            {
                quotaType = E_PlayerShopQuotaType.LevelTopUpVI;
            }
            else if (Level == 400)
            {
                quotaType = E_PlayerShopQuotaType.LevelTopUpVII;
            }
            saveContent = data[1].ToString();
            LoadLevelTopUp_Type();
            SetLevelTopUpValue();
        }
        public void OnVisible()
        {

        }
        public override void Dispose()
        {
            base.Dispose();
            ModelHide();
        }
    }
}
