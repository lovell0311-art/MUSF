using ETModel;
using System.Collections.Generic;
using CustomFrameWork.Component;
using CustomFrameWork;
using System.Threading.Tasks;
using System.Threading.Tasks;
using TencentCloud.Bri.V20190328.Models;
using System.Drawing;
using System;

namespace ETHotfix
{



    public static partial class GamePlayerSystem
    {
        public static void SetData(this GamePlayer b_GamePlayer, DBGamePlayerData b_GamePlayerData, DBPlayerUnitData b_UnitData)
        {
            b_GamePlayer.Data = b_GamePlayerData;
            b_GamePlayer.UnitData = b_UnitData;
        }

        public static void AfterAwake(this GamePlayer b_GamePlayer)
        {
            if (b_GamePlayer.GamePropertyDic == null)
                b_GamePlayer.GamePropertyDic = new Dictionary<E_GameProperty, int>();

            ReadConfigComponent mReadConfigComponent = CustomFrameWork.Root.MainFactory.GetCustomComponent<ReadConfigComponent>();
            if (mReadConfigComponent.GetJson<CreateRole_InfoConfigJson>().JsonDic.TryGetValue(b_GamePlayer.Data.PlayerTypeId, out var mHeroConfig) == false)
            {
                Log.Error($"角色id:{b_GamePlayer.Data.PlayerTypeId}不存在");
            }

            b_GamePlayer.GetNumerialFunc = (E_GameProperty b_GameProperty) =>
            {
                return b_GamePlayer.GetNumerial(b_GameProperty);
            };

            b_GamePlayer.AwakeProperty(mHeroConfig);

            //初始化装备加成总属性
            b_GamePlayer.AwakeEquipment();

            b_GamePlayer.AwakeSkill();

            b_GamePlayer.AwakeMaster();

            b_GamePlayer.AwakePets();

            b_GamePlayer.DataUpdate();

            //b_GamePlayer.DataAddPropertyBuffer();
        }
        public static void DataUpdate(this GamePlayer b_GamePlayer)
        {
            b_GamePlayer.DataUpdateProperty();
        }

        /// <summary>
        /// 1:拾取2:强化3:合成4:增加卓越条数5:开启宝箱6:幸运强化7:藏宝阁上架8:奇迹币商城兑换
        /// </summary>
        /// <param name="gamePlayer"></param>
        /// <param name="Type"></param>
        /// <param name="item"></param>
        /// <param name="IsOk"></param>
        /// <returns></returns>
        public static async Task SendItem(this GamePlayer gamePlayer, int Type, Item item,bool IsOk = false,int monsterId = 0)
        {
            try
            {

                //var PlayerList = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().GetAllByZone(gamePlayer.Player.GameAreaId);

                //G2C_MessageInChatRoom_notice notice = new G2C_MessageInChatRoom_notice();
                int LineId = gamePlayer.Player.SourceGameAreaId & 0xffff;
                G2C_FullServiceHornnotice notice = new G2C_FullServiceHornnotice();
                notice.LineId = LineId;

                var mMatchConfigs = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfos(AppType.Game);
                var Map = Root.MainFactory.GetCustomComponent<ReadConfigComponent>();
                if (!Map.GetJson<Map_InfoConfigJson>().JsonDic.TryGetValue(gamePlayer.UnitData.Index, out var MapInfo))
                {
                    return;
                }
                //notice.LineId = gamePlayer.Player.GameAreaId;
                switch (Type)
                {
                    case 1://拾取
                        {
                            if (item.NotSevedToDB)
                            {
                                string GetSceneName()
                                {
                                    Enemy_InfoConfig monsterConfig = null;
                                    if(monsterId == 0 || !Map.GetJson<Enemy_InfoConfigJson>().JsonDic.TryGetValue(monsterId, out monsterConfig))
                                    {
                                        monsterConfig = null;
                                    }
                                    if (monsterConfig == null)
                                    {
                                        return $"<color=#A67D3D>{MapInfo.SceneName}</color>";
                                    }
                                    else
                                    {
                                        return $"<color=#A67D3D>{MapInfo.SceneName}</color>-<color=#77d1ff>{monsterConfig.Name}</color>";
                                    }
                                }

                                notice.MessageInfo = "";
                                if (item.data.ExcellentEntry.Count > 0)
                                {
                                    notice.MessageInfo = $"玩家 <color=#00c000>{gamePlayer.Data.NickName}</color> 在{GetSceneName()}拾取了 {item.GetClientName()}";
                                    break;
                                }
                                if (item.HaveSetOption())
                                {
                                    var mReadConfigComponent = Root.MainFactory.GetCustomComponent<ReadConfigComponent>();
                                    if (mReadConfigComponent.GetJson<SetItem_TypeConfigJson>().JsonDic.TryGetValue(item.GetProp(EItemValue.SetId), out var SetInfo))
                                    {
                                        notice.MessageInfo = $"玩家 <color=#00c000>{gamePlayer.Data.NickName}</color> 在{GetSceneName()}拾取了 {item.GetClientName()}";
                                        break;
                                    }
                                }
                                EItemType TName = (EItemType)(item.ConfigData.Id / 10000);
                                if (TName == EItemType.Gemstone)
                                {
                                    notice.MessageInfo = $"玩家 <color=#00c000>{gamePlayer.Data.NickName}</color> 在{GetSceneName()}拾取了 {item.GetClientName()}";
                                    break;
                                }
                                if (item.ConfigData.Id == 320297 ||
                                    item.ConfigData.Id == 320298 ||
                                    item.ConfigData.Id == 320019 ||
                                    item.ConfigData.Id == 320407 ||
                                    item.ConfigData.Id == 320408 ||
                                    item.ConfigData.Id == 320409)
                                {
                                    notice.MessageInfo = $"玩家 <color=#00c000>{gamePlayer.Data.NickName}</color> 在{GetSceneName()}拾取了 {item.GetClientName()}";
                                    break;
                                }
                                if (notice.MessageInfo == "") return;
                                notice.SendGameUserId = 0;
                                //notice.SendUserName = "<color=#c0c000>系统</color>";
                                notice.SendUserName = "";
                                notice.SendTime = TimeHelper.ClientNowSeconds();

                                //foreach (var player in PlayerList)
                                //{
                                //    player.Value.Send(notice);
                                //}
                                foreach (var Server in mMatchConfigs)
                                {
                                    Dictionary<int, List<int>> keyValuePairs = Help_JsonSerializeHelper.DeSerialize<Dictionary<int, List<int>>>(Server.RunParameter);
                                    int AreaId = 1;
                                    foreach (var KeyValuePair in keyValuePairs)
                                    {
                                        AreaId = KeyValuePair.Key >> 16;
                                        break;
                                    }
                                    if (gamePlayer.Player.GameAreaId == AreaId)
                                        Game.Scene.GetComponent<NetInnerComponent>().Get(Server.ServerInnerIP).Send(notice);
                                }
                                return;
                            }
                            return;
                        }
                    case 2://强化
                        {
                            if (IsOk)
                                notice.MessageInfo = $"恭喜 <color=#00c000>{gamePlayer.Data.NickName}</color> 强化 {item.GetClientName()} 成功";
                            else
                                notice.MessageInfo = $"很遗憾 <color=#00c000>{gamePlayer.Data.NickName}</color> 强化 {item.GetClientName()} 失败";
                        }
                        break;
                    case 3://合成
                        {
                            if (IsOk)
                                notice.MessageInfo = $"恭喜 <color=#00c000>{gamePlayer.Data.NickName}</color> 合成 {item.GetClientName()} 成功";
                            else
                                notice.MessageInfo = $"很遗憾 <color=#00c000>{gamePlayer.Data.NickName}</color> 合成 {item.GetClientName()} 失败";
                        }
                        break;
                    case 4: 
                        {
                            if (IsOk)
                                notice.MessageInfo = $"恭喜 <color=#00c000>{gamePlayer.Data.NickName}</color> 增加 {item.GetClientName()} 卓越词条成功";
                            else
                                notice.MessageInfo = $"很遗憾 <color=#00c000>{gamePlayer.Data.NickName}</color> 增加 {item.GetClientName()} 卓越词条失败";

                        }
                        break;
                    case 5: 
                        {
                            notice.MessageInfo = $"恭喜 <color=#00c000>{gamePlayer.Data.NickName}</color> 开启宝箱获得 {item.GetClientName()}";
                        }
                        break;
                    case 6:
                        {
                            if (IsOk)
                                notice.MessageInfo = $"恭喜 <color=#00c000>{gamePlayer.Data.NickName}</color> 增加 {item.GetClientName()} 幸运属性成功";
                            else
                                notice.MessageInfo = $"很遗憾 <color=#00c000>{gamePlayer.Data.NickName}</color> 增加 {item.GetClientName()} 幸运属性失败";
                        }
                        break;
                    case 7: 
                        {
                            notice.MessageInfo = $"玩家 <color=#00c000>{gamePlayer.Data.NickName}</color> 上架藏宝阁物品 {item.GetClientName()} ";
                        }
                        break;
                    case 8: 
                        {
                            notice.MessageInfo = $"玩家 <color=#00c000>{gamePlayer.Data.NickName}</color> 在奇迹币商城兑换物品 {item.GetClientName()} ";
                        }
                        break;
                }

                notice.SendGameUserId = 0;
                //notice.SendUserName = "<color=#c0c000>系统</color>";
                notice.SendUserName = "";
                notice.SendTime = TimeHelper.ClientNowSeconds();

                //foreach (var player in PlayerList)
                //{
                //    player.Value.Send(notice);
                //}
                foreach (var Server in mMatchConfigs)
                {
                    Dictionary<int, List<int>> keyValuePairs = Help_JsonSerializeHelper.DeSerialize<Dictionary<int, List<int>>>(Server.RunParameter);
                    int AreaId = 1;
                    foreach (var KeyValuePair in keyValuePairs)
                    {
                        AreaId = KeyValuePair.Key >> 16;
                        break;
                    }
                    if (gamePlayer.Player.GameAreaId == AreaId)
                        Game.Scene.GetComponent<NetInnerComponent>().Get(Server.ServerInnerIP).Send(notice);
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}