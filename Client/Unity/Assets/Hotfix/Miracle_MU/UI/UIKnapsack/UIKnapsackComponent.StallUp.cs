
using UnityEngine;
using ETModel;
using UnityEngine.UI;


namespace ETHotfix
{

    /// <summary>
    /// 摆摊
    /// </summary>
    public partial class UIKnapsackComponent
    {
        private GameObject StallUpContent;

        private KnapsackGrid[][] StallUpGrids;
        public const int LENGTH_StallUp_X = 8;
        public const int LENGTH_StallUp_Y = 11;

        private InputField stallupNameInput;

        private RoleStallUpComponent stallUpComponent;

        public StallUpInfo StallUpInfo;
        

        KnapsackDataItem StallUpData;

        private void Init_StallUp()
        {

            stallUpComponent = roleEntity.GetComponent<RoleStallUpComponent>();
            StallUpInfo = LocalDataJsonComponent.Instance.LoadData<StallUpInfo>(LocalJsonDataKeys.StallUpName) ?? new StallUpInfo();
            ReferenceCollector collector = StallUp.GetReferenceCollector();
            StallUpContent = collector.GetGameObject("Grids");

            stallupNameInput = collector.GetInputField("InputName");

            stallupNameInput.interactable = !stallUpComponent.IsStallUp;
            //设置摊位的名字
            stallupNameInput.onEndEdit.AddSingleListener(async value =>
            {
                if (string.IsNullOrEmpty(value)) return;
                //设置摊位
                //判断是否包含非法字符
                string st = value;
                if (SystemUtil.IsInvaild(st))
                {
                    st = SystemUtil.ReplaceStr(st);
                    stallupNameInput.text=st;
                }
                G2C_BaiTanSetNameResponse g2C_BaiTanSetName = (G2C_BaiTanSetNameResponse)await SessionComponent.Instance.Session.Call(new C2G_BaiTanSetNameRequest { BaiTanName = st });
                if (g2C_BaiTanSetName.Error != 0)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_BaiTanSetName.Error.GetTipInfo());
                }
                else
                {
                    StallUpInfo.StallName =$"{st}_{roleEntity.Id}";
                    LocalDataJsonComponent.Instance.SavaData(StallUpInfo,LocalJsonDataKeys.StallUpName);
                }
                Log.DebugGreen($"摊铺名字设置完成");
            });
            InitStallName().Coroutine();
            stallupNameInput.text = stallUpComponent.curStallUpName;

          
            collector.GetButton("CloseStallUpBtn").onClick.AddListener(async () =>
            {
                if (stallUpComponent.IsStallUp == false)
                {
                    Log.DebugGreen($"摊位未开启");
                    return;
                }

                G2C_BaiTanCloseResponse g2C_BaiTanClose = (G2C_BaiTanCloseResponse)await SessionComponent.Instance.Session.Call(new C2G_BaiTanCloseRequest
                {
                    BaiTanInstanceId = roleEntity.Id
                });
                if (g2C_BaiTanClose.Error != 0)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_BaiTanClose.Error.GetTipInfo());
                    Log.DebugGreen($"{g2C_BaiTanClose.Error}");
                }
                else
                {

                    CloseKnapsack();

                }
            });
            collector.GetButton("StarStallUpStar").onClick.AddListener(StartStallUp);

            StallUpGrids = new KnapsackGrid[LENGTH_StallUp_X][];
            for (int i = 0; i < StallUpGrids.Length; i++)
            {
                StallUpGrids[i] = new KnapsackGrid[LENGTH_StallUp_Y];
            }
            //初始化格子
            CreatGrid(LENGTH_StallUp_X, LENGTH_StallUp_Y, StallUpContent.transform, E_Grid_Type.Stallup, ref StallUpGrids);
            ///通知服务器 开始摆摊
            InitStallUp().Coroutine();
            //显示摊位的物品
            Init_StallUpEquip().Coroutine();

            static async ETVoid InitStallUp()
            {
                G2C_BaiTanResponse g2C_BaiTanResponse = (G2C_BaiTanResponse)await SessionComponent.Instance.Session.Call(new C2G_BaiTanRequest { });
            }

            ///初始化摊名
            async ETVoid InitStallName() 
            {
                if (string.IsNullOrEmpty(StallUpInfo.StallName) == false&& long.Parse(StallUpInfo.StallName.Split('_')[1])==this.roleEntity.Id)
                {
                    stallUpComponent.curStallUpName = StallUpInfo.StallName.Split('_')[0];
                    G2C_BaiTanSetNameResponse g2C_BaiTanSetName = (G2C_BaiTanSetNameResponse)await SessionComponent.Instance.Session.Call(new C2G_BaiTanSetNameRequest { BaiTanName = stallUpComponent.curStallUpName });
                    if (g2C_BaiTanSetName.Error != 0)
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_BaiTanSetName.Error.GetTipInfo());
                    }
                    else
                    {
                        StallUpInfo.StallName = $"{stallUpComponent.curStallUpName}_{roleEntity.Id}";
                        LocalDataJsonComponent.Instance.SavaData(StallUpInfo, LocalJsonDataKeys.StallUpName);
                    }
                    Log.DebugGreen($"摊铺名字设置完成");
                }
            }


        }
        /// <summary>
        /// 开始摆摊
        /// </summary>
        public void StartStallUp()
        {
            if (stallUpComponent.IsStallUp)
            {
                return;
            }
            if (stallUpComponent.StallUpItemDic.Count == 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint,"摊位没有物品，无法开摊位");
                return;
            }

            if (string.IsNullOrEmpty(stallupNameInput.text))
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "请先输入摊铺名字");
                return;
            }

            StartStallUpAsync().Coroutine();

            async ETVoid StartStallUpAsync()
            {
                G2C_BaiTanOpenResponse g2C_BaiTanOpenResponse = (G2C_BaiTanOpenResponse)await SessionComponent.Instance.Session.Call(new C2G_BaiTanOpenRequest { });
                if (g2C_BaiTanOpenResponse.Error != 0)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_BaiTanOpenResponse.Error.GetTipInfo());
                }
                else
                {
                    stallUpComponent.StartStallUp(stallupNameInput.text);
                    CloseKnapsack();

                }
            }


        }
        /// <summary>
        /// 初始化摊位的物品
        /// </summary>
        /// <param name="itemList">商店物品ID的 集合</param>
        public async ETVoid Init_StallUpEquip()
        {

            //第一次打开 摊位物品为空 请求获取
            Log.DebugBrown($"stallUpComponent.StallUpItemDic.Count:{stallUpComponent.StallUpItemDic.Count}");
            stallUpComponent.StallUpItemDic.Clear();

            Log.DebugPurple("获取摊位上的物品");
            G2C_BaiTanLookLookResponse g2C_BaiTanLookLook = (G2C_BaiTanLookLookResponse)await SessionComponent.Instance.Session.Call(new C2G_BaiTanLookLookRequest
            {
                BaiTanInstanceId = this.roleEntity.Id
            });
            if (g2C_BaiTanLookLook.Error != 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_BaiTanLookLook.Error.GetTipInfo());
            }
            else
            {
               
                for (int i = 0, length = g2C_BaiTanLookLook.Prop.count; i < length; i++)
                {
                    var item = g2C_BaiTanLookLook.Prop[i];
                    item.ConfigId.GetItemInfo_Out(out Item_infoConfig item_Info);
                    KnapsackDataItem knapsackDataItem = ComponentFactory.CreateWithId<KnapsackDataItem>(item.ItemUUID);
                    knapsackDataItem.UUID = item.ItemUUID;
                    knapsackDataItem.ConfigId = item.ConfigId;
                    knapsackDataItem.PosInBackpackX = item.PosInBackpackX;
                    knapsackDataItem.PosInBackpackY = item.PosInBackpackY;
                    knapsackDataItem.X = item_Info.X;
                    knapsackDataItem.Y = item_Info.Y;
                    knapsackDataItem.SetProperValue(E_ItemValue.Stall_SellPrice, item.Price);
                    knapsackDataItem.SetProperValue(E_ItemValue.Stall_SellMoJingPrice, item.Price2);
                    stallUpComponent.StallUpItemDic[item.ItemUUID] = knapsackDataItem;
                    AddItem(knapsackDataItem, type: E_Grid_Type.Stallup);
                }
            }

        }
        //添加摊位上的物品
        public KnapsackDataItem AddStallUpItem()
        {
           /* KnapsackDataItem knapsackDataItem = ComponentFactory.CreateWithId<KnapsackDataItem>(itemUUID);
            knapsackDataItem.UUID = itemUUID;
            knapsackDataItem.ConfigId = StallUpData.ConfigId;
            knapsackDataItem.PosInBackpackX = StallUpData.PosInBackpackX;
            knapsackDataItem.PosInBackpackY = StallUpData.PosInBackpackY;
            knapsackDataItem.X = StallUpData.X;
            knapsackDataItem.Y = StallUpData.Y;
            //knapsackDataItem.SetProperValue(E_ItemValue.Stall_SellPrice, item.Price);
         //   knapsackDataItem.SetProperValue(E_ItemValue.Stall_SellMoJingPrice, item.Price2);*/
           
            AddItem(StallUpData, type: E_Grid_Type.Stallup);
            stallUpComponent.StallUpItemDic[StallUpData.Id] = StallUpData;
            return StallUpData;
        }

        /// <summary>
        ///移除摊位上的物品
        /// </summary>
        /// <param name="itemUUId"></param>
        public void RemoveStallUpItem(long itemUUId)
        {
            if (stallUpComponent.StallUpItemDic.TryGetValue(itemUUId, out KnapsackDataItem knapsack))
            {
                knapsack.Dispose();
            }
            stallUpComponent.StallUpItemDic.Remove(itemUUId);
        }

        /// <summary>
        /// 清理摊位
        /// </summary>
        public void CleanStallUp()
        {
            GetKnapsackGrid(ref grids, ref LENGTH_X, ref LENGTH_Y, E_Grid_Type.Stallup);
            for (int i = 0; i < LENGTH_StallUp_Y; i++)
            {
                for (int j = 0; j < LENGTH_StallUp_X; j++)
                {
                    KnapsackGrid grid = grids[j][i];
                    if (grid.IsOccupy)
                    {
                        grid.Data.ItemData.Dispose();
                        RemoveItem(grid.Data, true);
                    }
                }
            }
            StallUpData?.Dispose();
            StallUpData = null;
        }
    }
}