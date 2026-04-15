using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using System.Linq;
using UnityEngine.UI;
using LitJson;
using System.Text;
using static UnityEditor.Progress;
using System.Threading;
using Codice.CM.Common;

namespace ETHotfix
{

    public enum E_QueryType
    {
        Item,//物品查询
        Scene,//物品掉落
        Monster,//怪物查询
    }

    [ObjectSystem]
    public class UIChaXunComponentAwake : AwakeSystem<UIChaXunComponent>
    {
        public override void Awake(UIChaXunComponent self)
        {
            self.collector = self.GetParent<UI>().GameObject.GetReferenceCollector();
            self.collector.GetButton("CloseBtn").onClick.AddSingleListener(() =>
            {
                UIComponent.Instance.InVisibilityUI(UIType.UIChaXun);
            });

            self.InputField = self.collector.GetInputField("InputField");
            self.InputField.onValueChanged.AddSingleListener((value) => self.QueryEvent(value));


            self.searchBar = self.collector.GetGameObject("SearchBar").GetComponent<SearchBar>();
            self.dropdown = self.collector.GetImage("Dropdown").GetComponent<Dropdown>();

            self.collector.GetToggle("ItemChaXunToggle").onValueChanged.AddSingleListener(value => self.ChangeQueryType(value, E_QueryType.Item));
            self.collector.GetToggle("ItemDiaoluoToggle").onValueChanged.AddSingleListener(value => self.ChangeQueryType(value, E_QueryType.Scene));
            self.collector.GetToggle("MonsterToggle").onValueChanged.AddSingleListener(value => self.ChangeQueryType(value, E_QueryType.Monster));

            self.collector.GetButton("SearchBtn").onClick.AddSingleListener(self.Query);
            self.InitItemCustomDrop();
            self.InitItemInfos();
            self.DropDownAddListener();
            self.InitMonsterDrop();
            self.InitUICircularScrollView();
            self.InitSpecialInfos();
            self.InitSceneMonster();

            self.ChangeQueryType(true, E_QueryType.Item);
        }
    }

    /// <summary>
    /// 物品查询
    /// </summary>
    public class UIChaXunComponent : Component
    {
        public ReferenceCollector collector;
        public SearchBar searchBar;
        public Dropdown dropdown;
        List<string> libinfo = new List<string>();

        public InputField InputField;
        E_QueryType queryType = E_QueryType.Item;
        public string queryName;

        Dictionary<string, Item_infoConfig> ItemsDic = new Dictionary<string, Item_infoConfig>();
        //装备 等级 字典
        Dictionary<int, List<Item_infoConfig>> ItemsInfoDic = new Dictionary<int, List<Item_infoConfig>>();
        Dictionary<string, EnemyConfig_InfoConfig> MonsterInfoDic = new Dictionary<string, EnemyConfig_InfoConfig>();

        //怪物 等级 字典
        Dictionary<int, List<EnemyConfig_InfoConfig>> monsterDic = new Dictionary<int, List<EnemyConfig_InfoConfig>>();

        Dictionary<int, List<DropItem_SpecialConfig>> SpecialInfos = new Dictionary<int, List<DropItem_SpecialConfig>>();

        //场景 怪物
        Dictionary<string, List<SpawnPoint>> SceneMonsterDic = new Dictionary<string, List<SpawnPoint>>();

        //自定义掉落 集合
        List<ItemCustomDrop_InfoConfig> itemCustomDrop_InfoConfigs = new List<ItemCustomDrop_InfoConfig>();
        /// <summary>
        /// 新加
        /// </summary>
        List<DropItem_SpecialConfig> speciallist = new List<DropItem_SpecialConfig>();
        public ScrollRect InfoScrollrect;
        public GameObject InfoContent;
        public UICircularScrollView<string> InfoScrollView;
        List<string> infoList = new List<string>();

        public void InitItemCustomDrop()
        {
            IConfig[] configs = ConfigComponent.Instance.GetAll<ItemCustomDrop_InfoConfig>();
            foreach (ItemCustomDrop_InfoConfig item in configs.Cast<ItemCustomDrop_InfoConfig>())
            {
                itemCustomDrop_InfoConfigs.Add(item);
            }
        }
        /// <summary>
        /// 特殊掉落
        /// </summary>
        /// <param name="itemid"></param>
        /// <param name="list"></param>
        public void GetSpecial(int itemid,ref List<string> list)
        {
            Dictionary<long, int> dic = new Dictionary<long, int>();
            dic.Clear();
            foreach (DropItem_SpecialConfig infoConfig in speciallist)
            {
                if (infoConfig.ItemId == itemid)
                {
                    BoosEnemy_DropConfig enemyConfig_ = ConfigComponent.Instance.GetItem<BoosEnemy_DropConfig>(infoConfig.TreasureChestId);
                    if (!dic.ContainsKey(enemyConfig_.Id))
                    {
                        dic.Add(enemyConfig_.Id, 1);
                        list.Add($"{infoConfig.ItemName}--<color=green>{enemyConfig_.Name}</color>--及副本掉落");
                    }
                    //Log.DebugBrown("特殊掉落的组id：" + infoConfig.TreasureChestId + ":描述" + enemyConfig_.Name);
                    // list.Add($"{infoConfig.ItemName}-及副本掉落");
                }
            }
        }
        /// <summary>
        ///更具物品id 获取那些怪物会掉落
        /// </summary>
        /// <param name="itemid"></param>
        /// <param name="list"></param>
        public void GetItemDrop_Monster(int itemid, ref List<string> list)
        {
            Log.DebugBrown("当前查询的物品id：" + itemid);
            if (itemid== 340005)
            {
                itemid = 340008;
            }
            foreach (ItemCustomDrop_InfoConfig infoConfig in itemCustomDrop_InfoConfigs)
            {
                if (infoConfig.ItemId == itemid)
                {
                      Log.DebugBrown("掉落怪物的名字是："+infoConfig.MonsterName);
                    if (infoConfig.MonsterLevel.Count != 0)
                    {
                        //怪物 等级范围
                        for (int i = infoConfig.MonsterLevel[0]; i <= infoConfig.MonsterLevel[1]; i++)
                        {
                            if (monsterDic.TryGetValue(i, out List<EnemyConfig_InfoConfig> monsterlist))
                            {
                                foreach (var monster in monsterlist)
                                {
                                    list.Add($"{monster.Name}-{GetMonsterScene((int)monster.Id)}-{monster.Lvl}级");
                                }

                            }
                        }
                    }
                    else if (infoConfig.MonsterIdRange.Count != 0)
                    {
                        //怪物 ID范围
                        for (int i = infoConfig.MonsterIdRange[0]; i <= infoConfig.MonsterIdRange[1]; i++)
                        {
                            EnemyConfig_InfoConfig enemyConfig_ = ConfigComponent.Instance.GetItem<EnemyConfig_InfoConfig>(i);
                            list.Add($"{enemyConfig_.Name}-{GetMonsterScene((int)enemyConfig_.Id)}-{enemyConfig_.Lvl}级");
                        }
                    }
                    else
                    {
                        EnemyConfig_InfoConfig enemyConfig_ = ConfigComponent.Instance.GetItem<EnemyConfig_InfoConfig>(infoConfig.MonsterId);
                        list.Add($"{infoConfig.MonsterName}-{GetMonsterScene((int)infoConfig.MonsterId)}-{enemyConfig_.Lvl}级");
                    }
                }
            }


            //---------

         

        }

        string GetMonsterScene(int monsterID)
        {

            StringBuilder stringBuilder = new StringBuilder();
            foreach (var item in SceneMonsterDic)
            {
                foreach (var monsterInfo in item.Value)
                {
                    if (monsterInfo.Index == monsterID)
                    {
                        stringBuilder.Append($"<color=green>{item.Key.Split('_')[0].ToEnum<SceneName>().GetSceneName()}</color>\t");
                        break;
                        //str =$"<color=green>{item.Key.Split('_')[0].ToEnum<SceneName>().GetSceneName()}</color>";

                    }
                }

            }
            return stringBuilder.ToString(); ;

        }
        //更具怪物ID 获取会掉落那些装备
        public void GetItemDrop_Item(int monsterid, ref List<string> list)
        {
            //  Log.DebugGreen("获取自定掉落："+monsterid);
            foreach (ItemCustomDrop_InfoConfig infoConfig in itemCustomDrop_InfoConfigs)
            {
                if (infoConfig.MonsterId == monsterid)
                {
                    list.Add(infoConfig.ItemName);
                    //  Log.DebugGreen("获取自定掉落：" + infoConfig.ItemName);
                }
            }

        }


        //怪物分布
        public void InitSceneMonster()
        {
            foreach (var monster in UIMainComponent.Instance.MonsterInfoDic)
            {
                SceneMonsterDic[monster.Key] = JsonMapper.ToObject<SpawnPoint[]>(monster.Value).ToList();
            }
        }

        //初始化特殊掉落物品
        public void InitSpecialInfos()
        {
            IConfig[] configs = ConfigComponent.Instance.GetAll<DropItem_SpecialConfig>();
            foreach (DropItem_SpecialConfig info in configs.Cast<DropItem_SpecialConfig>())
            {
                speciallist.Add(info);
                if (SpecialInfos.TryGetValue(info.TreasureChestId, out List<DropItem_SpecialConfig> list))
                {
                    
                    list.Add(info);
                }
                else
                {
                    SpecialInfos[info.TreasureChestId] = new List<DropItem_SpecialConfig>() { info };
                }
            }
        }


        public void DropDownAddListener()
        {
            dropdown.onValueChanged.AddListener((value) =>
            {
                QueryEvent(dropdown.options[value].text);
                Query();
            });
        }
        public void InitUICircularScrollView()
        {
            InfoScrollrect = collector.GetImage("Scroll View").GetComponent<ScrollRect>();
            InfoContent = collector.GetGameObject("Content");
            InfoScrollView = ComponentFactory.Create<UICircularScrollView<string>>();
            InfoScrollView.ItemInfoCallBack = InfoCallBack;
            InfoScrollView.InitInfo(E_Direction.Vertical, 1, 0, 10);
            InfoScrollView.IninContent(InfoContent, InfoScrollrect);
        }
        public void InfoCallBack(GameObject obj, string info)
        {
            obj.GetComponent<Text>().text = info;
        }

        public void Query()
        {
            if (string.IsNullOrEmpty(queryName))
            {
                return;
            }
            switch (queryType)
            {
                case E_QueryType.Item:

                    QueryItem(queryName);
                    break;
                case E_QueryType.Scene:
                    break;
                case E_QueryType.Monster:
                    QueryMonster(queryName);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="queryName">被查询的名字</param>
        public void QueryEvent(string queryName)
        {
            this.queryName = queryName;
        }
        public void ChangeQueryType(bool isOn, E_QueryType queryType)
        {
            if (!isOn) return;
            infoList.Clear();
            InfoScrollView.Items = infoList;
            this.queryType = queryType;
          //  Log.DebugBrown("当前查询的类型" + queryType);
            switch (queryType)
            {
                case E_QueryType.Item:
                    searchBar.Clear();
                    libinfo.Clear();
                    dropdown.GetComponentInChildren<Text>().text = string.Empty;
                    dropdown.ClearOptions();
                    libinfo.AddRange(ItemsDic.Keys.ToList());
                    dropdown.AddOptions(ItemsDic.Keys.ToList());
                    searchBar.SetLibraryList(libinfo);
                    break;

                case E_QueryType.Scene:
                    break;
                case E_QueryType.Monster:
                    searchBar.Clear();
                    libinfo.Clear();
                    dropdown.GetComponentInChildren<Text>().text = string.Empty;
                    dropdown.ClearOptions();
                    libinfo.AddRange(MonsterInfoDic.Keys.ToList());
                    dropdown.AddOptions(MonsterInfoDic.Keys.ToList());
                    searchBar.SetLibraryList(libinfo);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 物品查询
        /// </summary>
        /// <param name="itemName"></param>
        public void QueryItem(string itemName)
        {
            infoList.Clear();
            Log.DebugGreen($"{itemName}->{ItemsDic.ContainsKey(itemName)}");
            if (ItemsDic.TryGetValue(itemName, out Item_infoConfig item_Info))
            {
                if (item_Info.Id / 10000 == 28)//宝石
                {
                    QueryGemDrop(itemName);
                    return;
                }

                GetItemDrop_Monster((int)item_Info.Id, ref infoList);
                if (infoList.Count==0)
                {
                    GetSpecial((int)item_Info.Id, ref infoList);
                    if (infoList.Count==0)
                    {
                        infoList.Add("未查询到相关获取路径");
                    }
                    InfoScrollView.Items = infoList;
                }
                //if (item_Info.Drop == 0)
                //{
                //    infoList.Add("不可掉落");
                //    InfoScrollView.Items = infoList;
                //}
                else
                {

                    //白装
                    if ((item_Info.NormalDropWeight + item_Info.AppendDropWeight + item_Info.SkillDropWeight + item_Info.LuckyDropWeight) > 0)
                    {
                        infoList.Add("白装:");
                       // Log.DebugBrown("白装装备");
                        //物品正负8级 的怪物掉落
                        for (int i = item_Info.DropLevel - 8; i <= item_Info.DropLevel + 8; i++)
                        {
                            if (monsterDic.TryGetValue(i, out List<EnemyConfig_InfoConfig> monsterList))
                            {
                                foreach (var monster in monsterList)
                                {
                                    // infoList.Add($"{monster.Name}-{monster.Lvl}级");
                                    bool isbreak = false;
                                    foreach (var item in SceneMonsterDic)
                                    {
                                        foreach (var monsterInfo in item.Value)
                                        {
                                            if (monsterInfo.Index == monster.Id)
                                            {

                                                infoList.Add($"{monster.Name}-<color=green>{item.Key.Split('_')[0].ToEnum<SceneName>().GetSceneName()}</color>[{monsterInfo.PositionX},{monsterInfo.PositionY}]-{monster.Lvl}级");
                                                isbreak = true;
                                                break;
                                            }
                                        }
                                        if (isbreak)
                                        {
                                            break;
                                        }

                                    }

                                }

                            }
                        }

                    }
                    //卓越
                    if (item_Info.ExcellentDropWeight > 0)
                    {
                       // Log.DebugBrown("卓越装备");
                        infoList.Add($"<color={ColorTools.ExcellenceItemColor}>卓越:</color>");

                        //物品正负26级 的怪物掉落
                        for (int i = item_Info.DropLevel + 26 - 8; i <= item_Info.DropLevel + 26 + 8; i++)
                        {
                            if (monsterDic.TryGetValue(i, out List<EnemyConfig_InfoConfig> monsterList))
                            {
                                foreach (var monster in monsterList)
                                {
                                    // infoList.Add($"{monster.Name}-{monster.Lvl}级");
                                    bool isbreak = false;
                                    foreach (var item in SceneMonsterDic)
                                    {
                                        foreach (var monsterInfo in item.Value)
                                        {
                                            if (monsterInfo.Index == monster.Id)
                                            {
                                                //Log.DebugBrown("数据a" + monster.Name);
                                                //Log.DebugBrown("数据aa" + item.Key);
                                                //Log.DebugBrown("数据b" + item.Key.Split('_')[0].ToEnum<SceneName>().GetSceneName());
                                                //Log.DebugBrown("数据c" + monsterInfo.PositionX+":::"+ monsterInfo.PositionY);
                                                infoList.Add($"<color={ColorTools.ExcellenceItemColor}>{monster.Name}</color>-<color=green>{item.Key.Split('_')[0].ToEnum<SceneName>().GetSceneName()}</color>[{monsterInfo.PositionX},{monsterInfo.PositionY}]-{monster.Lvl}级");



                                                isbreak = true;
                                                break;
                                            }
                                        }
                                        if (isbreak)
                                        {
                                            break;
                                        }

                                    }

                                }

                            }
                        }

                    }
                    //套装
                    if (item_Info.SetDropWeight > 0)
                    {
                      //  Log.DebugBrown("套装");
                        infoList.Add($"<color={ColorTools.ItemSetNameColor}>套装:</color>");

                        //物品正负36级 的怪物掉落
                        for (int i = item_Info.DropLevel + 36 - 8; i <= item_Info.DropLevel + 36 + 8; i++)
                        {
                            if (monsterDic.TryGetValue(i, out List<EnemyConfig_InfoConfig> monsterList))
                            {
                                foreach (var monster in monsterList)
                                {
                                    // infoList.Add($"{monster.Name}-{monster.Lvl}级");
                                    bool isbreak = false;
                                    foreach (var item in SceneMonsterDic)
                                    {
                                        foreach (var monsterInfo in item.Value)
                                        {
                                            if (monsterInfo.Index == monster.Id)
                                            {

                                                infoList.Add($"<color={ColorTools.ItemSetNameColor}>{monster.Name}</color>-<color=green>{item.Key.Split('_')[0].ToEnum<SceneName>().GetSceneName()}</color>[{monsterInfo.PositionX},{monsterInfo.PositionY}]-{monster.Lvl}级");
                                                isbreak = true;
                                                break;
                                            }
                                        }
                                        if (isbreak)
                                        {
                                            break;
                                        }

                                    }

                                }

                            }
                        }

                    }
                    //镶嵌装备
                    if (item_Info.SocketDropWeight > 0)
                    {
                       // Log.DebugBrown("镶嵌装备");
                        infoList.Add($"<color={ColorTools.Suit_Atr_Effective}>镶嵌装备:</color>");

                        //物品正负8级 的怪物掉落
                        for (int i = item_Info.DropLevel - 8; i <= item_Info.DropLevel + 8; i++)
                        {
                            if (monsterDic.TryGetValue(i, out List<EnemyConfig_InfoConfig> monsterList))
                            {
                                foreach (var monster in monsterList)
                                {
                                    // infoList.Add($"{monster.Name}-{monster.Lvl}级");
                                    bool isbreak = false;
                                    foreach (var item in SceneMonsterDic)
                                    {
                                        foreach (var monsterInfo in item.Value)
                                        {
                                            if (monsterInfo.Index == monster.Id)
                                            {

                                                infoList.Add($"<color={ColorTools.RoleNameNormalColor}>{monster.Name}</color>-<color=green>{item.Key.Split('_')[0].ToEnum<SceneName>().GetSceneName()}</color>[{monsterInfo.PositionX},{monsterInfo.PositionY}]-{monster.Lvl}级");
                                                isbreak = true;
                                                break;
                                            }
                                        }
                                        if (isbreak)
                                        {
                                            break;
                                        }

                                    }

                                }

                            }
                        }

                    }
                    InfoScrollView.Items = infoList;
                }
            }
        }

        //宝石掉落
        public void QueryGemDrop(string gemName)
        {
            IConfig[] configs = ConfigComponent.Instance.GetAll<EnemyConfig_DropConfig>();
            infoList.Clear();

            foreach (EnemyConfig_DropConfig info in configs.Cast<EnemyConfig_DropConfig>())
            {
                EnemyConfig_DropConfig enemyConfig_Drop = null;
                if (gemName.Contains("光"))
                {
                    if (info.LightStone != 0)
                    {
                        enemyConfig_Drop = info;

                    }
                }
                else if (gemName.Contains("玛雅"))
                {
                    if (info.MayaStone != 0)
                    {
                        enemyConfig_Drop = info;

                    }
                }
                else if (gemName.Contains("祝福"))
                {
                    if (info.BlessingGem != 0)
                    {
                        enemyConfig_Drop = info;

                    }
                }
                else if (gemName.Contains("灵魂"))
                {
                    if (info.SoulGem != 0)
                    {
                        enemyConfig_Drop = info;

                    }
                }
                else if (gemName.Contains("生命"))
                {
                    if (info.LifeGem != 0)
                    {
                        enemyConfig_Drop = info;

                    }
                }
                else if (gemName.Contains("创造"))
                {
                    if (info.CreateGem != 0)
                    {
                        enemyConfig_Drop = info;

                    }
                }
                else if (gemName.Contains("守护"))
                {
                    if (info.GuardGem != 0)
                    {
                        enemyConfig_Drop = info;

                    }
                }
                else if (gemName.Contains("再生"))
                {
                    if (info.ReviveOriginalStone != 0)
                    {
                        enemyConfig_Drop = info;

                    }
                }
                else if (gemName.Contains("幸运"))
                {
                    if (info.LuckyGem != 0)
                    {
                        enemyConfig_Drop = info;

                    }
                }
                else if (gemName.Contains("卓越"))
                {
                    if (info.ExcellentGem != 0)
                    {
                        enemyConfig_Drop = info;

                    }
                }
                if (enemyConfig_Drop == null) continue;
                if (monsterDic.TryGetValue(enemyConfig_Drop.MonsterLevel, out List<EnemyConfig_InfoConfig> monsterList))
                {
                    foreach (var monster in monsterList)
                    {
                        bool isbreak = false;
                        foreach (var item in SceneMonsterDic)
                        {
                            foreach (var monsterInfo in item.Value)
                            {
                                if (monsterInfo.Index == monster.Id)
                                {

                                    infoList.Add($"{monster.Name}-<color=green>{item.Key.Split('_')[0].ToEnum<SceneName>().GetSceneName()}</color>[{monsterInfo.PositionX},{monsterInfo.PositionY}]-{monster.Lvl}级");
                                    isbreak = true;
                                    break;
                                }
                            }
                            if (isbreak)
                            {
                                break;
                            }

                        }

                    }
                }
                InfoScrollView.Items = infoList;
            }
        }

        //怪物掉落查询
        public void QueryMonster(string monsterName)
        {
            Debug.Log("查询怪物掉落");
            if (MonsterInfoDic.TryGetValue(monsterName, out EnemyConfig_InfoConfig enemyConfig))
            {
                infoList.Clear();

                StringBuilder stringBuilder = new StringBuilder();
                foreach (var item in SceneMonsterDic)
                {
                    foreach (var monsterInfo in item.Value)
                    {
                        if (monsterInfo.Index == enemyConfig.Id)
                        {
                            stringBuilder.Append($"<color=green>{item.Key.Split('_')[0].ToEnum<SceneName>().GetSceneName()}</color>、");

                            break;
                        }
                    }

                }
                infoList.Add($"{enemyConfig.Name}\t{stringBuilder.ToString()}\t{enemyConfig.Lvl}级");

                GetItemDrop_Item((int)enemyConfig.Id, ref infoList);

                if (enemyConfig.Monster_Type != 0)
                {
                    //特殊掉落组
                    if (SpecialInfos.TryGetValue(enemyConfig.SpecialDrop, out List<DropItem_SpecialConfig> list))
                    {
                        foreach (DropItem_SpecialConfig item in list.Cast<DropItem_SpecialConfig>())
                        {
                            infoList.Add(item.ItemName);
                        }
                    }
                }
                else
                {
                    for (int i = enemyConfig.Lvl - 8; i <= enemyConfig.Lvl + 8; i++)
                    {
                        if (ItemsInfoDic.TryGetValue(i, out List<Item_infoConfig> list))
                        {
                            foreach (var item in list.Cast<Item_infoConfig>())
                            {
                                if (item.Drop == 0 || (item.NormalDropWeight + item.AppendDropWeight + item.SkillDropWeight + item.LuckyDropWeight + item.ExcellentDropWeight + item.SetDropWeight + item.SocketDropWeight) == 0)
                                {
                                    continue;
                                }
                                infoList.Add(item.Name);
                                // Log.DebugGreen("等级："+i+""+item.Name+" "+item.Drop);
                            }
                        }
                    }
                }
                InfoScrollView.Items = infoList;
            }
        }

        /// <summary>
        /// 初始化装备信息
        /// </summary>
        public void InitItemInfos()
        {

            for (int i = 1; i < 16; i++)
            {
                AddConfig(GetAllIconfig(i));
            }

            void AddConfig(IConfig[] configs)
            {

                foreach (var item in configs)
                {
                    item.Id.GetItemInfo_Out(out Item_infoConfig item_Info);
                    // if (item_Info.DropLevel == 0) continue;
                    if (item_Info.Name == null)
                    {
                        continue;
                    }
                    if (ItemsDic.ContainsKey(item_Info.Name) == false)
                    {
                        ItemsDic.Add(item_Info.Name, item_Info);
                    }

                    //缓存装备等级 
                    if (ItemsInfoDic.TryGetValue(item_Info.DropLevel, out List<Item_infoConfig> itemList))
                    {
                        if (item_Info.Drop != 0)
                        {
                            itemList.Add(item_Info);
                        }
                    }
                    else
                    {
                        if (item_Info.Drop != 0)
                        {
                            ItemsInfoDic[item_Info.DropLevel] = new List<Item_infoConfig>() { item_Info };
                        }
                    }
                }

            }
            ///获取配置表
            IConfig[] GetAllIconfig(int Index) => Index switch
            {
                1 => ConfigComponent.Instance.GetAll<Item_EquipmentConfig>(),
                2 => ConfigComponent.Instance.GetAll<Item_WingConfig>(),
                3 => ConfigComponent.Instance.GetAll<Item_NecklaceConfig>(),
                4 => ConfigComponent.Instance.GetAll<Item_RingsConfig>(),
                5 => ConfigComponent.Instance.GetAll<Item_DanglerConfig>(),
                6 => ConfigComponent.Instance.GetAll<Item_GemstoneConfig>(),
                7 => ConfigComponent.Instance.GetAll<Item_MountsConfig>(),
                8 => ConfigComponent.Instance.GetAll<Item_FGemstoneConfig>(),
                9 => ConfigComponent.Instance.GetAll<Item_SkillBooksConfig>(),
                10 => ConfigComponent.Instance.GetAll<Item_GuardConfig>(),
                11 => ConfigComponent.Instance.GetAll<Item_ConsumablesConfig>(),
                12 => ConfigComponent.Instance.GetAll<Item_OtherConfig>(),
                13 => ConfigComponent.Instance.GetAll<Item_TaskConfig>(),
                14 => ConfigComponent.Instance.GetAll<Item_FlagConfig>(),
                15 => ConfigComponent.Instance.GetAll<Item_BraceletConfig>(),
                _ => null
            };
        }

        public void InitMonsterDrop()
        {
            //根据等级将怪物分类
            IConfig[] configs = ConfigComponent.Instance.GetAll<EnemyConfig_InfoConfig>();
            foreach (EnemyConfig_InfoConfig monsterInfo in configs.Cast<EnemyConfig_InfoConfig>())
            {
                if (string.IsNullOrEmpty(monsterInfo.ResName)) continue;
                MonsterInfoDic[monsterInfo.Name] = monsterInfo;//初始化怪物


                if (monsterDic.TryGetValue(monsterInfo.Lvl, out List<EnemyConfig_InfoConfig> monsterList))
                {
                    monsterList.Add(monsterInfo);
                }
                else
                {
                    monsterDic[monsterInfo.Lvl] = new List<EnemyConfig_InfoConfig>() { monsterInfo };
                }
            }

        }


        public override void Dispose()
        {
            if (this.IsDisposed) return;
            InfoScrollView.Dispose();
            base.Dispose();
        }
    }
}