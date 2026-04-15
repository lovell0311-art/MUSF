using ETModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETHotfix
{
    /// <summary>
    /// 地图物品掉落组件
    /// </summary>
    public static partial class ItemDropComponentSystem
    {
        public static void Init(this ItemDropComponent b_Component)
        {
            b_Component.ItemDict.Clear();
        }

        public static bool DropItem(this ItemDropComponent b_Component, Item dropItem, int mapX, int mapY)
        {
            if (!b_Component.ItemDict.ContainsKey(dropItem.ItemUID))
            {
                b_Component.ItemDict.Add(dropItem.ItemUID, dropItem);
                dropItem.data.posX = mapX;
                dropItem.data.posY = mapY;
            }
            return true;
        }

        public static bool PickUpItem(this ItemDropComponent b_Component, Player player, long itemUID)
        {
            if (b_Component.ItemDict.TryGetValue(itemUID, out Item curItem))
            {
                //验证是否捡取距离足够
                GamePlayer gamePlayer = player.GetCustomComponent<GamePlayer>();
                if (!CheckDropDistance(curItem.data.posX, curItem.data.posY, gamePlayer.UnitData.X, gamePlayer.UnitData.Y)) { return false; }

                var backpackComponent = player.GetCustomComponent<BackpackComponent>();
                if (backpackComponent != null)
                {
                    if (backpackComponent.AddItem(curItem,"地面拾取"))
                    {
                        b_Component.ItemDict.Remove(itemUID);
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool CheckDropDistance(int itemPosX, int itemPosY, int playerPosX, int playerPosY)
        {
            int num1 = itemPosX - playerPosX;
            int num2 = itemPosY - playerPosY;
            float distance = (float)Math.Sqrt(num1 * num1 + num2 * num2);
            return distance <= ItemDropComponent.DROP_MAXDISTANCE;
        }
    }
}
