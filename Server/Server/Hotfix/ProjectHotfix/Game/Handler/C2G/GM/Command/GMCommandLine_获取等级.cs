
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CustomFrameWork;
using CustomFrameWork.Baseic;
using CustomFrameWork.Component;
using ETModel;

namespace ETHotfix
{
    [GameMasterCommandLine("获取等级")]
    public class GMCommandLine_获取等级 : C_GameMasterCommandLine<Player, IResponse>
    {
        public override async Task Run(Player b_Player, List<int> b_Parameter, IResponse b_Response)
        {
            if (b_Parameter.Count < 0)
            {
                b_Response.Error = 99;
                b_Response.Message = "参数不对";
                return;
            }

            var mGamePlayer = b_Player.GetCustomComponent<GamePlayer>();

            int oldLevel = mGamePlayer.Data.Level;
            mGamePlayer.Data.Level = b_Parameter[0];

            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, (int)b_Player.GameAreaId);
            var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get(b_Player.GameAreaId);
            mWriteDataComponent.Save(mGamePlayer.Data, dBProxy2).Coroutine();

            G2C_ChangeValue_notice mChangeValue_notice = new G2C_ChangeValue_notice();
            G2C_BattleKVData mBattleKVData = new G2C_BattleKVData();
            mBattleKVData.Key = (int)E_GameProperty.Level;
            mBattleKVData.Value = mGamePlayer.Data.Level;
            mChangeValue_notice.Info.Add(mBattleKVData);
            b_Player.Send(mChangeValue_notice);

            // 发布 GamePlayerLevelUp 事件
            ETModel.EventType.GamePlayerLevelUp.Instance.gamePlayer = mGamePlayer;
            ETModel.EventType.GamePlayerLevelUp.Instance.oldLevel = oldLevel;
            ETModel.EventType.GamePlayerLevelUp.Instance.newLevel = mGamePlayer.Data.Level;
            Root.EventSystem.OnRun("GamePlayerLevelUp", ETModel.EventType.GamePlayerLevelUp.Instance);
        }
    }
}