using System.Collections.Generic;
using UnityEngine;
using ETModel;
using UnityEngine.UI;
using ILRuntime.Runtime;


namespace ETHotfix
{
    /// <summary>
    /// 交易模块
    /// </summary>
    public partial class UIKnapsackComponent
    {

        Text OtherName, OtherWar, OtherLev, OtherGlodCoin, MyName;
        public InputField GlodCoinInput;
        GameObject TradeMask, OtherMask;
        private GameObject OtherContent, MyContent;
        public Toggle SureTog;

        private KnapsackGrid[][] OtherGrids;
        private KnapsackGrid[][] MyGrids;
        public const int LENGTH_Trade_X = 8;
        public const int LENGTH_Trade_Y = 4;
        /// <summary>
        /// 交易物品
        /// </summary>
        public Dictionary<long, KnapsackDataItem> OtherTradeItemDic = new Dictionary<long, KnapsackDataItem>();
        /// <summary>
        /// 初始化 交易模块
        /// </summary>
        public void Init_Trade()
        {
            ReferenceCollector collector_Trade = Trade.GetReferenceCollector();
            OtherTradeItemDic.Clear();
            OtherName = collector_Trade.GetText("OtherName");//交易对象的名字
            OtherWar = collector_Trade.GetText("OtherWarName");//交易对象的战盟名字
            OtherLev = collector_Trade.GetText("OtherLev");//交易对象的等级
            OtherGlodCoin = collector_Trade.GetText("OtherGlodCoin");//交易金币
            OtherGlodCoin.text = $"{0:N}";
            OtherContent = collector_Trade.GetGameObject("OtherGrids");//交易对象的格子
            OtherMask = collector_Trade.GetImage("OtherMask").gameObject;//对方交易确认遮罩
            MyContent = collector_Trade.GetGameObject("MyGrids");//自身的交易格子
            TradeMask = collector_Trade.GetImage("TradMask").gameObject;//交易确认遮罩
            MyName = collector_Trade.GetText("MyName");//自身的名字
            GlodCoinInput = collector_Trade.GetInputField("GlodCoinInputField");//本人要交易金币
            ChangeTradeStatus(false);
            ChangeOtherTradeStatus();

            //交易确认
            SureTog = collector_Trade.GetToggle("SureTog");
            SureTog.isOn = false;
            SureTog.onValueChanged.AddSingleListener(value =>
            {
                ChangeTradeStatusAsync().Coroutine();
                ///交易状态改变 true 交易锁定 false 取消锁定
                async ETVoid ChangeTradeStatusAsync()
                {
                    G2C_LockExchangeItem g2C_LockExchange = (G2C_LockExchangeItem)await SessionComponent.Instance.Session.Call(new C2G_LockExchangeItem { LockState = value });
                    if (g2C_LockExchange.Error != 0) UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_LockExchange.Error.GetTipInfo());
                    else
                    {
                        ChangeTradeStatus(value);
                      
                    }
                }
            });
            //初始化 自身的格子
            MyGrids = new KnapsackGrid[LENGTH_Trade_X][];
            for (int i = 0, length = MyGrids.Length; i < length; i++)
            {
                MyGrids[i] = new KnapsackGrid[LENGTH_Trade_Y];
            }
            CreatGrid(LENGTH_Trade_X, LENGTH_Trade_Y, MyContent.transform, E_Grid_Type.Trade, ref MyGrids);
            //初始化 交易对象的格子
            OtherGrids = new KnapsackGrid[LENGTH_Trade_X][];
            for (int i = 0, length = OtherGrids.Length; i < length; i++)
            {
                OtherGrids[i] = new KnapsackGrid[LENGTH_Trade_Y];
            }
            CreatGrid(LENGTH_Trade_X, LENGTH_Trade_Y, OtherContent.transform, E_Grid_Type.Trade_Other, ref OtherGrids);
            SetMyName();
            ///设置本地玩家的名字
            void SetMyName()
            {
                MyName.text = UnitEntityComponent.Instance.LocalRole.RoleName;
            }
            //交易金币
            GlodCoinInput.onEndEdit.AddSingleListener(async (value) =>
            {
                if (int.Parse(value) < 0)
                {
                    GlodCoinInput.text = $"{0.ToInt32():N}";
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "金币金额 不能为负数");
                    return;
                }
                if (string.IsNullOrEmpty(value) || int.Parse(value) is 0)
                {
                    GlodCoinInput.text = $"{0.ToInt32():N}";
                    ChangeGlodCoin(0);
                    return;
                };
                //限制金币数量
                if (long.TryParse(value, out long glod))
                {
                    glod = glod > UnitEntityComponent.Instance.LocalRole.Property.GetProperValue(E_GameProperty.GoldCoin) ? UnitEntityComponent.Instance.LocalRole.Property.GetProperValue(E_GameProperty.GoldCoin) : glod;
                }
                //设置金币数量
                G2C_SetExchangeGold g2C_SetExchange = (G2C_SetExchangeGold)await SessionComponent.Instance.Session.Call(new C2G_SetExchangeGold
                {
                    Gold = glod.ToInt32()
                });
                if (g2C_SetExchange.Error != 0)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_SetExchange.Error.GetTipInfo());
                }
                else
                {
                    GlodCoinInput.text = $"{glod}";
                }
            });
        }
        /// <summary>
        /// 是否存在 交易对象
        /// </summary>
        /// <returns></returns>
        public bool CanTrade()
        {
            return string.IsNullOrEmpty(OtherName.text);
        }
        /// <summary>
        /// 改变对方交易栏中的物品
        /// </summary>
        /// <param name="itemUUID">物品的UUID</param>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        public void ChangeTradeItemPos(long itemUUID, int X, int Y)
        {
            KnapsackGridData oldArea, newArea;
            Log.DebugGreen($"X;{X} Y:{Y}");
            if (OtherTradeItemDic.TryGetValue(itemUUID, out KnapsackDataItem dataItem))
            {
                dataItem.ConfigId.GetItemInfo_Out(out Item_infoConfig item_Info);
                var itemObj = OtherGrids[dataItem.PosInBackpackX][dataItem.PosInBackpackY].GridObj;
                oldArea = new KnapsackGridData
                {

                    UUID = dataItem.UUID,
                    ItemData = dataItem,
                    Point1 = new Vector2Int(dataItem.PosInBackpackX, dataItem.PosInBackpackY),
                    Point2 = new Vector2Int(dataItem.PosInBackpackX + item_Info.X - 1, dataItem.PosInBackpackY + item_Info.Y - 1),
                    Grid_Type = E_Grid_Type.Trade_Other
                };
                newArea = new KnapsackGridData
                {
                    UUID = dataItem.UUID,
                    ItemData = dataItem,
                    Point1 = new Vector2Int(X, Y),
                    Point2 = new Vector2Int(X + item_Info.X - 1, Y + item_Info.Y - 1),
                    Grid_Type = E_Grid_Type.Trade_Other
                };
                dataItem.PosInBackpackX = X;
                dataItem.PosInBackpackY = Y;

                AddKnapsackItem(newArea, itemObj);
                RemoveItem(oldArea);
            }
        }


        /// <summary>
        /// 更新显示对方的信息
        /// </summary>
        /// <param name="RoleName">名字</param>
        /// <param name="warName">战盟名字</param>
        /// <param name="lev">等级</param>
        public void SetOtherInfo(string RoleName, string warName, int lev)
        {
            OtherName.text = RoleName;
            OtherWar.text = string.IsNullOrEmpty(warName) ? "未加入战盟" : $"{warName}";
            OtherLev.text = $"{lev}级";
        }
        /// <summary>
        /// 改变确认交易面板的状态
        /// </summary>
        /// <param name="status">是否隐藏 （默认隐藏状态）</param>
        public void ChangeTradeStatus(bool status = false)
        {
            GlodCoinInput.interactable = !status;
            TradeMask.SetActive(status);
        }
        /// <summary>
        /// 改变对方交易状态
        /// </summary>
        /// <param name="status"></param>
        public void ChangeOtherTradeStatus(bool status = false)
        {
            OtherMask.SetActive(status);
        }
        /// <summary>
        /// 改变交易金币数量
        /// </summary>
        /// <param name="glogcoin"></param>
        public void ChangeGlodCoin(int glogcoin)
        {
            OtherGlodCoin.text = $"{glogcoin:N}";
        }
        /// <summary>
        /// 取消交易
        /// </summary>
        /// <returns></returns>
        public async ETVoid CancleTrade()
        {
            G2C_CancelExchange g2C_CancelExchange = (G2C_CancelExchange)await SessionComponent.Instance.Session.Call(new C2G_CancelExchange { });
        }
        /// <summary>
        /// 是否可以关闭交易面板
        /// </summary>
        /// <returns></returns>
        public bool IsHaveTradeItem()
        {
            GetKnapsackGrid(ref grids, ref LENGTH_X, ref LENGTH_Y, E_Grid_Type.Trade);

            for (int i = 0; i < LENGTH_Trade_Y; i++)
            {
                for (int j = 0; j < LENGTH_Trade_X; j++)
                {
                    KnapsackGrid grid = grids[j][i];
                    if (grid.IsOccupy)
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, "交易面板上的物品 还未放入背包");
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// 清理 自己交易面板上的物品
        /// </summary>
        public void ClearTrade()
        {
            GetKnapsackGrid(ref grids, ref LENGTH_X, ref LENGTH_Y, E_Grid_Type.Trade);

            for (int i = 0; i < LENGTH_Trade_Y; i++)
            {
                for (int j = 0; j < LENGTH_Trade_X; j++)
                {
                    KnapsackGrid grid = grids[j][i];
                    if (grid.IsOccupy)
                    {

                        RemoveItem(grid.Data, true);
                    }
                }
            }
            ClearOtherTrade();
            ChangeTradeStatus();
            ChangeOtherTradeStatus();
            SetOtherInfo(null, null, 0);
            ChangeGlodCoin(0);
            GlodCoinInput.text = $"{0}:N";
        }
        /// <summary>
        /// 清理对方的交易物品
        /// </summary>
        public void ClearOtherTrade()
        {
            GetKnapsackGrid(ref grids, ref LENGTH_X, ref LENGTH_Y, E_Grid_Type.Trade_Other);

            for (int i = 0; i < LENGTH_Trade_Y; i++)
            {
                for (int j = 0; j < LENGTH_Trade_X; j++)
                {
                    KnapsackGrid grid = grids[j][i];
                    if (grid.IsOccupy)
                    {

                        RemoveItem(grid.Data, true);
                    }
                }
            }
            foreach (var item in OtherTradeItemDic.Values)
            {
                item.Dispose();
            }
            OtherTradeItemDic.Clear();
        }

    }
}