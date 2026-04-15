using CustomFrameWork.Baseic;
using CustomFrameWork;
using ETModel;
using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;
using CustomFrameWork.Component;
using System.Linq;

namespace ETHotfix
{
    [Timer(TimerType.HyPayUpdate)]
    public class HyPayUpdateTimer : ATimer<MySqlComponent>
    {
        public override void Run(MySqlComponent self)
        {
            self.UpdateHyPay().Coroutine();
        }
    }


    [EventMethod(typeof(MySqlComponent), EventSystemType.INIT)]
    public class MySqlComponentEventOnInit : ITEventMethodOnInit<MySqlComponent>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="b_Component"></param>
        public void OnInit(MySqlComponent self)
        {
            self.TimerId = ETModel.ET.TimerComponent.Instance.NewRepeatedTimer(5000, TimerType.HyPayUpdate, self);
        }
    }


    [EventMethod(typeof(MySqlComponent), EventSystemType.DISPOSE)]
    public class MySqlComponentEventOnDispose : ITEventMethodOnDispose<MySqlComponent>
    {
        public override void OnDispose(MySqlComponent self)
        {
            ETModel.ET.TimerComponent.Instance.Remove(ref self.TimerId);
        }
    }

    public static partial class MySqlComponentSystem
    {
        public const string ConnectionString = "server=localhost;port=33091;database=MuOnline;";

        public static async Task UpdateHyPay(this MySqlComponent self)
        {
            if (self.State == true) return;    // 正在操作
            self.State = true;

            try
            {
                await self.ReadDB();
                await self.SetDB();
            }
            finally
            {
                self.State = false;
            }
        }

        public static async Task ReadDB(this MySqlComponent self)
        {
            using MySqlConnection conn = new MySqlConnection(ConnectionString);
            await conn.OpenAsync();
            using MySqlCommand command = new MySqlCommand("SELECT * FROM HyPayInfo where PayStatus = 0 ", conn);
            using MySqlDataReader reader = await command.ExecuteReaderAsync(CommandBehavior.Default);

            while (reader.Read())
            {
                MySqlStructure mySqlStructure = new MySqlStructure();
                mySqlStructure.Payid = (int)reader[0];
                mySqlStructure.Region = (int)reader[1];
                mySqlStructure.AccountId = (string)reader[2];
                // 充值金额单位为分
                mySqlStructure.Money = ((int)reader[3]) ;
                mySqlStructure.Money_RMB = ((int)reader[4]) ;
                mySqlStructure.PayStatus = (int)reader[5];
                mySqlStructure.CreateTime = (DateTime)reader[6];


                self.mySqlStructures.Enqueue(mySqlStructure);
            }
        }
        public static async Task SetDB(this MySqlComponent self)
        {
            while(self.mySqlStructures.Count != 0)
            {
                MySqlStructure PayInfo = self.mySqlStructures.Dequeue();
                var mDBProxy = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>().GetZoneDB(DBType.Core, DBProxyComponent.CommonDBId);
                DBAccountInfo dbLoginInfo = null;
                if (mDBProxy != null)
                {
                    var list = await mDBProxy.Query<DBAccountInfo>(p => p.Phone == PayInfo.AccountId);
                    if (list.Count > 0)
                    {
                        dbLoginInfo = list[0] as DBAccountInfo;
                    }
                    else
                    {
                        using MySqlConnection conn = new MySqlConnection(ConnectionString);
                        await conn.OpenAsync();
                        using (var command = new MySqlCommand("update HyPayInfo SET PayStatus = @param1 WHERE Payid = @Payid and AccountId = @Id", conn))
                        {
                            // 添加参数
                            command.Parameters.AddWithValue("@param1", 2);
                            command.Parameters.AddWithValue("@Payid", PayInfo.Payid);
                            command.Parameters.AddWithValue("@Id", PayInfo.AccountId);
                            await command.ExecuteNonQueryAsync();
                            Log.PLog("ThreePay", $"未检出账号存在AccountId:{PayInfo.AccountId}");
                            continue;
                        }
                    }
                }
                if (dbLoginInfo != null)
                {
                    LoginCenter2S_GetLoginRecord l2sGetLoginRecord = null;
                    try
                    {
                        var loginCenterList = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfos(AppType.LoginCenter);
                        Session loginCenterSession = Game.Scene.GetComponent<NetInnerComponent>().Get(loginCenterList[0].ServerInnerIP);
                        IResponse response = await loginCenterSession.Call(new S2LoginCenter_GetLoginRecord()
                        {
                            UserId = dbLoginInfo.Id
                        });
                        l2sGetLoginRecord = response as LoginCenter2S_GetLoginRecord;
                        if (l2sGetLoginRecord == null) continue;
                        if (l2sGetLoginRecord.GameUserId != 0)
                        {
                            // 锁住
                            using CoroutineLock coLock = await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, l2sGetLoginRecord.GameUserId);

                            Player mPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(PayInfo.Region, l2sGetLoginRecord.GameUserId);
                            if (mPlayer == null) continue;

                            var PlayerShop = mPlayer.GetCustomComponent<PlayerShopMallComponent>();
                            if (PlayerShop == null) continue;

                            if (await PlayerShop.ThreePayInfo(PayInfo.Money_RMB, dbLoginInfo))
                            {
                                Log.PLog("ThreePay", $"充值成功 AccountID;{dbLoginInfo.Phone} Money:{PayInfo.Money_RMB} PayId:{PayInfo.Payid}");

                                using MySqlConnection conn = new MySqlConnection(ConnectionString);
                                await conn.OpenAsync();
                                using (var command = new MySqlCommand("update HyPayInfo SET PayStatus = @param1 WHERE Payid = @Payid and AccountId = @Id", conn))
                                {
                                    // 添加参数
                                    command.Parameters.AddWithValue("@param1", 1);
                                    command.Parameters.AddWithValue("@Payid", PayInfo.Payid);
                                    command.Parameters.AddWithValue("@Id", PayInfo.AccountId);
                                    await command.ExecuteNonQueryAsync();
                                }
                            }
                        }

                    }
                    catch (Exception e)
                    {
                        Log.Error(e.ToString());
                    }
                }
            }
        }
    }

}
