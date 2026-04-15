using CustomFrameWork;
using CustomFrameWork.Baseic;
using CustomFrameWork.Component;
using ETModel;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETHotfix
{
    [EventMethod(typeof(CumulativeRechargeGiftComponent), EventSystemType.INIT)]
    public class CumulativeRechargeGiftComponentInitSystem : ITEventMethodOnInit<CumulativeRechargeGiftComponent>
    {
        public void OnInit(CumulativeRechargeGiftComponent self)
        {
            self.OnInit();
        }
    }

    [EventMethod(typeof(CumulativeRechargeGiftComponent), EventSystemType.LOAD)]
    public class CumulativeRechargeGiftComponentLoadSystem : ITEventMethodOnLoad<CumulativeRechargeGiftComponent>
    {
        public override void OnLoad(CumulativeRechargeGiftComponent self)
        {
            self.OnInit();
        }
    }

    [FriendOf(typeof(CumulativeRechargeGiftComponent))]
    public static class CumulativeRechargeGiftComponentSystem
    {
        public static void OnInit(this CumulativeRechargeGiftComponent self)
        {
            self.id2ItemInfoId.Clear();


            ReadConfigComponent readConfig = Root.MainFactory.GetCustomComponent<ReadConfigComponent>();
            var jsonDic = readConfig.GetJson<CumulativeRecharge_ItemInfoConfigJson>().JsonDic;
            foreach (var item in jsonDic.Values)
            {
                (int configId, int id2, E_GameOccupation roleType) key = (item.TypeId, item.Id2, (E_GameOccupation)item.RoleType);
                self.id2ItemInfoId.Add(key, item.Id);
            }
        }

        public static List<int> GetAllItemInfoId(this CumulativeRechargeGiftComponent self,int configId,int id2,E_GameOccupation roleType)
        {
            List<int> ids = new List<int>();
            if (roleType == E_GameOccupation.None) return ids;
            ids.AddRange(self.id2ItemInfoId.GetAll((configId, id2, roleType)));
            ids.AddRange(self.id2ItemInfoId.GetAll((configId, id2, E_GameOccupation.None)));
            return ids;
        }

    }
}
