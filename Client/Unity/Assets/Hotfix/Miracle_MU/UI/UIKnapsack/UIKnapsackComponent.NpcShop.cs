using System.Collections.Generic;
using UnityEngine;
using ETModel;
using UnityEngine.UI;

namespace ETHotfix
{
    public enum E_BuyType 
    {
        Remote = 0,//远程购买
        Normal = 1,//默认购买
    }

    /// <summary>
    /// NPC 商城
    /// </summary>
    public partial class UIKnapsackComponent
    {
        private GameObject NpcShopContent;
        private readonly HashSet<GameObject> npcShopVisuals = new HashSet<GameObject>();

        private KnapsackGrid[][] NpcShopGrids;
        public const int LENGTH_NpcShop_X = 8;
        public const int LENGTH_NpcShop_Y = 12;

        public long CurNpcUUid;
        public E_BuyType buyType=E_BuyType.Normal;

        public Button RepairBtn;//维修按钮
        public Text icontxt;//金币
        private void Init_Shop()
        {
            NpcShopContent = NpcShop.GetReferenceCollector().GetGameObject("Grids");

            NpcShopGrids = new KnapsackGrid[LENGTH_NpcShop_X][];
            for (int i = 0; i < NpcShopGrids.Length; i++)
            {
                NpcShopGrids[i] = new KnapsackGrid[LENGTH_NpcShop_Y];
            }
            //初始化格子
            CreatGrid(LENGTH_NpcShop_X, LENGTH_NpcShop_Y, NpcShopContent.transform, E_Grid_Type.Shop, ref NpcShopGrids);
            RepairBtn = NpcShop.GetReferenceCollector().GetButton("RepairBtn");//维修按钮
            RepairBtn.gameObject.SetActive(false);
            RepairBtn.onClick.AddSingleListener(() => 
            {
            var npc = UnitEntityComponent.Instance.Get<NPCEntity>(CurNpcUUid);
             RepairEquips(CurNpcUUid, npc.CurrentNodePos.x,npc.CurrentNodePos.z);
            });
            //金币
            icontxt = NpcShop.GetReferenceCollector().GetText("icon");
            icontxt.text = roleEntity.Property.GetProperValue(E_GameProperty.GoldCoin).ToString();
            Game.EventCenter.EventListenner<long>(EventTypeId.GLOD_CHANGE, ChangeGlogIcon);
        }

        public void ChangeGlogIcon(long icon)
        {
            icontxt.text = icon.ToString();
        }
        /// <summary>
        /// 初始化商店的物品
        /// </summary>
        /// <param name="itemDic">商店物品字典</param>
        /// <param name="npcUid">商店NPC的UUID</param>
        public void Init_ShopEquip(Dictionary<long,KnapsackDataItem> itemDic,long npcUid,E_BuyType buyType)
        {
            ClearNpcShopVisuals();
            CurNpcUUid = npcUid;
            this.buyType = buyType;
            foreach (var item in itemDic.Values)
            {
                AddItem(item,type:E_Grid_Type.Shop);
            }
        }

        public void TrackNpcShopVisual(GameObject go)
        {
            if (go == null)
            {
                return;
            }

            npcShopVisuals.Add(go);
        }

        public void UntrackNpcShopVisual(GameObject go)
        {
            if (go == null)
            {
                return;
            }

            npcShopVisuals.Remove(go);
        }

        private void ClearNpcShopVisuals(bool clearNpcCache = false)
        {
            if (NpcShopGrids != null)
            {
                for (int y = 0; y < LENGTH_NpcShop_Y; y++)
                {
                    for (int x = 0; x < LENGTH_NpcShop_X; x++)
                    {
                        KnapsackGrid grid = NpcShopGrids[x][y];
                        if (grid == null)
                        {
                            continue;
                        }

                        if (grid.GridObj != null)
                        {
                            npcShopVisuals.Add(grid.GridObj);
                        }

                        grid.GridObj = null;
                        grid.IsOccupy = false;
                        grid.Data.UUID = 0;
                        grid.Data.EquipmentPart = 0;
                        grid.Data.ItemData = null;
                    }
                }
            }

            foreach (GameObject visual in new List<GameObject>(npcShopVisuals))
            {
                if (visual == null)
                {
                    continue;
                }

                ResourcesComponent.Instance.DestoryGameObjectImmediate(visual, visual.name.StringToAB());
            }

            npcShopVisuals.Clear();

            if (clearNpcCache)
            {
                UnitEntityComponent.Instance.Get<NPCEntity>(CurNpcUUid)?.ClearNpcShop();
                Game.EventCenter.RemoveEvent<long>(EventTypeId.GLOD_CHANGE, ChangeGlogIcon);
            }
        }
        /// <summary>
        /// 清理NPC商店
        /// </summary>
        public void CleanNpcShop()
        {
            ClearNpcShopVisuals(true);
        }
    }
}