using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using MongoDB.Bson.Serialization.Serializers;

namespace ETHotfix
{
    /// <summary>
    /// 特殊物品属性 程序写死
    /// </summary>
    public partial class KnapsackDataItem
    {
        /// <summary>
        /// 黑王马属性
        /// </summary>
        /// <param name="list"></param>
        public void GetHeiWangMaAtrs(ref List<string> list) 
        {
            list.Add($"<color=#79aaf4>乘骑</color>");
            Mounts_DarkHorseExpConfig mounts_DarkHorseExp = ConfigComponent.Instance.GetItem<Mounts_DarkHorseExpConfig>(this.GetProperValue(E_ItemValue.MountsLevel));
            list.Add($"等级:{this.GetProperValue(E_ItemValue.MountsLevel)}");
            list.Add($"经验值:{this.GetProperValue(E_ItemValue.MountsExp)}/{mounts_DarkHorseExp.Exp}");
            if (this.GetProperValue(E_ItemValue.RequireLevel) is int RequireLevel && RequireLevel != 0)
            {
                var difValue = RequireLevel - roleEntity.Property.GetProperValue(E_GameProperty.Level);
                list.Add(difValue <= 0 ? $"所需等级：{RequireLevel}" : $"<color={ColorTools.CanNotWareItemColor}>所需等级：{RequireLevel} (还需 {Mathf.Abs(difValue)})</color>");
            }
            GetUserType(ref list);
            GetItemSkill(ref list);
            list.Add($"<color=#79aaf4>防御力 增加 +{roleEntity.Property.GetProperValue(E_GameProperty.Property_Agility)/20+5+(this.GetProperValue(E_ItemValue.MountsLevel)*2)}</color>");
            list.Add($"<color=#79aaf4>吸收{(this.GetProperValue(E_ItemValue.MountsLevel)+10)/2}%伤害</color>");
        }
        /// <summary>
        /// 炎狼兽之角 幻影
        /// </summary>
        /// <param name="list"></param>
        public void GetYangLangShouZhiJiaoHuanYingAtrs(ref List<string> list)
        {
            list.Add($"坐骑"); 
            if (this.GetProperValue(E_ItemValue.RequireLevel) is int RequireLevel && RequireLevel != 0)
            {
                var difValue = RequireLevel - roleEntity.Property.GetProperValue(E_GameProperty.Level);
                list.Add(difValue <= 0 ? $"可使用等级：{RequireLevel}" : $"<color={ColorTools.CanNotWareItemColor}>可使用等级：{RequireLevel} (还需 {Mathf.Abs(difValue)})</color>");
            }

            GetBaseAtrs(ref list);
            GetUserType(ref list);
            GetItemSkill(ref list);

          /*  list.Add($"生命 {roleEntity.Property.GetProperValue(E_GameProperty.Level)/2} 提高");
            list.Add($"魔法值 {roleEntity.Property.GetProperValue(E_GameProperty.Level)/2} 提高");
            list.Add($"攻击力 {roleEntity.Property.GetProperValue(E_GameProperty.Level)/12} 提高");
            list.Add($"魔力 {roleEntity.Property.GetProperValue(E_GameProperty.Level)/25} 提高");*/
        }

        /// <summary>

        /// 天鹰

        /// </summary>

        /// <param name="list"></param>

        public void GetTianYingAtr(ref List<string> list)
        {

            if (this.GetProperValue(E_ItemValue.MountsLevel) < 1)

            {

                this.SetProperValue(E_ItemValue.MountsLevel, 1);

            }

            Mounts_DarkHorseExpConfig mounts_DarkHorseExp = ConfigComponent.Instance.GetItem<Mounts_DarkHorseExpConfig>(this.GetProperValue(E_ItemValue.MountsLevel));

            list.Add($"等级:{this.GetProperValue(E_ItemValue.MountsLevel)}");
            list.Add($"经验值:{this.GetProperValue(E_ItemValue.MountsExp)}/{mounts_DarkHorseExp.Exp}");

            //最小攻击力 40 + 天鹰等级*15 + 统率 / 8

            list.Add($"最小攻击力：{40 + GetProperValue(E_ItemValue.MountsLevel) * 1.5f + roleEntity.Property.GetProperValue(E_GameProperty.Property_Command) / 10}");
            //最大攻击力 50 + 天鹰等级*15 + 统率 / 4
            list.Add($"最大攻击力：{50 + GetProperValue(E_ItemValue.MountsLevel) * 1.5f + roleEntity.Property.GetProperValue(E_GameProperty.Property_Command) / 5}");
            //攻击速度 2 +  天鹰等级* 0.8 + 统率 / 50
            list.Add($"攻击速度：{2 + GetProperValue(E_ItemValue.MountsLevel) * .8f + roleEntity.Property.GetProperValue(E_GameProperty.Property_Command) / 50}");
            //攻击成功率 天鹰等级*16 + 10
            list.Add($"攻击成功率：{GetProperValue(E_ItemValue.MountsLevel) * 16f + 10}");
            GetUserType(ref list);
        }
        /// <summary>

        /// 烈火凤凰

        /// </summary>

        /// <param name="list"></param>
        public void GetLieHuoFengHuangAtr(ref List<string> list)
        {

            if (this.GetProperValue(E_ItemValue.MountsLevel) < 1)

            {

                this.SetProperValue(E_ItemValue.MountsLevel, 1);

            }

            Mounts_DarkHorseExpConfig mounts_DarkHorseExp = ConfigComponent.Instance.GetItem<Mounts_DarkHorseExpConfig>(this.GetProperValue(E_ItemValue.MountsLevel));

            list.Add($"等级:{this.GetProperValue(E_ItemValue.MountsLevel)}");
            list.Add($"经验值:{this.GetProperValue(E_ItemValue.MountsExp)}/{mounts_DarkHorseExp.Exp}");

            //最小攻击力 60 + 天鹰等级*15 + 统率 / 8

            list.Add($"最小攻击力：{60 + GetProperValue(E_ItemValue.MountsLevel) * 1.5f + roleEntity.Property.GetProperValue(E_GameProperty.Property_Command) / 10}");
            //最大攻击力 80 + 天鹰等级*15 + 统率 / 4
            list.Add($"最大攻击力：{80 + GetProperValue(E_ItemValue.MountsLevel) * 1.5f + roleEntity.Property.GetProperValue(E_GameProperty.Property_Command) / 5}");
            //攻击速度 3 +  天鹰等级 * 0.8 + 统率 / 50
            list.Add($"攻击速度：{3 + GetProperValue(E_ItemValue.MountsLevel) * .8f + roleEntity.Property.GetProperValue(E_GameProperty.Property_Command) / 50}");
            //攻击成功率 天鹰等级*16 + 20
            list.Add($"攻击成功率：{GetProperValue(E_ItemValue.MountsLevel) * 16f + 20}");
            GetUserType(ref list);
        }


        /// <summary>
        /// 是宝藏物品
        /// </summary>
        /// <returns></returns>
        public bool IsTreasureItem()
        {
            switch (this.ConfigId)
            {
                case 310073:
                case 310075:
                case 310076:
                case 310086:
                case 310087:
                    return true;
            }
            return false;
        }
        /// <summary>
        /// 宝藏显示
        /// </summary>
        /// <param name="list"></param>
        public void GetTreasureAtrs(ref List<string> list)
        {
            if (this.GetProperValue(E_ItemValue.IsUsing) == 1)
            {
                if (this.ConfigId == 310073)
                {
                    // 藏宝图 已开启
                    Npc_InfoConfig npc_Info = ConfigComponent.Instance.GetItem<Npc_InfoConfig>(this.GetProperValue(E_ItemValue.TreasureNpcConfigId));


                    list.Add("藏宝图已开启");
                    list.Add($"Boss: <color=red>{(npc_Info != null ? npc_Info.Name : "未知的")}</color>");
                    list.Add($"地图: <color=#00c000>{((SceneName)this.GetProperValue(E_ItemValue.TreasureMapId)).GetSceneName()}</color>");
                    if (this.GetProperValue(E_ItemValue.TreasureZoneId) == GlobalDataManager.EnterLineID)
                    {
                        list.Add($"坐标: <color=#00c000>[{this.GetProperValue(E_ItemValue.TreasurePosX)},{this.GetProperValue(E_ItemValue.TreasurePosY)}]</color>");
                        list.Add($"可点击小地图查看");
                    }
                    else
                    {
                        list.Add($"线路: <color=#00c000>{this.GetProperValue(E_ItemValue.TreasureZoneId)} 线</color>");
                        list.Add($"前往指定线路寻找宝藏");
                    }
                    list.Add("");
                    long time = this.GetProperValue(E_ItemValue.TimeLimit) - TimeHelper.GetNowSecond();
                    list.Add($"消失时间：{Mathf.Floor(time / (60 * 60 * 24))}天{Mathf.Floor((time % (60 * 60 * 24)) / (60 * 60))}时{Mathf.Floor((time % (60 * 60)) / 60)}分");
                }
                else if(this.ConfigId == 310075 ||
                    this.ConfigId == 310076 ||
                    this.ConfigId == 310086 ||
                    this.ConfigId == 310087)
                {
                    EnemyConfig_InfoConfig enemy_Info = ConfigComponent.Instance.GetItem<EnemyConfig_InfoConfig>(this.GetProperValue(E_ItemValue.TreasureNpcConfigId));
                    list.Add("宝藏已开启");
                    list.Add($"变异怪: <color=red>{(enemy_Info != null ? enemy_Info.Name : "未知的")}</color>");
                    list.Add($"地图: <color=#00c000>{((SceneName)this.GetProperValue(E_ItemValue.TreasureMapId)).GetSceneName()}</color>");
                    if (this.GetProperValue(E_ItemValue.TreasureZoneId) == GlobalDataManager.EnterLineID)
                    {
                        list.Add($"坐标: <color=#00c000>[{this.GetProperValue(E_ItemValue.TreasurePosX)},{this.GetProperValue(E_ItemValue.TreasurePosY)}]</color>");
                        list.Add($"可点击小地图查看");
                    }
                    else
                    {
                        list.Add($"线路: <color=#00c000>{this.GetProperValue(E_ItemValue.TreasureZoneId)} 线</color>");
                        list.Add($"前往指定线路寻找宝藏");
                    }
                }

            }
            else
            {
                GetUserType(ref list);
                GetRemarks(ref list);//备注提示信息
            }

        }
    }
}
