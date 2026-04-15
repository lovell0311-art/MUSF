using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    public partial class UIKnapsackNewComponent : Component, IUGUIStatus
    {
        public ReferenceCollector collector;
        public GameObject plane;
        public bool isBuildingView;
        public int pendingVisualRefreshFrames;
        public E_KnapsackState curKnapsackState = E_KnapsackState.KS_Equipment;//当前所选显示的面板(默认装备面板)

        public Text TopTitle, TopTipSucceed, TopTipMoney, FailedDeleTip, Title_Txt;
        public Text Tips, TipsTxt;
        public Button MergeBtn;
        public RoleEntity roleEntity => UnitEntityComponent.Instance.LocalRole;//本地玩家
        public RoleEquipmentComponent EquipmentComponent=new RoleEquipmentComponent();
        public RoleEntity RoleEntity => UnitEntityComponent.Instance.LocalRole;//本地玩家
        public UIConfirmComponent confirmComponent;

        public const int LENGTH_Knapsack_X = 8;//列数(LEGNTH_X)
        public const int LENGTH_Knapsack_Y = 12;//行数(LEGNTH_Y)
        public int ringSlot = 11;//已经穿戴的戒指卡槽

        public bool IsSplit = false;


        public UIIntroductionComponent uIIntroduction;//物品简介组件

        public KnapsackGridData curChooseArea;//当前所选择的格子区域(目标区域)
        public KnapsackGridData originArea;//被选择的格子区域（起始区域）
        public bool isDroping = false;//是否拖动中
        public GameObject curDropObj;//当前正在拖拽的物品
        public Vector3 originObjPos = Vector3.zero;//起始物品的坐标
        public Quaternion originObjRotation = Quaternion.identity;//起始物品的角度
                                                                  //是否使用垂直布局属性显示
        public bool isVertical = true;

        #region package
        public ReferenceCollector packageCollector;
        public KnapsackNewGrid[][] BackGrids;//背包格子交错数组
        public KnapsackNewGrid[][] grids = null;
        public int LENGTH_X = 0;
        public int LENGTH_Y = 0;
        public Text coinText;//金币数量
        public Text ruibiText;//元宝数量
        public Text qijibiText;//奇迹币数量

        public KnapsackNewGrid[][] FinishGrids = null;
        public bool IsCanFinish = true;
        public List<ItemPositionInBackpack> itemPositionInBackpacks = null;
        public List<KnapsackDataItem> knapdataList = null;

        public List<KnapsackDataItem> MedicineList = null;
        #endregion

        #region equip
        public ReferenceCollector equipCollector;
        public Dictionary<E_Grid_Type, KnapsackNewGrid> EquipmentPartDic;
        public int curWarePart = 0;

        #endregion

        #region hecheng

        public List<MergerMethod> allMergerMethod = new List<MergerMethod>();//全部的合成方法
        public MergerMethod curMergerMethod = null;

        public List<KnapsackDataItem> alreadyAddMergerItems = new List<KnapsackDataItem>();//当前 已经放入合成面板的物品
        private GameObject MergerContent;

        private KnapsackGrid[][] MergerGrids;
        public const int LENGTH_Merger_X = 8;
        public const int LENGTH_Merger_Y = 4;

        private ReferenceCollector reference_Merger;
        public Dictionary<long, KnapsackDataItem> mergerItemList = new Dictionary<long, KnapsackDataItem>();//合成成功的物品

        public Transform OrderContent, ItemPool;
        public Transform fristOrder_Item, twoOrder_Item, threeOrder_Item, FourGrade_Item, FiveGradeItem;
        public Dictionary<int, Queue<Transform>> PoolsObjDic = new Dictionary<int, Queue<Transform>>();
        public Dictionary<int, List<Transform>> OrderItemDic = new Dictionary<int, List<Transform>>();

        //public Dictionary<int, List<NewSynthesis_InfoConfig>> HCInfoDic = new Dictionary<int, List<NewSynthesis_InfoConfig>>();

        public ReferenceCollector collector_HC;

        public Text needCoin;

        public List<NewSynthesisItemInfo> SynthesisItemList = new List<NewSynthesisItemInfo>();//合成所需的必要材料
        public List<NewSynthesisItemInfo> SynthesisSIItemList = new List<NewSynthesisItemInfo>();//合成必要可选材料
        public List<NewSynthesisItemInfo> SynthesisFIItemList = new List<NewSynthesisItemInfo>();//合成辅助材料
        public List<NewSynthesisItemInfo> SynthesisFIItemListQiangHua = new List<NewSynthesisItemInfo>();//合成辅助材料
        public List<NewSynthesisItemInfo> SynthesisEItemList2 = new List<NewSynthesisItemInfo>();//合成辅助材料
        public List<long> SynthEquipIdList = new List<long>();//合成所需的装备id
                                                       //合成所需全部材料（包括可选材料）
        //public Dictionary<long, SynthesisItemInfo> NeedSynthesisInfosDic = new Dictionary<long, SynthesisItemInfo>();
        //public NewSynthesis_InfoConfig newSynthesis_Info;
        public int MaxRate;
        public int MinRate;
        public int curSynthesisId;//当前的合成ID

        public Text newTips;
        public Text desc;
        public List<int> ints = new List<int>();
        /*必要道具 ： 默认全部选择 （不加成功率）
        * 可选道具：多选一 （加成功率）
        * 辅助道具： （加成功率）
        * 
        * 
        * 280003,50,5-》ID，需求最大数，成功率
        * 
        * **/
        public int ConfigID = 0;
        public int ChengGongLv = 0;
        public Transform showSelectItemPos;
        public int curIndex = 1;

        public GameObject SelectItemPanel, SelectBg;
        public Transform SelectItems, tempItem;
        public ReferenceCollector reference_select;
        public Text selectItempage;
        //public List<SynthesisItemInfo> SelectItemList = new List<SynthesisItemInfo>();//可选择的装备集合
        //public List<SynthesisItemInfo> SelectItemListQianghua = new List<SynthesisItemInfo>();//可选择的装备集合2
        //public List<SynthesisItemInfo> SelectItemListChengGuang = new List<SynthesisItemInfo>();//可选择的装备集合3
        public List<GameObject> SelectMatModelList = new List<GameObject>();//材料模型集合
        public int pageCount = 8;//每页数量
        public Text title;

        public GameObject curSelectObj;//当前选择的物品
        public GameObject subBg;
        public ReferenceCollector reference_sub;
        //public List<SynthesisItemInfo> SubItemList = new List<SynthesisItemInfo>();
        public List<GameObject> SubMatModelList = new List<GameObject>();
        public Transform SubItems, tempsubItem;
        public int subIndex = 30001;
        #endregion

        #region 仓库
        public static int LENGTH_WareHouse_Y = 11;
        public static int MAX_HOUSE_LENGSH_Y = 11;//每页最大行数 11
        /// <summary>
        /// 仓库 页数列表
        /// </summary>
        public readonly List<List<KnapsackDataItem>> PageList = new List<List<KnapsackDataItem>>();

        public bool isOpen = false;//仓库面板是否打开
        public int curPage = 1;//当前页数
        public int allPage = 1;//总页数
        public Text pageText;//页数

        public Text wareCoin;//仓库金币

        public GameObject WareHouseContent;

        public KnapsackNewGrid[][] WareHouseGrids;
        public const int LENGTH_WareHouse_X = 8;

        public ReferenceCollector wareHouseCollector;
        #endregion

        #region 摆摊
        public bool isOpenStallUp;
        public ReferenceCollector stallUpCollector;
        public StallUpInfo StallUpInfo;
        public RoleStallUpComponent stallUpComponent;
        public GameObject startUpContent;
        public KnapsackNewGrid[][] StallUpGrids;
        public const int LENGTH_StallUp_X = 8;
        public const int LENGTH_StallUp_Y = 11;
        public InputField stallupNameInput;

        public bool isOpenStallUpOther;

        public RoleEntity otherRole;
        public RoleStallUpComponent StallUpOtherComponent;
        public GameObject StallUpOtherContent;

        public KnapsackNewGrid[][] StallUp_OtherGrids;
        public const int LENGTH_StallUp_Other_X = 8;
        public const int LENGTH_StallUp_Other_Y = 11;
        public Text OtherStasllName;



        public KnapsackDataItem StallUpData;

        #endregion

        #region 交易
        public Text OtherName, OtherWar, OtherLev, OtherGlodCoin, MyName;
        public InputField GlodCoinInput;
        public GameObject TradeMask, OtherMask;
        public GameObject OtherContent, MyContent;
        public Toggle SureTog;

        public KnapsackGrid[][] OtherGrids;
        public KnapsackGrid[][] MyGrids;
        public const int LENGTH_Trade_X = 8;
        public const int LENGTH_Trade_Y = 4;
        /// <summary>
        /// 交易物品
        /// </summary>
        public Dictionary<long, KnapsackDataItem> OtherTradeItemDic = new Dictionary<long, KnapsackDataItem>();
        #endregion

        #region 特权卡
        public bool isOpenVipPlane = false;//是否打开特权卡界面
        public GameObject NpcShopContent;
        public ReferenceCollector vipCollector;
        public KnapsackNewGrid[][] NpcShopGrids;
        public const int LENGTH_NpcShop_X = 8;
        public const int LENGTH_NpcShop_Y = 9;

        public long CurNpcUUid;
        public E_BuyType buyType = E_BuyType.Normal;

        public Button RepairBtn;//维修按钮
        public UnityEngine.UI.Text icontxt;//金币
        public E_Medicine curMedicine = E_Medicine.HP;
        public int curMedicineCount = 1;
        public E_MedicineType curMedicineType = E_MedicineType.Small;
        #endregion

        #region 自动出售
        public bool isOpenSell = false;
        public ReferenceCollector sellCollector;

        //可回收的装备集合
        public List<KnapsackDataItem> RecycleEquipList = new List<KnapsackDataItem>();


        public long glodcoinCount;//卖出获得的金币

        public Transform content, verticaItem;
        public List<string> itemNames = new List<string>();
        public Text coin;
        #endregion

        #region 强化
        public ReferenceCollector qianghuaCollector;
        public Toggle beibao, zhuangbei;

        public Text equipname, equipname1;//强化装备的名字
        public Text successfultxt;//成功率
        public Text tips;//提示语
        public Text Desc;//描述
        public Transform equipObjPos, equipObjPos2;//当前选择的模型显示位置
        public Transform Items;//所需材料
        public Text curLev, laterLev, Count;
        public Button Next, Last;
        public int index = 1, AllIndex = 1;
        public Button RequestBtn, Btn_One, Btn_Two;//开始 请求按钮
        public KnapsackDataItem curChooseKnapsackDataItem;//当前选择的装备
        public List<string> nameList = new List<string>();
        public List<KnapsackDataItem> BeiBaoItemList = new List<KnapsackDataItem>();//背包中 要显示的装备
        public List<ExpendItemInfo> ExpendItemList = new List<ExpendItemInfo>();//消耗道具集合
        public List<GameObject> MatModelList = new List<GameObject>();//材料模型集合
        public GameObject curShowModel, curShowModeltwo;
        //所需全部材料（包括可选材料）
        //public Dictionary<long, EnhanceItemInfo> NeedItemInfosDic = new Dictionary<long, EnhanceItemInfo>();

        public int curSuccessfulRate;//当前成功率
        public E_Grid_Type curQHPart;//当前强化部位

        public KnapsackDataItem curChooseKnapsackDataItemTwo;//继承当前选择第二的装备
        public Action<KnapsackDataItem> RequestAction;
        public Action<bool, long> TogAction;//必要材料Tog事件
        public bool GoldIsEnough = false, IsBtn = false;//金币是否足够
        public Dictionary<int, GameObject> curSelectObjList = new Dictionary<int, GameObject>();
        public int CruSessce = 0;
        #endregion

        #region 镶嵌
        public bool isOpenInlay = false;

        public enum E_InlayType
        {
            YingZhiShiHeCheng,
            YingGuangBaoShiHeCheng,
            YingGuangBaoShiXiangQian,
            YingGuangBaoShiChouQu,
            YingGuangBaoShiQiangHua,
            Info
        }

        public ReferenceCollector inlayCollector;
        public ReferenceCollector collector_YingZhiShiHeCheng;

        public ToggleGroup toggleGroup;

        public GameObject YingZhiShiHeChengPanel, YingGuangBaoShiHeChengPanel, YingGuangBaoShiXiangQianPanel, YingGuangBaoShiChouQuPanel, YingGuangBaoShiQiangHuaPanel, InfoPanel;
        /// <summary>
        /// 镶嵌面板上的 格子字典
        /// key:格子的名字
        /// value:格子
        /// </summary>
        public Dictionary<string, KnapsackNewGrid> InlayGridDic = new Dictionary<string, KnapsackNewGrid>();
        /// <summary>
        ///镶嵌所需的物品字典
        ///key:物品序号
        ///value:物品的UUID集合
        /// </summary>
        public Dictionary<int, List<long>> InlayItemDic = new Dictionary<int, List<long>>();
        /// <summary>
        /// 当前所选的 镶嵌类别
        /// </summary>
        public E_InlayType curInlayType;
        public KnapsackGrid CurInlayGrid;//当前所选的格子
        public string CurInlayGridName;//当前格子的名字
        public int CurInlayGridIndex;//当前格子的索引
        public bool GlodCoinEnough;//金币是否足够

        public readonly int Glod_ChouQu = 1_000_000;
        public readonly int Success_ChouQu = 90;

        public Transform Atrs;
        public ToggleGroup AtrtoggleGroup;

        public readonly int Glod = 1_000_000;
        public readonly int Success = 100;

        public ReferenceCollector collector_YingGuangBaoShiXiangQian;
        public readonly int Glod_XiangQian = 1_000_000;
        public readonly int Success_XiangQian = 100;

        public ReferenceCollector collector_YingGuangBaoShiQiangHua;
        public readonly int Glod_QiangHua = 1_000_000;
        public readonly int Success_QiangHua = 0;
        public Text SecceedRateTxt;//成功率

        public ReferenceCollector collector_YingGuangBaoShiHeCheng;
        public readonly int Glod_HeCheng = 1_000_000;
        public readonly int Success_HeCheng = 100;

        public ReferenceCollector collector_YingGuangBaoShiChouQu;
        #endregion

        public static List<long> MedicineHpIdList = new List<long>() { 310002, 310003, 310004, 310045 };///治疗药水
        public static List<long> MedicineMpIdList = new List<long>() { 310005, 310006, 310007 };///魔力药水
        public static List<long> MountUUIDList = new List<long>();//背包中的坐骑
        ///坐骑材料
        public static Dictionary<long, string> MountMatDic = new Dictionary<long, string>()
        {
            { 320006,"破烂的铠甲片"},
            { 320007,"女神的智慧"},
            { 320008,"猛兽的脚甲"},
            { 320009,"碎角片"},
            { 320010,"折断的角"},
            { 260008,"炎狼兽之角"},
        };
        ///羽毛材料
        public static Dictionary<long, string> YuMaoDic = new Dictionary<long, string>()
        {
            { 320307,"天魔菲尼斯的羽毛"},
            { 320300,"加鲁达之羽"},
            { 320297,"洛克之羽"},
            { 320020,"神鹰之羽"},
            { 320129,"天魔菲尼斯的羽毛"},
        };

        /// <summary>
        /// 背包中的物品 字典
        /// </summary>
        public static Dictionary<long, KnapsackDataItem> KnapsackItems = new Dictionary<long, KnapsackDataItem>();

        /// <summary>
        /// 整理背包的间隔时间
        /// </summary>
        public static float PackBackpackTime = 0;
        //每隔30秒 可以整理一次
        public static float PackBackpackSpaceTime = 30;
        //是否正在整理背包
        public static bool IsPackBackpack = false;
        //掉落的物品集合
        public static List<long> DisDropItemList = new List<long>();
        //上一个物品是否拾取完成
        public static bool LastItemIsComplete = true;
        /// <summary>
        /// 仓库中的装备
        /// </summary>
        public static Dictionary<long, KnapsackDataItem> WareHouseItems = new Dictionary<long, KnapsackDataItem>();

        /// <summary>
        /// 仓库中的金币
        /// </summary>
        public static long WareGlodCoin;
        /// <summary>
        /// 仓库 扩容（默认11行）
        /// </summary>
        public static int WareHouseRows = 11;

        /// <summary>
        /// 根据配置表ID判断字典中是否存在这个值
        /// </summary>
        /// <param name="ConfigId"></param>
        /// <returns></returns>
        public static bool GetIncludeValue(long ConfigId)
        {
            var list = KnapsackItems.Values.ToList();
            foreach (var item in list)
            {
                if (KnapsackItems.ContainsKey(item.Id) == false) continue;
                if (item.ConfigId == ConfigId)
                {
                    return true;
                }
            }
            return false;
        }


        public void OnInVisibility()
        {
            
        }

        public void OnVisible(object[] data)
        {
            if(data.Length >= 1)
            {
                E_KnapsackState knapsackState = (E_KnapsackState)data[0];
                if(knapsackState == E_KnapsackState.KS_Stallup_OtherPlayer)
                {

                }
                curKnapsackState = knapsackState;
            }

            if(data.Length == 2)
            {
                if ((int)data[1] != 0)
                {
                    subIndex = (int)data[1];
                }
            }

            this.Show().Coroutine();
        }

        public void OnVisible()
        {
            Log.Debug("打开默认");
            curKnapsackState = E_KnapsackState.KS_Knapsack;
            this.Show().Coroutine();    
        }

        

    }
}
