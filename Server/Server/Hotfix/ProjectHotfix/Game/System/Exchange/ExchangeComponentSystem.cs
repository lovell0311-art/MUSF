using System;
using System.Collections;
using System.Diagnostics;

using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using CustomFrameWork;
using CustomFrameWork.Baseic;
using CustomFrameWork.Component;
using ETModel;

namespace ETHotfix
{
    [EventMethod(typeof(ExchangeComponent), EventSystemType.INIT)]
    public class ExchangeComponentEventOnInit : ITEventMethodOnInit<ExchangeComponent>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="b_Component"></param>
        public void OnInit(ExchangeComponent self)
        {
            self.OnInit();
        }
    }

    //[EventMethod(typeof(TeamComponent), EventSystemType.DISPOSE)]
    //public class TeamComponentEventOnDispose : ITEventMethodOnDispose<TeamComponent>
    //{
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="b_Component"></param>
    //    public void OnDispose(TeamComponent b_Component)
    //    {
    //        b_Component.OnDispose();
    //    }
    //}

    public static partial class ExchangeComponentSystem
    {
        public static void OnInit(this ExchangeComponent self)
        {
            
        }

        /// <summary>
        /// 开始交易
        /// </summary>
        public static bool StartExchange(this ExchangeComponent self, long targetPlayerGameUserID)
        {
            if (self.ExchangeTargetGameUserId == 0) 
            {
                self.InitState(targetPlayerGameUserID);
                return true;
            }
            //已经在交易中，开始交易失败
            return false;
        }

        /// <summary>
        /// 结束交易
        /// </summary>
        public static void EndExchange(this ExchangeComponent self)
        {
            if(self.ItemPosDict.Count != 0)
            {
                // 交易结束，解锁物品
                BackpackComponent backpack = self.Parent.GetCustomComponent<BackpackComponent>();
                foreach(long uid in self.ItemPosDict.Keys)
                {
                    Item item = backpack.GetItemByUID(uid);
                    if (item == null) continue;
                    item.SetProp(EItemValue.IsLocking, 0, self.Parent);
                }
            }
            self.ClearState();
        }
        /// <summary>
        /// 设置交易冷却时间
        /// </summary>
        /// <param name="self"></param>
        public static void SetExchangeCD(this ExchangeComponent self)
        {
            self.ExchangeTime = Help_TimeHelper.GetNowSecond() + 20;
        }
        /// <summary>
        /// 放入物品
        /// </summary>
        public static bool AddItem(this ExchangeComponent self, Item item,int posX,int posY)
        {
            //检查物品是否已经加入交易物品栏中
            if (self.ItemPosDict.ContainsKey(item.ItemUID))
            {
                return false;
            }
            if (self.BoxStatus.AddItem(item.ConfigData.X, item.ConfigData.Y, posX, posY))
            {
                self.ItemPosDict.Add(item.ItemUID,new Vector2(posX,posY));
                // 进入交易系统，解锁物品
                item.SetProp(EItemValue.IsLocking, 1, self.Parent);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 移动物品
        /// </summary>
        public static bool MoveItem(this ExchangeComponent self, Item item, int posX, int posY)
        {
            if (self.ItemPosDict.TryGetValue(item.ItemUID,out var curPos) == false)
            {
                return false;
            }
            self.BoxStatus.RemoveItem(item.ConfigData.X, item.ConfigData.Y, (int)curPos.X, (int)curPos.Y);
            if (self.BoxStatus.AddItem(item.ConfigData.X, item.ConfigData.Y, posX, posY))
            {
                self.ItemPosDict[item.ItemUID] = new Vector2(posX,posY);
                return true;
            }
            else {
                self.BoxStatus.AddItem(item.ConfigData.X, item.ConfigData.Y, (int)curPos.X, (int)curPos.Y);
                return false;
            }
        }

        /// <summary>
        /// 移除物品
        /// </summary>
        public static bool RemoveItem(this ExchangeComponent self, Item item)
        {
            //检查物品是否已经加入交易物品栏中
            if (self.ItemPosDict.TryGetValue(item.ItemUID, out var curPos) == false)
            {
                return false;
            }

            self.BoxStatus.RemoveItem(item.ConfigData.X, item.ConfigData.Y, (int)curPos.X, (int)curPos.Y);
            self.ItemPosDict.Remove(item.ItemUID);
            // 离开交易系统，解锁物品
            item.SetProp(EItemValue.IsLocking, 0, self.Parent);
            return true;
        }

        /// <summary>
        /// 检测是否能装下交易物品
        /// </summary>
        /// <param name="self"></param>
        /// <param name="message"></param>
        /// <returns>ErrorCodeHotfix错误码</returns>
        public static int CheckExchange(this ExchangeComponent self,out string message)
        {
            Player targetPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(self.mPlayer.GameAreaId, self.ExchangeTargetGameUserId);
            if (targetPlayer != null)
            {
                ExchangeComponent targetExchange = targetPlayer.GetCustomComponent<ExchangeComponent>();
                BackpackComponent targetBackpack = targetPlayer.GetCustomComponent<BackpackComponent>();
                BackpackComponent mBackpack = self.mPlayer.GetCustomComponent<BackpackComponent>();
                //Clone一份Itembox用于验证
                ItemsBoxStatus mItemBox = mBackpack.mItemBox.Clone();
                //移除自己交易的物品
                foreach (var item in self.ItemPosDict)
                {
                    if (mBackpack.mItemDict.TryGetValue(item.Key, out Item curItem))
                    {
                        mItemBox.RemoveItem(curItem.ConfigData.X, curItem.ConfigData.Y, (int)item.Value.X, (int)item.Value.Y);
                    }
                    else
                    {
                        message = "物品未找到!";
                        return 807;
                    }
                }
                //放入交易物品验证
                foreach (var item in targetExchange.ItemPosDict)
                {
                    if (targetBackpack.mItemDict.TryGetValue(item.Key, out Item curItem))
                    {
                        int posX = 0, posY = 0;
                        if (!mItemBox.AddItem(curItem.ConfigData.X, curItem.ConfigData.Y,ref posX,ref posY))
                        {
                            message = "背包空间不足，交易失败!";
                            return 812;
                        }
                    }
                    else
                    {
                        message = "物品未找到!";
                        return 813;
                    }
                }
                message = string.Empty;
                return ErrorCodeHotfix.ERR_Success;
            }
            message = "未找到目标交易玩家";
            return 200;
        }
    }
}