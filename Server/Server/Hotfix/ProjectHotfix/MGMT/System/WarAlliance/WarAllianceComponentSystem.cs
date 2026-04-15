using System.Collections.Generic;
using System.Threading.Tasks;
using CustomFrameWork;
using CustomFrameWork.Component;
using CustomFrameWork.Baseic;
using ETModel;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PF;
using System;

namespace ETHotfix
{
    [EventMethod(typeof(WarAllianceComponent), EventSystemType.INIT)]
    public class WarAllianceComponentEventOnInit : ITEventMethodOnInit<WarAllianceComponent>
    {
        /// <summary>
        /// 服务器开启时读取战盟数据
        /// </summary>
        /// <param name="b_Component"></param>
        public void OnInit(WarAllianceComponent b_Component)
        {
            OnInitAsync(b_Component).Coroutine();
        }

        public async Task OnInitAsync(WarAllianceComponent b_Component)
        {
            try
            {
                if (await b_Component.InitServerDBID())
                    b_Component.OnInit().Coroutine();
            }
            catch (Exception e)
            {
                Log.Fatal("战盟初始化失败", e);
            }
        }
    }
    public static class WarAllianceComponentSystem
    {
        public static async Task<bool> InitServerDBID(this WarAllianceComponent self)
        {
            TimerComponent mTimerComponent = Root.MainFactory.GetCustomComponent<TimerComponent>();
            await mTimerComponent.WaitAsync(5000);

            var startConfig = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfo(OptionComponent.Options.AppId);
            if (startConfig != null)
            {
                List<int> ServerDB = Help_JsonSerializeHelper.DeSerialize<List<int>>(startConfig.RunParameter);
                if (ServerDB.Count > 0)
                {
                    foreach (int DBID in ServerDB)
                    {
                        int Id = DBID >> 16;
                        if (self.DBID.Contains(Id))
                            continue;

                        self.DBID.Add(Id);
                    }
                    return true;
                }
            }
            return false;
        }
        public static async Task OnInit(this WarAllianceComponent self)
        {
            TimerComponent mTimerComponent = Root.MainFactory.GetCustomComponent<TimerComponent>();
            await mTimerComponent.WaitAsync(10000);

            foreach (var ID in self.DBID)
            {

                var mDBProxy = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>().GetZoneDB(DBType.Core, ID);
                if (mDBProxy == null) return;

                var WarAllianceList = await mDBProxy.Query<DBWarAllianceData>(p => p.IsDisabled != 1);
                if (WarAllianceList == null) return;

                if (WarAllianceList.Count >= 1)
                {
                    foreach (var WarAllianceInfo in WarAllianceList)
                    {
                        DBWarAllianceData dBWarAllianceData = WarAllianceInfo as DBWarAllianceData;//new DBWarAllianceData();

                        if (self.AddWarAlliance(dBWarAllianceData, ID))
                        {
                            var MemberInfoList = await mDBProxy.Query<DBMemberInfo>(p => p.DBWarAllianceID == dBWarAllianceData.DBWarAllianceID);
                            if (MemberInfoList != null || MemberInfoList.Count >= 1)
                            {
                                foreach (var MemberInfo in MemberInfoList)
                                {
                                    DBMemberInfo dBMemberInfo = MemberInfo as DBMemberInfo;
                                    if (dBMemberInfo.IsDisabled == 1 || dBMemberInfo.IsDisabled == 0)
                                        self.AddWarAllianceMember(dBMemberInfo);
                                }
                            }
                        }
                    }
                }
            }
            Log.Info($"ServerMGMT  WarAlliance Loading......OK");

        }

        /// <summary>
        /// 添加新的战盟
        /// </summary>
        /// <param name="b_CustomComponent"></param>
        /// <param name="dBWarAllianceData"></param>
        /// <returns></returns>
        public static bool AddWarAlliance(this WarAllianceComponent b_CustomComponent, DBWarAllianceData dBWarAllianceData, int mAreaId)
        {
            if (b_CustomComponent == null) return false;
            if (dBWarAllianceData == null) return false;

            if (b_CustomComponent.WarAllianceList == null)
            {
                b_CustomComponent.WarAllianceList = new Dictionary<long, WarAllianceInfo>();
            }

            WarAllianceInfo warAllianceInfo;
            if (!b_CustomComponent.WarAllianceList.TryGetValue(dBWarAllianceData.DBWarAllianceID, out warAllianceInfo))
            {
                warAllianceInfo = new WarAllianceInfo();

                warAllianceInfo.WarAllianceID = dBWarAllianceData.DBWarAllianceID;
                warAllianceInfo.WarAllianceName = dBWarAllianceData.DBWarAllianceName;
                warAllianceInfo.WarAllianceLevel = dBWarAllianceData.DBWarAllianceLevel;
                warAllianceInfo.WarAllianceBadge = dBWarAllianceData.DBWarAllianceBadge;
                warAllianceInfo.WarAllianNotice = dBWarAllianceData.DBWarAllianNotice;
                warAllianceInfo.mAreaId = mAreaId;
                warAllianceInfo.MemberList = new Dictionary<long, MemberInfo>();
                warAllianceInfo.MemberList2 = new Dictionary<long, MemberInfo>();

                b_CustomComponent.WarAllianceList.Add(warAllianceInfo.WarAllianceID, warAllianceInfo);
                return true;
            }
            Log.Error($"战盟信息加载出错 ID:{dBWarAllianceData.Id} Name:{dBWarAllianceData} 已存在");
            return false;
        }

        /// <summary>
        /// 添加战盟成员
        /// </summary>
        /// <param name="b_CustomComponent"></param>
        /// <param name="dBMemberInfo"></param>
        /// <returns></returns>
        public static bool AddWarAllianceMember(this WarAllianceComponent b_CustomComponent, DBMemberInfo dBMemberInfo, int ServerId = 0)
        {
            if (dBMemberInfo == null) return false;
            if (b_CustomComponent == null) return false;

            if (b_CustomComponent.WarAllianceList == null) return false;

            WarAllianceInfo warAllianceInfo = new WarAllianceInfo();
            MemberInfo memberInfo;
            if (b_CustomComponent.WarAllianceList.TryGetValue(dBMemberInfo.DBWarAllianceID, out warAllianceInfo))
            {
                if (warAllianceInfo != null)
                {
                    if (dBMemberInfo.IsDisabled == 1)//成员列表
                    {
                        if (warAllianceInfo.MemberList == null)
                        {
                            warAllianceInfo.MemberList = new Dictionary<long, MemberInfo>();
                        }
                        if (!warAllianceInfo.MemberList.TryGetValue(dBMemberInfo.MemberID, out memberInfo))
                        {
                            memberInfo = new MemberInfo();
                            memberInfo.MemberID = dBMemberInfo.MemberID;
                            memberInfo.MemberName = dBMemberInfo.MemberName;
                            memberInfo.MemberLevel = dBMemberInfo.MemberLevel;
                            memberInfo.MemberClassType = dBMemberInfo.MemberClassType;
                            memberInfo.MemberPost = dBMemberInfo.MemberPost;
                            memberInfo.AllianceScore = dBMemberInfo.AllianceScore;
                            memberInfo.MeberServerID = ServerId;
                            if (memberInfo.MemberPost == (int)PostType.AllianceLeader)
                                warAllianceInfo.AllianceLeaderName = memberInfo.MemberName;

                            memberInfo.MeberState = 1;//默认离线
                            if (ServerId != 0)
                            {
                                memberInfo.MeberState = 0;
                            }
                            warAllianceInfo.MemberList.Add(memberInfo.MemberID, memberInfo);
                            return true;
                        }
                        Log.Error($"战盟添加成员出错 成员ID:{dBMemberInfo.MemberID} 成员Name;{dBMemberInfo.MemberName}");
                        return false;
                    }
                    else //申请列表
                    {
                        if (warAllianceInfo.MemberList2 == null)
                        {
                            warAllianceInfo.MemberList2 = new Dictionary<long, MemberInfo>();
                        }
                        if (!warAllianceInfo.MemberList2.TryGetValue(dBMemberInfo.MemberID, out memberInfo))
                        {
                            memberInfo = new MemberInfo();
                            memberInfo.MemberID = dBMemberInfo.MemberID;
                            memberInfo.MemberName = dBMemberInfo.MemberName;
                            memberInfo.MemberLevel = dBMemberInfo.MemberLevel;
                            memberInfo.MemberClassType = dBMemberInfo.MemberClassType;
                            memberInfo.MemberPost = dBMemberInfo.MemberPost;
                            memberInfo.AllianceScore = dBMemberInfo.AllianceScore;
                            memberInfo.MeberState = 1;//默认离线
                            if (ServerId != 0)
                            {
                                memberInfo.MeberState = 0;
                            }
                            memberInfo.MeberServerID = ServerId;
                            warAllianceInfo.MemberList2.Add(memberInfo.MemberID, memberInfo);
                            return true;
                        }
                        Log.Error($"战盟添加申请列表出错 成员ID:{dBMemberInfo.MemberID} 成员Name;{dBMemberInfo.MemberName}");
                        return false;
                    }
                }
                Log.Error($"战盟添加成员出错 战盟信息错误 战盟ID:{dBMemberInfo.DBWarAllianceID}");
                return false;
            }
            Log.Error($"战盟添加成员出错 成员ID:{dBMemberInfo.MemberID} 成员Name;{dBMemberInfo.MemberName} 已存在 ");
            return false;
        }
        /// <summary>
        /// 战盟数据写库
        /// </summary>
        /// <param name="b_CustomComponent"></param>
        /// <param name="dBWarAllianceData"></param>
        /// <returns></returns>
        public static async Task<bool> SetWarAllianceDB(this WarAllianceComponent b_CustomComponent, DBWarAllianceData dBWarAllianceData, long AreaId)
        {
            if (dBWarAllianceData == null) return false;
            int mAreaId = (int)AreaId;//UnitIdStruct.GetUnitZone(AreaId);
            var mDBProxy = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>().GetZoneDB(DBType.Core, mAreaId);
            if (mDBProxy == null) return false;

            dBWarAllianceData.Id = IdGeneraterNew.Instance.GenerateUnitId(mAreaId);

            var WarAllianceList = await mDBProxy.Query<DBWarAllianceData>(p => p.DBWarAllianceID == dBWarAllianceData.DBWarAllianceID);
            if (WarAllianceList.Count == 0 || WarAllianceList == null)
            {
                await mDBProxy.Save(dBWarAllianceData);
                return true;
            }
            else
            {
                var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get(mAreaId);
                dBWarAllianceData.Id = WarAllianceList[0].Id;
                await mDBProxy.Save(dBWarAllianceData);
                //mWriteDataComponent.Save(dBWarAllianceData, mDBProxy);
                return true;
            }
        }
        public static async Task UpMemberInfo(this WarAllianceComponent b_CustomComponent, MemberInfo MemberInfoData, int AreaId)
        {
            if (MemberInfoData == null) return;

            var mDBProxy = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>().GetZoneDB(DBType.Core, AreaId);
            if (mDBProxy == null) return;

            var MemberList = await mDBProxy.Query<DBMemberInfo>(p => p.MemberID == MemberInfoData.MemberID && p.IsDisabled == 1);
            if (MemberList != null && MemberList.Count >= 1)
            {
                (MemberList[0] as DBMemberInfo).MemberName = MemberInfoData.MemberName;
                (MemberList[0] as DBMemberInfo).MemberLevel = MemberInfoData.MemberLevel;
                (MemberList[0] as DBMemberInfo).AllianceScore = MemberInfoData.AllianceScore;
                //Log.Warning($"更新战盟积分后存库{MemberInfoData.MemberID}:{MemberInfoData.AllianceScore}");
                await mDBProxy.Save(MemberList[0] as DBMemberInfo);
            }
        }
        /// <summary>
        /// 战盟成员信息写库
        /// </summary>
        /// <param name="b_CustomComponent"></param>
        /// <param name="dBWarAllianceData"></param>
        /// <returns></returns>
        public static async Task<bool> SetWarAllianceMemberDB(this WarAllianceComponent b_CustomComponent, DBMemberInfo dBMemberInfoData, int AreaId)
        {
            if (dBMemberInfoData == null) return false;

            var mDBProxy = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>().GetZoneDB(DBType.Core, AreaId);
            //var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get(mAreaId);
            if (mDBProxy == null/* || mWriteDataComponent == null*/) return false;

            dBMemberInfoData.Id = IdGeneraterNew.Instance.GenerateUnitId(AreaId);

            switch (dBMemberInfoData.IsDisabled)
            {

                case 0:
                    var ApplyList = await mDBProxy.Query<DBMemberInfo>(p => p.MemberID == dBMemberInfoData.MemberID && p.IsDisabled != 1 && p.IsDisabled != 2);
                    if (ApplyList.Count < 5 || ApplyList == null)
                    {
                        await mDBProxy.Save(dBMemberInfoData);
                        return true;
                    }
                    else
                    {
                        foreach (DBMemberInfo Apply in ApplyList)
                        {
                            if (Apply.IsDisabled == 3)
                            {
                                dBMemberInfoData.Id = Apply.Id;
                                await mDBProxy.Save(dBMemberInfoData);
                                //mWriteDataComponent.Save(dBMemberInfoData, mDBProxy);
                                return true;
                            }
                        }
                    }
                    break;
                case 1:
                    var MemberList = await mDBProxy.Query<DBMemberInfo>(p => p.MemberID == dBMemberInfoData.MemberID && p.IsDisabled != 0 && p.IsDisabled != 3);
                    if (MemberList.Count == 0 || MemberList == null)
                    {
                        await mDBProxy.Save(dBMemberInfoData);
                    }
                    else
                    {
                        dBMemberInfoData.Id = MemberList[0].Id;
                        await mDBProxy.Save(dBMemberInfoData);
                        //mWriteDataComponent.Save(dBMemberInfoData, mDBProxy);
                    }

                    var ApplyList2 = await mDBProxy.Query<DBMemberInfo>(p => p.MemberID == dBMemberInfoData.MemberID && p.IsDisabled != 1);
                    foreach (DBMemberInfo m in ApplyList2)
                    {
                        m.IsDisabled = 3;
                        await mDBProxy.Save(m);
                        //mWriteDataComponent.Save(m, mDBProxy);
                    }
                    return true;

                case 2:
                    var DeleteList = await mDBProxy.Query<DBMemberInfo>(p => p.MemberID == dBMemberInfoData.MemberID && p.IsDisabled == 1);
                    if (DeleteList.Count == 0 || DeleteList == null)
                    {
                        await mDBProxy.Save(dBMemberInfoData);
                        return true;
                    }
                    else
                    {
                        dBMemberInfoData.Id = DeleteList[0].Id;
                        await mDBProxy.Save(dBMemberInfoData);
                        //mWriteDataComponent.Save(dBMemberInfoData, mDBProxy);
                        return true;
                    }
                case 3:
                    var DeleteList2 = await mDBProxy.Query<DBMemberInfo>(p => p.MemberID == dBMemberInfoData.MemberID && p.DBWarAllianceID == dBMemberInfoData.DBWarAllianceID && p.IsDisabled == 0);
                    if (DeleteList2.Count == 0 || DeleteList2 == null)
                    {
                        await mDBProxy.Save(dBMemberInfoData);
                        return true;
                    }
                    else
                    {
                        dBMemberInfoData.Id = DeleteList2[0].Id;
                        await mDBProxy.Save(dBMemberInfoData);
                        //mWriteDataComponent.Save(dBMemberInfoData, mDBProxy);
                        return true;
                    }
                default:
                    break;
            }
            return false;
        }
        /// <summary>
        /// 获取战盟信息
        /// </summary>
        /// <param name="b_CustomComponent"></param>
        /// <param name="WarAllianceID"></param>
        /// <returns></returns>
        public static Dictionary<long, MemberInfo> GetWarAllianceMember(this WarAllianceComponent b_CustomComponent, long WarAllianceID, int Type = 0)
        {
            WarAllianceInfo warAllianceInfo = null;

            if (b_CustomComponent.WarAllianceList.TryGetValue(WarAllianceID, out warAllianceInfo))
            {
                Dictionary<long, MemberInfo> keyValuePairs = new Dictionary<long, MemberInfo>();

                if (Type == 0)
                {
                    keyValuePairs = warAllianceInfo.MemberList;
                    return keyValuePairs;
                }
                keyValuePairs = warAllianceInfo.MemberList2;
                return keyValuePairs;
            }
            return null;
        }
        /// <summary>
        /// 获取战盟信息
        /// </summary>
        /// <param name="b_CustomComponent"></param>
        /// <param name="WarAllianceID"></param>
        /// <returns></returns>
        public static WarAllianceInfo GetWarAlliance(this WarAllianceComponent b_CustomComponent, long WarAllianceID)
        {
            WarAllianceInfo warAllianceInfo = null;
            if (b_CustomComponent.WarAllianceList.TryGetValue(WarAllianceID, out warAllianceInfo))
            {
                return warAllianceInfo;
            }
            return warAllianceInfo;
        }
        /// <summary>
        /// 解散战盟踢出成员
        /// </summary>
        /// <param name="b_CustomComponent"></param>
        /// <param name="WarAllianceID"></param>
        /// <returns></returns>
        public static async Task<bool> DeleteMemberList(this WarAllianceComponent b_CustomComponent, long WarAllianceID, int Areald)
        {
            WarAllianceInfo warAllianceInfo = null;

            if (b_CustomComponent.WarAllianceList.TryGetValue(WarAllianceID, out warAllianceInfo))
            {
                foreach (var Info in warAllianceInfo.MemberList)//成员列表
                {
                    DBMemberInfo dBMemberInfoData = new DBMemberInfo();
                    dBMemberInfoData.DBWarAllianceID = WarAllianceID;
                    dBMemberInfoData.MemberID = Info.Value.MemberID;
                    dBMemberInfoData.MemberName = Info.Value.MemberName;
                    dBMemberInfoData.MemberLevel = Info.Value.MemberLevel;
                    dBMemberInfoData.MemberPost = Info.Value.MemberPost;
                    dBMemberInfoData.IsDisabled = 2;

                    if (Info.Value.MemberPost == (int)PostType.AllianceLeader)
                    {
                        dBMemberInfoData.DeleteTime = Help_TimeHelper.GetNowSecond();
                    }

                    if (await b_CustomComponent.SetWarAllianceMemberDB(dBMemberInfoData, Areald) == false)
                    {
                        Log.Error($"退出写库失败 WarAllianceID{WarAllianceID} MemberID{Info.Value.MemberID}");
                    }
                    warAllianceInfo.MemberList.Remove(Info.Key);
                }

                foreach (var Info in warAllianceInfo.MemberList2)//申请列表
                {
                    DBMemberInfo dBMemberInfoData = new DBMemberInfo();
                    dBMemberInfoData.DBWarAllianceID = WarAllianceID;
                    dBMemberInfoData.MemberID = Info.Value.MemberID;
                    dBMemberInfoData.MemberName = Info.Value.MemberName;
                    dBMemberInfoData.MemberLevel = Info.Value.MemberLevel;
                    dBMemberInfoData.MemberPost = Info.Value.MemberPost;
                    dBMemberInfoData.IsDisabled = 3;
                    if (await b_CustomComponent.SetWarAllianceMemberDB(dBMemberInfoData, Areald) == false)
                    {
                        Log.Error($"退出写库失败 WarAllianceID{WarAllianceID} MemberID{Info.Value.MemberID}");
                    }
                    warAllianceInfo.MemberList2.Remove(Info.Key);
                }
                return true;
            }
            return false;
        }
        public static WarAllianceInfo GetNameWarAlliance(this WarAllianceComponent b_CustomComponent, string Name)
        {
            foreach (var Info in b_CustomComponent.WarAllianceList)
            {
                if (Info.Value.WarAllianceName == Name)
                    return Info.Value;
            }
            return null;
        }
        public static async Task<bool> DeleteWarAlliance(this WarAllianceComponent b_CustomComponent, long WarAllianceID, long Areald)
        {
            if (b_CustomComponent == null) return false;
            if (WarAllianceID == 0) return false;

            WarAllianceInfo warAllianceInfo = null;
            DBWarAllianceData dBWarAllianceData = new DBWarAllianceData();

            if (b_CustomComponent.WarAllianceList.TryGetValue(WarAllianceID, out warAllianceInfo))
            {
                dBWarAllianceData.DBWarAllianceID = warAllianceInfo.WarAllianceID;
                dBWarAllianceData.DBWarAllianceName = warAllianceInfo.WarAllianceName;
                dBWarAllianceData.DBWarAllianceLevel = warAllianceInfo.WarAllianceLevel;
                dBWarAllianceData.DBWarAllianceBadge = warAllianceInfo.WarAllianceBadge;
                dBWarAllianceData.DBWarAllianNotice = warAllianceInfo.WarAllianNotice;
                dBWarAllianceData.IsDisabled = 1;

                if (await b_CustomComponent.SetWarAllianceDB(dBWarAllianceData, Areald) == false)
                {
                    Log.Error($"战盟解散失败 战盟ID{warAllianceInfo.WarAllianceID}");
                }
                b_CustomComponent.WarAllianceList.Remove(WarAllianceID);
                return true;
            }
            return false;
        }

        public static (long, long) CheckGameUser(this WarAllianceComponent b_CustomComponent, long GameUesr)
        {
            if (b_CustomComponent.keyValuePairs != null)
            {
                if (b_CustomComponent.keyValuePairs.TryGetValue(GameUesr, out var value))
                {
                    return value;
                }
            }
            return (0, 0);
        }

        public static void UpGameUser(this WarAllianceComponent b_CustomComponent, long GameUesr, (long, long) BEId)
        {
            b_CustomComponent.keyValuePairs[GameUesr] = BEId;
        }

        public static void DleGameUser(this WarAllianceComponent b_CustomComponent, long GameUesr)
        {
            if (b_CustomComponent.keyValuePairs.ContainsKey(GameUesr))
            {
                b_CustomComponent.keyValuePairs.Remove(GameUesr);
            }
        }
    }

}
