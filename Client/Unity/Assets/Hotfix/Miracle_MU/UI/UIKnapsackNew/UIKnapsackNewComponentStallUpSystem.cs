using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;

namespace ETHotfix
{
    public static class UIKnapsackNewComponentStallUpSystem
    {
        public static async ETTask InitStallUp(this UIKnapsackNewComponent self)
        {
            string res = "StallUp";
            //加载对应面板
            await ResourcesComponent.Instance.LoadGameObjectAsync(res.StringToAB(), res);
            //实例化面板 
            GameObject stallUp = ResourcesComponent.Instance.LoadGameObject(res.StringToAB(), res);
            stallUp.transform.SetParent(self.plane.transform, false);
            stallUp.transform.localPosition = Vector3.zero;
            stallUp.transform.localScale = Vector3.one;
            self.stallUpCollector = stallUp.GetReferenceCollector();
            self.stallUpComponent = self.roleEntity.GetComponent<RoleStallUpComponent>();

            self.StallUpInfo = LocalDataJsonComponent.Instance.LoadData<StallUpInfo>(LocalJsonDataKeys.StallUpName) ?? new StallUpInfo();


            self.startUpContent = self.stallUpCollector.GetGameObject("Grids");

            self.stallupNameInput = self.stallUpCollector.GetInputField("InputName");

            self.stallupNameInput.interactable = !self.stallUpComponent.IsStallUp;
            //设置摊位的名字
            self.stallupNameInput.onEndEdit.AddSingleListener(async value =>
            {
                if (string.IsNullOrEmpty(value)) return;
                //设置摊位
                //判断是否包含非法字符
                string st = value;
                if (self.CountDigitsInString(st) > 10)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "输入信息有误");
                    return;
                }
                if (self.CountDigitsInString(st) > 7)
                {
                    st = self.ReplaceDigits(st, '*');
                }
                Log.DebugBrown("摆摊" + st);
                self.stallupNameInput.text = st;
                G2C_BaiTanSetNameResponse g2C_BaiTanSetName = (G2C_BaiTanSetNameResponse)await SessionComponent.Instance.Session.Call(new C2G_BaiTanSetNameRequest { BaiTanName = st });
                if (g2C_BaiTanSetName.Error != 0)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_BaiTanSetName.Error.GetTipInfo());
                }
                else
                {
                    self.StallUpInfo.StallName = $"{st}_{self.roleEntity.Id}";
                    LocalDataJsonComponent.Instance.SavaData(self.StallUpInfo, LocalJsonDataKeys.StallUpName);
                }
                Log.DebugGreen($"摊铺名字设置完成");
            });
     
            self.stallupNameInput.text = self.stallUpComponent.curStallUpName;


            self.stallUpCollector.GetButton("CloseStallUpBtn").onClick.AddListener(() =>
            {
                self.CloseStallUpBtnClick().Coroutine();
            });
            self.stallUpCollector.GetButton("StarStallUpStar").onClick.AddListener(() =>
            {
                self.StartStallUp().Coroutine();
            });

            self.StallUpGrids = new KnapsackNewGrid[UIKnapsackNewComponent.LENGTH_StallUp_X][];
            for (int i = 0; i < self.StallUpGrids.Length; i++)
            {
                self.StallUpGrids[i] = new KnapsackNewGrid[UIKnapsackNewComponent.LENGTH_StallUp_Y];
            }
            //初始化格子
            self.CreatGrid(UIKnapsackNewComponent.LENGTH_StallUp_X, UIKnapsackNewComponent.LENGTH_StallUp_Y, self.startUpContent.transform, E_Grid_Type.Stallup, ref self.StallUpGrids);
            ///通知服务器 开始摆摊
            //G2C_BaiTanResponse g2C_BaiTanResponse = (G2C_BaiTanResponse)await SessionComponent.Instance.Session.Call(new C2G_BaiTanRequest { });
            ///初始化摊名
            await self.InitStallName();

            //显示摊位的物品
            await self.Init_StallUpEquip();

        }

        public static string ReplaceDigits(this UIKnapsackNewComponent self,string originalString, char replacement)
        {
            // 使用正则表达式替换所有数字
            return Regex.Replace(originalString, @"\d", replacement.ToString());
        }

        public static async ETTask InitStallName(this UIKnapsackNewComponent self)
        {
            if (string.IsNullOrEmpty(self.StallUpInfo.StallName) == false && long.Parse(self.StallUpInfo.StallName.Split('_')[1]) == self.roleEntity.Id)
            {
                self.stallUpComponent.curStallUpName = self.StallUpInfo.StallName.Split('_')[0];
                G2C_BaiTanSetNameResponse g2C_BaiTanSetName = (G2C_BaiTanSetNameResponse)await SessionComponent.Instance.Session.Call(new C2G_BaiTanSetNameRequest { BaiTanName = self.stallUpComponent.curStallUpName });
                if (g2C_BaiTanSetName.Error != 0)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_BaiTanSetName.Error.GetTipInfo());
                }
                else
                {
                    self.StallUpInfo.StallName = $"{self.stallUpComponent.curStallUpName}_{self.roleEntity.Id}";
                    LocalDataJsonComponent.Instance.SavaData(self.StallUpInfo, LocalJsonDataKeys.StallUpName);
                }
                Log.DebugGreen($"摊铺名字设置完成");
            }
        }

        public static void ClearStallUp(this UIKnapsackNewComponent self)
        {
            self.ReleaseGridCollectionVisuals(self.StallUpGrids);
            if (self.stallUpCollector)
            {
                GameObject.Destroy(self.stallUpCollector.gameObject);
                self.stallUpCollector = null;   
            }
            self.isOpenStallUp = false;
        }

        /// <summary>
        /// 初始化摊位的物品
        /// </summary>
        /// <param name="itemList">商店物品ID的 集合</param>
        public static async ETTask Init_StallUpEquip(this UIKnapsackNewComponent self)
        {

            //第一次打开 摊位物品为空 请求获取
            Log.DebugBrown($"stallUpComponent.StallUpItemDic.Count:{self.stallUpComponent.StallUpItemDic.Count}");
            self.stallUpComponent.StallUpItemDic.Clear();

            Log.DebugPurple("获取摊位上的物品");
            G2C_BaiTanLookLookResponse g2C_BaiTanLookLook = (G2C_BaiTanLookLookResponse)await SessionComponent.Instance.Session.Call(new C2G_BaiTanLookLookRequest
            {
                BaiTanInstanceId = self.roleEntity.Id
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
                    self.stallUpComponent.StallUpItemDic[item.ItemUUID] = knapsackDataItem;
                    self.AddItem(knapsackDataItem, type: E_Grid_Type.Stallup);
                }
            }

        }

        /// <summary>
        /// 开始摆摊
        /// </summary>
        public static async  ETTask StartStallUp(this UIKnapsackNewComponent self)
        {
            if (self.stallUpComponent.IsStallUp)
            {
                return;
            }
            if (self.stallUpComponent.StallUpItemDic.Count == 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "摊位没有物品，无法开摊位");
                return;
            }

            if (string.IsNullOrEmpty(self.stallupNameInput.text))
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "请先输入摊铺名字");
                return;
            }
            if (UnitEntityComponent.Instance.LocalRole.MaxMonthluCardTimeSpan.TotalSeconds<= 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "没有高级月卡禁止摆摊");
                return;
            }
            G2C_BaiTanOpenResponse g2C_BaiTanOpenResponse = (G2C_BaiTanOpenResponse)await SessionComponent.Instance.Session.Call(new C2G_BaiTanOpenRequest { });
            if (g2C_BaiTanOpenResponse.Error != 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_BaiTanOpenResponse.Error.GetTipInfo());
            }
            else
            {
                self.stallUpComponent.StartStallUp(self.stallupNameInput.text);
                self.OnCloseClick();
            }


        }

        public static async ETTask CloseStallUpBtnClick(this UIKnapsackNewComponent self)
        {
            if (self.stallUpComponent.IsStallUp == false)
            {
                Log.DebugGreen($"摊位未开启");
                return;
            }

            G2C_BaiTanCloseResponse g2C_BaiTanClose = (G2C_BaiTanCloseResponse)await SessionComponent.Instance.Session.Call(new C2G_BaiTanCloseRequest
            {
                BaiTanInstanceId = self.roleEntity.Id
            });
            if (g2C_BaiTanClose.Error != 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_BaiTanClose.Error.GetTipInfo());
                Log.DebugGreen($"{g2C_BaiTanClose.Error}");
            }
            else
            {
                self.OnCloseClick();
            }
        }

        public static int CountDigitsInString(this UIKnapsackNewComponent self, string str)
        {
            int count = 0;
            foreach (char c in str)
            {
                if (char.IsDigit(c))
                {
                    count++;
                }
            }
            return count;
        }
    }
}
