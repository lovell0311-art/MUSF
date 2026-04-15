using CustomFrameWork.Baseic;
using CustomFrameWork;
using ETModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CustomFrameWork.Component;
using TencentCloud.Hcm.V20181106.Models;
using System.Linq;
using System.Drawing.Drawing2D;
using TencentCloud.Tics.V20181115.Models;
using Aop.Api.Domain;
using System.Net;
using TencentCloud.Ecm.V20190719.Models;
using System.Data.SqlTypes;
using TencentCloud.Mrs.V20200910.Models;

namespace ETHotfix
{
    [EventMethod(typeof(PromotionComponent), EventSystemType.INIT)]
    public class PromotionComponentOnInit : ITEventMethodOnInit<PromotionComponent>
    {
        /// <summary>
        /// 服务器开启时读取数据
        /// </summary>
        /// <param name="b_Component"></param>
        public void OnInit(PromotionComponent b_Component)
        {
            OnInitAsync(b_Component).Coroutine();
        }

        public async Task OnInitAsync(PromotionComponent b_Component)
        {
            try
            {
                b_Component.OnInit().Coroutine();
            }
            catch (Exception e)
            {
                Log.Fatal("推广码初始化失败", e);
            }
        }
    }
    public static class PromotionComponentSystem
    {
        public static async Task OnInit(this PromotionComponent self)
        {
            TimerComponent mTimerComponent = Root.MainFactory.GetCustomComponent<TimerComponent>();
            await mTimerComponent.WaitAsync(10000);
            if (self.PromotionUser == null || self.PromotionUserCode == null || self.chars == null)
            {
                self.PromotionUser = new Dictionary<long, PlayerPromotionInfo>();
                self.PromotionUserCode = new Dictionary<string, long>();
                self.chars = new List<string>() { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
            }

            var mDBProxy = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>().GetZoneDB(DBType.Core, DBProxyComponent.CommonDBId);
            if (mDBProxy == null) return;

            var UserList = await mDBProxy.Query<PlayerPromotionInfo>(p => p.IsDispose == 0);
            if (UserList == null) return;

            if (UserList.Count >= 1)
            {
                foreach (var Info in UserList)
                {
                    PlayerPromotionInfo dBData = Info as PlayerPromotionInfo;
                    if (dBData.MemberIdI != "")
                        dBData.MemberIUserId = Help_JsonSerializeHelper.DeSerialize<List<long>>(dBData.MemberIdI);
                    if (dBData.MemberIdII != "")
                        dBData.MemberIIUserId = Help_JsonSerializeHelper.DeSerialize<List<long>>(dBData.MemberIdII);
                    if (dBData.MemberIdIII != "")
                        dBData.MemberIIIUserId = Help_JsonSerializeHelper.DeSerialize<List<long>>(dBData.MemberIdIII);

                    self.PromotionUser.Add(dBData.UserID, dBData);
                    self.PromotionUserCode.Add(dBData.Code, dBData.UserID);
                }
            }
        }

        public static string CreateCode(this PromotionComponent self,long UserID)
        {
            string NewCode = "";
            var Rd = new Random();
            int Index = 0;
            while (true) 
            {
                for (int i = 0; i < 6; i++) 
                {
                    Index = Rd.Next(0, 26);
                    NewCode += self.chars[Index];
                }
                if (!self.PromotionUserCode.ContainsKey(NewCode))
                {
                    self.PromotionUserCode.Add(NewCode, UserID);

                    PlayerPromotionInfo playerPromotionInfo = new PlayerPromotionInfo();
                    playerPromotionInfo.Id = IdGeneraterNew.Instance.GenerateId();
                    playerPromotionInfo.UserID = UserID;
                    playerPromotionInfo.Code = NewCode;

                    self.PromotionUser.Add(UserID, playerPromotionInfo);

                    self.SetDB(playerPromotionInfo).Coroutine();
                    break;
                }
            }
            return NewCode;
        }
        public static PlayerPromotionInfo GetUserInfo(this PromotionComponent self, long mUserID)
        {
            if (self.PromotionUser.TryGetValue(mUserID, out var Info))
            {
                return Info;
            }
            return null;
        }
        public static async Task<bool> UPAwardStatus(this PromotionComponent self, int Type, long mUserID)
        {
            if (self.PromotionUser.TryGetValue(mUserID, out var Info))
            {
                if (Type == 1)
                {
                    if (Info.MemberStatusI == 1)
                    {
                        Info.MemberStatusI = 2;
                        await self.SetDB(Info);
                        return true;
                    }
                    return false;
                }
                else if (Type == 2)
                {
                    if (Info.MemberStatusII == 1)
                    {
                        Info.MemberStatusII = 2;
                        await self.SetDB(Info);
                        return true;
                    }
                    return false;
                }
                else if (Type == 3)
                {
                    if (Info.MemberStatusIII == 1)
                    {
                        Info.MemberStatusIII = 0;
                        await self.SetDB(Info);
                        return true;
                    }
                    return false;
                }
            }
            return false;
        }
        public static async Task<bool> UPNumberOfGuests(this PromotionComponent self,int Type,string mCode,long mUserID)
        {
            if (self.PromotionUserCode.TryGetValue(mCode, out long Value))
            {
                if (self.PromotionUser.TryGetValue(Value, out var Info))
                {
                    if (Type == 0)
                    {
                        if (!Info.MemberIUserId.Contains(mUserID))
                        {
                            Info.MemberI++;
                            Info.MemberIUserId.Add(mUserID);
                            Info.MemberIdI = Help_JsonSerializeHelper.Serialize(Info.MemberIUserId);
                            if (Info.MemberI >= 10)
                                Info.MemberStatusI = 1;
                        }
                    }
                    else if (Type == 120)
                    {
                        if (!Info.MemberIIUserId.Contains(mUserID))
                        {
                            Info.MemberII++;
                            Info.MemberIIUserId.Add(mUserID);
                            Info.MemberIdII = Help_JsonSerializeHelper.Serialize(Info.MemberIIUserId);
                            if (Info.MemberII >= 10)
                                Info.MemberStatusII = 1;
                        }
                    }
                    else if (Type == 220)
                    {
                        if (!Info.MemberIIIUserId.Contains(mUserID))
                        {
                            Info.MemberIII++;
                            Info.MemberIIIUserId.Add(mUserID);
                            Info.MemberIdIII = Help_JsonSerializeHelper.Serialize(Info.MemberIIIUserId);

                            if (Info.MemberIII % 10 == 0)
                            {
                                Info.MemberStatusIII = 1;
                            }
                        }
                    }

                    await self.SetDB(Info);
                    return true;
                }
            }
            return false;
        }
        public static async Task SetDB(this PromotionComponent self, PlayerPromotionInfo playerPromotionInfo)
        {
            var mDBProxy = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>().GetZoneDB(DBType.Core, DBProxyComponent.CommonDBId);
            if (mDBProxy == null) return;

            await mDBProxy.Save(playerPromotionInfo);
            
        }
    }
}
