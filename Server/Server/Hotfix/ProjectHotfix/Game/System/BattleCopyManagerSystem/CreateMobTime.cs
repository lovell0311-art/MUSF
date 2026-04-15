using CustomFrameWork;
using CustomFrameWork.Baseic;
using CustomFrameWork.Component;
using ETModel;
using ETModel.Robot;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using TencentCloud.Bri.V20190328.Models;
using UnityEngine;
using TencentCloud.Cme.V20191029.Models;

namespace ETHotfix
{
    [Timer(CopyTimerType.CreateBobTime)]
    public class CreateMobTimer : ATimer<MobTime>
    {
        public override void Run(MobTime self)
        {
            if (self.TimerId == 0 || self.BelongGameNpcId==0  || self.GameAreaId == 0) return;

            C_ServerArea mServerArea = Root.MainFactory.GetCustomComponent<ServerAreaManagerComponent>().GetGameArea(self.GameAreaId);
            if (mServerArea != null)
            {
                BatteCopyManagerComponent batteCopyManagerCpt = mServerArea.GetCustomComponent<BatteCopyManagerComponent>();
                if (batteCopyManagerCpt != null)
                {
                    BattleCopyComponent battleCopyCpt = batteCopyManagerCpt.Get((int)CopyType.CangBaoTu);
                    if (battleCopyCpt != null)
                    {
                        if (battleCopyCpt.battleCopyRoomDic.TryGetValue(self.BelongGameNpcId, out var copyRoomList) != false && copyRoomList.Count > 0)
                        {
                            List<int> ints = new List<int>() { 712,713,714,715,716,717,718,719};
                            //创建怪物
                            var enemyList = new List<List<int>> { 
                                new List<int>() { 717, 138, 139, 140, 141 },
                                new List<int>() { 713, 305, 306, 307, 309 },
                                new List<int>() { 714, 397, 399, 510, 400 },
                                new List<int>() { 712, 397, 399, 510, 400 },
                                new List<int>() { 715, 305, 306, 307, 309 },
                                new List<int>() { 716, 138, 139, 140, 141 },
                                new List<int>() { 718, 305, 306, 307, 309 },
                                new List<int>() { 719, 397, 399, 510, 400 },};
                            var enemyPosList = new List<Vector2Int> { new Vector2Int(64, 50), new Vector2Int(64, 48), new Vector2Int(64, 52), new Vector2Int(62, 59), new Vector2Int(66, 59) };
                            var enemyJsonDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<Enemy_InfoConfigJson>().JsonDic;
                            //每个点放入怪物
                            try
                            {
                                for (int i = 0; i < enemyPosList.Count; i++)
                                {
                                    Vector2Int curPos = enemyPosList[i];
                                    if (enemyJsonDic.TryGetValue(enemyList[self.copyRandomIndex][i], out var curEnemyConfig) == false)
                                    {
                                        return;
                                    }

                                    EnemyComponent b_EnemyComponent = copyRoomList[0].mapComponent.EnsureGetCustomComponent<EnemyComponent>();

                                    var mEnemy = Root.CreateBuilder.GetInstance<Enemy>(false);
                                    mEnemy.Awake(b_EnemyComponent);

                                    mEnemy.SetConfig(curEnemyConfig);
                                    mEnemy.AfterAwake();

                                    int Cnt = copyRoomList[0].playerDic.Count;
                                    if (Cnt > 1 && ints.Contains(self.copyRandomIndex))
                                    {
                                        int Hp = (int)(curEnemyConfig.HP * (1 + Cnt * 0.03f));
                                        mEnemy.UnitData.Hp = Hp;
                                        mEnemy.GamePropertyDic[E_GameProperty.PROP_HP_MAX] = Hp;

                                        int MinAtteck = (int)(curEnemyConfig.DmgMin * (1 + Cnt * 0.03f));
                                        mEnemy.GamePropertyDic[E_GameProperty.MinAtteck] = MinAtteck;
                                        int MaxAtteck = (int)(curEnemyConfig.DmgMax * (1 + Cnt * 0.03f));
                                        mEnemy.GamePropertyDic[E_GameProperty.MaxAtteck] = MaxAtteck;

                                        int Def = (int)(curEnemyConfig.Def * (1 + Cnt * 0.03f));
                                        mEnemy.GamePropertyDic[E_GameProperty.Defense] = Def;

                                        int AttRate = (int)(curEnemyConfig.AttRate * (1 + Cnt * 0.03f));
                                        mEnemy.GamePropertyDic[E_GameProperty.AtteckSuccessRate] = AttRate;

                                        int BloRate = (int)(curEnemyConfig.BloRate * (1 + Cnt * 0.03f));
                                        mEnemy.GamePropertyDic[E_GameProperty.DefenseRate] = BloRate;
                                    }
                                    mEnemy.AwakeSkill();
                                    mEnemy.DataUpdateSkill();

                                    mEnemy.SetInstanceId(mEnemy.Id);

                                    mEnemy.SourcePosX = curPos.x;
                                    mEnemy.SourcePosY = curPos.y;

                                    b_EnemyComponent.AllEnemyDic[mEnemy.InstanceId] = mEnemy;

                                    var mFindTheWay = copyRoomList[0].mapComponent.GetFindTheWay2D(curPos.x, curPos.y);
                                    copyRoomList[0].mapComponent.MoveSendNotice(null, mFindTheWay, mEnemy);
                                }
                                copyRoomList[0].IsJionState = true;
                                copyRoomList[0].MobTime.Dispose();
                            }
                            catch (Exception e)
                            {
                                return;
                            }
                        
                        }
                        return;
                    }
                    return;
                }
                return;
            }
            return;
        }
    }
}