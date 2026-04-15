using System.Collections.Generic;
using UnityEngine;
using ETModel;
using UnityEngine.UI;
using System.Linq;
using ILRuntime.Runtime;

namespace ETHotfix
{
    [ObjectSystem]
    public class UIFeatureGuideComponentAwake : AwakeSystem<UIFeatureGuideComponent>
    {
        public override void Awake(UIFeatureGuideComponent self)
        {
            self.collector = self.GetParent<UI>().GameObject.GetReferenceCollector();
            self.collector.GetButton("CloseBtn").onClick.AddSingleListener(() => UIComponent.Instance.Remove(UIType.UIFeatureGuide));
            self.chuangsongTran = self.collector.GetButton("TransformBtn").transform;
            self.collector.GetButton("TransformBtn").onClick.AddSingleListener(self.Transmit);
            self.oneGradeItem = self.collector.GetGameObject("OneGradeItem").transform;
            self.twoGradeItem = self.collector.GetGameObject("TwoGradeItem").transform;
            self.threeGradeItem = self.collector.GetGameObject("ThreeGradeItem").transform;
            self.poolItem = self.collector.GetGameObject("ItemPool").transform;
            self.content = self.collector.GetGameObject("Content").transform;
            self.contentInfo = self.collector.GetText("InfoTxt");
            self.roleEntity = UnitEntityComponent.Instance.LocalRole;
        }
    }
    [ObjectSystem]
    public class UIFeatureGuideComponentStart : StartSystem<UIFeatureGuideComponent>
    {
        public override void Start(UIFeatureGuideComponent self)
        {
            self.InitData();
            self.InitShow();
        }
    }
    public class UIFeatureGuideComponent : Component
    {
        public ReferenceCollector collector;
        public Dictionary<string, FunctionList_InfoConfig> functionList_InfoConfigs=new Dictionary<string, FunctionList_InfoConfig>();
        public Dictionary<string, List<FunctionList_InfoConfig>> dic_functionList_InfoConfigs = new Dictionary<string, List<FunctionList_InfoConfig>>();
        public Transform oneGradeItem;
        public Transform twoGradeItem;
        public Transform threeGradeItem;
        public Transform poolItem;
        public Transform content;
        public Dictionary<int,Queue<Transform>> poolItemDic= new Dictionary<int, Queue<Transform>>();
        public Transform selectIndexOne =null;
        public Transform selectIndexTwo =null;
        public Transform selectIndexThree = null;
        public Dictionary<int, List<Transform>> saveItemDic = new Dictionary<int, List<Transform>>();
        public Text contentInfo;
        public bool isShowAllItem=false;
        public bool isInit = true;
        public FunctionList_InfoConfig selectConfig;
        public RoleEntity roleEntity;
        public Transform chuangsongTran;
        public void InitData() { 
            FunctionList_InfoConfig functionList_InfoConfig = new FunctionList_InfoConfig();
            var Config_Cnnoun = ConfigComponent.Instance.GetAll<FunctionList_InfoConfig>();
            foreach (FunctionList_InfoConfig announc_Info in Config_Cnnoun.Cast<FunctionList_InfoConfig>())
            {
                functionList_InfoConfigs.Add(announc_Info.Id.ToString(), announc_Info);
            }

            foreach (KeyValuePair<string, FunctionList_InfoConfig> announc_ in functionList_InfoConfigs) {
                FunctionList_InfoConfig value =announc_.Value;
                if (value != null) {
                    //一级标题
                    string dicKey = null;
                    if (value.Hierarchy == 1)
                    {

                        dicKey = "0";

                    }
                    else if (value.Hierarchy == 2) {

                        dicKey = value.ParentId.ToString();

                    }else if (value.Hierarchy == 3)
                    {
                        string parentId = value.ParentId.ToString();
                        FunctionList_InfoConfig parenInfo = functionList_InfoConfigs[parentId];
                        dicKey = $"{parenInfo.ParentId.ToString()}_{parentId}";

                    }
                    if (!dic_functionList_InfoConfigs.ContainsKey(dicKey))
                    {
                        dic_functionList_InfoConfigs[dicKey] = new List<FunctionList_InfoConfig>();
                    }
                    dic_functionList_InfoConfigs[dicKey].Add(value);
                }
            }
        }

        public void InitShow() {
            //初始化一级标题        
            List<FunctionList_InfoConfig> functionList_ = dic_functionList_InfoConfigs["0"];
            FunctionList_InfoConfig firstData = null;
            Transform firstItem = null;
            for (int i = 0; i < functionList_.Count; i++)
            {
                Transform useItem = GetItemByInde(1);
                useItem.parent = content;
                useItem.gameObject.SetActive(true);
                Button button= useItem.GetComponentInChildren<Button>();
                Text text= useItem.GetComponentInChildren<Text>();
                text.text = functionList_[i].FunctionName;
                useItem.name = functionList_[i].Id.ToString();
                int xuhao = i;
                button.onClick.AddSingleListener(() => {
                    OnClickOneTitle(functionList_[xuhao], useItem);
                });
                if (firstData == null)
                    firstData = functionList_[i];
                if (firstItem == null)
                    firstItem = useItem;
            }
            if (firstData != null&&firstItem!=null) {
                OnClickOneTitle(firstData, firstItem);
            }
        }
        /// <summary>
        /// 点击标题效果（右边展示）
        /// </summary>
        /// <param name="useData"></param>
        public void ShowRightContent(FunctionList_InfoConfig useData) { 
           
            if (useData != null) {
                selectConfig = useData;
                //内容文字描述
                contentInfo.text = useData.Desk;
                //是否显示文字内容
                if (selectConfig.TargetIndex != "")
                {
                    chuangsongTran.gameObject.SetActive(true);
                }
                else {
                    chuangsongTran.gameObject.SetActive(false);
                }
            }
        }
        public void Transmit() {
            if (selectConfig != null) {
                //传送相关
                OnChangeScene();
            }
        }
        public void OnChangeScene()
        {
            //判断等级 金币是否足够 
            //if (roleEntity.Property.GetProperValue(E_GameProperty.GoldCoin) < info.MapCostGold)
            //{
            //    UIComponent.Instance.VisibleUI(UIType.UIHint, "金币不足");
            //    return;
            //}
            if (roleEntity.Property.GetProperValue(E_GameProperty.Level) < selectConfig.MapMinLevel)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "等级不足");
                return;
            }

            //天空之城需要佩戴翅膀才能进入
            if (selectConfig.SceneName == (int)SceneName.TianKongZhiCheng)
            {
                if (roleEntity.GetComponent<RoleEquipmentComponent>().curWareEquipsData_Dic.ContainsKey(E_Grid_Type.Wing) == false)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "进入天空之城 需要翅膀");
                    return;
                }
            }
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
            //if (SceneComponent.Instance.CurrentSceneName != ((SceneName)selectConfig.SceneName).ToString())
            //{
            //    AssetBundleComponent.Instance.LoadBundle(((SceneName)selectConfig.SceneName).ToString().StringToAB());
            //    AstarComponent.Instance.LoadSceneNodes(((SceneName)selectConfig.SceneName).ToString(), SceneComponent.Instance.CurrentSceneName);
            //}
            ///地图传送
            async ETVoid ChangeMap()
            {
                //Log.DebugGreen(selectConfig.SceneName.ToString());
                G2C_MapDeliveryResponse response = (G2C_MapDeliveryResponse)await SessionComponent.Instance.Session.Call(new C2G_MapDeliveryRequest
                {
                    MapId = (int)selectConfig.SceneName
                });
                if (response.Error != 0)
                {
                    UIComponent.Instance.Remove(UIType.UISceneLoading);
                    AstarComponent.Instance.LoadSceneNodes(SceneComponent.Instance.CurrentSceneName, "null");
                    UIComponent.Instance.VisibleUI(UIType.UIHint, response.Error.GetTipInfo());
                }
                else
                {
                    //传送完成 移动到目标点
                    TaskDatas.AutoNavCallBack = () =>
                    {
                        Vector2 pos = GetPos(selectConfig.TargetIndex);
                        //传送后场景后寻路
                        UnitEntityComponent.Instance.LocalRole.GetComponent<UnitEntityPathComponent>().NavTarget(AstarComponent.Instance.GetNode(pos.x.ToInt32(), pos.y.ToInt32()), () => TaskDatas.AutoNavCallBack = null);
                    };
                    Close();
                }
            }
        }
        public Vector2 GetPos(string targetStr)
        {
            string[] strings= targetStr.Split(',');
            string x = strings[0].Replace("[", "");
            string y = strings[1].Replace("]", "");
            return new Vector2(x.ToInt32(),y.ToInt32());
        }
        public void Close()
        {
            UIComponent.Instance.Remove(UIType.UIFeatureGuide);
        }
        /// <summary>
        /// 点击一级标题
        /// </summary>
        /// <param name="data"></param>
        /// <param name="targetItem"></param>
        public void OnClickOneTitle(FunctionList_InfoConfig data,Transform targetItem) {
            //回收
            RecoveryItem();

            if (selectIndexOne!=null) {
                Transform select = selectIndexOne.Find("OneGrade/Bg/Select");
                select.gameObject.SetActive(false);
            }

            //是否一次展开
            isShowAllItem = isInit ? true : selectIndexOne != null && targetItem.name != selectIndexOne.name;
            if (isInit) { isInit = false; }

            //点击
            if (selectIndexOne==null||targetItem.name != selectIndexOne.name)
            {
                selectIndexOne = targetItem; 
                Transform select = selectIndexOne.Find("OneGrade/Bg/Select");
                select.gameObject.SetActive(true);

                //判断该条目是否又下拉栏
                if (data.Desk != "") {
                    ShowRightContent(data);
                }
                else {          
                    string key = data.Id.ToString();
                    //二级标题数据
                    if (!dic_functionList_InfoConfigs.ContainsKey(key))
                    {
                        dic_functionList_InfoConfigs[key] = new List<FunctionList_InfoConfig>() { };
                    }
                    List<FunctionList_InfoConfig> functionList_ = dic_functionList_InfoConfigs[key];
                    //二级标题条目缓存
                    if (!saveItemDic.ContainsKey(2))
                    {
                        saveItemDic[2] = new List<Transform>();
                    }
                    if (functionList_ != null && functionList_.Count > 0)
                    {
                        FunctionList_InfoConfig firstData = null;
                        Transform firstItem = null;
                        for (int i = 0; i < functionList_.Count; i++)
                        {
                            Transform useItem = GetItemByInde(2);
                            saveItemDic[2].Add(useItem);
                            useItem.parent = targetItem;
                            useItem.gameObject.SetActive(true);
                            useItem.name = functionList_[i].Id.ToString();
                            Button button = useItem.GetComponentInChildren<Button>();
                            Text text = useItem.GetComponentInChildren<Text>();
                            text.text = functionList_[i].FunctionName;
                            int xuhao = i;
                            button.onClick.AddSingleListener(() => { OnClickTwoTitle(functionList_[xuhao], useItem); });

                            if (firstData == null)
                                firstData = functionList_[i];
                            if (firstItem == null)
                                firstItem = useItem;
                        }
                        if (isShowAllItem&& firstData!=null&&firstItem!=null) {
                            OnClickTwoTitle(firstData, firstItem);
                        }
                    }
                }
            }
            else {
                selectIndexOne = null;
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(content.GetComponent<RectTransform>());
            LayoutRebuilder.ForceRebuildLayoutImmediate(targetItem.GetComponent<RectTransform>());
        }
        /// <summary>
        /// 点击二级标题
        /// </summary>
        /// <param name="data"></param>
        /// <param name="targetItem"></param>
        public void OnClickTwoTitle(FunctionList_InfoConfig data, Transform targetItem) {
            //回收
            RecoveryItem(3);
            if (selectIndexTwo!=null)
            {
                Transform select = selectIndexTwo.Find("TwoGrade/Bg/Select");
                select.gameObject.SetActive(false);
            }
            //点击
            if (selectIndexTwo == null || targetItem.name != selectIndexTwo.name)
            {
                selectIndexTwo = targetItem;
                Transform select = selectIndexTwo.Find("TwoGrade/Bg/Select");
                select.gameObject.SetActive(true);

                //判断该条目是否又下拉栏
                if (data.Desk != "")
                {
                    ShowRightContent(data);
                }
                else
                {
                    string key1 = selectIndexOne.name;
                    string key2 = data.Id.ToString();
                    string key = $"{key1}_{key2}";
                    //三级条目数据 
                    if (!dic_functionList_InfoConfigs.ContainsKey(key))
                    {
                        dic_functionList_InfoConfigs[key] = new List<FunctionList_InfoConfig>() { };
                    }
                    List<FunctionList_InfoConfig> functionList_ = dic_functionList_InfoConfigs[key];
                    //三级条目缓存
                    if (!saveItemDic.ContainsKey(3))
                    {
                        saveItemDic[3] = new List<Transform>();
                    }
                    if (functionList_ != null && functionList_.Count > 0)
                    {
                        FunctionList_InfoConfig firstData = null;
                        Transform firstItem = null;
                        for (int i = 0; i < functionList_.Count; i++)
                        {
                            Transform useItem = GetItemByInde(3);
                            
                            useItem.parent = targetItem;
                            useItem.gameObject.SetActive(true);
                            useItem.name = functionList_[i].Id.ToString();
                            saveItemDic[3].Add(useItem);

                            Text text = useItem.GetComponentInChildren<Text>();
                            text.text = functionList_[i].FunctionName;
                            Button button = useItem.GetComponentInChildren<Button>();
                            int xuhao = i;
                            button.onClick.AddSingleListener(() => { OnClickThreeTitle(functionList_[xuhao], useItem); });
                            if (firstData == null)
                                firstData = functionList_[i];
                            if (firstItem == null)
                                firstItem = useItem;
                        }
                        if (isShowAllItem && firstData != null && firstItem != null)
                        {
                            OnClickThreeTitle(firstData, firstItem);
                        }
                    }
                }
            }
            else
            {
                selectIndexTwo = null;
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(content.GetComponent<RectTransform>());
            LayoutRebuilder.ForceRebuildLayoutImmediate(selectIndexOne.GetComponent<RectTransform>());
            LayoutRebuilder.ForceRebuildLayoutImmediate(targetItem.GetComponent<RectTransform>());
        }
        /// <summary>
        /// 点击三级标题
        /// </summary>
        /// <param name="data"></param>
        /// <param name="targetItem"></param>
        public void OnClickThreeTitle(FunctionList_InfoConfig data, Transform targetItem) {
            if (selectIndexThree != null)
            {
                Transform select = selectIndexThree.Find("Bg/Select");
                select.gameObject.SetActive(false);
            }
            //点击
            if (selectIndexThree == null || targetItem.name != selectIndexThree.name)
            {
                ShowRightContent(data);

                selectIndexThree = targetItem;
                Transform select = selectIndexThree.Find("Bg/Select");
                select.gameObject.SetActive(true);
            }
            else
            {
                selectIndexThree = null;
            }
        }
        public void RecoveryItem(int index=0) 
        {

            if(index != 3)
            {
                //回收二级标题
                if (saveItemDic.ContainsKey(2))
                {
                    List<Transform> listTran = saveItemDic[2];
                    if (listTran != null && listTran.Count > 0)
                    {
                        for (int i = 0; i < listTran.Count; i++)
                        {
                            Transform t = listTran[i];
                            RecoveryPool(t, 2);
                        }
                    }
                }
                //重置二级标题选中显示
                if (selectIndexTwo != null)
                {
                    Transform tempTran = selectIndexTwo.Find("TwoGrade/Bg/Select");
                    tempTran.gameObject.SetActive(false);
                    selectIndexTwo = null;
                }
            }
            //回收三级标题
            if (saveItemDic.ContainsKey(3))
            {
                List<Transform> listTran1 = saveItemDic[3];
                if (listTran1 != null && listTran1.Count > 0)
                {
                    for (int i = 0; i < listTran1.Count; i++)
                    {
                        //Transform t = listTran1[i];
                        RecoveryPool(listTran1[i], 3);
                    }
                }
            }
            //重置三级标题选中显示
            if (selectIndexThree != null)
            {
                Transform tempTran = selectIndexThree.Find("Bg/Select");
                tempTran.gameObject.SetActive(false);
                selectIndexThree = null;
            }
        }

        /// <summary>
        /// 从对象池里取出条目
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Transform GetItemByInde(int index) {
            if (!poolItemDic.ContainsKey(index)) {
                poolItemDic[index] = new Queue<Transform>();
            }
            if(poolItemDic[index].Count > 0) 
                return poolItemDic[index].Dequeue();
            Transform tempItem = null;
            if (index == 1)
            {
                tempItem = GameObject.Instantiate<Transform>(oneGradeItem);
            }
            else if (index == 2)
            {
                tempItem = GameObject.Instantiate<Transform>(twoGradeItem);
            }
            else if (index == 3)
            {
                tempItem = GameObject.Instantiate<Transform>(threeGradeItem);
            }
            tempItem.parent = poolItem;
            tempItem.localScale = Vector3.one;
            tempItem.localPosition = new Vector3(tempItem.localPosition.x, tempItem.localPosition.y, 0);
            return tempItem;

        }
        public void RecoveryPool(Transform tempItem,int index) {
            if (!poolItemDic.ContainsKey(index))
            {
                poolItemDic[index] = new Queue<Transform>();
            }
            //GameObject.Destroy(tempItem);
            tempItem.parent = poolItem;
            tempItem.gameObject.SetActive(false);
            if (!poolItemDic[index].Contains(tempItem))
            {
                poolItemDic[index].Enqueue(tempItem);
            }
        }
    } 
}
