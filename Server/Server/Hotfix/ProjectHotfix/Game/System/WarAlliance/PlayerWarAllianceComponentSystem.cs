using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CustomFrameWork;
using CustomFrameWork.Baseic;
using CustomFrameWork.Component;
using ETModel;
using TencentCloud.Ame.V20190916.Models;


namespace ETHotfix
{
    public static partial class PlayerWarAllianceComponentSystem
    {
        /// <summary>
        /// 更新MGMT服务器上的数据
        /// </summary>
        /// <param name="b_Component"></param>
        public static void UpDateWarAlliancePlayerInfo(this PlayerWarAllianceComponent b_Component, int Gamestate = 0, int Type = 0, long ID = 0)
        {
            if (b_Component.WarAllianceID == 0 && Type == 0) return;

            G2M_UpdatePlayerWarAlliance g2M_UpdatePlayerWarAlliance = new G2M_UpdatePlayerWarAlliance();
            g2M_UpdatePlayerWarAlliance.WarAllianceID = b_Component.WarAllianceID;
            g2M_UpdatePlayerWarAlliance.GameUserID = b_Component.Parent.GameUserId;
            g2M_UpdatePlayerWarAlliance.GameLevel = b_Component.Parent.GetCustomComponent<GamePlayer>().Data.Level;
            g2M_UpdatePlayerWarAlliance.GameServerID = OptionComponent.Options.AppId;
            g2M_UpdatePlayerWarAlliance.MeberState = Gamestate;//在线状态 0在线 1离线
            g2M_UpdatePlayerWarAlliance.Name = b_Component.Parent.GetCustomComponent<GamePlayer>().Data.NickName;
            if (Type == 1)
            {
                g2M_UpdatePlayerWarAlliance.Type = 1;//0以加入战盟修改战盟内数据  1是还未加入战盟修改申请
                g2M_UpdatePlayerWarAlliance.WarAllianceID = ID;
            }
            g2M_UpdatePlayerWarAlliance.AllianceScore = b_Component.AllianceScore;
            b_Component.Parent.GetSessionMGMT().Send(g2M_UpdatePlayerWarAlliance);
        }
        /// <summary>
        /// 向MGMT发送消息
        /// </summary>
        /// <param name="b_Component"></param>
        /// <param name="G_Mmessage"></param>
        /// <returns></returns>
        public async static Task<IResponse> WarAllianceSendMessageG_M(this PlayerWarAllianceComponent b_Component,IRequest G_Mmessage)
        {
            IResponse mResult = await b_Component.Parent.GetSessionMGMT().Call(G_Mmessage);
            return mResult;
        }
        /// <summary>
        /// 更新战盟数据
        /// </summary>
        /// <param name="b_Component"></param>
        /// <param name="Info"></param>
        /// <param name="DeleteTiem"></param>
        public static void UpData(this PlayerWarAllianceComponent b_Component, GMStruct_WarAllinceInfo Info, long DeleteTiem = 0, long ExitTime = 0)
        {
            b_Component.WarAllianceID = Info.WarAllianceID;
            b_Component.WarAllianceName = Info.WarAllianceName;
            b_Component.WarAllianceLevel = Info.WarAllianceLevel;
            b_Component.WarAllianNotice = Info.WarAllianceNotice;
            b_Component.WarAllianceBadge = Info.WarAllianceBadge.array;
            b_Component.DeleteTime = DeleteTiem;
            b_Component.ExitTime = ExitTime;
            b_Component.AllianceLeaderName = Info.LeaderName;
            if (Info.MemberPost != 0)
            {
                b_Component.MemberPost = Info.MemberPost;
            }
        }

        /// <summary>
        /// 获取消息结构数据
        /// </summary> 
        /// <param name="b_Component"></param>
        /// <returns></returns>
        public static Struct_WarAllinceInfo GetInfo(this PlayerWarAllianceComponent b_Component)
        {
            Struct_WarAllinceInfo struct_WarAllinceInfo = new Struct_WarAllinceInfo();
            struct_WarAllinceInfo.WarAllianceID = b_Component.WarAllianceID;
            struct_WarAllinceInfo.WarAllianceName = b_Component.WarAllianceName;
            //struct_WarAllinceInfo.WarAllianceBadge.array = b_Component.WarAllianceBadge;
            struct_WarAllinceInfo.WarAllianceBadge.AddRange(b_Component.WarAllianceBadge);
            struct_WarAllinceInfo.WarAllianceLevel = b_Component.WarAllianceLevel;
            struct_WarAllinceInfo.WarAllianceNotice = b_Component.WarAllianNotice;
            struct_WarAllinceInfo.LeaderName = b_Component.AllianceLeaderName;
            struct_WarAllinceInfo.MemberPost = b_Component.MemberPost;
            struct_WarAllinceInfo.AllianceScore = b_Component.AllianceScore;
            return struct_WarAllinceInfo;
        }
        /// <summary>
        /// 检查是否申请过或者是否超过上限
        /// </summary>
        /// <param name="b_Component"></param>
        /// <param name="WarAllianceID"></param>
        /// <returns></returns>
        public static bool CheckWarAllianceList(this PlayerWarAllianceComponent b_Component, long WarAllianceID)
        {
            if (b_Component.WarAllianceList[4] != 0) return true;

            foreach (long ID in b_Component.WarAllianceList)
            {
                if (ID == WarAllianceID) return true;
            }

            return false;
        }
        public static async Task<bool> CheckRequestList(this PlayerWarAllianceComponent b_Component, int mAreaId)
        {
            var mDBProxy = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>().GetZoneDB(DBType.Core, mAreaId);
            if (mDBProxy == null) return false;

            b_Component.WarAllianceList = new long[5];
            var MemberInfoList = await mDBProxy.Query<DBMemberInfo>(p => p.MemberID == b_Component.Parent.GameUserId && p.IsDisabled == 0);
            if (MemberInfoList != null && MemberInfoList.Count >= 1)
            {
                int indxe = 0;
                foreach (var m in MemberInfoList)
                {
                    DBMemberInfo dBMemberInfo = m as DBMemberInfo;
                    if (Help_TimeHelper.GetNowSecond() - dBMemberInfo.DeleteTime >= 86400)
                    {
                        dBMemberInfo.IsDisabled = 3;
                        b_Component.UpDateWarAlliancePlayerInfo(0,1, dBMemberInfo.DBWarAllianceID);
                        await mDBProxy.Save(dBMemberInfo);
                        continue;
                    }

                    if (indxe >= 5) break;
                        b_Component.WarAllianceList[indxe] = dBMemberInfo.DBWarAllianceID;

                    indxe++;
                }
            }
            return true;
        }
        /// <summary>
        /// 初始化战盟
        /// </summary>
        /// <param name="b_Component"></param>
        public static async Task<bool> OnInit(this PlayerWarAllianceComponent b_Component,int mAreaId)
        {
            var mDBProxy = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>().GetZoneDB(DBType.Core, mAreaId);
            if (mDBProxy == null) return false;

            b_Component.WarAllianceID = 0;
            b_Component.WarAllianceName = "";
            b_Component.WarAllianceBadge = new int[64];
            b_Component.WarAllianceLevel = 0;
            b_Component.WarAllianNotice = "";
            b_Component.DeleteTime = 0;
            b_Component.ExitTime = 0;
            b_Component.MemberPost = 0;
            b_Component.AllianceScore = 0;
            b_Component.WarAllianceList = new long[5];

            var MemberInfoList = await mDBProxy.Query<DBMemberInfo>(p => p.MemberID == b_Component.Parent.GameUserId && p.IsDisabled != 3 && p.IsDisabled != 0);
            if (MemberInfoList != null && MemberInfoList.Count >= 1)
            {
                int indxe = 0;
                foreach (var m in MemberInfoList)
                {
                    DBMemberInfo dBMemberInfo = m as DBMemberInfo;

                    if (dBMemberInfo.IsDisabled == 1)
                    {
                        var WarAllianceList = await mDBProxy.Query<DBWarAllianceData>(p => p.DBWarAllianceID == dBMemberInfo.DBWarAllianceID);
                        if (WarAllianceList != null || WarAllianceList.Count >= 1)
                        {
                            DBWarAllianceData dBWarAllianceData = WarAllianceList[0] as DBWarAllianceData;
                            b_Component.WarAllianceID = dBMemberInfo.DBWarAllianceID;
                            b_Component.WarAllianceName = dBWarAllianceData.DBWarAllianceName;
                            b_Component.WarAllianceBadge = dBWarAllianceData.DBWarAllianceBadge;
                            b_Component.WarAllianceLevel = dBWarAllianceData.DBWarAllianceLevel;
                            b_Component.WarAllianNotice = dBWarAllianceData.DBWarAllianNotice;
                            b_Component.DeleteTime = dBMemberInfo.DeleteTime;
                            b_Component.MemberPost = dBMemberInfo.MemberPost;
                            b_Component.ExitTime = dBMemberInfo.ExitTime;
                            b_Component.WarAllianceList = new long[5];
                            b_Component.AllianceScore = dBMemberInfo.AllianceScore;
                            b_Component.Parent.GetCustomComponent<GamePlayer>().Data.WarAllianceID = b_Component.WarAllianceID;
                            b_Component.UpDateWarAlliancePlayerInfo();
                            G2C_ChangeValue_notice mChangeValueMessage = new G2C_ChangeValue_notice();
                            G2C_BattleKVData mGoldCoinData = new G2C_BattleKVData();
                            mGoldCoinData.Key = (int)E_GameProperty.AllianceScoreChange;
                            mGoldCoinData.Value = b_Component.AllianceScore;
                            mChangeValueMessage.Info.Add(mGoldCoinData);
                            b_Component.Parent.Send(mChangeValueMessage);
                        }
                        break;
                    }
                    if (dBMemberInfo.IsDisabled == 2)
                    {
                        b_Component.DeleteTime = dBMemberInfo.DeleteTime;
                        b_Component.ExitTime = dBMemberInfo.ExitTime;
                        break;
                    }
                }
            }
            if (b_Component.WarAllianceID != 0)
            {
                switch (b_Component.MemberPost)
                {
                    case 3:
                        b_Component.Parent.GetCustomComponent<GamePlayer>().Data.WallTile = $"<{b_Component.WarAllianceName}>%盟主";
                        break;
                    case 2:
                        b_Component.Parent.GetCustomComponent<GamePlayer>().Data.WallTile = $"<{b_Component.WarAllianceName}>%副盟主";
                        break;
                    case 1:
                        b_Component.Parent.GetCustomComponent<GamePlayer>().Data.WallTile = $"<{b_Component.WarAllianceName}>%大队长";
                        break;
                    case 0:
                        b_Component.Parent.GetCustomComponent<GamePlayer>().Data.WallTile = b_Component.WarAllianceName;
                        break;
                }
            }
            return true;
        }

    }
}
