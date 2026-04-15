using System.Collections.Generic;
using UnityEngine;
using ETModel;
using UnityEngine.UI;
using System.Linq;
using ILRuntime.Runtime;


namespace ETHotfix
{
    [ObjectSystem]
    public class UIAddEquipMentsComponetAwake : AwakeSystem<UIAddEquipMentsComponet>
    {
        public override void Awake(UIAddEquipMentsComponet self)
        {
            self.collector = self.GetParent<UI>().GameObject.GetReferenceCollector();
            self.searchBar = self.collector.GetGameObject("SearchBar").GetComponent<SearchBar>();
            self.collector.GetButton("AddBtn").onClick.AddSingleListener(()=>{ self.AddEquipMentAsync().Coroutine(); });
            self.collector.GetButton("CloseBtn").onClick.AddSingleListener(()=>{ UIComponent.Instance.Remove(UIType.UIAddEquipMents); });
            self.dropdown = self.collector.GetImage("Dropdown").GetComponent<Dropdown>();

            self.Init();
        }
    }



    public class UIAddEquipMentsComponet : Component
    {
        public SearchBar searchBar;
        public Dropdown dropdown;
       
        public ReferenceCollector collector;
        List<string> libinfo = new List<string>();

        public long curEquipIndex=-1;//当前装备的ConfigID
        public int QiangHuaLev = 0;//强化等级
        public int Count = 1;//数量
        public int OptAtr = 0;//追加属性
        Dropdown Optdropdown;
        public int OptAtrLev = 0;//追加等级
        public int skillId = 0;//是否添加技能
        public int SuitID = -1;//套装ID
        public int kongshu = -1;//镶嵌孔数
        public List<int> Excellencelist = new List<int>();//卓越属性

        public List<int> FluoreSlotList = new List<int>();//镶嵌孔属性集合

        int curRoleType = 0;//默认全部职业
        int curEquipType = 0;//当卡装备类型

        public void Init() 
        {
            
            InitEquipType();
            DropDownAddListener();
            InitQiangHuaLev();
            InitCount();
            InitOpts();
            OptLev();
            AddSkill();
            InitSuit();
            InitExcellence();
            InitOther();
            InitClear();
            ChangeAtr();
            InlayKongShu();
            InitPet();
            InitRoleType();

            //初始化装备类型
            void InitEquipType() 
            {
                var equiptype = collector.GetImage("EquipType").GetComponent<Dropdown>();
                for (int i = 0; i < 17; i++)
                {
                    Dropdown.OptionData optionData = new Dropdown.OptionData() 
                    { 
                        text = $"{GetTypeName(i)}",
                    };
                    equiptype.options.Add(optionData);
                }
                equiptype.onValueChanged.AddListener((value)=>
                {
                    curEquipType = value;
                    AddConfig(GetAllIconfig(value));
                });

               
               
            }
            static string GetTypeName(int Index) => Index switch
            {
                1 => "装备",
                2 => "翅膀",
                3 => "项链",
                4 => "戒指",
                5 => "耳环",
                6 => "宝石",
                7 => "坐骑",
                8 => "荧光宝石",
                9 => "技能书/石",
                10 => "守护",
                11 => "消耗品(血瓶|药水|实力提神卷轴)",
                12 => "其他物品",
                13 => "任务物品",
                14 => "旗帜",
                15 => "手环",
                16 => "宠物",
                _ => string.Empty,
            };
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
                16 => ConfigComponent.Instance.GetAll<Item_PetConfig>(),
                _ => null
            };

            void AddConfig(IConfig[] configs)
            {
                searchBar.Clear();
                libinfo.Clear();
                dropdown.GetComponentInChildren<Text>().text = string.Empty;
                dropdown.options.Clear();
                dropdown.value = 1;
                curEquipIndex = -1;
                dropdown.options.Add(new Dropdown.OptionData { });
                foreach (var item in configs)
                {
                    item.Id.GetItemInfo_Out(out Item_infoConfig item_Info);
                    //过滤
                    if (!string.IsNullOrEmpty(item_Info.UseRole) && curRoleType != 0)
                    {
                        if (item_Info.UseRole.StringToDictionary().Keys.Contains(curRoleType) == false)
                        {
                            continue;
                        }
                    }

                    Dropdown.OptionData optionData = new Dropdown.OptionData
                    {
                        text = $"{item_Info.Id}_{item_Info.Name}"
                    };
                    dropdown.options.Add(optionData);
                    libinfo.Add(optionData.text);
                }
                searchBar.SetLibraryList(libinfo);
            }

            //添加监听
            void DropDownAddListener()
            {
                dropdown.onValueChanged.AddListener(delegate
                {
                    string strs = dropdown.options[dropdown.value].text;
                    strs = System.Text.RegularExpressions.Regex.Replace(strs.Split('_')[0], @"[^0-9]+", "");
                    curEquipIndex = int.Parse(strs);
                    UpdateOpts();
                    Excellencelist.Clear();
                });
            }
           
            //强化等级
            void InitQiangHuaLev() 
            {
                collector.GetInputField("QiangHuaLev").onValueChanged.AddSingleListener(value => 
                {
                    if (string.IsNullOrEmpty(value)) return;
                    QiangHuaLev =Mathf.Clamp(value.ToInt32(),0,15);
                });
            }
            //数量
            void InitCount() 
            {
                collector.GetInputField("Count").onValueChanged.AddSingleListener(value => 
                {
                 Count= Mathf.Clamp(value.ToInt32(),1, 255);
                }); ;
            }
            //追加属性
            void InitOpts() 
            {
                Optdropdown = collector.GetImage("Opt").GetComponent<Dropdown>();
                Optdropdown.onValueChanged.AddListener(value => 
                {
                    
                    OptAtr= GetValue(Optdropdown);
                  
                });

            }
            //更新追加属性
            void UpdateOpts() 
            {
               
                Optdropdown.options.Clear();
                Optdropdown.value = 0;
                curEquipIndex.GetItemInfo_Out(out Item_infoConfig info);
                if (info == null || info.AppendAttrId ==null|| info.AppendAttrId.Count == 0)
                {
                    OptAtr = 0;
                    return;
                }
                Optdropdown.options.Add(new Dropdown.OptionData {  });
                for (int i = 0, length = info.AppendAttrId.Count; i < length; i++)
                {
                    Optdropdown.options.Add(new Dropdown.OptionData { text = $"第 {i.ToString()} 条追加属性" }) ;
                }
            }

            ///追加等级
            void OptLev()
            {
                collector.GetImage("OptLev").GetComponent<Dropdown>().onValueChanged.AddListener(value => 
                {
                    OptAtrLev =value.ToInt32();
                });
            }
            //技能
            void AddSkill() 
            {
                collector.GetImage("Skill").GetComponent<Dropdown>().onValueChanged.AddListener(value => 
                {
                    skillId=value.ToInt32();
                });
            }
            ///初始化套装
            void InitSuit() 
            {
                Dropdown Suitdropdown = collector.GetImage("Suit").GetComponent<Dropdown>();
                Suitdropdown.onValueChanged.AddListener(delegate
                {
                    string strs = Suitdropdown.options[Suitdropdown.value].text;
                    strs = System.Text.RegularExpressions.Regex.Replace(strs, @"[^0-9]+", "");
                    SuitID = strs.ToInt32();
                });
                IConfig[] configs = ConfigComponent.Instance.GetAll<SetItem_TypeConfig>();
                foreach (SetItem_TypeConfig item in configs.Cast<SetItem_TypeConfig>())
                {
                    Suitdropdown.options.Add(new Dropdown.OptionData { text=$"{item.Id}_{item.SetName}"});
                }
            }
            //卓越属性
            void InitExcellence() 
            {
                Dropdown Excellencedropdown = collector.GetImage("Excellent").GetComponent<Dropdown>();
                Excellencedropdown.onValueChanged.AddListener(delegate 
                {

                    string strs = Excellencedropdown.options[Excellencedropdown.value].text;
                    if (string.IsNullOrEmpty(strs)) return;
                    strs = System.Text.RegularExpressions.Regex.Replace(strs.Split('_')[0], @"[^0-9]+", "");
                    int value = strs.ToInt32();
                    if (Excellencelist!=null/*&&Excellencelist.Count == 0*/)
                    {
                        if (!Excellencelist.Exists(r => r == value))
                        {
                            Excellencelist.Add(value);
                        }
                    }
                });
                IConfig[] configs = ConfigComponent.Instance.GetAll<ItemAttrEntry_ExcConfig>();
                foreach (ItemAttrEntry_ExcConfig item in configs)
                {
                    Excellencedropdown.options.Add(new Dropdown.OptionData { text=$"{item.Id}_{item.Name}"}) ;
                }
            }
            ///修改属性
            void ChangeAtr()
            {
                int E_GameProperty = 0;
                int E_GameProperty_Value = 0;

                Dropdown ChangeAtrDropdown= collector.GetImage("ChangeAtr").GetComponent<Dropdown>();
                ChangeAtrDropdown.onValueChanged.AddListener(delegate 
                {
                    string strs = ChangeAtrDropdown.options[ChangeAtrDropdown.value].text;
                    strs = System.Text.RegularExpressions.Regex.Replace(strs, @"[^0-9]+", "");
                    E_GameProperty = strs.ToInt32();
                });
                collector.GetInputField("AtrInput").onValueChanged.AddSingleListener(value => 
                {
                    if (string.IsNullOrEmpty(value)) return;
                    E_GameProperty_Value = value.ToInt32();
                });
                collector.GetButton("SureChange").onClick.AddSingleListener(() => 
                {
                    ChangeAtr().Coroutine();
                });

                async ETVoid ChangeAtr() 
                {

                    if (E_GameProperty == 0)
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint,"选择一个要修改的属性");
                        return;
                    }
                   
                        G2C_GMResponse g2C_GM = (G2C_GMResponse)await SessionComponent.Instance.Session.Call(new C2G_GMRequest
                        {
                            Command = "修改属性",
                            Parameter = new Google.Protobuf.Collections.RepeatedField<int> { E_GameProperty, E_GameProperty_Value },
                        });
                        if (g2C_GM.Error != 0)
                        {
                            Log.DebugRed($"GM:{g2C_GM.Message}");
                        }
                        else
                        {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, "修改属性 成功！");
                         
                        }

                }

            }
            //镶嵌空数
            void InlayKongShu() 
            {
                collector.GetInputField("镶嵌空数").onValueChanged.AddSingleListener(value => 
                {
                    if (string.IsNullOrEmpty(value)) return;
                    kongshu = Mathf.Clamp(int.Parse(value),0,5);
                    //随机 镶嵌孔属性
                    for (int i = 0; i < kongshu; i++)
                    {
                        // Optdropdown.options.Add(new Dropdown.OptionData { text = $"第 {i} 个孔" });
                        var s = RandomHelper.RandomNumber(1,7);
                        var e = RandomHelper.RandomNumber(1, 5);
                        var lev = RandomHelper.RandomNumber(0,10);
                        FluoreSlotList.Add((s*10000+e)*100+lev);
                       
                    }
                });
                collector.GetToggle("Toggle").onValueChanged.AddSingleListener(value => 
                {
                    if (value)
                    {
                        FluoreSlotList.Clear();
                        //随机 镶嵌孔属性
                        for (int i = 0; i < kongshu; i++)
                        {
                            // Optdropdown.options.Add(new Dropdown.OptionData { text = $"第 {i} 个孔" });
                            var s = RandomHelper.RandomNumber(1, 7);
                            var e = RandomHelper.RandomNumber(1, 5);
                            var lev = RandomHelper.RandomNumber(0, 10);
                            FluoreSlotList.Add((s * 10000 + e) * 100 + lev);

                        }
                    }
                    else
                    {
                        FluoreSlotList.Clear();
                      
                    }
                });
            }
            //其他操作
            void InitOther() 
            {
                int glodCoin=1000;
                string strs = string.Empty;
                Text text = collector.GetText("other (1)");
                Dropdown dropdownOther = collector.GetImage("Others").GetComponent<Dropdown>();
                dropdownOther.onValueChanged.AddListener(delegate 
                {
                    strs = dropdownOther.options[dropdownOther.value].text;
                    
                    ChangeTitle(strs);
                    
                });
                collector.GetInputField("GlodCoinInputField").onValueChanged.AddSingleListener(value => 
                {
                    if (string.IsNullOrEmpty(value)) return;
                    if (dropdownOther.options[dropdownOther.value].text == "等级数量")
                    {
                        if (value.ToInt32() > 399)
                        {
                            glodCoin = 399;
                        }
                    }
                    else
                    {
                        glodCoin = value.ToInt32();
                    }
                });

                collector.GetButton("ChangeAtrBtn").onClick.AddSingleListener(() => 
                {
                    if (string.IsNullOrEmpty(strs))
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint,"请先选择 类型");
                        return;
                    }
                    TestChangedLev(strs).Coroutine();
                });

                void ChangeTitle(string title)
                {
                    switch (title)
                    {
                        case "获取等级":
                            text.text = "等级数量";
                            break;
                        case "获取金币":
                            text.text = "金币数量";
                            break;
                        default:
                            text.text = "数量";
                            break;
                    }
                }

                async ETVoid TestChangedLev(string str)
                {
                    G2C_GMResponse g2C_GM = (G2C_GMResponse)await SessionComponent.Instance.Session.Call(new C2G_GMRequest
                    {
                        Command = str,
                        Parameter = new Google.Protobuf.Collections.RepeatedField<int> { glodCoin },
                    });
                    if (g2C_GM.Error != 0)
                    {
                        Log.DebugRed($"GM:{g2C_GM.Message}");
                    }
                    else
                    {
                       
                    }

                }
            }
            ///清理背包
            void InitClear()
            {
                collector.GetButton("ClearBtn").onClick.AddSingleListener(async () =>
                {
                    
                    for (int i = KnapsackItemsManager.KnapsackItems.Count-1; i>=0 ; i--)
                    {
                        G2C_DelBackpackItemResponse g2C_Del = (G2C_DelBackpackItemResponse)await SessionComponent.Instance.Session.Call(new C2G_DelBackpackItemRequest 
                        {
                            ItemUUID = KnapsackItemsManager.KnapsackItems.ElementAt(i).Key,
                            PosInSceneX = GetNearNode().x,
                            PosInSceneY = GetNearNode().z,
                        });
                        if (g2C_Del.Error != 0)
                        {
                            UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_Del.Error.GetTipInfo());
                        }
                        else
                        {
                           
                           
                        }
                       
                    }
                    KnapsackItemsManager.KnapsackItems.Clear();
                    KnapsackTools.Clean();
                   

                });
               
            }
            ///添加宠物
            void InitPet()
            {
                collector.GetButton("AddMonkeyPetBtn").onClick.AddSingleListener(() =>
                {
                    InsertPetsRequest(102).Coroutine();
                });
                collector.GetButton("AddRabbitPetBtn").onClick.AddSingleListener(() =>
                {
                    InsertPetsRequest(104).Coroutine();
                });
                collector.GetButton("AddAngelPetBtn").onClick.AddSingleListener(() =>
                {
                    InsertPetsRequest(103).Coroutine();
                });
                collector.GetButton("AddDragonPetBtn").onClick.AddSingleListener(() =>
                {
                    InsertPetsRequest(101).Coroutine();
                });
                collector.GetButton("AddTrialPetBtn").onClick.AddSingleListener(() =>
                {
                    InsertPetsRequest(100).Coroutine();
                });
                collector.GetButton("AddDuableDragonPetBtn").onClick.AddSingleListener(() =>
                {
                    InsertPetsRequest(105).Coroutine();
                });
                collector.GetButton("AddAnQiLaPetBtn").onClick.AddSingleListener(() =>
                {
                    InsertPetsRequest(106).Coroutine();
                });
            }
            /// <summary>
            /// 获得新宠物
            /// </summary>
            /// <param name="petsConfigID"></param>
            /// <returns></returns>
            async ETTask InsertPetsRequest(int petsConfigID)
            {
               
                G2C_InsertPetsResponse g2C_OpenPets = (G2C_InsertPetsResponse)await SessionComponent.Instance.Session.Call(new C2G_InsertPetsRequest()
                {
                    PetsConfigID = petsConfigID
                });
                if (g2C_OpenPets.Error != 0)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_OpenPets.Error.GetTipInfo());
                }
                //else
                //{
                //    InitPetList().Coroutine();
                //    Log.DebugGreen("获得新宠物成功");
                //}
            }

            void InitRoleType()
            {
                List<string> roletypeList = new List<string>() { "全部职业", "魔法师", "剑士", "弓箭手", "魔剑士", "圣导师", "召唤术士", "格斗家", "梦幻骑士" };
                var dropdown = collector.GetImage("RoleType").GetComponent<Dropdown>();
                dropdown.AddOptions(roletypeList);

                dropdown.onValueChanged.AddListener(value =>
                {
                    curRoleType = value;
                    if(curEquipType!=0)
                    AddConfig(GetAllIconfig(curEquipType));

                });

            }
        }

        public int GetValue(Dropdown dropdown)
        {
            string strs = dropdown.options[dropdown.value].text;
            strs = System.Text.RegularExpressions.Regex.Replace(strs, @"[^0-9]+", "");
            return int.Parse(strs);
        }

        public async ETVoid AddEquipMentAsync() 
        {
          
            if (curEquipIndex == 0) 
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint,"请先选择一件装备");
                return;
            }

            
            (curEquipIndex).GetItemInfo_Out(out Item_infoConfig Config);
        
            if (Config == null) return;
          //  Log.DebugGreen($"{Config?.Name}  X:{Config?.X} Y:{Config?.Y}");
           // Vector2Int? index = KnapsackTools.GetKnapsackItemUnoccupied(KnapsackTools.CreateGridData(0, 0, Config.X - 1, Config.Y - 1));
            Vector2Int index = KnapsackTools.GetKnapsackItemUnoccupied(KnapsackTools.CreateGridData(0, 0, Config.X - 1, Config.Y - 1));
            if (index.x == -1)
            {
                Log.DebugRed("拾取装备背包放不下:" + Config.Name);
                UIComponent.Instance.VisibleUI(UIType.UIHint, "背包空间不足");
                return;
            }
            Log.DebugGreen($"index:{index}");
            var atr = (RandomHelper.RandomNumber(1, 7) * 10000 + RandomHelper.RandomNumber(1, 5)) * 100 + QiangHuaLev;
            Log.DebugGreen($"请求添加：{(int)curEquipIndex}名字：{Config.Name}数量：{Count} 等级：{QiangHuaLev} 第几条追加属性：{OptAtr} 追加等级：{OptAtrLev} 是否有技能：{skillId} 套装ID:{SuitID} 卓越属性条数：{Excellencelist.Count} \n荧光宝石属性:{atr} 镶嵌孔数:{kongshu} ");
            for (int i = 0; i < Excellencelist.Count; i++)
            {

                Log.DebugBrown($"卓越属性ID:{Excellencelist[i]}");
            }
            G2C_AddBackpackItemResponse g2C_Add = (G2C_AddBackpackItemResponse)await SessionComponent.Instance.Session.Call(new C2G_AddBackpackItemRequest
            {
                ItemConfigID = (int)curEquipIndex,
                PosInBackpackX = index.x,//背包中的x坐标
                PosInBackpackY = index.y,
                Level=QiangHuaLev,
                Quantity = Count,//数量
                OptListId = OptAtr,//追加属性
                OptLevel = OptAtrLev,//追加等级
                HasSkill = skillId,//是否有技能
                SetId = SuitID,//套装ID
                OptionExcellent = new Google.Protobuf.Collections.RepeatedField<int> { Excellencelist},//卓越属性
                FluoreAttr = atr,
                FluoreSlotCount=kongshu,
                FluoreSlot=new Google.Protobuf.Collections.RepeatedField<int> { FluoreSlotList}
            });
            if (g2C_Add.Error != 0)
            {
                KnapsackTools.ChangeGridsStates((int)index.x, (int)index.y, index.x+ Config.X - 1,index.y+ Config.Y - 1, false);
                Log.DebugGreen($"物品添加失败：{g2C_Add.Error.GetTipInfo()}");
            }
            else
            {
                Log.DebugGreen($"物品添加成功-{Config.Name}");
                Excellencelist.Clear();
            }
        }

        public AstarNode GetNearNode()
        {

            AstarNode astarNode = null;
            for (int i = -5; i < 0; i++)
            {
                for (int j = -5; j < 5; j++)
                {

                    if (Mathf.Abs(i) <= 2 && Mathf.Abs(j) <= 2) continue;
                    var nearNode = UnitEntityComponent.Instance.LocalRole.CurrentNodePos;
                    //AstarNode node = AstarComponent.Instance.GetNodeVector(vector.x, vector.z);
                    AstarNode node = AstarComponent.Instance.GetNode(nearNode.x + i, nearNode.z + j);
                    if (node.isWalkable)
                    {
                        //判断该点是否有实体

                        if (IsNull(node) == false)
                        {
                            continue;
                        }

                        Log.DebugGreen($"附近的目标点：{node}");
                        return node;
                    }
                }
            }
            return astarNode;

            bool IsNull(AstarNode node)
            {
                List<KnapsackItemEntity> allentity = UnitEntityComponent.Instance.KnapsackItemEntityDic.Values.ToList();

                for (int k = 0; k < allentity.Count; k++)
                {
                    var item = allentity[k];
                    if (node.Compare(item.CurrentNodePos))
                    {
                        astarNode ??= node;
                        //当前格子有装备
                        return false;
                    }
                }
                return true;
            }
        }


      
    }
}
