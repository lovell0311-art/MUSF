using Aop.Api.Domain;
using CustomFrameWork;
using CustomFrameWork.Baseic;
using CustomFrameWork.Component;
using ETModel;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Servers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TencentCloud.Mongodb.V20190725.Models;

namespace ETHotfix
{
    [Timer(TimerType.AutoAreaTimers)]
    public class AutoAreaNoticeTimer : ATimer<AutoAreaComponent>
    {
        public override void Run(AutoAreaComponent self)
        {

            var Dic = Root.MainFactory.AddCustomComponent<ReadConfigComponent>().GetJson<Auto_AreaConfigJson>().JsonDic;
            DBProxyManagerComponent mDBProxyManagerComponent = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            var dBProxy = mDBProxyManagerComponent.GetZoneDB(DBType.Core, DBProxyComponent.CommonDBId);
            foreach (var kvp in Dic)
            {
                if (!string.IsNullOrEmpty(kvp.Value.MergeTime))
                {
                    DateTime dateTime = DateTime.Parse(kvp.Value.MergeTime);
                    DateTimeOffset dateTimeOffset = new DateTimeOffset(dateTime);
                    // 获取Unix时间戳并转换为秒
                    long CnuntS = dateTimeOffset.ToUnixTimeSeconds();
                    long time = Help_TimeHelper.GetNowSecond();
                    if (CnuntS < time) continue;
                    else if (CnuntS == time)
                    {
                        if (self.keyValuePairs.ContainsKey(kvp.Key))
                            self.keyValuePairs.Remove(kvp.Key);
                        self.SendMsgAuxiliaryProgram(kvp.Key, kvp.Value.MergeArea,kvp.Value.DBAddreas).Coroutine();
                        DBCoincidentdata dBCoincidentdata = new DBCoincidentdata();
                        dBCoincidentdata.Id = IdGeneraterNew.Instance.GenerateUnitId(DBProxyComponent.CommonDBId);
                        dBCoincidentdata.OldAreaId = kvp.Key;
                        dBCoincidentdata.NewAreaId = kvp.Value.MergeArea;
                        dBProxy.Save(dBCoincidentdata).Coroutine();
                        Console.WriteLine($"通知合区");
                        continue;
                    }
                }
                if (!string.IsNullOrEmpty(kvp.Value.OpeningTime))
                {
                    DateTime dateTime = DateTime.Parse(kvp.Value.OpeningTime);
                    DateTimeOffset dateTimeOffset = new DateTimeOffset(dateTime);
                    // 获取Unix时间戳并转换为毫秒
                    long CnuntS = dateTimeOffset.ToUnixTimeSeconds();
                    long time = Help_TimeHelper.GetNowSecond();
                    if (CnuntS - kvp.Value.PreparationTime <= time)
                    {
                        if (!self.keyValuePairs.ContainsKey(kvp.Key))
                        {
                            Dictionary<(int, int), (int, string)> AreaInfoList = new Dictionary<(int, int), (int, string)>();
                            if (!string.IsNullOrEmpty(kvp.Value.LinesParameter))
                            {
                                AreaInfoList = kvp.Value.LinesParameter.Split(';').Select(P => P.Split(',')).ToDictionary(
                                P => (int.Parse(P[0]), int.Parse(P[1])),
                                P => (int.Parse(P[2]), P[3]));
                            }
                            else
                                continue;

                            foreach (var AreaInfo in AreaInfoList)
                            {
                                //处理数据库
                                DBGameAreaData dBGameAreaData = new DBGameAreaData()
                                {
                                    Id = IdGeneraterNew.Instance.GenerateUnitId(DBProxyComponent.CommonDBId),
                                    GameAreaId = kvp.Key,
                                    RealLine = AreaInfo.Key.Item2,
                                    NickName = AreaInfo.Value.Item2,
                                    CreateTime = 0,
                                    State = 1
                                };
                                dBProxy.Save(dBGameAreaData).Coroutine();
                            }
                            foreach (var AreaInfo in AreaInfoList)
                            {
                                //处理数据库
                                DBServiceRegistryInfo dBServiceRegistryInfo = new DBServiceRegistryInfo()
                                {
                                    Id = IdGeneraterNew.Instance.GenerateUnitId(DBProxyComponent.CommonDBId),
                                    PlayerCount = $"{{\"{AreaInfo.Value.Item1}\":0}}",
                                    GameServerId = AreaInfo.Key.Item1,
                                    GameAreaIds = $"{{\"{AreaInfo.Value.Item1}\":[{AreaInfo.Value.Item1}]}}",
                                    UpdateTime = DateTime.Now.Ticks,
                                    UpdateTime2 = DateTime.UtcNow
                                };
                                dBProxy.Save(dBServiceRegistryInfo).Coroutine();
                            }
                            AutoInfo autoInfo = new AutoInfo();
                            autoInfo.Id = kvp.Value.Id;
                            autoInfo.Name = kvp.Value.AreaName;
                            autoInfo.State = 2;
                            self.keyValuePairs.Add(kvp.Value.Id, autoInfo);
                            self.SendMsgAuxiliaryProgram(kvp.Key).Coroutine();
                            Console.WriteLine($"通知开区");
                            self.CareatDB(kvp.Value.Id, kvp.Value.DBAddreas).Coroutine();
                        }
                        else
                        {
                            if (CnuntS >= time)
                            {
                                if (self.keyValuePairs[kvp.Key].State == 2)
                                    self.keyValuePairs[kvp.Key].State = 1;
                            }

                        }
                    }
                }
            }
        }
    }
    [ObjectSystem]
    public class AutoAreaComponentDestroySystem : DestroySystem<AutoAreaComponent>
    {
        public override void Destroy(AutoAreaComponent self)
        {
            ETModel.ET.TimerComponent.Instance.Remove(ref self._timerId);
        }
    }
    [EventMethod(typeof(AutoAreaComponent), EventSystemType.INIT)]
    public class AutoAreaComponentEventOnInit : ITEventMethodOnInit<AutoAreaComponent>
    {
        public void OnInit(AutoAreaComponent b_Component)
        {
            b_Component.Load();
            b_Component._timerId = ETModel.ET.TimerComponent.Instance.NewRepeatedTimer(1000, TimerType.AutoAreaTimers, b_Component);
        }
    }
    public static class AutoAreaComponentSystem
    {
        public static void Load(this AutoAreaComponent self)
        {
            self.keyValuePairs = new Dictionary<int, AutoInfo>();
            Console.WriteLine("自动开区1");
            var Dic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<Auto_AreaConfigJson>().JsonDic;
            foreach (var kvp in Dic) 
            {
                if (!string.IsNullOrEmpty(kvp.Value.MergeTime))
                {
                    DateTime dateTime = DateTime.Parse(kvp.Value.MergeTime);
                    DateTimeOffset dateTimeOffset = new DateTimeOffset(dateTime);
                    // 获取Unix时间戳并转换为秒
                    long CnuntS = dateTimeOffset.ToUnixTimeSeconds();
                    long time = Help_TimeHelper.GetNowSecond();
                    if (CnuntS < time)
                    {
                        Console.WriteLine($"合并过的区服不加载：{kvp.Key}");
                        continue;
                    }
                }
                if (!string.IsNullOrEmpty(kvp.Value.OpeningTime))
                {
                    DateTime dateTime = DateTime.Parse(kvp.Value.OpeningTime);
                    DateTimeOffset dateTimeOffset = new DateTimeOffset(dateTime);
                    // 获取Unix时间戳并转换为毫秒
                    long unixTimestampMilliseconds = dateTimeOffset.ToUnixTimeMilliseconds();
                    if (unixTimestampMilliseconds <= Help_TimeHelper.GetNow())
                    {
                        AutoInfo autoInfo = new AutoInfo();
                        autoInfo.Id = kvp.Value.Id;
                        autoInfo.Name = kvp.Value.AreaName;
                        autoInfo.State = 1;
                        Console.WriteLine($"区服加载：{kvp.Key}");
                        self.keyValuePairs.Add(autoInfo.Id, autoInfo);
                    }
                }
            }
            Console.WriteLine("自动开区2");
        }
        /// <summary>
        /// 通知辅助程序运行bat文件 
        /// Id对应自动合区开区里面的Id辅助程序也会读取
        /// 辅助程序地址和端口是在程序中写死的，连接后发送一条命令就断开
        /// </summary>
        /// <returns></returns>
        public static async Task SendMsgAuxiliaryProgram(this AutoAreaComponent self,int Id)
        {
            try
            {
                //等待10秒
                await Task.Delay(10000);
                Console.WriteLine($"通知开区区{Id}");
                string MsgStr = $"AreaId={Id}";
                byte[] messageType = Encoding.UTF8.GetBytes("04");
                byte[] IdLength = BitConverter.GetBytes(0);
                var heartbeatMessage = Encoding.UTF8.GetBytes(MsgStr);
                int totalLength = heartbeatMessage.Length;
                byte[] Msglen = BitConverter.GetBytes(totalLength);
                byte[] buffr = new byte[1024];
                Array.Copy(messageType, 0, buffr, 0, messageType.Length);
                Array.Copy(IdLength, 0, buffr, messageType.Length, IdLength.Length);
                Array.Copy(Msglen, 0, buffr, messageType.Length + IdLength.Length, Msglen.Length);
                Array.Copy(heartbeatMessage, 0, buffr, messageType.Length + IdLength.Length + Msglen.Length, heartbeatMessage.Length);
                TcpClient tcpClient = new TcpClient();
                await tcpClient.ConnectAsync("127.0.0.1", 49001);
                NetworkStream _stream = tcpClient.GetStream();
                _stream.Write(buffr, 0, buffr.Length);
                tcpClient.Close();
            }
            catch (SocketException ex)
            {
                Console.WriteLine("无法连接到服务器: " + ex.Message);
                // 进行错误处理，例如重试连接或退出程序
            }
        }
        /// <summary>
        /// 通知辅助程序运行合区AreaIdA合并到AreaIdB
        /// Id对应自动合区开区里面的Id辅助程序也会读取
        /// 辅助程序地址和端口是在程序中写死的，连接后发送一条命令就断开
        /// </summary>
        /// <returns></returns>
        public static async Task SendMsgAuxiliaryProgram(this AutoAreaComponent self,int AreaIdA,int AreaIdB,string Addreas)
        {
            try 
            { 
                //等待10秒
                await Task.Delay(100000);
                Console.WriteLine($"通知合区{AreaIdA}=>{AreaIdB}");
                string MsgStr = $"OldAreaId={AreaIdA}&NewAreaId={AreaIdB}&AddReas={Addreas}";
                byte[] messageType = Encoding.UTF8.GetBytes("05");
                byte[] IdLength = BitConverter.GetBytes(0);
                var heartbeatMessage = Encoding.UTF8.GetBytes(MsgStr);
                int totalLength = heartbeatMessage.Length;
                byte[] Msglen = BitConverter.GetBytes(totalLength);
                byte[] buffr = new byte[1024];
                Array.Copy(messageType, 0, buffr, 0, messageType.Length);
                Array.Copy(IdLength, 0, buffr, messageType.Length, IdLength.Length);
                Array.Copy(Msglen, 0, buffr, messageType.Length + IdLength.Length, Msglen.Length);
                Array.Copy(heartbeatMessage, 0, buffr, messageType.Length + IdLength.Length + Msglen.Length, heartbeatMessage.Length);
                TcpClient tcpClient = new TcpClient();
                await tcpClient.ConnectAsync("127.0.0.1", 49001);
                NetworkStream _stream = tcpClient.GetStream();
                _stream.Write(buffr, 0, buffr.Length);
                tcpClient.Close();
            }
            catch (SocketException ex)
            {   
                Console.WriteLine("无法连接到服务器: " + ex.Message);
                // 进行错误处理，例如重试连接或退出程序
            }
        }
        public static async Task CareatDB(this AutoAreaComponent self,int Id,string DBAddreas)
        {
            Dictionary<int, Dictionary<string, bool>> mDBDataStringDic = new Dictionary<int, Dictionary<string, bool>>();

            MongoClient mMongoClient = new MongoClient($"mongodb://{DBAddreas}");
            //MongoClient mMongoClient = new MongoClient($"mongodb://localhost:27017");
            string DBName = $"ET{Id}";
            Dictionary<string, Type[]> typeDics = new Dictionary<string, Type[]>()
            {
                [DBName] = new Type[] {
                    typeof(DBAccountZoneData),
                    typeof(DBBattleCopyData),
                    typeof(DBFriendData),
                    typeof(DBGamePlayerData),
                    typeof(DBGameSkillData),
                    typeof(DBItemData),
                    typeof(DBMasterData),
                    typeof(DBMiracleActivities),
                    typeof(DBMailData),
                    typeof(DBPetsData),
                    typeof(DBPlayerShopMall),
                    typeof(DBPlayerUnitData),
                    typeof(DBStallItem),
                    typeof(DBWarehouseItem),
                    typeof(DBWarAllianceData),
                    typeof(DBRCodeData),
                    typeof(DBRCodeLogData),
                    typeof(DBRCodeErrorData),
                    typeof(DBTHRecord),
                    typeof(THItemInfo),

                },
            };
                var mMongoNamelist = typeDics.Keys.ToArray();
                for (int i = 0; i < mMongoNamelist.Length; i++)
                {
                    var mMongoName = mMongoNamelist[i];
                    var mMongoTypelist = typeDics[mMongoName];

                    IMongoDatabase mMongoDatabase = mMongoClient.GetDatabase(mMongoName);

                    for (int h = 0; h < mMongoTypelist.Length; h++)
                    {
                        Type type = mMongoTypelist[h];

                        if (mDBDataStringDic.Count > 0) mDBDataStringDic.Clear();

                        //await mMongoDatabase.CreateCollectionAsync(type.Name);

                        IMongoCollection<BsonDocument> mMongoCollection = mMongoDatabase.GetCollection<BsonDocument>(type.Name);

                        PropertyInfo[] mPropertyInfoArray = type.GetProperties();
                        for (int j = 0, jlen = mPropertyInfoArray.Length; j < jlen; j++)
                        {
                            PropertyInfo mCurrentPropertyInfo = mPropertyInfoArray[j];
                            DBMongodbAttribute[] mPropertyInfoAttributeArray = mCurrentPropertyInfo.GetCustomAttributes<DBMongodbAttribute>().ToArray();

                            for (int z = 0, zlen = mPropertyInfoAttributeArray.Length; z < zlen; z++)
                            {
                                DBMongodbAttribute mPropertyInfoAttribute = mPropertyInfoAttributeArray[z];
                                if (mPropertyInfoAttribute != null)
                                {
                                    if (mDBDataStringDic.TryGetValue(mPropertyInfoAttribute.GroupID, out Dictionary<string, bool> mTempDic) == false)
                                    {
                                        mDBDataStringDic[mPropertyInfoAttribute.GroupID] = mTempDic = new Dictionary<string, bool>();
                                    }

                                    mTempDic[mCurrentPropertyInfo.Name] = mPropertyInfoAttribute.SortType;
                                }
                            }
                        }

                        FieldInfo[] mFieldInfoArray = type.GetFields();
                        for (int j = 0, jlen = mFieldInfoArray.Length; j < jlen; j++)
                        {
                            FieldInfo mCurrentFieldInfo = mFieldInfoArray[j];
                            DBMongodbAttribute[] mFieldInfoAttributeArray = mCurrentFieldInfo.GetCustomAttributes<DBMongodbAttribute>().ToArray();

                            for (int z = 0, zlen = mFieldInfoAttributeArray.Length; z < zlen; z++)
                            {
                                DBMongodbAttribute mFieldInfoAttribute = mFieldInfoAttributeArray[z];
                                if (mFieldInfoAttribute != null)
                                {
                                    if (mDBDataStringDic.TryGetValue(mFieldInfoAttribute.GroupID, out Dictionary<string, bool> mTempDic) == false)
                                    {
                                        mDBDataStringDic[mFieldInfoAttribute.GroupID] = mTempDic = new Dictionary<string, bool>();
                                    }

                                    mTempDic[mCurrentFieldInfo.Name] = mFieldInfoAttribute.SortType;
                                }
                            }
                        }

                        if (mDBDataStringDic.Count > 0)
                        {
                            int[] mGroupIdKeys = mDBDataStringDic.Keys.ToArray();

                            for (int j = 0, jlen = mGroupIdKeys.Length; j < jlen; j++)
                            {
                                int mGroupId = mGroupIdKeys[j];
                                Dictionary<string, bool> mValue = mDBDataStringDic[mGroupId];

                                List<string> mIndexNameKeys = mValue.Keys.ToList();

                                IndexKeysDefinition<BsonDocument> mIndexKey;
                                CreateIndexOptions mIndexOptions;
                                if (mIndexNameKeys.Count > 1)
                                {
                                    List<IndexKeysDefinition<BsonDocument>> mIndexs = new List<IndexKeysDefinition<BsonDocument>>();
                                    for (int x = 0, xlen = mIndexNameKeys.Count; x < xlen; x++)
                                    {
                                        string mIndexName = mIndexNameKeys[x];

                                        IndexKeysDefinition<BsonDocument> mIndexKeyTemp;
                                        if (mValue[mIndexName])
                                        {
                                            mIndexKeyTemp = Builders<BsonDocument>.IndexKeys.Ascending(mIndexName);
                                        }
                                        else
                                        {
                                            mIndexKeyTemp = Builders<BsonDocument>.IndexKeys.Descending(mIndexName);
                                        }

                                        mIndexs.Add(mIndexKeyTemp);
                                    }
                                    mIndexKey = Builders<BsonDocument>.IndexKeys.Combine(mIndexs);
                                    mIndexOptions = new CreateIndexOptions() { Name = string.Join('_', mIndexNameKeys), Background = true, Sparse = true };
                                }
                                else
                                {
                                    string mIndexName = mIndexNameKeys[0];

                                    if (mValue[mIndexName])
                                    {
                                        mIndexKey = Builders<BsonDocument>.IndexKeys.Ascending(mIndexName);
                                    }
                                    else
                                    {
                                        mIndexKey = Builders<BsonDocument>.IndexKeys.Descending(mIndexName);
                                    }
                                    mIndexOptions = new CreateIndexOptions() { Name = mIndexName, Background = true, Sparse = true };
                                }

                                CreateIndexModel<BsonDocument> mCreateIndexModel = new CreateIndexModel<BsonDocument>(mIndexKey, mIndexOptions);
                                string mIndexResult = await mMongoCollection.Indexes.CreateOneAsync(mCreateIndexModel);

                                Console.WriteLine($"库:{mMongoName} 表:{type.Name} 插入索引组Id:{mGroupId} 结果{mIndexResult}!");
                                if (mIndexResult != mIndexOptions.Name)
                                {
                                    Console.WriteLine($"库:{mMongoName} 表:{type.Name} 插入索引组Id:{mGroupId}失败!");
                                }
                            }
                        }
                    }
                }
                Console.WriteLine("over");

        }
    }
}
