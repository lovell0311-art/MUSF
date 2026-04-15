using ETModel;
using MongoDB.Bson;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    public class MasterPoint
    {
        public long Id;//大师技能点id
        public string Name;
        public string skillType;//大师技能类型
        public long lev;//大师技能点 等级
        public long LayerLevel;//大师技能点 等级
        public string Describe;//介绍
        public int percenttage;//是否是百分数 1百分数 0不是百分数
        public List<int> FrontIds = new List<int>();//前置技能
        public List<int> LastIds = new List<int>();//上一阶技能
        public bool IsLearn = false;
        public int Unlock = 0;
        public int Consume = 0;
        public string OtherData;
    }

    [ObjectSystem]
    public class UIDashiComponentAwake : AwakeSystem<UIDashiComponent>
    {
        public override void Awake(UIDashiComponent self)
        {
            self.DashiInit();
        }
    }
    public class UIDashiComponent : Component, IUGUIStatus
    {
        public ReferenceCollector reference;
        public Dictionary<int, MasterPoint> masterDic = new Dictionary<int, MasterPoint>();
        /// <summary>
        /// 本地玩家
        /// </summary>
        RoleEntity RoleEntity_ => UnitEntityComponent.Instance.LocalRole;

        private int totalPoint;//当前大师属性的最高等级
        private int addPointId;//当前所点击的大师的id
        private GameObject curPointObj;

        public Transform Fashi, Jianshi, GongJianShou, MoJianShi, ShengDaoShi;
        public Transform ZhaoHuanSuShi,GeDouJia,MengHuanQiShi,FuWenFaShi,JiFeng,HuoQiangShou;
        private RectTransform panel;
        public Transform TipInfos;
        public Text Name,levelTxt,countTxt,exeTxt;

        public Text title, levelInfo, info, needTip, tip_1, tip_2, tip_3;
        public GameObject AddReturnBtn;
        //根据职业获得对应的大师技能
        Transform Curcontent;
        RoleConfig_ExperienceConfig experienceConfig;//经验配置表
        //根据不同的技能增加ID
        int skillName = 0;

        MasterPoint Curbattle = null;
        public void DashiInit()
        {
            reference = GetParent<UI>().GameObject.GetReferenceCollector();

            Name = reference.GetText("Name");
            levelTxt = reference.GetText("levelTxt");
            countTxt = reference.GetText("countTxt");
            exeTxt = reference.GetText("exeTxt");

            panel = reference.GetImage("Panel").rectTransform;
            TipInfos = reference.GetImage("TipInfos").transform;
            TipInfos.gameObject.SetActive(false);

            //获取职业
            Jianshi = reference.GetGameObject("Jianshi").transform;
            Fashi = reference.GetGameObject("Fashi").transform;
            GongJianShou = reference.GetGameObject("GongJianShou").transform;
            MoJianShi = reference.GetGameObject("MoJianShi").transform;
            ShengDaoShi = reference.GetGameObject("ShengDaoShi").transform;
            ZhaoHuanSuShi = reference.GetGameObject("ZhaoHuanSuShi").transform;
            GeDouJia = reference.GetGameObject("GeDouJia").transform;
            MengHuanQiShi = reference.GetGameObject("MengHuanQiShi").transform;
            FuWenFaShi = reference.GetGameObject("FuWenFaShi").transform;
            JiFeng = reference.GetGameObject("JiFeng").transform;
            HuoQiangShou = reference.GetGameObject("HuoQiangShou").transform;
            //默认隐藏
            Jianshi.gameObject.SetActive(false);
            Fashi.gameObject.SetActive(false);
            GongJianShou.gameObject.SetActive(false);
            MoJianShi.gameObject.SetActive(false);
            ShengDaoShi.gameObject.SetActive(false);
            ZhaoHuanSuShi.gameObject.SetActive(false);
            GeDouJia.gameObject.SetActive(false);
            MengHuanQiShi.gameObject.SetActive(false);
            FuWenFaShi.gameObject.SetActive(false);
            JiFeng.gameObject.SetActive(false);
            HuoQiangShou.gameObject.SetActive(false);
            //获取对应的角色列表
            GetContent(ref Curcontent);
            reference.GetButton("CloseBtn").onClick.AddSingleListener(delegate{ UIComponent.Instance.Remove(UIType.DaShiCanvas); });
            reference.GetButton("BG").onClick.AddSingleListener(delegate { if (TipInfos.gameObject.activeSelf) TipInfos.gameObject.SetActive(false); });

            Name.text = RoleEntity_.RoleName;

            AddReturnBtn = reference.GetGameObject("Btn");
            title = reference.GetText("title");
            levelInfo = reference.GetText("levelInfo");
            info = reference.GetText("info");
            needTip = reference.GetText("needTip");
            tip_1 = reference.GetText("tip_1");
            tip_2 = reference.GetText("tip_2");
            tip_3 = reference.GetText("tip_3");

            AddReturnBtn.transform.GetChild(0).GetComponent<Button>()?.onClick.AddListener(delegate{ AddMaterPoint().Coroutine(); });
            AddReturnBtn.transform.GetChild(2).GetComponent<Button>()?.onClick.AddListener(delegate{

                var confirm = UIConfirmComponentExtend.GetUIConfirmComponent();
                confirm.SetTipText($"是否花费{Curbattle.Unlock}魔晶解锁");
                confirm.AddActionEvent(() =>
                {
                    AddMaterPoint().Coroutine();
                });
            });
            AddReturnBtn.transform.GetChild(1).GetComponent<Button>()?.onClick.AddListener(delegate { TipInfos.gameObject.SetActive(false); });

            RegisterClickEvent();

            Curcontent.gameObject.SetActive(true);
        }
        public void SetSkillTitle()
        {
            if (Curcontent == null) return;
            ReferenceCollector reference = Curcontent.GetReferenceCollector();
            string title_1 = null, title_2 = null, title_3 = null, Title_4 = null;

            switch (RoleEntity_.RoleType)
            {
                case E_RoleType.Magician:
                    title_1 = "平稳";
                    title_2 = "智慧";
                    title_3 = "超越";
                    break;
                case E_RoleType.Swordsman:
                    title_1 = "保护";
                    title_2 = "勇猛";
                    title_3 = "愤怒";
                    break;
                case E_RoleType.Archer:
                    title_1 = "加护";
                    title_2 = "救援";
                    title_3 = "疾风";
                    break;
                case E_RoleType.Magicswordsman:
                    title_1 = "坚固";
                    title_2 = "斗志";
                    title_3 = "终结";
                    break;
                case E_RoleType.Holymentor:
                    title_1 = "坚决";
                    title_2 = "正义";
                    title_3 = "征服";
                    break;
                case E_RoleType.Summoner:
                    title_1 = "守护";
                    title_2 = "混沌";
                    title_3 = "荣誉";
                    break;
                case E_RoleType.Gladiator:
                    title_1 = "根恨";
                    title_2 = "意志";
                    title_3 = "破坏";
                    break;
                case E_RoleType.GrowLancer:
                    title_1 = "神圣";
                    title_2 = "惩处";
                    title_3 = "信念";
                    break;
                case E_RoleType.Runemage:
                    title_1 = "复仇";
                    title_2 = "残酷";
                    title_3 = "冷血"; 
                    break;
                case E_RoleType.StrongWind:
                    title_1 = "惩处";
                    title_2 = "严酷";
                    title_3 = "无情"; 
                    break;
                case E_RoleType.Gunners:
                    title_1 = "意识";
                    title_2 = "流血";
                    title_3 = "组织";
                    break;
                case E_RoleType.WhiteMagician:
                    break;
                case E_RoleType.WomanMagician:
                    break;
                default:
                    break;
            }
            Title_4 = "超神大师";
            reference.GetText("Title_1").text = $"{title_1}: {GetPointsByType(title_1)}";
            reference.GetText("Title_2").text = $"{title_2}: {GetPointsByType(title_2)}";
            reference.GetText("Title_3").text = $"{title_3}: {GetPointsByType(title_3)}";
            reference.GetText("Title_4").text = $"{Title_4}: {GetPointsByType(Title_4)}";
        }

        /// <summary>
        /// 总的加点数
        /// </summary>
        /// <param name="typename"></param>
        /// <returns></returns>
        private int GetPointsByType(string typename)
        {
            int count = 0;
            foreach (var item in masterDic)
            {
                if (item.Value.skillType == typename)
                    count += (int)item.Value.lev;

            }
            return count;
        }
        /// <summary>
        /// 注册大师属性点击事件
        /// </summary>
        /// <param name="content"></param>
        public void RegisterClickEvent()
        {

            
            if (Curcontent == null) return;
            int length = Curcontent.childCount;
            for (int i = 0; i < length; i++)
            {
                Button button;
                if (Curcontent.GetChild(i).GetComponent<Button>() == null)
                {
                    continue;
                }
                button = Curcontent.GetChild(i).GetComponent<Button>();
                int configId = int.Parse(button.name) + skillName;

                MasterPoint battleMasterInfos = new MasterPoint();
                if (configId > 2000)
                {
                    configId -= skillName;
                    configId.GetBattleMasterALL_RoleType_Ref(RoleEntity_.RoleType, ref battleMasterInfos);
                }
                else
                {
                    configId.GetBattleMasterInfos_RoleType_Ref(RoleEntity_.RoleType, ref battleMasterInfos);
                }
               
                if (battleMasterInfos.Id == 0)
                {
                    button.gameObject.SetActive(false);
                    continue;
                }
                battleMasterInfos.skillType = configId.GetSkillType(RoleEntity_.RoleType);
                masterDic.Add(configId, battleMasterInfos);
                button.transform.Find("count").GetComponent<Text>().text = masterDic[configId].LayerLevel.ToString();
                button.gameObject.GetComponent<Image>().color = new Color(255 / 255f, 255 / 255f, 255 / 255f, 25 / 255f);//设置默认状态
                button.onClick.AddSingleListener(() => { ClickEvent(button.gameObject, configId, battleMasterInfos); });

            }
        }
        /// <summary>
        /// 点击技能显示信息
        /// </summary>
        /// <param name="item"></param>
        /// <param name="id"></param>
        /// <param name="battleMasterInfos"></param>
        public void ClickEvent(GameObject item, int id, MasterPoint battleMasterInfos)
        {
            curPointObj = item;
            addPointId = id;
            Curbattle = battleMasterInfos;
            title.text = battleMasterInfos.Name;
            //Log.DebugPurple("附加值：" + battleMasterInfos.OtherData);
           // var values = JsonConvert.DeserializeObject<Dictionary<long, long>>(battleMasterInfos.OtherData);//附加数值

            var values =  battleMasterInfos.OtherData.StringToDictionary();
            if (masterDic.TryGetValue(id, out MasterPoint value))
            {
                if(values.Count == 0)
                {
                    levelInfo.text = $"{battleMasterInfos.LayerLevel}阶技能，技能等级{value.lev}/{10}";
                }
                else
                {
                    levelInfo.text = $"{battleMasterInfos.LayerLevel}阶技能，技能等级{value.lev}/{values.Count}";
                }
                
            }

            //Log.DebugGreen($"当前等级->{value.lev},满级->{values.Count}");

            int value1 = (int)value.lev;//技能等级
            if (value1 < 1)//0级可升级技能
            {
                value1 += 1;
                //显示下一等级
            }
            string infodata = value.Describe;
            if (values.TryGetValue(value1, out int value2))
            {
                if (value.percenttage == 2)
                {
                    if (value.Describe.Equals("{0:F}%"))
                    {
                        infodata = value.Describe.Replace("{0:F}", ((1f / value2) * 10000 * 100).ToString("F2"));
                    }
                    else
                    {
                        infodata = value.Describe.Replace("{0:F}", (value2/10000).ToString("F2"));
                    }
                    
                    infodata = infodata.Replace("/", "");
                }
                else if (value.percenttage == 1)
                {
                    infodata = value.Describe.Replace("{0:F}", ((float)value2 / 100).ToString());
                }else
                {
                    infodata = value.Describe.Replace("{0:F}", value2.ToString());
                }
            }
            info.text = infodata + '\n';
            //Log.DebugGreen($"{value.lev} + {values.Count}");
            if ((values.Count == 0 && value.lev != 0) || value.lev == values.Count && values.Count != 0)//满级
            {
                //Log.DebugGreen("满级");
                needTip.gameObject.SetActive(false);
                tip_1.gameObject.SetActive(false);
                tip_2.gameObject.SetActive(false);
                tip_3.gameObject.SetActive(false);
                AddReturnBtn.SetActive(false);
            }
            else
            {

                int count = 0;
                needTip.gameObject.SetActive(true);
                tip_1.gameObject.SetActive(true);
                tip_1.text = $"所需点数：{battleMasterInfos.Consume}点";
                //Log.DebugGreen(tip_1.text);

                

                if (battleMasterInfos.LastIds.Count < 1)//上一阶技能
                {
                    //Log.DebugGreen("上一阶技能为null");
                    tip_2.gameObject.SetActive(false);
                    tip_3.gameObject.SetActive(false);
                    if (addPointId > 2000)
                    {
                        AddReturnBtn.transform.Find("adBtn").gameObject.SetActive(battleMasterInfos.IsLearn);
                        AddReturnBtn.transform.Find("UnlockBtn").gameObject.SetActive(!battleMasterInfos.IsLearn);
                    }
                    else
                    {
                        AddReturnBtn.transform.Find("adBtn").gameObject.SetActive(true);
                        AddReturnBtn.transform.Find("UnlockBtn").gameObject.SetActive(false);
                    }
                    AddReturnBtn.SetActive(true);
                }
                else
                {
                    if (id < 2000)
                    {
                        tip_2.gameObject.SetActive(true);
                        for (int i = 0; i < battleMasterInfos.LastIds.Count; i++)
                        {
                            //Log.DebugGreen($"上一阶技能：{battleMasterInfos.LastIds[i]}");
                            if (masterDic.TryGetValue(battleMasterInfos.LastIds[i], out MasterPoint value3)) { count += (int)value3.lev; }
                            //Log.DebugGreen($"上一阶技能总数：{count}");
                            if (count < 10)//条件未满足
                            {
                                tip_2.text = $"<color=\"#FF0000\">需要10级以上的{battleMasterInfos.LayerLevel - 1}阶技能</color>";
                                AddReturnBtn.SetActive(false);
                            }
                            else
                            {
                                tip_2.text = $"需要10级以上的{battleMasterInfos.LayerLevel - 1}阶技能";
                                AddReturnBtn.SetActive(true);
                                AddReturnBtn.transform.Find("adBtn").gameObject.SetActive(true);
                                AddReturnBtn.transform.Find("UnlockBtn").gameObject.SetActive(false);
                            }
                        }
                    }
                    
                }
                //Log.DebugGreen($"前置技能大小：{battleMasterInfos.FrontIds.Count}");
                if (battleMasterInfos.FrontIds.Count < 1)//前置技能
                {
                    tip_3.gameObject.SetActive(false);
                    if(addPointId > 2000)
                    {
                        AddReturnBtn.transform.Find("adBtn").gameObject.SetActive(battleMasterInfos.IsLearn);
                        AddReturnBtn.transform.Find("UnlockBtn").gameObject.SetActive(!battleMasterInfos.IsLearn);
                    }

                }
                else
                {
                    //Log.DebugGreen($"前置技能：{battleMasterInfos.FrontIds[0]}");
                    tip_3.gameObject.SetActive(true);
                    if (masterDic.TryGetValue(battleMasterInfos.FrontIds[0], out MasterPoint value4))
                    {
                        //Log.DebugGreen($"前置技能：{battleMasterInfos.FrontIds[0]}");
                        if (value4.lev < 10)//条件不满足
                        {
                            tip_3.text = $"<color=\"#FF0000\">需要10级以上的\"{value4.Name}\"需要学习</color>";
                            AddReturnBtn.SetActive(false);
                        }
                        else
                        {
                            tip_3.text = $"需要10级以上的\"{value4.Name}\"需要学习";
                            if (count > 9)
                            {
                                AddReturnBtn.SetActive(true);
                                AddReturnBtn.transform.Find("UnlockBtn").gameObject.SetActive(addPointId > 2000);
                                AddReturnBtn.transform.Find("adBtn").gameObject.SetActive(addPointId < 2000);
                            }
                        }

                    }

                }
            }

            //提示加点信息框出现位置

            Vector3 screenPos = CameraComponent.Instance.UICamera.WorldToScreenPoint(item.transform.position);
            Vector2 pivot = Vector2.one * .5f;

            pivot.x = screenPos.x >= Screen.width / 2 ? 1 : 0;
            pivot.y = screenPos.y > Screen.height / 2 ? 1 : 0;

            TipInfos.GetComponent<RectTransform>().pivot = pivot;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(panel, screenPos, CameraComponent.Instance.UICamera, out Vector2 localPos);
            TipInfos.localPosition = localPos;
            TipInfos.gameObject.SetActive(true);
        }

        /// <summary>
        /// 加点
        /// </summary>
        /// <returns></returns>
        public async ETTask AddMaterPoint()
        {
           // Log.DebugGreen($"{masterDic[addPointId].pointId}加点之前{addPointId},等级:{masterDic[addPointId].lev}");
            //判断是否可以加点
            G2C_BattleMasterUpdateLevelResponse g2C_BattleMaster = (G2C_BattleMasterUpdateLevelResponse)await SessionComponent.Instance.Session.Call(new C2G_BattleMasterUpdateLevelRequest()
            {
                BattleMasterId = addPointId
            });
            if (g2C_BattleMaster.Error != 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_BattleMaster.Error.GetTipInfo());
            }
            else
            {
               
                countTxt.text = g2C_BattleMaster.PropertyPoint.ToString();
                foreach (var item in g2C_BattleMaster.Info)
                {
                    if (masterDic.ContainsKey(item.Key) && masterDic[item.Key].lev != item.Value)
                    {
                       // Log.DebugBrown($"{item.Key}加点成功,等级:{item.Value}");
                        masterDic[item.Key].lev = item.Value;
                    }
                }
                MasterPoint battleMasterInfos = new MasterPoint();
                if (addPointId > 2000)
                {
                    addPointId.GetBattleMasterALL_RoleType_Ref(RoleEntity_.RoleType, ref battleMasterInfos);
                }
                else
                {
                    addPointId.GetBattleMasterInfos_RoleType_Ref(RoleEntity_.RoleType, ref battleMasterInfos);
                }
                battleMasterInfos.IsLearn = true;
                masterDic[addPointId].IsLearn = true;
                //addPointId.GetBattleMasterInfos_RoleType_Out(RoleEntity_.RoleType, out MasterPoint battleMasterInfos);
                ClickEvent(curPointObj, addPointId, battleMasterInfos);
                SetTatenInfo();
                SetSkillTitle();
            }
        }
        //public Transform Fashi, Jianshi, GongJianShou, MoJianShi, ShengDaoShi;
        //public Transform ZhaoHuanSuShi, GeDouJia, MengHuanQiShi, FuWenFaShi, JiFeng, HuoQiangShou;
        public void GetContent(ref Transform content)
        {
            switch (RoleEntity_.RoleType)
            {
                case E_RoleType.Magician:
                    content = Fashi;
                    skillName = 0;
                    break;
                case E_RoleType.Swordsman:
                    content = Jianshi;
                    skillName = 100;
                    break;
                case E_RoleType.Archer:
                    content = GongJianShou;
                    skillName = 200;
                    break;
                case E_RoleType.Magicswordsman:
                    content = MoJianShi;
                    skillName = 300;
                    break;
                case E_RoleType.Holymentor:
                    content = ShengDaoShi;
                    skillName = 400;
                    break;
                case E_RoleType.Summoner:
                    content = ZhaoHuanSuShi;
                    skillName = 500;
                    break;
                case E_RoleType.Gladiator:
                    content = GeDouJia;
                    skillName = 600;
                    break;
                case E_RoleType.GrowLancer:
                    content = MengHuanQiShi;
                    skillName = 700;
                    break;
                case E_RoleType.Runemage:
                    content = FuWenFaShi;
                    skillName = 800;
                    break;
                case E_RoleType.StrongWind:
                    content = JiFeng;
                    skillName = 900;
                    break;
                case E_RoleType.Gunners:
                    content = HuoQiangShou;
                    skillName = 1000;
                    break;
                case E_RoleType.WhiteMagician:
                    break;
                case E_RoleType.WomanMagician:
                    break;
                default:
                    break;
            }
        }
        public void OnInVisibility()
        {
            
        }
        public void OnVisible(object[] data)
        {
            
        }
        public void OnVisible()
        {
            levelTxt.text = RoleEntity_.Level > 399 ? levelTxt.text = $"{RoleEntity_.Level - 399}" : levelTxt.text = $"{0}";
            countTxt.text = RoleEntity_.Level > 399 ? levelTxt.text = $"{RoleEntity_.Level - 399}" : levelTxt.text = $"{0}";
            exeTxt.text = $"0.00%";
           
            //Log.DebugGreen("请求大师技能");

            experienceConfig = ConfigComponent.Instance.GetItem<RoleConfig_ExperienceConfig>(RoleEntity_.Level);//根据玩家当前等级 得到当前等级升级到下一级 所需的最大经验值
            if (experienceConfig != null)
            {
                exeTxt.text = (RoleEntity_.Property.GetProperValue(E_GameProperty.Exprience) / (float)experienceConfig.Exprience).ToString("f2") + "%";//当前所拥有的经验/当前升级所需的最大经验值 =》经验进度条
            }
                
            //请求数据
            RequestDaShi().Coroutine();
        }

        public void SetTatenInfo()
        {
            if (Curcontent == null) return;
            int length = Curcontent.childCount;
            for (int i = 0; i < length; i++)
            {
                Button button = Curcontent.GetChild(i).GetComponent<Button>();
                if (Curcontent.GetChild(i).GetComponent<Button>() == null)
                {
                    continue;
                }
                int configId = int.Parse(button.name) + skillName;
                if (configId > 2000)
                {
                    configId -= skillName;
                    if (masterDic[configId].IsLearn)
                        button.gameObject.GetComponent<Image>().color = new Color(255 / 255f, 255 / 255f, 255 / 255f, 255 / 255f);//设置默认状态

                    Text item1 = Curcontent.GetChild(i).Find("count").gameObject.GetComponent<Text>();
                    if (masterDic.ContainsKey(configId))
                    {
                        item1.text = masterDic[configId].lev.ToString();
                    }
                    continue;
                }

                int count = 0;
                bool last = false;
                bool front = false;

                
                
                Text item = Curcontent.GetChild(i).Find("count").gameObject.GetComponent<Text>();
                if (masterDic.ContainsKey(configId))
                {
                    item.text = masterDic[configId].lev.ToString();
                }
                else { continue; }
                //如果大师当前技能又点数，直接显示
                //如果没有点数，就判断上一阶技能&前置技能是否满足条件
                if (masterDic[configId].lev > 0)
                {
                    button.gameObject.GetComponent<Image>().color = new Color(255 / 255f, 255 / 255f, 255 / 255f, 255 / 255f);//设置默认状态
                }
                else
                {
                    if (masterDic[configId].LastIds.Count >= 1)//上一阶技能
                    {
                        for (int j = 0; j < masterDic[configId].LastIds.Count; j++)
                        {
                            if (masterDic.TryGetValue(masterDic[configId].LastIds[j], out MasterPoint value3)) { count += (int)value3.lev; }
                            if (count >= 10)//条件满足
                            {
                                front = true;
                            }
                        }
                    }
                    else
                    {
                        front = true;
                    }
                    if (masterDic[configId].FrontIds.Count >= 1)//前置技能
                    {
                        if (masterDic.TryGetValue(masterDic[configId].FrontIds[0], out MasterPoint value4))
                        {
                            if (value4.lev >= 10)//条件满足
                            {
                                if (count > 9)
                                    last = true;
                            }

                        }
                    }
                    else
                    {
                        last = true;
                    }
                    if (front && last)
                    {
                        masterDic[configId].IsLearn = true;
                        button.gameObject.GetComponent<Image>().color = new Color(255 / 255f, 255 / 255f, 255 / 255f, 255 / 255f);//设置默认状态
                    }
                }
            }

         }
        public async ETTask RequestDaShi()
        {
            G2C_OpenBattleMasterResponse g2C_OpenBattleMaster = (G2C_OpenBattleMasterResponse)await SessionComponent.Instance.Session.Call(new C2G_OpenBattleMasterRequest() { });
            if (g2C_OpenBattleMaster.Error != 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, $"{g2C_OpenBattleMaster.Error.GetTipInfo()}");
            }
            else
            {
                countTxt.text = g2C_OpenBattleMaster.PropertyPoint.ToString();
                foreach (var item in g2C_OpenBattleMaster.Info)
                {
                    //Log.DebugGreen($"Key_Value:{item.Key}->{item.Value}");
                    if (masterDic.ContainsKey(item.Key))
                    {
                        masterDic[item.Key].lev = item.Value;
                        masterDic[item.Key].IsLearn = true;
                    }
                }
                SetTatenInfo();

                SetSkillTitle();
            }
        }

        public override void Dispose()
        {
            if (this.IsDisposed)
                return;
            base.Dispose();
        }
    }

}
