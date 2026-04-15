using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using UnityEngine.UI;
using System.Linq;

namespace ETHotfix
{

    [ObjectSystem]
    public class UISceneTranslateComponentAwake : AwakeSystem<UISceneTranslateComponent>
    {
        public override void Awake(UISceneTranslateComponent self)
        {
            self.Awake();
        }
    }
    /// <summary>
    /// 地图选择组件
    /// </summary>
    public class UISceneTranslateComponent : Component
    {


        public Transform content;
        public RoleEntity roleEntity;

        //public Button UIBeginnerGuide;
        public void Awake()
        {
            ReferenceCollector referenceCollector = GetParent<UI>().GameObject.GetReferenceCollector();
            content = referenceCollector.GetGameObject("Content").transform;
            referenceCollector.GetButton("CloseBtn").onClick.AddSingleListener(Close);
           // referenceCollector.GetButton("BtnClose").onClick.AddSingleListener(Close);
            roleEntity = UnitEntityComponent.Instance.LocalRole;
            Init();
            //UIBeginnerGuide = referenceCollector.GetButton("UIBeginnerGuide");
            //UIBeginnerGuide.onClick.AddSingleListener(() =>
            //{
            //    UIBeginnerGuide.gameObject.SetActive(false);
            //    //if (BeginnerGuideData.IsCompleteTrigger(61, 58))
            //    //{
            //    //    BeginnerGuideData.SetBeginnerGuide(61);
            //    //}
            //});
            //UIBeginnerGuide.gameObject.SetActive(false);
            //if (BeginnerGuideData.IsCompleteTrigger(60, 58))
            //{
            //    BeginnerGuideData.SetBeginnerGuide(60);
            //    UIBeginnerGuide.gameObject.SetActive(true);
            //}

        }

        public void Init()
        {
            IConfig[] configs = ConfigComponent.Instance.GetAll<Maps_infoConfig>();
            Transform temp = content.GetChild(0);
            int index = 0;
            int count = content.childCount;
            foreach (Maps_infoConfig config in configs.Cast<Maps_infoConfig>())
            {

                Transform item;
                if (index < count)
                    item = content.GetChild(index++);
                else
                    item = GameObject.Instantiate(temp, content);
               // Log.DebugBrown("地图限制等级"+ "::::id" + config.MapId);
                Map_TransferPointConfig map_Transfer = ConfigComponent.Instance.GetItem<Map_TransferPointConfig>(config.MapId);
                if (map_Transfer==null)
                {
                   // break;
                }
                item.Find("name").GetComponent<Text>().text = config.SceneName;
                item.Find("lev").GetComponent<Text>().text = map_Transfer.MapMinLevel.ToString() + "级";
                item.Find("coin").GetComponent<Text>().text = map_Transfer.MapCostGold.ToString();
                
                item.GetComponent<Button>().onClick.AddSingleListener(() =>
                {
                    if (map_Transfer == null)
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, "该地图还未开放");
                    }
                    else
                    {
                        SceneName sceneName = SceneComponent.Instance.CurrentSceneName.ToEnum<SceneName>();
                        string scenceName = SceneNameExtension.GetSceneName(sceneName);

                        if (sceneName != SceneName.XueSeChengBao && sceneName != SceneName.EMoGuangChang)
                        {
                            OnChangeScene(map_Transfer);
                        }
                        else
                        {
                            UIComponent.Instance.VisibleUI(UIType.UIHint, "副本中不允许传送");
                        }
                    }
                });

            }

        }
        public void OnChangeScene(Map_TransferPointConfig info)
        {

            //判断等级 金币是否足够 
            if (roleEntity.Property.GetProperValue(E_GameProperty.GoldCoin) < info.MapCostGold)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "金币不足");
                return;
            }
            if (roleEntity.Property.GetProperValue(E_GameProperty.Level) < info.MapMinLevel)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "等级不足");
                return;
            }
            Log.DebugBrown("点击传送" + info.MapId);
            //天空之城需要佩戴翅膀才能进入
            if (info.MapId == (int)SceneName.TianKongZhiCheng)
            {
                if (roleEntity.GetComponent<RoleEquipmentComponent>().curWareEquipsData_Dic.ContainsKey(E_Grid_Type.Wing) == false)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "进入天空之城 需要翅膀");
                    return;
                }
            }
            if (info.MapId > 7)
            {
                bool jk = false;
                foreach (var item in roleEntity.GetComponent<RoleEquipmentComponent>().curWareEquipsData_Dic)
                {
                    if (item.Key== E_Grid_Type.Guard)
                    {
                        jk = true;
                        break;
                    }
                };
                //foreach (var item1 in KnapsackItemsManager.KnapsackItems.Values)
                //{
                //    if (item1.ItemType == (int)E_ItemType.Guard)
                //    {
                //        jk = true;
                //        break;
                //    }
                //}
                if (jk==false)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "进入该地图，需要拥有守护");
                    return;
                }
            }
            Close();
            //场景加载进度
            UIComponent.Instance.VisibleUI(UIType.UISceneLoading);
            ChangeMap().Coroutine();


            //主动暂停挂机
            if (RoleOnHookComponent.Instance.IsOnHooking)
            {
                UIMainComponent.Instance.HookTog.isOn = false;
            }
            //暂停自动寻路
            if (TaskDatas.AutoNavCallBack != null)
                TaskDatas.AutoNavCallBack = null;

            //加载地图格子数据
            if (SceneComponent.Instance.CurrentSceneName != ((SceneName)info.MapId).ToString())
            {
                AssetBundleComponent.Instance.LoadBundle(((SceneName)info.MapId).ToString().StringToAB());
                AstarComponent.Instance.LoadSceneNodes(((SceneName)info.MapId).ToString(), "null");
            }
            ///地图传送
            async ETVoid ChangeMap()
            {

                G2C_MapDeliveryResponse response = (G2C_MapDeliveryResponse)await SessionComponent.Instance.Session.Call(new C2G_MapDeliveryRequest
                {
                    MapId = (int)info.Id
                });
                if (response.Error != 0)
                {
                    UIComponent.Instance.Remove(UIType.UISceneLoading);
                    AstarComponent.Instance.LoadSceneNodes(SceneComponent.Instance.CurrentSceneName, "null");
                    UIComponent.Instance.VisibleUI(UIType.UIHint, response.Error.GetTipInfo());
                }
                else
                {
                    Log.DebugBrown("本地点击的地图" + SceneComponent.Instance.CurrentSceneName.ToEnum<SceneName>().GetSceneName());
                    PlayerPrefs.SetString("pos", SceneComponent.Instance.CurrentSceneName.ToEnum<SceneName>().GetSceneName());
                    Close();
                }
            }
        }
        public void Close()
        {
            UIComponent.Instance.Remove(UIType.UISceneTranslate);
        }
        public override void Dispose()
        {
            if (this.IsDisposed)
                return;
            base.Dispose();

        }
    }
}