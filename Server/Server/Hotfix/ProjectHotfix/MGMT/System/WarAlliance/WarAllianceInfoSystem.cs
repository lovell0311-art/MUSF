using System.Collections.Generic;
using System.Threading.Tasks;
using CustomFrameWork;
using CustomFrameWork.Component;
using CustomFrameWork.Baseic;
using ETModel;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PF;
using System.Net;
using System.Drawing.Drawing2D;
using static MongoDB.Bson.Serialization.Serializers.SerializerHelper;
using System;

namespace ETHotfix
{
    public static class WarAllianceInfoSystem
    {

        /// <summary>
        /// 获取成员信息
        /// </summary>
        /// <param name="b_CustomComponent"></param>
        /// <param name="GameUserID"></param>
        /// <param name="Type"></param>
        /// <returns></returns>
        public static MemberInfo GetMemberInfo(this WarAllianceInfo b_CustomComponent, long GameUserID, int Type = 0)
        {
            if(b_CustomComponent == null) return null;
            MemberInfo memberInfo = null;
            if (Type == 0)
            {
                if (b_CustomComponent.MemberList.TryGetValue(GameUserID, out memberInfo))
                { 
                    return memberInfo;
                }
                return null;
            }

            if (b_CustomComponent.MemberList2.TryGetValue(GameUserID, out memberInfo))
            {
                return memberInfo;
            }
            return null;
        }
        /// <summary>
        /// 更新成员数据
        /// </summary>
        /// <param name="b_CustomComponent"></param>
        /// <param name="dBMemberInfoData"></param>
        /// <returns></returns>
        public static bool UpDateMemberInfo(this WarAllianceInfo b_CustomComponent, DBMemberInfo dBMemberInfoData)
        {
            if (dBMemberInfoData == null)return false;

            if (b_CustomComponent.MemberList.TryGetValue(dBMemberInfoData.MemberID, out MemberInfo memberInfo))
            {
                memberInfo.MemberID = dBMemberInfoData.MemberID;
                memberInfo.MemberName = dBMemberInfoData.MemberName;
                memberInfo.MemberPost = dBMemberInfoData.MemberPost;
                memberInfo.MemberLevel = dBMemberInfoData.MemberLevel;
                memberInfo.MemberClassType = dBMemberInfoData.MemberClassType;

                return true;
            }
            return false;
        }
        /// <summary>
        /// 更新成员数据
        /// </summary>
        /// <param name="b_CustomComponent"></param>
        /// <param name="MemberInfoData"></param>
        /// <returns></returns>
        public static bool SetMemberInfo(this WarAllianceInfo b_CustomComponent, MemberInfo MemberInfoData)
        {
            if (MemberInfoData == null) return false;
            MemberInfo memberInfo = null;
            if (b_CustomComponent.MemberList.TryGetValue(MemberInfoData.MemberID, out memberInfo))
            {
                memberInfo= MemberInfoData;
                //Log.Warning($"更新战盟积分后{memberInfo.MemberID}:{memberInfo.AllianceScore}");
                return true;
            }
            return false;
        }
        /// <summary>
        /// 删除成员
        /// </summary>
        /// <param name="b_CustomComponent"></param>
        /// <param name="GameUserID"></param>
        /// <returns></returns>
        public static MemberInfo DeleteMember(this WarAllianceInfo b_CustomComponent, long GameUserID,int Type = 0)
        {
            MemberInfo member;
            MemberInfo memberInfo = new MemberInfo();
            if (Type == 0)
            {
                if (b_CustomComponent.MemberList.TryGetValue(GameUserID, out member))
                {
                    memberInfo = member;
                    b_CustomComponent.MemberList.Remove(GameUserID);
                    return memberInfo;
                }
            }
            if (Type == 1)
            {
                if (b_CustomComponent.MemberList2.TryGetValue(GameUserID, out member))
                {
                    memberInfo = member;
                    b_CustomComponent.MemberList2.Remove(GameUserID);
                    return memberInfo;
                }
            }
                return null;
        }
        /// <summary>
        /// 检查成员是否存在
        /// </summary>
        /// <param name="b_CustomComponent"></param>
        /// <param name="GameUserID"></param>
        /// <returns></returns>
        public static bool CheckMember(this WarAllianceInfo b_CustomComponent, long GameUserID,out MemberInfo memberInfo)
        {
            MemberInfo member;
            if (b_CustomComponent.MemberList2.TryGetValue(GameUserID, out member))
            {
                memberInfo = member;
                return true;
            }
            if (b_CustomComponent.MemberList.TryGetValue(GameUserID, out member))
            {
                memberInfo = member;
                return false;
            }
            memberInfo = null;
            return false;
        }
        /// <summary>
        /// 信息接口
        /// </summary>
        /// <param name="b_CustomComponent"></param>
        /// <param name="G_Mmessage"></param>
        /// <returns></returns>
        public static bool SendNoticeMember(this WarAllianceInfo b_CustomComponent, IMessage G_Mmessage,int MeberServerID)
        {
            if(MeberServerID == 0) return false;
            var mMatchConfigs = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfo(AppType.Game, MeberServerID);
            //int mServerIndex = RandomHelper.RandomNumber(0, mMatchConfigs.Length);
            Game.Scene.GetComponent<NetInnerComponent>().Get(mMatchConfigs.ServerInnerIP).Send(G_Mmessage);
            return true;
        }

        public static bool AddMember(this WarAllianceInfo b_CustomComponent,long MemberID, int ServerID = 0,int State = 1)
        {
            if (b_CustomComponent == null) return false;

            MemberInfo memberInfo = new MemberInfo();
            if (b_CustomComponent.MemberList2.TryGetValue(MemberID, out MemberInfo member))
            {
                memberInfo.MemberID = member.MemberID;
                memberInfo.MemberLevel = member.MemberLevel;
                memberInfo.MemberName = member.MemberName;
                memberInfo.MeberServerID = ServerID;
                memberInfo.MemberPost = member.MemberPost;
                memberInfo.MemberClassType = member.MemberClassType;
                memberInfo.MeberState = State;
            }

            b_CustomComponent.MemberList.Add(MemberID,memberInfo);
            b_CustomComponent.MemberList2.Remove(MemberID);
            return true;
        }
    }
}
