using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using CustomFrameWork.Component;
using static ETHotfix.MapManageComponentSystem;
using CustomFrameWork;

namespace ETHotfix
{
    /// <summary>
    /// 单个地图
    /// </summary>
    public static partial class NpcComponentSystem
    {
        public static void InitMapNpc(this NpcComponent b_Component, List<MapSafeAreaInfo> b_EnemyInfos)
        {
            var mMapComponent = b_Component.Parent;
            var mJsonDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<Npc_InfoConfigJson>().JsonDic;

            for (int i = 0, len = b_EnemyInfos.Count; i < len; i++)
            {
                var mEnemyInfo = b_EnemyInfos[i];

                if (mJsonDic.TryGetValue(mEnemyInfo.Index, out var mNpcConfig))
                {
                    var mNpc = Root.CreateBuilder.GetInstance<GameNpc, long>(Help_UniqueValueHelper.GetServerUniqueValue());
                    mNpc.SetConfig(mNpcConfig);
                    mNpc.AfterAwake();
                    b_Component.AllNpcDic[mNpc.Id] = mNpc;

                    if (mNpcConfig.NpcType == (int)NPCTypeEnum.Shop || mNpcConfig.NpcType == (int)NPCTypeEnum.Shop_EquipRepair)
                    {
                        //添加商店Component
                        //Log.Debug("添加商人："+mNpcConfig.Name);

                        if (OptionComponent.Options.AppType == AppType.Game)
                        {
                            var shopComponnet = new NpcShopComponent();
                            shopComponnet.InitShop(mNpc);

                            mNpc.SetShopComponent(shopComponnet);
                        }

                        //Log.Debug("商人背包：");
                        //foreach (var item in shopComponnet.mItemDict)
                        //{
                        //    Log.Debug(item.Value.ConfigData.Name);
                        //}
                        //shopComponnet.itemBox.Debug();

                    }
                    // 地图
                    var mCellFieldTemp = mMapComponent.GetMapCellFieldByPos(mEnemyInfo.PositionX, mEnemyInfo.PositionY);
                    if (mCellFieldTemp != null)
                    {
                        // 地图分域
                        mNpc.X = mEnemyInfo.PositionX;
                        mNpc.Y = mEnemyInfo.PositionY;
                        mNpc.Angle = mEnemyInfo.Direction;
                        mCellFieldTemp.FieldNpcDic[mNpc.Id] = mNpc;
                    }
                }
            }
        }

    }

}