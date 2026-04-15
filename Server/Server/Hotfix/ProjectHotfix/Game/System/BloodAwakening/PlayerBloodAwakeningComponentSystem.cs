using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TencentCloud.Bri.V20190328.Models;

namespace ETHotfix
{
    public static partial class PlayerBloodAwakeningComponentSystem
    {
        public static async Task<bool> LoadPalyerBloodAwakening(this PlayerBloodAwakeningComponent self)
        {
            if (self.Parent == null) return false;
            self.BloodAwakeningInfo = new Dictionary<int, DBBloodAwakeningInfo>();
            self.UseBloodAwakeningId = 0;
            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, self.Parent.GameAreaId);

            var ListInfo =await dBProxy2.Query<DBBloodAwakeningInfo>(p => p.GameUserId == self.Parent.GameUserId && p.IsDisabled != 1);
            if (ListInfo.Count >= 1)
            {
                foreach (var BloodAwakeningInfo in ListInfo)
                {
                    DBBloodAwakeningInfo Info = (BloodAwakeningInfo as DBBloodAwakeningInfo);
                    if (!string.IsNullOrEmpty(Info.AttributeNodeStr))
                        Info.AttributeNode = Help_JsonSerializeHelper.DeSerialize<Dictionary<int, List<int>>>(Info.AttributeNodeStr);

                    self.BloodAwakeningInfo.Add(Info.BloodAwakeningId, Info);
                    if (Info.IsUse)
                    {
                        self.UseBloodAwakeningId = Info.BloodAwakeningId;
                    }
                }
            }
            if (self.UseBloodAwakeningId != 0)
            {
                self.EditAttribute(self.UseBloodAwakeningId);
                self.AttributeApplication(self.UseBloodAwakeningId);
            }
            return true;
        }
        /// <summary>
        /// 激活血脉
        /// </summary>
        /// <returns></returns>
        public static int ActivateBloodAwakening(this PlayerBloodAwakeningComponent self, int BloodAwakeningId)
        {
            var BloodJson = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<BloodAwakening_InfoConfigJson>().JsonDic;
            BloodJson.TryGetValue(BloodAwakeningId, out var Info);

            DBBloodAwakeningInfo dBBloodAwakeningInfo = new DBBloodAwakeningInfo();
            dBBloodAwakeningInfo.Id = IdGeneraterNew.Instance.GenerateUnitId(self.mPlayer.GameAreaId);
            dBBloodAwakeningInfo.GameUserId = self.mPlayer.GameUserId;
            dBBloodAwakeningInfo.BloodAwakeningId = BloodAwakeningId;
            
            if (Info.PurityTimeDic.TryGetValue(1, out var NeedTime))
            {
                if (NeedTime == 0)
                {
                    dBBloodAwakeningInfo.UrrentRingNumber++;
                    dBBloodAwakeningInfo.AttributeNode.Add(1, new List<int>());
                   var AttributeNodeJson = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<AttributeNode_InfoConfigJson>().JsonDic;
                    if (Info.AttributeNode.TryGetValue(1, out var Attribute))
                    {
                        foreach (var AttributeId in Attribute)
                        {
                            if (AttributeNodeJson.ContainsKey(AttributeId.Key))
                            {
                                if (AttributeId.Value)
                                {
                                    dBBloodAwakeningInfo.AttributeNode[1].Add(AttributeId.Key);
                                }
                            }
                            else
                                return 3802;
                        }
                        if (dBBloodAwakeningInfo.AttributeNode.Count >= 1)
                        {
                            dBBloodAwakeningInfo.AttributeNodeStr = Help_JsonSerializeHelper.Serialize(dBBloodAwakeningInfo.AttributeNode);
                        }
                    }
                    else
                        return 3802;
                }
                else
                {
                    dBBloodAwakeningInfo.ActivateNeedTime = Help_TimeHelper.GetNowSecond() + NeedTime;
                }

                self.BloodAwakeningInfo.Add(dBBloodAwakeningInfo.BloodAwakeningId, dBBloodAwakeningInfo);
                self.SetDB(dBBloodAwakeningInfo.BloodAwakeningId);
                return 3801;
            }
            else
                return 3803;
        }
        /// <summary>
        /// 净化血脉
        /// </summary>
        /// <param name="self"></param>
        /// <param name="BloodAwakeningId"></param>
        /// <returns></returns>
        public static int PurityBloodAwakening(this PlayerBloodAwakeningComponent self, int BloodAwakeningId,int RingId)
        {
            if (self.BloodAwakeningInfo.Count <= 0) return 3805;
            if (self.BloodAwakeningInfo.TryGetValue(BloodAwakeningId, out var info))
            {
                if (info.UrrentRingNumber + 1 != RingId) return 3811;
                var Json = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<BloodAwakening_InfoConfigJson>().JsonDic;
                if (Json.TryGetValue(BloodAwakeningId, out var ConfigInfo))
                {
                    if (ConfigInfo.PurityTimeDic.TryGetValue(RingId, out var time))
                    {
                        info.ActivateNeedTime = Help_TimeHelper.GetNowSecond() + time;
                        self.SetDB(BloodAwakeningId);
                        return 3806;
                    }
                    else
                        return 3807;
                }
                else
                    return 3804;
            }
            else
                return 3804;
        }
        /// <summary>
        /// 净化提速
        /// </summary>
        /// <returns></returns>
        public static int CleanUpSpeed(this PlayerBloodAwakeningComponent self, int BloodAwakeningId, int SubtractTime)
        {
            if (self.BloodAwakeningInfo.Count <= 0) return 3805;

            var BloodJson = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<BloodAwakening_InfoConfigJson>().JsonDic;
            BloodJson.TryGetValue(BloodAwakeningId, out var Info);
            if (self.BloodAwakeningInfo.TryGetValue(BloodAwakeningId, out var info))
            {
                info.ActivateNeedTime -= SubtractTime;
                if (info.ActivateNeedTime <= Help_TimeHelper.GetNowSecond())
                {
                    info.ActivateNeedTime = 0;
                    var AttributeNodeJson = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<AttributeNode_InfoConfigJson>().JsonDic;
                    if (Info.AttributeNode.TryGetValue(info.UrrentRingNumber+1, out var Attribute))
                    {
                        info.UrrentRingNumber++;
                        info.AttributeNode.Add(info.UrrentRingNumber, new List<int>());
                        foreach (var AttributeId in Attribute)
                        {
                            if (AttributeNodeJson.ContainsKey(AttributeId.Key))
                            {
                                if (AttributeId.Value)
                                {
                                    info.AttributeNode[info.UrrentRingNumber].Add(AttributeId.Key);
                                }
                            }
                            else
                                return 3802;
                        }

                        info.AttributeNodeStr = Help_JsonSerializeHelper.Serialize(info.AttributeNode);
                        //属性应用
                        if (self.UseBloodAwakeningId == BloodAwakeningId)
                        {
                            self.EditAttribute(BloodAwakeningId);
                            self.AttributeApplication(BloodAwakeningId);
                        }
                        self.SetDB(BloodAwakeningId);
                        return 3809;
                    }
                    else
                        return 3802;

                }
                else
                {
                    self.SetDB(BloodAwakeningId);
                    return 3808;
                }
            }
            else
                return 3804;
        }
        /// <summary>
        /// 激活节点
        /// </summary>
        /// <param name="self"></param>
        /// <param name="BloodAwakeningId"></param>
        /// <returns></returns>
        public static int ActivateAttribute(this PlayerBloodAwakeningComponent self, int BloodAwakeningId,int RingId,int AttributeId)
        {
            if (self.BloodAwakeningInfo.Count <= 0) return 3805;

            if (self.BloodAwakeningInfo.TryGetValue(BloodAwakeningId,out var dBBloodAwakeningInfo))
            {
                if (dBBloodAwakeningInfo.AttributeNode.TryGetValue(RingId, out var value))
                    value.Add(AttributeId);
                else
                    return 3814;
                dBBloodAwakeningInfo.AttributeNodeStr = Help_JsonSerializeHelper.Serialize(dBBloodAwakeningInfo.AttributeNode);
                self.EditAttribute(BloodAwakeningId);
                self.AttributeApplication(BloodAwakeningId);
                self.SetDB(BloodAwakeningId);
                return 3810;
            }
            else
                return 3804;
        }
        public static void EditAttribute(this PlayerBloodAwakeningComponent self, int BloodAwakeningId)
        {
            if (self.BloodAwakeningInfo.TryGetValue(BloodAwakeningId, out var dBBloodAwakeningInfo))
            {
                if (dBBloodAwakeningInfo.Attribute == null)
                    dBBloodAwakeningInfo.Attribute = new List<int>();
                else
                    dBBloodAwakeningInfo.Attribute.Clear();

                var AttributeNodeJson = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<AttributeNode_InfoConfigJson>().JsonDic;
                foreach (var Ring in dBBloodAwakeningInfo.AttributeNode)
                {
                    foreach (var Id in Ring.Value)
                    {
                        if (AttributeNodeJson.TryGetValue(Id, out var Info))
                        {
                            foreach (var Value in Info.AttributeNode)
                            {
                                dBBloodAwakeningInfo.Attribute.Add(Value);
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 属性应用
        /// </summary>
        /// <returns></returns>
        public static bool AttributeApplication(this PlayerBloodAwakeningComponent self, int BloodAwakeningId)
        {
            if (self.BloodAwakeningInfo.Count <= 0) return false;

            if (self.BloodAwakeningInfo.TryGetValue(BloodAwakeningId, out var dBBloodAwakeningInfo))
            {
                var equipComponent = self.mPlayer.GetCustomComponent<EquipmentComponent>();
                if (equipComponent != null)
                {
                    equipComponent.ApplyEquipProp();
                }
                return true;
            }
            else
                return false;
        }
        /// <summary>
        /// 使用血脉
        /// </summary>
        /// <returns></returns>
        public static bool UseBloodAwakening(this PlayerBloodAwakeningComponent self, int BloodAwakeningId)
        {
            if (self.BloodAwakeningInfo.Count <= 0) return false;

            if (BloodAwakeningId != self.UseBloodAwakeningId)
            {
                if (self.BloodAwakeningInfo.TryGetValue(self.UseBloodAwakeningId, out var dBBloodAwakeningInfo))
                {
                    dBBloodAwakeningInfo.IsUse = false;
                    dBBloodAwakeningInfo.Attribute.Clear();
                    //self.AttributeApplication(BloodAwakeningId);
                }
                self.SetDB(self.UseBloodAwakeningId);
            }
            if (self.BloodAwakeningInfo.TryGetValue(BloodAwakeningId, out var bBloodAwakeningInfo))
            {
                bBloodAwakeningInfo.IsUse = true;
                self.UseBloodAwakeningId = BloodAwakeningId;
                self.EditAttribute(BloodAwakeningId);
                self.AttributeApplication(BloodAwakeningId);
                self.SetDB(BloodAwakeningId);
                return true;
            }
            else
                return false;
        }
        public static void SetDB(this PlayerBloodAwakeningComponent self, int BloodAwakeningId)
        {
            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, self.Parent.GameAreaId);
            if (self.BloodAwakeningInfo.TryGetValue(BloodAwakeningId, out var info))
                dBProxy2.Save(info).Coroutine();
        }
        /// <summary>
        /// 获取血脉
        /// </summary>
        /// <returns></returns>
        public static DBBloodAwakeningInfo GetInfo(this PlayerBloodAwakeningComponent self, int BloodAwakeningId)
        {
            if (self.BloodAwakeningInfo.TryGetValue(BloodAwakeningId, out var info))
                return info;
            return null;
        }
        public static BloodVesselInfo GetBloodSendInfo(this PlayerBloodAwakeningComponent self, int BloodAwakeningId)
        {
            BloodVesselInfo bloodVesselInfo = new BloodVesselInfo();
            if (self.BloodAwakeningInfo.TryGetValue(BloodAwakeningId, out var info))
            {
                bloodVesselInfo.BloodId = BloodAwakeningId;
                bloodVesselInfo.NextStage = info.ActivateNeedTime;
                bloodVesselInfo.IsUse = info.IsUse?1:0;
                foreach (var Ring in info.AttributeNode)
                {
                    RingInfo ringInfo = new RingInfo();
                    ringInfo.RingId = Ring.Key;
                    ringInfo.ActivatedNode.AddRange(Ring.Value);
                    bloodVesselInfo.NodeList.Add(ringInfo);
                }
            }
            return bloodVesselInfo;
        }

        public static List<(long,int)> CheckItem(this PlayerBloodAwakeningComponent self, int BloodAwakeningId,int NodeId, int Type)
        {
            var BloodJson = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<BloodAwakening_InfoConfigJson>().JsonDic;
            BloodJson.TryGetValue(BloodAwakeningId, out var bloodAwakening_InfoConfig);
            var AttributeNodeJson = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<AttributeNode_InfoConfigJson>().JsonDic;
            AttributeNodeJson.TryGetValue(NodeId, out var attributeNode_InfoConfig);
            if(bloodAwakening_InfoConfig == null && BloodAwakeningId != 0)
                return null;
            if (attributeNode_InfoConfig == null && NodeId != 0)
                return null;

            Dictionary<int,int> NeedItem = new Dictionary<int,int>();
            if (Type == 0)//检查激活的道具
                NeedItem = bloodAwakening_InfoConfig.ActivateNeedDic;
            else if (Type == 1)//净化提速检查道具
                NeedItem = bloodAwakening_InfoConfig.PurityNeedDic;
            else if (Type == 2)//节点激活检查道具
                NeedItem = attributeNode_InfoConfig.ActivateNeedDic;
            
            var Bk = self.mPlayer.GetCustomComponent<BackpackComponent>();
            if (Bk == null)
                return null;
            List<(long,int)> Need = new List<(long, int)>();
            int Cnt = 0;
            foreach (var item in NeedItem)
            {
                var ItemList = Bk.GetAllItemByConfigID(item.Key);
                if (ItemList == null)
                    return null;

                Cnt = item.Value;
                foreach (var It in ItemList)
                {
                    if (It.Value.GetProp(EItemValue.Quantity) >= Cnt)
                    {
                        Need.Add((It.Key, Cnt));
                        Cnt = 0;
                        break;
                    }
                    else
                    {
                        Cnt -= It.Value.GetProp(EItemValue.Quantity);
                        Need.Add((It.Key, It.Value.GetProp(EItemValue.Quantity)));
                    }
                }
                if (Cnt != 0)
                    return null;
            }
            return Need;
        }

        public static bool UseItem(this PlayerBloodAwakeningComponent self,List<(long, int)> itemlist)
        {
            var Bk = self.mPlayer.GetCustomComponent<BackpackComponent>();
            if (Bk == null)
                return false;
            foreach (var item in itemlist)
            {
                Bk.UseItem(item.Item1,"血脉消耗", item.Item2);
            }
            return true;
        }
    }
}
