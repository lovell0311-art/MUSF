
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CustomFrameWork;
using CustomFrameWork.Baseic;
using CustomFrameWork.Component;
using ETModel;
using UnityEngine;

namespace ETHotfix
{
    /// <summary>
    /// 大师洗点果实
    /// </summary>
    [ItemUseRule(typeof(Use310090))]
    public class Use310090 : C_ItemUseRule<Player, Item, IResponse>
    {
        public override async Task Run(Player b_Player, Item b_Item, IResponse b_Response)
        {
            C_ServerArea mServerArea = Root.MainFactory.GetCustomComponent<ServerAreaManagerComponent>().GetGameArea(b_Player.SourceGameAreaId);
            if (mServerArea == null)
            {
                b_Response.Error = 99;
                b_Response.Message = "参数不对";
                return;
            }

            var mCurrentTemp = b_Player.GetCustomComponent<GamePlayer>();

            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            DataCacheManageComponent mDataCacheComponent = b_Player.AddCustomComponent<DataCacheManageComponent>();
            var mDataCache = mDataCacheComponent.Get<DBMasterData>();
            if (mDataCache == null)
            {
                b_Response.Error = 99;
                b_Response.Message = "大师系统不满足开放条件";
                return;
            }
            var mData = mDataCache.OnlyOne();
            ReadConfigComponent readConfig = Root.MainFactory.GetCustomComponent<ReadConfigComponent>();
            var jsonDic = readConfig.GetJson<Reincarnate_InfoConfigJson>().JsonDic;
            int Value = 0;  
            foreach(var reincarnate_Infoconfig in jsonDic)
            {
                if(mCurrentTemp.Data.ReincarnateCnt >= reincarnate_Infoconfig.Value.Id)
                    Value = reincarnate_Infoconfig.Value.MasterPoints;
            }
            mData.PropertyPoint = Value + mData.ExtraPoints;

            if (mData.SkillId.Count > 0)
            {
                var mKeylist = mData.SkillId.Keys.ToList();
                for (int i = 0; i < mKeylist.Count; i++)
                {
                    var mkey = mKeylist[i];

                    switch ((E_BattleMaster_Id)mkey)
                    {
                        case E_BattleMaster_Id.Common2001:
                        case E_BattleMaster_Id.Common2002:
                        case E_BattleMaster_Id.Common2003:
                        case E_BattleMaster_Id.Common2004:
                        case E_BattleMaster_Id.Common2005:
                        case E_BattleMaster_Id.Common2006:
                        case E_BattleMaster_Id.Common2007:
                        case E_BattleMaster_Id.Common2008:
                        case E_BattleMaster_Id.Common2009:
                        case E_BattleMaster_Id.Common2010:
                        case E_BattleMaster_Id.Common2011:
                        case E_BattleMaster_Id.Common2012:
                        case E_BattleMaster_Id.Common2013:
                            {
                                mData.SkillId[mkey] = 0;
                            }
                            break;
                        default:
                            mData.SkillId.Remove(mkey);
                            break;
                    }
                }
                mData.Serialize();
            }

            foreach (var item in mCurrentTemp.MasterGroup)
            {
                item.Value.Dispose();
            }
            mCurrentTemp.MasterGroup.Clear();
            mCurrentTemp.BattleMasteryDic.Clear();

            var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, (int)b_Player.GameAreaId);
            var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get(b_Player.GameAreaId);
            mWriteDataComponent.Save(mData, dBProxy2).Coroutine();

            //发送推送
            void AddPropertyNotice(G2C_ChangeValue_notice b_ChangeValue_notice, E_GameProperty b_GameProperty)
            {
                G2C_BattleKVData mBattleKVData = new G2C_BattleKVData();
                mBattleKVData.Key = (int)b_GameProperty;
                mBattleKVData.Value = mCurrentTemp.GetNumerial(b_GameProperty);
                b_ChangeValue_notice.Info.Add(mBattleKVData);
            }

            G2C_ChangeValue_notice mChangeValueMessage = new G2C_ChangeValue_notice();
            mChangeValueMessage.GameUserId = mCurrentTemp.InstanceId;

            AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_AG);
            AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_AG_MAX);

            AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_SD);
            AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_SD_MAX);

            AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_MP);
            AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_MP_MAX);

            AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_HP);
            AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_HP_MAX);

            var mMapComponent = Help_MapHelper.GetMapByMapId(mServerArea, mCurrentTemp.UnitData.Index, b_Player.GameUserId);
            if (mMapComponent == null)
            {
                b_Response.Error = 99;
                b_Response.Message = "参数不对";
                return;
            }
            mMapComponent.SendNotice(mCurrentTemp, mChangeValueMessage);
        }
    }
}