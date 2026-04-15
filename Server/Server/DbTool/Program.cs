using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CustomFrameWork.Component;
using ETModel;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DbTool
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Dictionary<int, Dictionary<string, bool>> mDBDataStringDic = new Dictionary<int, Dictionary<string, bool>>();

            MongoClient mMongoClient = new MongoClient($"mongodb://127.0.0.1:58030");
            //MongoClient mMongoClient = new MongoClient($"mongodb://localhost:27017");

            Dictionary<string, Type[]> typeDics = new Dictionary<string, Type[]>()
            {
                ["ET"] = new Type[] {
                    typeof(DBAccountInfo),
                    typeof(DBServiceRegistryInfo),
                    typeof(DBGameAreaData),
                },
                ["ET1"] = new Type[] {
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
                ["ETLog"] = new Type[] {
                    typeof(DBTradeLog),
                },
            };

            async void Run()
            {
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
            Run();

            Console.WriteLine("hello word");
            Console.ReadKey();
        }
    }
}
