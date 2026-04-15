using ETModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETHotfix
{
    /// <summary>
    /// 背包面板管理类
    /// </summary>
    public partial class UIKnapsackComponent
    {
        private Transform panelParent;
        public GameObject Knapsack, EquipPanel, NpcShop, WareHouse, StallUp, Stallup_OtherPlayer, Merger, InlayPanel, Trade,Sell;

        public GameObject TipsPanel;

        public E_KnapsackState curKnapsackState = E_KnapsackState.KS_Equipment;//当前所选显示的面板(默认装备面板)

        public void Init_Panel()
        {
            ReferenceCollector referenceCollector_Knapsack = GetParent<UI>().GameObject.GetReferenceCollector();
            referenceCollector_Knapsack.gameObject.GetComponent<Canvas>().planeDistance = 90;
            panelParent = referenceCollector_Knapsack.GetImage("OtherPanel").transform;
            referenceCollector_Knapsack.GetButton("CloseBtn_1").onClick.AddSingleListener(CloseKnapsack);

            UIBeginnerGuide = referenceCollector_Knapsack.GetImage("UIBeginnerGuide").gameObject;
            UIBeginnerGuideTwo = referenceCollector_Knapsack.GetImage("UIBeginnerGuideTwo").gameObject;
            UIBeginnerGuideThree = referenceCollector_Knapsack.GetImage("UIBeginnerGuideThree").gameObject;


            Knapsack = referenceCollector_Knapsack.GetImage("Knapsack").gameObject;//背包面板
            EquipPanel = referenceCollector_Knapsack.GetGameObject("EquipPanel");//装备面板
            NpcShop = referenceCollector_Knapsack.GetGameObject("NPCShop");//NPC商城
            WareHouse = referenceCollector_Knapsack.GetGameObject("WareHouse");//仓库
            StallUp = referenceCollector_Knapsack.GetGameObject("StallUp");//摆摊
            Stallup_OtherPlayer = referenceCollector_Knapsack.GetGameObject("Stallup_OtherPlayer");//其他玩家的摆摊
            Merger = referenceCollector_Knapsack.GetGameObject("Merger");//合成面板
            InlayPanel = referenceCollector_Knapsack.GetGameObject("InlayPanel");//镶嵌面板
            Trade = referenceCollector_Knapsack.GetGameObject("Trade");//交易面板
            Sell = referenceCollector_Knapsack.GetGameObject("Sell");//出售面板
            HidePanel();

            UIBeginnerGuide.SetActive(false);
            SetBeginnerGuide();
        }

        public void SetBeginnerGuide()
        {
            if (BeginnerGuideData.IsComplete(9))
            {
                BeginnerGuideData.SetBeginnerGuide(9);
                UIBeginnerGuide.SetActive(true);
            }
            else if (BeginnerGuideData.IsComplete(15))
            {
                BeginnerGuideData.SetBeginnerGuide(15);
                UIBeginnerGuideTwo.SetActive(true);
                UIBeginnerGuideThree.SetActive(true);
            }

            //else if (BeginnerGuideData.IsComplete(3))
            //{
            //    BeginnerGuideData.SetBeginnerGuide(3);
            //    UIBeginnerGuide.SetActive(true);
            //}
        }

        private void HidePanel()
        {
            for (int i = 0, length = panelParent.childCount - 1; i < length; i++)
            {
                panelParent.GetChild(i).gameObject.SetActive(false);
            }
            InlayPanel.SetActive(false);
        }

        /// <summary>
        /// 切换面板
        /// </summary>
        /// <param name="knapsackState"></param>
        public void ChangePanel(E_KnapsackState knapsackState)
        {
            PanelManager(false);
            curKnapsackState = knapsackState;
            PanelManager(true);
        }

        /// <summary>
        /// 面板管理
        /// </summary>
        /// <param name="isShow">是否显示（默认显示）</param>
        public void PanelManager(bool isShow = true, SynthesisData synthesisData = SynthesisData.Null)
        {
            switch (curKnapsackState)
            {
                case E_KnapsackState.KS_Knapsack:
                    if (isShow)
                    {
                        InitEquipGrids();
                        InitEquip();
                    }
                    else
                    {
                        CloseEquipPanel();

                    }
                    EquipPanel.SetActive(isShow);
                    break;
                case E_KnapsackState.KS_Gem_Merge:
                    if (isShow)
                        Init_Merger(synthesisData);
                    else
                        CleanMerger();
                    Merger.SetActive(isShow);
                    break;
                case E_KnapsackState.KS_Ware_House:
                    if (isShow)
                    {
                        Init_WareHouse();
                    }
                    else
                    {
                        CleanWareHouse();
                        CloseWareHouse();
                    }
                    WareHouse.SetActive(isShow);
                    
                    break;
                case E_KnapsackState.KS_Shop:
                    if (isShow)
                    {
                        Init_Shop();
                    }
                    else
                    {
                        CleanNpcShop();
                    }
                    NpcShop.SetActive(isShow);
                    break;
                case E_KnapsackState.KS_Stallup:
                    if (isShow)
                    {
                        HideUpIcon();
                        Init_StallUp();
                    }
                    else
                    {
                        CleanStallUp();
                    }
                    StallUp.SetActive(isShow);
                    break;
                case E_KnapsackState.KS_Stallup_OtherPlayer:
                    if (isShow)
                    {
                        Init_StallOther();
                    }
                    else
                    {
                        CleanStallUpOther();
                    }
                    Stallup_OtherPlayer.SetActive(isShow);
                    break;
                case E_KnapsackState.KS_GiveCoin:
                    break;
                case E_KnapsackState.KS_GiveGoods:
                    break;
                case E_KnapsackState.KS_Consignment:
                    break;
                case E_KnapsackState.KS_Reduction:
                    break;
                case E_KnapsackState.KS_Inlay:
                    if (isShow)
                    {
                        Init_Inlay();
                    }
                    else
                    {
                        ClearInlayPanel();
                    }
                    InlayPanel.SetActive(isShow);
                    break;
                case E_KnapsackState.KS_Trade:
                    if (isShow)
                    {
                        Init_Trade();
                    }
                    else
                    {
                        ClearTrade();
                        CancleTrade().Coroutine();
                    }
                    Trade.SetActive(isShow);
                    break;
                case E_KnapsackState.KS_Recycle:
                    if (isShow)
                    {
                        CloseEquipPanel();
                        EquipPanel.SetActive(false);

                        Init_Recycle();
                    }
                    else
                    {
                        CleanRecycle();
                        RecycleEquipTools.Sava();
                    }
                    Sell.SetActive(isShow);
                    break;
                default:
                    break;
            }


        }

        /// <summary>
        /// 面板上 是否有装备 为放回背包
        /// </summary>
        /// <returns></returns>
        public bool IsHavaItem()
        {
            switch (curKnapsackState)
            {
                case E_KnapsackState.KS_Knapsack:
                    break;
                case E_KnapsackState.KS_Equipment:
                    break;
                case E_KnapsackState.KS_Gem_Merge:
                    return MergerPanelHavaItem();
                case E_KnapsackState.KS_Ware_House:
                    break;
                case E_KnapsackState.KS_Shop:
                    break;
                case E_KnapsackState.KS_Stallup:
                    break;
                case E_KnapsackState.KS_Stallup_OtherPlayer:
                    break;
                case E_KnapsackState.KS_GiveCoin:
                    break;
                case E_KnapsackState.KS_GiveGoods:
                    break;
                case E_KnapsackState.KS_Consignment:
                    break;
                case E_KnapsackState.KS_Reduction:
                    break;
                case E_KnapsackState.KS_Inlay:
                    break;
                case E_KnapsackState.KS_Trade:
                    return IsHaveTradeItem();
                default:
                    break;
            }
            return false;
        }


        public void HideUpIcon()
        {

            GetKnapsackGrid(ref grids, ref LENGTH_X, ref LENGTH_Y, E_Grid_Type.Knapsack);
            for (int i = 0; i < LENGTH_Knapsack_Y; i++)
            {
                for (int j = 0; j < LENGTH_Knapsack_X; j++)
                {

                    grids[j][i].Image.transform.Find("up").gameObject.SetActive(false);

                }

            }


        }
    }
}