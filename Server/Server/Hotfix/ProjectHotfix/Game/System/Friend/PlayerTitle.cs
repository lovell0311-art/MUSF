using System.Collections.Generic;
using System.Threading.Tasks;
using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;

namespace ETHotfix
{
    /// <summary>
    /// 好友组件
    /// </summary>
    public static partial class PlayerTitleComponentSystem
    {
        /// <summary>
        /// 初始称号
        /// </summary>
        /// <param name="b_Component"></param>
        /// <returns></returns>
        public async static Task<bool> Init(this PlayerTitle b_Component, int mAreaId)
        {
            if (b_Component == null) return false;
            if (mAreaId < 0) return false;
            b_Component.UseTitle = 0;
            b_Component.ListString = new List<DBPlayerTitle>();

            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            DataCacheManageComponent mDataCacheComponent = b_Component.Parent.AddCustomComponent<DataCacheManageComponent>();
            long PlayerGameID = b_Component.Parent.GameUserId;
            var TitledataCache = mDataCacheComponent.Get<DBPlayerTitle>();
            var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, (int)b_Component.Parent.GameAreaId);
            if (TitledataCache == null)
            {
                TitledataCache = await mDataCacheComponent.Add<DBPlayerTitle>(dBProxy2, p => p.UserId == PlayerGameID && p.IsDisabled == 0);
                var UserTitle = await dBProxy2.Query<DBPlayerTitle>(p => p.UserId == b_Component.Parent.UserId && p.IsDisabled == 0);
                foreach (var title in UserTitle)
                {
                    DBPlayerTitle titleInfo = title as DBPlayerTitle;
                    TitledataCache._DataCacheDic[0].Add(titleInfo.Id, titleInfo);
                }
                // TitledataCache = await mDataCacheComponent.Add<DBPlayerTitle>(dBProxy2, p => p.UserId == b_Component.Parent.UserId,1);
            }
            var TitleList = TitledataCache.DataQuery(p => p.UserId == PlayerGameID);
            if (TitleList != null && TitleList.Count > 0)
            {
                foreach (var item in TitleList)
                {
                    if (item.IsDisabled == 1) continue;
                    if (item.Type == 0 && item.UseID == PlayerGameID)
                    {
                        b_Component.UseTitle = item.TitleID;
                    }
                    var list = b_Component.ListString.Find(P => P.TitleID == item.TitleID);
                    if (list == default(DBPlayerTitle))
                        b_Component.ListString.Add(item);
                    else
                    {
                        item.IsDisabled = 1;
                        TitledataCache.DataRemove(item.Id);
                        b_Component.SetTitleDB(item);
                    }
                }
            }

            var TitleList2 = TitledataCache.DataQuery(p => p.UserId == b_Component.Parent.UserId);
            if (TitleList2 != null && TitleList2.Count > 0)
            {
                foreach (var item in TitleList2)
                {
                    if (item.IsDisabled == 1) continue;
                    if (item.Type == 1 && item.UseID == PlayerGameID)
                    {
                        b_Component.UseTitle = item.TitleID;
                    }
                    var list = b_Component.ListString.Find(P => P.TitleID == item.TitleID);
                    if (list == default(DBPlayerTitle))
                        b_Component.ListString.Add(item);
                    else
                    {
                        item.IsDisabled = 1;
                        TitledataCache.DataRemove(item.Id);
                        b_Component.SetTitleDB(item);
                    }
                }
            }

            b_Component.Parent.GetCustomComponent<GamePlayer>().Data.Title = b_Component.UseTitle;
            bool IsSend = false;
            G2C_ServerSendTitleMessage g2C_ServerSendTitleMessage = new G2C_ServerSendTitleMessage();
            g2C_ServerSendTitleMessage.UseTitle = b_Component.UseTitle;
            foreach (var item in b_Component.ListString)
            {
                Title_Status title_Status = new Title_Status();
                title_Status.TitleID = item.TitleID;
                title_Status.BingTime = item.BingTime;
                title_Status.EndTime = item.EndTime;
                g2C_ServerSendTitleMessage.TitleList.Add(title_Status);
                if (item.TitleID == 60001 && item.IsDisabled != 1)
                    IsSend = true;
                //if (item.TitleID == 60012)
                //    b_Component.Parent.GetCustomComponent<GamePlayer>().Data.SpecialTitle = true;
            }
            if (IsSend)
            {
                G2C_SendPointOutMessage g2C_SendPointOutMessage = new G2C_SendPointOutMessage();
                g2C_SendPointOutMessage.Pointout = 2700;
                g2C_SendPointOutMessage.PlayerName = b_Component.Parent.GetCustomComponent<GamePlayer>().Data.NickName;
                g2C_SendPointOutMessage.WarName = "";
                g2C_SendPointOutMessage.Time = 0;
                g2C_SendPointOutMessage.TitleName = 0;

                var mMatchConfigs = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfos(AppType.Game);
                foreach (var Server in mMatchConfigs)
                {
                    Dictionary<int, List<int>> keyValuePairs = Help_JsonSerializeHelper.DeSerialize<Dictionary<int, List<int>>>(Server.RunParameter);
                    int AreaId = 1;
                    foreach (var KeyValuePair in keyValuePairs)
                    {
                        AreaId = KeyValuePair.Key >> 16;
                        break;
                    }
                    if (mAreaId == AreaId)
                        Game.Scene.GetComponent<NetInnerComponent>().Get(Server.ServerInnerIP).Send(g2C_SendPointOutMessage);
                }
            }
            if (b_Component.UseTitle != 0)
            {
                var equipComponent = b_Component.Parent.GetCustomComponent<EquipmentComponent>();
                if (equipComponent != null)
                {
                    equipComponent.ApplyEquipProp();
                }
            }
            // 定时检查期限称号是否过期
            if (b_Component.ListString.Count >= 1)
            {
                b_Component.Parent.AddCustomComponent<CastleMasterCheckComponent>();
                ETModel.EventType.EquipmentRelatedSettings.Instance.player = b_Component.Parent;
                ETModel.EventType.EquipmentRelatedSettings.Instance.item = null;
                ETModel.EventType.EquipmentRelatedSettings.Instance.TitleCount = b_Component.ListString.Count;
                ETModel.EventType.EquipmentRelatedSettings.Instance.ItemCount = 0;
                Root.EventSystem.OnRun("EquipmentRelatedSettings", ETModel.EventType.EquipmentRelatedSettings.Instance);
            }

            b_Component.Parent.Send(g2C_ServerSendTitleMessage);
            //b_Component.Parent.GetCustomComponent<PlayerShopMallComponent>().UpdateTitle(0).Coroutine();
            return true;
        }
        public static bool SetTitle(this PlayerTitle b_Component, int TitleID)
        {
            if (b_Component.UseTitle == TitleID) return true;

            int OldTitleID = b_Component.UseTitle;
            long Crrt = Help_TimeHelper.GetNowSecond();

            foreach (var Info in b_Component.ListString)
            {
                if (OldTitleID == Info.TitleID)
                {
                    Info.UseID = 0;
                    b_Component.SetTitleDB(Info);
                    b_Component.UseTitle = 0;
                    b_Component.Parent.GetCustomComponent<GamePlayer>().Data.Title = 0;
                }
                if (Info.TitleID == TitleID)
                {
                    if (Info.BingTime != 0 && Info.BingTime <= Crrt)
                    {
                        if (Info.EndTime != 0 && Info.EndTime > Crrt)
                        {
                            b_Component.UseTitle = TitleID;
                            Info.UseID = b_Component.Parent.GameUserId;
                            b_Component.SetTitleDB(Info);
                            b_Component.Parent.GetCustomComponent<GamePlayer>().Data.Title = TitleID;
                        }
                    }
                    else if (Info.BingTime == 0 && Info.EndTime == 0)
                    {
                        b_Component.UseTitle = TitleID;
                        Info.UseID = b_Component.Parent.GameUserId;
                        b_Component.SetTitleDB(Info);
                        b_Component.Parent.GetCustomComponent<GamePlayer>().Data.Title = TitleID;
                    }
                }
            }
            return true;
        }
        public static bool DelTitle(this PlayerTitle b_Component, int TitleID)
        {
            if (TitleID == 0) return false;

            foreach (var Info in b_Component.ListString)
            {
                if (Info.TitleID == TitleID)
                {
                    b_Component.ListString.Remove(Info);
                    Info.IsDisabled = 1;
                    b_Component.SetTitleDB(Info);
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Type =0角色级称号;Type =1账号级称号
        /// </summary>
        /// <param name="b_Component"></param>
        /// <param name="TitleID"></param>
        /// <param name="Type"></param>
        /// <returns></returns>
        public static async Task LoadNewTitle(this PlayerTitle b_Component, int TitleID, int Type)
        {
            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            DataCacheManageComponent mDataCacheComponent = b_Component.Parent.AddCustomComponent<DataCacheManageComponent>();
            long PlayerGameID = b_Component.Parent.GameUserId;
            var TitledataCache = mDataCacheComponent.Get<DBPlayerTitle>();
            var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, (int)b_Component.Parent.GameAreaId);
            List<ComponentWithId> TitleInfo = null;
            if (Type == 0)
            {
                TitleInfo = await dBProxy2.Query<DBPlayerTitle>(p => p.UserId == b_Component.Parent.GameUserId && p.TitleID == TitleID);
            }
            else if (Type == 1)
            {
                TitleInfo = await dBProxy2.Query<DBPlayerTitle>(p => p.UserId == b_Component.Parent.UserId && p.TitleID == TitleID);
            }
            foreach (var Title in TitleInfo)
            {
                DBPlayerTitle Info = Title as DBPlayerTitle;
                var TitleList = TitledataCache.DataQuery(p => p.TitleID == TitleID);
                if (TitleList != null && TitleList.Count > 0)
                {
                    for (int i = 0; i < b_Component.ListString.Count; ++i)
                    {
                        if (b_Component.ListString[i].TitleID == TitleList[0].TitleID )
                        {
                            TitleList[0] = Info;
                            b_Component.ListString[i] = Info;
                            return;
                        }
                    }
                    TitleList[0] = Info;
                    b_Component.ListString.Add(Info);
                }
                else
                {
                    if (Type == 0)
                        TitledataCache.DataAdd(Info);
                    else if (Type == 1)
                        TitledataCache._DataCacheDic[0].Add(Info.Id, Info);

                    b_Component.ListString.Add(Info);
                }
            }
            // 定时检查期限称号是否过期
            if (b_Component.ListString.Count >= 1)
            {
                ETModel.EventType.EquipmentRelatedSettings.Instance.player = b_Component.Parent;
                ETModel.EventType.EquipmentRelatedSettings.Instance.item = null;
                ETModel.EventType.EquipmentRelatedSettings.Instance.TitleCount = b_Component.ListString.Count;
                ETModel.EventType.EquipmentRelatedSettings.Instance.ItemCount = 0;
                Root.EventSystem.OnRun("EquipmentRelatedSettings", ETModel.EventType.EquipmentRelatedSettings.Instance);
                b_Component.Parent.AddCustomComponent<CastleMasterCheckComponent>();
            }
        }
        /// <summary>
        ///name:称号 Type:0角色，1账号 Time:持续时间，永久为0(秒)
        /// </summary>
        /// <param name="b_Component"></param>
        /// <param name="Name"></param>
        /// <param name="Time"></param>
        /// <returns></returns>
        public static bool AddTitle(this PlayerTitle b_Component, int TitleID, int Type = 0, long Time = 0)
        {
            DBPlayerTitle Title = new DBPlayerTitle();
            Title.Id = IdGeneraterNew.Instance.GenerateUnitId(b_Component.Parent.GameAreaId);
            Title.TitleID = TitleID;
            Title.Type = Type;
            Title.BingTime = 0;
            Title.EndTime = 0;
            Title.IsDisabled = 0;
            if (Type == 0)
            {
                Title.UserId = b_Component.Parent.GameUserId;
            }
            else if (Type == 1)
            {
                Title.UserId = b_Component.Parent.UserId;
            }

            if (Time != 0)
            {
                Title.BingTime = Help_TimeHelper.GetNowSecond();
                Title.EndTime = Time;
            }
            bool isUpData = true;
            for (int i = 0; i < b_Component.ListString.Count; i++)
            {
                if (b_Component.ListString[i].TitleID == TitleID)
                {
                    Title.Id = b_Component.ListString[i].Id;
                    b_Component.ListString[i] = Title;
                    isUpData = false;

                }
            }
            if (isUpData)
            {
                b_Component.ListString.Add(Title);
                ETModel.EventType.EquipmentRelatedSettings.Instance.player = b_Component.Parent;
                ETModel.EventType.EquipmentRelatedSettings.Instance.item = null;
                ETModel.EventType.EquipmentRelatedSettings.Instance.TitleCount = b_Component.ListString.Count;
                ETModel.EventType.EquipmentRelatedSettings.Instance.ItemCount = 0;
                Root.EventSystem.OnRun("EquipmentRelatedSettings", ETModel.EventType.EquipmentRelatedSettings.Instance);
            }

            b_Component.SetTitleDB(Title);
            return true;
        }
        public static void SendTitle(this PlayerTitle b_Component)
        {
            G2C_ServerSendTitleMessage g2C_ServerSendTitleMessage = new G2C_ServerSendTitleMessage();
            g2C_ServerSendTitleMessage.UseTitle = b_Component.UseTitle;
            foreach (var item in b_Component.ListString)
            {
                Title_Status title_Status = new Title_Status();
                title_Status.TitleID = item.TitleID;
                title_Status.BingTime = item.BingTime;
                title_Status.EndTime = item.EndTime;
                g2C_ServerSendTitleMessage.TitleList.Add(title_Status);
            }
            b_Component.Parent.Send(g2C_ServerSendTitleMessage);
            b_Component.Parent.AddCustomComponent<CastleMasterCheckComponent>();
        }
        public static bool CheckTitle(this PlayerTitle b_Component, int TitleId)
        {
            if (b_Component == null) return false;

            foreach (var info in b_Component.ListString)
            {
                if (info.TitleID == TitleId)
                {
                    return false;
                }
            }
            return true;
        }
        public static bool SetTitleDB(this PlayerTitle b_Component, DBPlayerTitle dBPlayerTitle)
        {
            int mAreaId = b_Component.Parent.GameAreaId;
            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            DataCacheManageComponent mDataCacheComponent = b_Component.Parent.AddCustomComponent<DataCacheManageComponent>();
            var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get(mAreaId);
            var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, mAreaId);
            var TitledataCache = mDataCacheComponent.Get<DBPlayerTitle>();
            var TitleList = TitledataCache.DataQuery(p => p.UserId == dBPlayerTitle.UserId && p.TitleID == dBPlayerTitle.TitleID);
            if (TitleList.Count > 0)
            {
                dBPlayerTitle.Id = TitleList[0].Id;
                TitleList[0] = dBPlayerTitle;
                mWriteDataComponent.Save(TitleList[0], dBProxy2).Coroutine();
            }
            else
            {
                DBPlayerTitle dBPlayerTitle1 = new DBPlayerTitle();
                dBPlayerTitle1.TitleID = dBPlayerTitle.TitleID;
                dBPlayerTitle1.Id = dBPlayerTitle.Id;
                dBPlayerTitle1.UserId = dBPlayerTitle.UserId;
                dBPlayerTitle1.BingTime = dBPlayerTitle.BingTime;
                dBPlayerTitle1.EndTime = dBPlayerTitle.EndTime;
                dBPlayerTitle1.Type = dBPlayerTitle.Type;
                dBPlayerTitle1.IsDisabled = dBPlayerTitle.IsDisabled;
                TitledataCache.DataAdd(dBPlayerTitle1);
                dBProxy2.Save(dBPlayerTitle1).Coroutine();
            }
            return true;
        }
    }
}
