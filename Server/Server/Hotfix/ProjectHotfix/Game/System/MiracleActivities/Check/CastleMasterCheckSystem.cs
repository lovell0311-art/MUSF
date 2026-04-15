using CustomFrameWork;
using CustomFrameWork.Baseic;
using ETModel;
using ETModel.Robot;
using System.Collections.Generic;
using TencentCloud.Bri.V20190328.Models;

namespace ETHotfix
{
    [Timer(TimerType.CastleMasterCheck)]
    public class CastleMasterCheckTimer : ATimer<CastleMasterCheckComponent>
    {
        public override void Run(CastleMasterCheckComponent self)
        {
            if (self.Parent.OnlineStatus != EOnlineStatus.Online) return;   // 玩家正在进入游戏 或 正在下线

            var gamePlayer = self.Parent.GetCustomComponent<GamePlayer>();
            var mServerArea = Root.MainFactory.GetCustomComponent<ServerAreaManagerComponent>().GetGameArea(self.Parent.SourceGameAreaId);
            var Title = self.Parent.GetCustomComponent<PlayerTitle>();
            if (mServerArea == null) return;
            bool isChanged = false;
            List<DBPlayerTitle> TitleList = new List<DBPlayerTitle>(Title.ListString);
            
            foreach (var info in TitleList)
            {
                if(info.BingTime > 0 && info.EndTime <= Help_TimeHelper.GetNowSecond() && info.IsDisabled != 1)
                {
                    isChanged = true;
                    info.IsDisabled = 1;
                    Title.SetTitleDB(info);
                    if (info.TitleID == Title.UseTitle)
                    {
                        Title.UseTitle = 0;
                        gamePlayer.Data.Title = 0;
                        var mData = gamePlayer.UnitData;
                        if (self.Parent == null || gamePlayer == null)
                        {
                            return;
                        }
                        var mapComponent = Help_MapHelper.GetMapByMapId(mServerArea, mData.Index, self.Parent.GameUserId);
                        if (mapComponent == null)
                        {
                            return;
                        }

                        self.Parent.Send(new G2C_MovePos_notice()
                        {
                            UnitType = (int)E_Identity.Hero,
                            GameUserId = gamePlayer.InstanceId,
                            //MapId = mGamePlayer.UnitData.Index,
                            X = mData.X,
                            Y = mData.Y,
                            Angle = mData.Angle,
                            IsNeedMove = 0,
                            Title = gamePlayer.Data.Title,
                            WallTitle = gamePlayer.Data.WallTile,
                            ReincarnateCnt = gamePlayer.Data.ReincarnateCnt
                        });
                        var mFindTheWaySource = mapComponent.GetFindTheWay2D(mData.X, mData.Y);
                        var mMapCellTarget = mapComponent.GetFindTheWay2D(mData.X, mData.Y);
                        mapComponent.MoveSendNotice(mFindTheWaySource, mMapCellTarget, gamePlayer);

                        var equipComponent = self.Parent.GetCustomComponent<EquipmentComponent>();
                        if (equipComponent != null)
                        {
                            equipComponent.ApplyEquipProp();
                        }
                    }
                    Title.DelTitle(info.TitleID);
                    if (info.TitleID == 60001 || info.TitleID == 60002)
                    {
                        if (gamePlayer.UnitData.Index == 17)
                        {
                            MapComponent map = Help_MapHelper.GetMapByMapId(mServerArea, gamePlayer.UnitData.Index, gamePlayer.InstanceId);
                            if (map == null)
                            {
                                Log.Error("GamePlayer 生命周期错乱");
                                // 将这个组件移除掉，防止继续报错
                                self.Parent.RemoveCustomComponent<AncientBattlefieldCheckComponent>();
                                return;
                            }
                            var mMapCellSource = map.GetFindTheWay2D(gamePlayer.UnitData.X, gamePlayer.UnitData.Y);
                            var mMapCellTarget = mServerArea.GetCustomComponent<MapManageComponent>().GetMapByMapIndex(ConstMapId.YongZheDaLu).GetFindTheWay2D(187, 200);

                            //map.QuitMap(mMapCellSource, gamePlayer);
                            // 公告移动信息
                            map.MoveSendNotice(mMapCellSource, mMapCellTarget, gamePlayer);

                            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
                            var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, (int)self.Parent.GameAreaId);
                            var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get(self.Parent.GameAreaId);
                            mWriteDataComponent.Save(gamePlayer.UnitData, dBProxy2).Coroutine();
                        }
                    }
                }
            }
            if(isChanged)
            {
                G2C_ServerSendTitleMessage g2C_ServerSendTitleMessage = new G2C_ServerSendTitleMessage();
                g2C_ServerSendTitleMessage.UseTitle = Title.UseTitle;
                foreach (var item in Title.ListString)
                {
                    Title_Status title_Status = new Title_Status();
                    title_Status.TitleID = item.TitleID;
                    title_Status.BingTime = item.BingTime;
                    title_Status.EndTime = item.EndTime;
                    g2C_ServerSendTitleMessage.TitleList.Add(title_Status);
                }
                self.Parent.Send(g2C_ServerSendTitleMessage);
            }
        }
    }

    [EventMethod(typeof(CastleMasterCheckComponent), EventSystemType.INIT)]
    public class CastleMasterCheckComponentEventOnInit : ITEventMethodOnInit<CastleMasterCheckComponent>
    {
        public void OnInit(CastleMasterCheckComponent b_Component)
        {
            // 10秒循环检查，如果不需要那么准确。间隔可以改长
            b_Component.TimerId = ETModel.ET.TimerComponent.Instance.NewRepeatedTimer(1000, TimerType.CastleMasterCheck, b_Component);
        }
    }

    [EventMethod(typeof(CastleMasterCheckComponent), EventSystemType.DISPOSE)]
    public class CastleMasterCheckComponentEventOnDispose : ITEventMethodOnDispose<CastleMasterCheckComponent>
    {
        public override void OnDispose(CastleMasterCheckComponent b_Component)
        {
            ETModel.ET.TimerComponent.Instance.Remove(ref b_Component.TimerId);
        }
    }
}
