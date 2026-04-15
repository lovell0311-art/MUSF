using System;
using System.Collections.Generic;
using System.Text;
using ETModel;
using CustomFrameWork;

namespace ETHotfix
{
    public static partial class ItemSystem
    {
        /// <summary>
        /// 设置属性
        /// </summary>
        /// <param name="self"></param>
        /// <param name="propId">修改的属性id</param>
        /// <param name="value">设置的目标值</param>
        /// <param name="ownPlayer">物品所属玩家</param>
        public static void SetProp(this Item self, EItemValue propId, int value, Player ownPlayer = null)
        {
            if (self.__Property.TryGetValue(propId, out var v))
            {
                if (v == value)
                {
                    return;
                }
                self.__Property[propId] = value;
            }
            else
            {
                self.__Property.Add(propId, value);
            }

            // TODO 属性变动
            // 检查这个属性需不需要落地
            if (propId.NeedToSave())
            {
                // TODO 将变动的值，同步到DBItemData中
                if (self.data.PropertyData.ContainsKey((int)propId))
                {
                    self.data.PropertyData[(int)propId] = value;
                }
                else
                {
                    self.data.PropertyData.Add((int)propId, value);
                }
                if (ownPlayer != null)
                {
                    // TODO 保存变动数据
                    self.OnlySaveDB();
                }
            }
            if (ownPlayer != null)
            {
                // 通知玩家，物品属性变动
                var notice = new G2C_ItemsPropChange_notice();
                notice.PropList.Add(new Struct_Property()
                {
                    PropID = (int)propId,
                    Value = value
                });
                notice.ItemUUID = self.ItemUID;
                ownPlayer.Send(notice);
            }
        }

        /// <summary>
        /// 获取属性
        /// </summary>
        /// <param name="self"></param>
        /// <param name="propId"></param>
        /// <returns></returns>
        public static int GetProp(this Item self, EItemValue propId)
        {
            if (self.__Property.TryGetValue(propId, out var v))
            {
                return v;
            }
            return 0;
        }

        /// <summary>
        /// 获取要保存到DB中的属性
        /// </summary>
        /// <param name="self"></param>
        /// <param name="propId"></param>
        /// <returns></returns>
        public static int GetDBProp(this Item self, EDBItemValue propId)
        {
            if (self.data.PropertyData.TryGetValue((int)propId, out var v))
            {
                return v;
            }
            return 0;
        }

    }
}
