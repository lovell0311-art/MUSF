using CustomFrameWork.Component;
using CustomFrameWork;
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TencentCloud.Mrs.V20200910.Models;
using System.Linq;

namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_ResurrectionInSituRequestHandler : AMActorRpcHandler<C2G_ResurrectionInSituRequest, G2C_ResurrectionInSituResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_ResurrectionInSituRequest b_Request, G2C_ResurrectionInSituResponse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_ResurrectionInSituRequest b_Request, G2C_ResurrectionInSituResponse b_Response, Action<IMessage> b_Reply)
        {
            int mAreaId = (int)(b_Request.AppendData >> 16);
            Player mPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(mAreaId, b_Request.ActorId);
            if (mPlayer == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(102);
                b_Reply(b_Response);
                return false;
            }
            var PlayerShop = mPlayer.GetCustomComponent<PlayerShopMallComponent>();
            if (PlayerShop == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2200);
                b_Reply(b_Response);
                return false;
            }

            if (!PlayerShop.GetPlayerShopState(DeviationType.MaxMonthlyCard))
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2202);
                b_Reply(b_Response);
                return false;
            }

            if (Help_TimeHelper.GetNow() < PlayerShop.dBPlayerShopMall.InSituCd)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2203);
                b_Reply(b_Response);
                return false;
            }

            var gameplayer = mPlayer.GetCustomComponent<GamePlayer>();
            if (gameplayer == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2200);
                b_Reply(b_Response);
                return false;
            }

            if(gameplayer.IsDeath == false)
            {
                // 玩家没死

                // 角色没有死亡，无需复活
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2209);
                b_Reply(b_Response);
                return false;
            }
            RebirthComponent rebirth = gameplayer.GetCustomComponent<RebirthComponent>();
            if(rebirth == null)
            {
                // 兼容代码
                // 等下一帧添加 RebirthComponent 组件

                // 角色没有死亡，无需复活
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2209);
                b_Reply(b_Response);
                return false;
            }

            //var mMapComponent = mServerArea.GetCustomComponent<MapManageComponent>().GetMapByMapIndex(gameplayer.UnitData.Index);
            //if (mMapComponent == null)
            //{
            //    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(102);
            //    b_Reply(b_Response);
            //    return false;
            //}
            //var mMapComponent = Help_MapHelper.GetMapByMapId(mServerArea, gameplayer.UnitData.Index, mPlayer.GameUserId);
            var mMapComponent = rebirth.DeathMap;
            if (mMapComponent == null)
            {
                // 死亡时所在的地图已经销毁

                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(102);
                b_Reply(b_Response);
                return false;
            }

            var b_BattleComponent = mMapComponent.GetCustomComponent<BattleComponent>();

            RebirthHelper.Rebirth(
                gameplayer,
                mMapComponent,
                rebirth.DeathPosX,
                rebirth.DeathPosY,
                "月卡复活");

            //PlayerShop.dBPlayerShopMall.InSituCd = Help_TimeHelper.GetNow() + 60000;
            //PlayerShop.SetPlayerShopDB();
            //PlayerShop.SendPlayerShopState();
            var mReadConfigComponent = Root.MainFactory.GetCustomComponent<ReadConfigComponent>();

            if (mReadConfigComponent.GetJson<Map_InfoConfigJson>().JsonDic.TryGetValue(mMapComponent.MapId, out var mMapConfig) != false)
            {
                if (mMapConfig.IsCopyMap == 0)
                {
                    PlayerShop.dBPlayerShopMall.InSituCd = Help_TimeHelper.GetNow() + 300000;
                    PlayerShop.SetPlayerShopDB();
                    PlayerShop.SendPlayerShopState();
                }
                else
                {
                    gameplayer.CopyLiveCnt--;
                }
            }
            //if (gameplayer.Pets != null)
            //gameplayer.Pets.IsDeath = false;

            //无敌buf

            Func<CombatSource.BattleSyncTimerTask> mCreateFunc = () =>
            {
                var mBattleSyncTimer = CustomFrameWork.Root.CreateBuilder.GetInstance<CombatSource.BattleSyncTimerTask>();
                mBattleSyncTimer.TaskType = CombatSource.E_SyncTimerTaskType.DeathRemoveTask;
                mBattleSyncTimer.CombatRoundId = IdGeneraterNew.Instance.GenerateId();
                mBattleSyncTimer.SyncWaitTime = 5000;
                mBattleSyncTimer.NextWaitTime = b_BattleComponent.CurrentTimeTick + mBattleSyncTimer.SyncWaitTime;

                mBattleSyncTimer.DisposeAction = (b_CombatSource, b_BattleComponent, b_TimerTask) =>
                {
                    if (b_CombatSource.IsDisposeable) return;

                    b_CombatSource.RemoveHealthState(E_BattleSkillStats.WuDi, b_BattleComponent);
                    b_CombatSource.UpdateHealthState();
                };
                mBattleSyncTimer.SyncAction = (b_CombatSource, b_BattleComponent, b_TimerTask, b_TimeTick) =>
                {
                    if (b_CombatSource.IsDisposeable) return CombatSource.E_SyncTimerTaskResult.Dispose;

                    if (b_CombatSource.HealthStatsDic.TryGetValue(E_BattleSkillStats.WuDi, out var hp_Curse) == false)
                    {
                        return CombatSource.E_SyncTimerTaskResult.Dispose;
                    }

                    if (b_TimerTask.NextWaitTime == hp_Curse.ContinueTimeMax)
                    {
                        return CombatSource.E_SyncTimerTaskResult.Dispose;
                    }
                    if (b_TimeTick > hp_Curse.ContinueTimeMax)
                    {
                        return CombatSource.E_SyncTimerTaskResult.Dispose;
                    }
                    else
                    {
                        b_TimerTask.NextWaitTime = hp_Curse.ContinueTimeMax;

                        b_CombatSource.AddTask(b_TimerTask);
                    }
                    return CombatSource.E_SyncTimerTaskResult.NextRound;
                };

                return mBattleSyncTimer;
            };

            // 移除进入场景用的无敌buff
            gameplayer.RemoveCustomComponent<BuffWuDiForEnterMap>();

            gameplayer.AddHealthState(E_BattleSkillStats.WuDi, 0, 5000, 0, mCreateFunc, b_BattleComponent);
            gameplayer.UpdateHealthState();


            b_Reply(b_Response);
            return true;
        }
    }
}