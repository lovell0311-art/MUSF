
using ETModel;

using System.Collections.Generic;
using System.Linq;
using UnityEditor.Sprites;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class UIChallengeComponentAwake : AwakeSystem<UIChallengeComponent>
    {
        public override void Awake(UIChallengeComponent self)
        {
            self.Awake();
        }
    }
    public class UIChallengeComponent : Component
    {
        public UnityAction renderAction1;//renderTexture 渲染委托
        public ReferenceCollector collector;
        /// <summary>
        /// 怪物类
        /// </summary>
        public GameObject MonsterType;
        public UICircularScrollView<Item_infoConfig> uICircular_Item;
        public UICircularScrollView<EnemyConfig_Challenge> uICircular_Monster;
        public GameObject ItemContent, MonsterContent;
        public ScrollRect ItemView, MonsterScrollView;
        public List<EnemyConfig_Challenge> GoldEnemyConfigs = new List<EnemyConfig_Challenge>();
        public List<EnemyConfig_Challenge> BossEnemyConfigs = new List<EnemyConfig_Challenge>();
        public List<EnemyConfig_Challenge> CBTBossEnemyConfigs = new List<EnemyConfig_Challenge>();
        public List<EnemyConfig_Challenge> CurEnemyConfigs = new List<EnemyConfig_Challenge>();
        public List<EnemyConfig_Challenge> EliteConfigs = new List<EnemyConfig_Challenge>();//精英怪
        public List<Item_infoConfig> itemDropDatas = new List<Item_infoConfig>();
        public EnemyConfig_ChallengeConfig config_ChallengeConfig;
        public EnemyConfig ListEnemyConfig = new EnemyConfig();
        BossType SelectType = BossType.Gold;
        public List<DropItem_SpecialConfig> dropItem_Specials = new List<DropItem_SpecialConfig>();


        public GameObject MonsterIcon;
        public void Awake()
        {
            collector = GetParent<UI>().GameObject.GetReferenceCollector();
            MonsterType = collector.GetGameObject("MonsterType");
            ItemContent = collector.GetGameObject("ItemContent");
            MonsterContent = collector.GetGameObject("MonsterContent");
            ItemView = collector.GetImage("ItemScrollView").GetComponent<ScrollRect>();
            MonsterScrollView = collector.GetImage("MonsterScrollView").GetComponent<ScrollRect>();
            collector.GetButton("CloseBtn").onClick.AddSingleListener(() =>
            {
                UIComponent.Instance.Remove(UIType.UIChallenge);
            });
            MonsterIcon = ResourcesComponent.Instance.LoadGameObject("Montser_Config".StringToAB(), "Montser_Config");
            LoadSpecialConfig();// 加载特殊掉落物品表
            InitUICircular_Item();// 怪物掉落的装备列表初始化
            InitUICircular_Monster();// 怪物列表初始化
            InitMonster();// 加载怪物配置表资源
            LoadEnemyType();// 加载怪物类别
        }

        /// <summary>
        /// 加载特殊掉落物品表
        /// </summary>
        public void LoadSpecialConfig()
        {
            var dropItem_SpecialConfig = ConfigComponent.Instance.GetAll<DropItem_SpecialConfig>();
            foreach (var item in dropItem_SpecialConfig.Cast<DropItem_SpecialConfig>())
            {
                dropItem_Specials.Add(item);
            }
        }

        /// <summary>
        /// 加载怪物配置表资源
        /// </summary>
        public void InitMonster()
        {
            GoldEnemyConfigs.Clear();
            BossEnemyConfigs.Clear();
            CBTBossEnemyConfigs.Clear();
            EliteConfigs.Clear();
            var monsterConfig = ConfigComponent.Instance.GetAll<EnemyConfig_ChallengeConfig>();
            foreach (var item in monsterConfig.Cast<EnemyConfig_ChallengeConfig>())
            {
                EnemyConfig_Challenge config_Challenge = new EnemyConfig_Challenge();
                config_Challenge.config_ChallengeConfig = item;
                config_Challenge.IsSelect = false;
                switch (config_Challenge.config_ChallengeConfig.MonsterType)
                {

                    case 1:
                        GoldEnemyConfigs.Add(config_Challenge);

                        break;
                    case 2:
                        BossEnemyConfigs.Add(config_Challenge);
                        break;
                    case 3:
                        CBTBossEnemyConfigs.Add(config_Challenge);
                        Log.DebugGreen("3");
                        break;
                    case 4:
                        EliteConfigs.Add(config_Challenge);
                        Log.DebugGreen("4");
                        break;
                    default:
                        break;
                }
            }
            Log.DebugBrown("EliteConfigs" + EliteConfigs);
            GoldEnemyConfigs.First().IsSelect = true;
            BossEnemyConfigs.First().IsSelect = true;
            CBTBossEnemyConfigs.First().IsSelect = true;
            EliteConfigs.First().IsSelect = true;
            //uICircular_Monster.Items = GoldEnemyConfigs;
        }
        /// <summary>
        /// 加载怪物类别
        /// </summary>
        public void LoadEnemyType()
        {
            Toggle toggle = MonsterType.transform.GetChild(0).GetComponent<Toggle>();
            toggle.onValueChanged.AddSingleListener(isOn =>
            {
                if (!isOn) return;
                if (SelectType == BossType.Gold) return;
                TogsClickEvent(BossType.Gold);
            });
            Toggle toggle1 = MonsterType.transform.GetChild(1).GetComponent<Toggle>();
            toggle1.onValueChanged.AddSingleListener(isOn =>
            {
                if (!isOn) return;
                if (SelectType == BossType.Boss) return;
                TogsClickEvent(BossType.Boss);
            });
            Toggle toggle2 = MonsterType.transform.GetChild(2).GetComponent<Toggle>();
            toggle2.onValueChanged.AddSingleListener(isOn =>
            {
                if (!isOn) return;
                if (SelectType == BossType.CBT) return;
                TogsClickEvent(BossType.CBT);
            });
            Toggle toggle3 = MonsterType.transform.GetChild(3).GetComponent<Toggle>();
            toggle3.onValueChanged.AddSingleListener(isOn =>
            {
                if (!isOn) return;
                if (SelectType == BossType.Elite) return;
                TogsClickEvent(BossType.Elite);
            });
            TogsClickEvent(BossType.Gold);
        }
        public void TogsClickEvent(BossType type)
        {
            SelectType = type;
            switch (type)
            {
                case BossType.Gold:
                    CurEnemyConfigs = GoldEnemyConfigs;
                    break;
                case BossType.Boss:
                    CurEnemyConfigs = BossEnemyConfigs;
                    break;
                case BossType.CBT:
                    CurEnemyConfigs = CBTBossEnemyConfigs;
                    break;
                case BossType.Elite:
                    CurEnemyConfigs = EliteConfigs;
                    break;
                default:
                    break;
            }
            for (int i = 0, length = CurEnemyConfigs.Count; i < length; i++)
            {
                CurEnemyConfigs[i].IsSelect = i == 0 ? true : false;
            }
            uICircular_Monster.Items = CurEnemyConfigs;
        }
        /// <summary>
        /// 怪物掉落的装备列表初始化
        /// </summary>
        public void InitUICircular_Item()
        {
            uICircular_Item = ComponentFactory.Create<UICircularScrollView<Item_infoConfig>>();
            uICircular_Item.InitInfo(E_Direction.Horizontal, 1, 10, 0);
            uICircular_Item.ItemInfoCallBack = InitChallengeItem;
            uICircular_Item.IninContent(ItemContent, ItemView);
        }
        /// <summary>
        /// 怪物列表初始化
        /// </summary>
        public void InitUICircular_Monster()
        {
            uICircular_Monster = ComponentFactory.Create<UICircularScrollView<EnemyConfig_Challenge>>();
            uICircular_Monster.InitInfo(E_Direction.Horizontal, 1, 2, 0);
            uICircular_Monster.ItemInfoCallBack = InitChallengeMonster;
            uICircular_Monster.IninContent(MonsterContent, MonsterScrollView);
        }
        /// <summary>
        /// 物品列表
        /// </summary>
        public List<GameObject> itemList = new List<GameObject>();
        /// <summary>
        /// 装备列表
        /// </summary>
        /// <param name="item"></param>
        /// <param name="dataInfo"></param>
        private void InitChallengeItem(GameObject item, Item_infoConfig dataInfo)
        {

            if (item.transform.Find("CreatPoint").childCount > 1)
            {
                for (int i = 0; i < item.transform.Find("CreatPoint").childCount; i++)
                {

                    item.transform.Find("CreatPoint").GetChild(i).gameObject.SetActive(false);
                }
                //  ResourcesComponent.Instance.DestoryGameObjectImmediate(item.transform.Find("CreatPoint").GetChild(0).gameObject, item.transform.Find("CreatPoint").GetChild(0).name.StringToAB());
                //  GameObject.Destroy(item.transform.Find("CreatPoint").GetChild(0).gameObject);
            }
            // Log.DebugBrown("物品表" + dataInfo.ResName + "id"+item.name);
            item.transform.Find("Name").GetComponent<Text>().text = dataInfo.Name;
            if (dataInfo.ExcellentDropWeight > 0)
            {
                item.transform.Find("Name").GetComponent<Text>().color = Color.green;
            }
            else
            {
                item.transform.Find("Name").GetComponent<Text>().color = Color.white;
            }
            GameObject itemObj = ResourcesComponent.Instance.LoadGameObject(dataInfo.ResName.StringToAB(), dataInfo.ResName);//显示当前角色的模型
            itemObj.transform.SetParent(item.transform.Find("CreatPoint"));
            itemObj.transform.localPosition = new Vector3(0, 10, -50);
            itemObj.transform.localScale = Vector3.one * 50;
            itemObj.SetUI();
            itemList.Add(itemObj);
        }
        /// <summary>
        /// 怪物列表
        /// </summary>
        /// <param name="item"></param>
        /// <param name="dataInfo"></param>
        private void InitChallengeMonster(GameObject item, EnemyConfig_Challenge dataInfo)
        {

            item.transform.Find("Background").GetComponent<Button>().onClick.AddSingleListener(() =>
            {
                TogChange(item, dataInfo);
                SelectMonster(dataInfo);
            });
            item.transform.Find("Checkmark").gameObject.SetActive(dataInfo.IsSelect);
            item.transform.Find("RefreshTime").GetComponent<Text>().text = $"刷新时间：{dataInfo.config_ChallengeConfig.RefreshTime}";
            item.transform.Find("RefreshPlace").GetComponent<Text>().text = $"刷新地点：{dataInfo.config_ChallengeConfig.RefreshPlace}";

            EnemyConfig_InfoConfig enemyConfig_Info = ConfigComponent.Instance.GetItem<EnemyConfig_InfoConfig>(dataInfo.config_ChallengeConfig.MonsterId);
            if (enemyConfig_Info == null)
            {
                Log.Info("未查找到怪物 " + dataInfo.config_ChallengeConfig.MonsterId + " MonsterName = " + dataInfo.config_ChallengeConfig.MonsterName);
                return;
            }
            item.transform.Find("Level_Name").GetComponent<Text>().text = $"LV:{enemyConfig_Info.Lvl}-{enemyConfig_Info.Name}";
            if (dataInfo.IsSelect)
            {
                ListEnemyConfig.item = item;
                ListEnemyConfig.enemyConfig = dataInfo;
                SelectMonster(dataInfo);
            }
            Log.Debug("怪物" + enemyConfig_Info.ResName);
            Sprite initSprite = MonsterIcon.GetReferenceCollector().GetSprite($"{enemyConfig_Info.ResName}_UI");
            item.transform.Find("Icon").GetComponent<Image>().sprite = initSprite;
            item.transform.Find("Icon").GetComponent<Image>().SetNativeSize();
            item.transform.Find("Icon").localScale = Vector3.one * 0.5f;
        }
        public void SelectMonster(EnemyConfig_Challenge dataInfo)
        {
            itemDropDatas.Clear();
            for (int i = 0, length = itemList.Count; i < length; i++)
            {
                if (itemList[i] != null)
                {
                    GameObject.Destroy(itemList[i]);
                    //ResourcesComponent.Instance.DestoryGameObjectImmediate(itemList[i], itemList[i].name.StringToAB());
                }
            }
            itemList.Clear();
            // Log.DebugBrown("SelectMonster" + dataInfo.config_ChallengeConfig.MonsterType+":掉落"+ dataInfo.config_ChallengeConfig.Equip.Count);
            if (dataInfo.config_ChallengeConfig.MonsterType == 1)
            {
                //for (int i = 0,length = dataInfo.config_ChallengeConfig.Equip.Count; i < length; i++)
                //{

                //    ((long)dataInfo.config_ChallengeConfig.Equip[i]).GetItemInfo_Out(out Item_infoConfig item_Info);
                //    Log.DebugBrown("数据" + item_Info.Name + "id" + item_Info.ResName+":"+ dataInfo.config_ChallengeConfig.Equip[i]);
                //    itemDropDatas.Add(item_Info);
                //}
                for (int i = 0, length = dropItem_Specials.Count; i < length; i++)
                {
                    if (dropItem_Specials[i].TreasureChestId == dataInfo.config_ChallengeConfig.Equip[0])
                    {
                        ((long)dropItem_Specials[i].ItemId).GetItemInfo_Out(out Item_infoConfig item_Info);
                        // Log.DebugBrown("数据" + item_Info.Name + "id" + item_Info.ResName + ":" + dataInfo.config_ChallengeConfig.Equip[i]);
                        itemDropDatas.Add(item_Info);
                    }
                }
            }
            else
            {
                for (int i = 0, length = dropItem_Specials.Count; i < length; i++)
                {
                    if (dropItem_Specials[i].TreasureChestId == dataInfo.config_ChallengeConfig.Equip[0])
                    {
                        ((long)dropItem_Specials[i].ItemId).GetItemInfo_Out(out Item_infoConfig item_Info);
                        itemDropDatas.Add(item_Info);
                    }
                }
            }
            uICircular_Item.Items = itemDropDatas;
        }
        private void TogChange(GameObject item, EnemyConfig_Challenge dataInfo)
        {
            if (ListEnemyConfig == null) return;
            ListEnemyConfig.item.transform.Find("Checkmark").gameObject.SetActive(false);
            ListEnemyConfig.enemyConfig.IsSelect = false;
            item.transform.Find("Checkmark").gameObject.SetActive(true);
            dataInfo.IsSelect = true;
            ListEnemyConfig.item = item;
            ListEnemyConfig.enemyConfig = dataInfo;
        }
        public override void Dispose()
        {
            if (IsDisposed) return;
            uICircular_Item.Dispose();
            uICircular_Monster.Dispose();
        }
    }

}
