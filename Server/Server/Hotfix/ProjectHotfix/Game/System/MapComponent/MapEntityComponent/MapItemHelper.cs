using System;
using System.Collections.Generic;
using System.Text;
using ETModel;
using CustomFrameWork;

namespace ETHotfix
{
    public static class MapItemHelper
    {
        public static G2C_ItemDropData ToMessage(this MapItem self)
        {
            self.g2C_ItemDropData.InstanceId = self.InstanceId;
            self.g2C_ItemDropData.Key = self.ConfigId;
            self.g2C_ItemDropData.PosX = self.Position.x;
            self.g2C_ItemDropData.PosY = self.Position.y;
            self.g2C_ItemDropData.ProtectTick = self.ProtectTick;
            self.g2C_ItemDropData.KillerId.Clear();
            self.g2C_ItemDropData.KillerId.AddRange(self.KillerId);
            self.g2C_ItemDropData.CreateType = (int)self.CreateType;

            if (self.Item == null)
            {
                self.g2C_ItemDropData.Value = self.Count;
                self.g2C_ItemDropData.Quality = self.Quality;
                self.g2C_ItemDropData.Level = self.Level;
                self.g2C_ItemDropData.SetId = self.SetId;
                self.g2C_ItemDropData.OptLevel = self.OptLevel;
            }
            else
            {
                self.g2C_ItemDropData.Value = self.Item.GetProp(EItemValue.Quantity);
                self.g2C_ItemDropData.Quality = self.Item.GetQuality();
                self.g2C_ItemDropData.Level = self.Item.GetProp(EItemValue.Level);
                self.g2C_ItemDropData.SetId = self.Item.GetProp(EItemValue.SetId);
                self.g2C_ItemDropData.OptLevel = self.Item.GetProp(EItemValue.OptLevel);
            }
            return self.g2C_ItemDropData;
        }

        public static string ToLogString(this MapItem self)
        {
            if(self.Item == null)
            {
                ItemConfigManagerComponent itemConfigManager = Root.MainFactory.GetCustomComponent<ItemConfigManagerComponent>();
                ItemConfig itemConfig = itemConfigManager.Get(self.ConfigId);
                string tag = "";
                if ((self.Quality & 1) == 1)
                {
                    // 有技能
                    tag += "#技能#";
                }
                if ((self.Quality & 1 << 2) == 1 << 2)
                {
                    // 有幸运
                    tag += "#幸运#";
                }
                if ((self.Quality & 1 << 3) == 1 << 3)
                {
                    // 有卓越
                    tag += "#卓越#";
                }
                if ((self.Quality & 1 << 4) == 1 << 4)
                {
                    // 有套装
                    tag += "#套装#";
                }
                if ((self.Quality & 1 << 5) == 1 << 5)
                {
                    // 有镶嵌
                    tag += "#镶嵌#";
                }
                return $"name={itemConfig.Id},id={itemConfig.Name},lv={self.Level},z={self.OptLevel},tag={tag}";
            }
            else
            {
                string tag = "";
                if (self.Item.HaveSkill())
                {
                    tag += "#技能#";
                }
                if (self.Item.HaveLuckyAttr())
                {
                    tag += "#幸运#";
                }
                if (self.Item.HaveExcellentOption())
                {
                    tag += "#卓越#";
                }
                if (self.Item.HaveSetOption())
                {
                    tag += "#套装#";
                }
                if (self.Item.HaveEnableSocket())
                {
                    tag += "#镶嵌#";
                }
                return $"name={self.Item.ConfigID},id={self.Item.ConfigData.Name},lv={self.Item.GetProp(EItemValue.Level)},z={self.Item.GetProp(EItemValue.OptLevel)},tag={tag}";
            }
        }
    }
}
