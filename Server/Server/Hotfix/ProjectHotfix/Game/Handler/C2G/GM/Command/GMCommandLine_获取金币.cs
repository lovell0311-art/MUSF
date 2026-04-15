
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
    [GameMasterCommandLine("获取金币")]
    public class GMCommandLine_获取金币 : C_GameMasterCommandLine<Player, IResponse>
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

            mGamePlayer.UpdateCoin(E_GameProperty.GoldCoin, b_Parameter[0], "GM设置");

            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, (int)b_Player.GameAreaId);
            var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get(b_Player.GameAreaId);
            mWriteDataComponent.Save(mGamePlayer.Data, dBProxy2).Coroutine();

            // 金币扣除
            G2C_ChangeValue_notice mChangeValue_notice = new G2C_ChangeValue_notice();
            mChangeValue_notice.GameUserId = mGamePlayer.InstanceId;
            G2C_BattleKVData mBattleKVData = new G2C_BattleKVData();
            mBattleKVData.Key = (int)E_GameProperty.GoldCoin;
            mBattleKVData.Value = mGamePlayer.Data.GoldCoin;
            mChangeValue_notice.Info.Add(mBattleKVData);
            b_Player.Send(mChangeValue_notice);
        }
    }
}