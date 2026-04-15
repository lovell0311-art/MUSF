using System;
using UnityEngine;
using ETModel;
using System.Collections.Generic;
using System.Net.Sockets;

namespace ETHotfix
{
    /// <summar
    /// 全局唯一数据管理
    /// </summary>
    public static class GlobalDataManager
    {
        private const string LocalServerHost = "10.0.2.2";
        private const string LocalRealmAddress = LocalServerHost + ":10002";

        /// <summary>
        /// 服务器地址
        /// </summary>

        
        private static readonly string[] DefaultLoginConnectIPs = new[]
        {
            LocalRealmAddress,
        };

        public static string LoginConnetIP = DefaultLoginConnectIPs[0];//模拟器宿主机别名
        public static string LastRealmAddress = string.Empty;
        /// <summary>
        /// 好易充值
        /// </summary>
        public static void HaoYiTopUp() 
        {
            Log.DebugBrown("");
            Application.OpenURL("http://24118.89pay.icu:6677/buy/?wid=24118");
        }

        /// <summary>q
        /// 重连，登录Gate用的Key
        /// </summary>
        public static string GateLoginKey = "";
        //网关地址
        public static string Address = "";

        public static List<string> GetLoginConnectAddresses()
        {
            List<string> results = new List<string>();
            HashSet<string> seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            AddAddressCandidate(results, seen, LoginConnetIP);
            foreach (string candidate in DefaultLoginConnectIPs)
            {
                AddAddressCandidate(results, seen, candidate);
            }

            return results;
        }

        public static List<string> GetGateConnectAddresses(string gateAddress)
        {
            List<string> results = new List<string>();
            HashSet<string> seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            AddAddressCandidate(results, seen, ReplacePort(LastRealmAddress, gateAddress));
            AddAddressCandidate(results, seen, gateAddress);
            foreach (string loginAddress in DefaultLoginConnectIPs)
            {
                AddAddressCandidate(results, seen, ReplacePort(loginAddress, gateAddress));
            }

            return results;
        }

        public static bool ShouldRetryWithAlternativeAddress(Exception exception)
        {
            Exception current = exception;
            while (current != null)
            {
                if (current is RpcException rpcException)
                {
                    return IsNetworkAddressError(rpcException.Error);
                }

                if (current is SocketException || current is TimeoutException)
                {
                    return true;
                }

                string message = current.Message ?? string.Empty;
                if (message.IndexOf("TimeoutException", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    message.IndexOf("timeout", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    message.IndexOf(ErrorCode.ERR_PeerDisconnect.ToString(), StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return true;
                }

                current = current.InnerException;
            }

            return false;
        }

        public static bool IsNetworkAddressError(int errorCode)
        {
            return errorCode == ErrorCode.ERR_PeerDisconnect || (errorCode != 0 && Enum.IsDefined(typeof(SocketError), errorCode));
        }

        public static bool IsLocalServerAddress(string address)
        {
            return TrySplitAddress(address, out string host, out _) &&
                   string.Equals(host, LocalServerHost, StringComparison.OrdinalIgnoreCase);
        }

        private static void AddAddressCandidate(List<string> results, HashSet<string> seen, string address)
        {
            string normalized = NormalizeAddress(address);
            if (string.IsNullOrEmpty(normalized))
            {
                return;
            }

            if (seen.Add(normalized))
            {
                results.Add(normalized);
            }
        }

        private static string ReplacePort(string baseAddress, string templateAddress)
        {
            if (!TrySplitAddress(baseAddress, out string host, out _))
            {
                return NormalizeAddress(templateAddress);
            }

            if (!TrySplitAddress(templateAddress, out _, out int port))
            {
                return NormalizeAddress(templateAddress);
            }

            return $"{host}:{port}";
        }

        private static bool TrySplitAddress(string address, out string host, out int port)
        {
            host = string.Empty;
            port = 0;

            string normalized = NormalizeAddress(address);
            if (string.IsNullOrEmpty(normalized))
            {
                return false;
            }

            int separatorIndex = normalized.LastIndexOf(':');
            if (separatorIndex <= 0 || separatorIndex >= normalized.Length - 1)
            {
                return false;
            }

            host = normalized.Substring(0, separatorIndex).Trim();
            return !string.IsNullOrEmpty(host) && int.TryParse(normalized.Substring(separatorIndex + 1), out port);
        }

        private static string NormalizeAddress(string address)
        {
            return string.IsNullOrWhiteSpace(address) ? string.Empty : address.Trim();
        }

        //是否开启重连
        public static bool IsStartReConnect = false;
        //是否被顶号
        public static bool IsOFFLINE = false;
        //切换账号
        public static bool IsLoginOut=false;
        /// <summary>
        /// XYSDK 账号uuid
        /// </summary>
        public static int XYUUID=0;

        public static string ShouQUUID;

        /// <summary>
        /// 七天充值信息
        /// </summary>
        public static Dictionary<int,bool> SevenDaysToRechargeDic=new Dictionary<int, bool>();

        /// <summary>
        /// 七天充值信息2
        /// </summary>
        public static Dictionary<int,string> SevenDaysToRechargeDic2=new Dictionary<int, string>();

        public static AstarNode astarNode;
        public static int MapId;

        /// <summary>
        /// 服务器与本地时间 差值
        /// </summary>
        public static long ServerTime;
       

        /// <summary>
        /// 攻击间隔时间
        /// </summary>
        public static long AttackSpaceTime;
        /// <summary>
        /// 切换角色中
        /// </summary>
        public static bool ChangeSceneIsChooseRole = false;
        /// <summary>
        /// 角色进入游戏中。用于屏蔽进图阶段不安全的附加请求。
        /// </summary>
        public static bool IsEnteringGame = false;
        /// <summary>
        /// 本次进图的 LoadingComplete 初始化是否已经处理过。
        /// </summary>
        public static bool EnterGameLoadingCompleteProcessed = false;
        /// <summary>
        /// 本次进图的本地 MovePos 是否已经处理过。
        /// </summary>
        public static bool EnterGameMovePosProcessed = false;
        /// <summary>
        /// 是否已经挂起了基于 LoadingComplete 的进图恢复。
        /// </summary>
        public static bool EnterGameLoadingRecoveryPending = false;
        /// <summary>
        /// 是否隐藏周围玩家
        /// </summary>
        public static bool IsHideRole = true;

#region 服务器列表
        // 切换路线
        public static bool isChangeLine = false;

        /// <summary>
        /// 选择的大区ID
        /// </summary>
        public static int EnterZoneID = 0;
        public static string EnterZoneName;
        /// <summary>
        /// 线路id
        /// </summary>
        public static int EnterLineID = 0;
#endregion
        /// <summary>
        /// 当前战斗模式:
        ///PK 模式选择
        ///0:和平->PVE模式 开启后不能攻击除红名以外的任何人、可以被开启PVP模式的其他人攻击
        ///1：全体->PVP模式 开启后能无差别攻击任何模式下的任何玩家 可被开启PVP模式的其他人攻击
        ///2：友方 ->PVP模式 开启后不能攻击组队、好友、战盟中的玩家、可攻击除这三者以外饿任何玩家、可被开启PVP模式的其他人攻击
        ///注意:目前恶魔广场、血色城堡副本地图两张图中不可开启PVP模式 （即全体、友方两种模式）
        /// </summary>
        public static E_BattleType BattleModel = E_BattleType.Peace;
        public static bool IsBeatBack = false;//是否可以反击
        public static float BeatBackTimer;
        public static long curBeatUUID;//当前PK我的人
        public static int Battlelev = 50;//PK等级


        public static void GCClear() 
        {
           Resources.UnloadUnusedAssets();//用于释放所有没有引用的Asset对象
           GC.Collect();
            
        }


        /// <summary>
        /// 获取字符串数据，存入字典
        /// </summary>
        /// <param name="str"></param>
        public static Dictionary<int,int> GetDictionary(string str)
        {


          //  Log.DebugBrown("要解的数据" + str);
            // 创建字典
            Dictionary<int, int> myDictionary = new Dictionary<int, int>();
            // 去掉花括号
            str = str.Trim('{', '}');

            // 按逗号分割成多个键值对
            string[] pairs = str.Split(',');
            // 遍历每个键值对
            foreach (string pair in pairs)
            {
                // 按冒号分割成键和值
                string[] keyValue = pair.Split(':');

                // 将键和值转换为整数
                int key = int.Parse(keyValue[0]);
                int value = int.Parse(keyValue[1]);

                // 将键值对存入字典
                myDictionary[key] = value;
            }

            return myDictionary;
        }



        public static Dictionary<string, int> ParseStringToDictionary(string input)
        {
            Dictionary<string, int> dictionary = new Dictionary<string, int>();

            Log.DebugBrown("111111111|||");
            Log.DebugBrown("2222222222|||"+input);
            // 去掉花括号
            input = input.Trim('{', '}');

            // 按逗号分割成多个键值对
            string[] pairs = input.Split(',');
            // 遍历每个键值对
            foreach (string pair in pairs)
            {
                Log.DebugBrown("ddd" + pair);
                // 按冒号分割成键和值
                //string[] keyValue = pair.Split(':');

                //// 将键和值转换为整数
                //string key = keyValue[0];
                //int value = int.Parse(keyValue[1]);

                //// 将键值对存入字典
                //dictionary[key] = value;
            }

            foreach (var item in dictionary)
            {
                Log.DebugBrown("key" + item.Key + ":::" + item.Value);
            }
            //// Split the input string by the colon to separate the key and value
            //var parts = input.Split(':');

            //if (parts.Length == 2)
            //{
            //    // Trim any leading or trailing whitespace
            //    string key = parts[0].Trim().Trim('"'); // Remove surrounding quotes
            //    string valueStr = parts[1].Trim();

            //    // Try to parse the value as an integer
            //    if (int.TryParse(valueStr, out int value))
            //    {
            //        dictionary[key] = value;
            //    }
            //    else
            //    {
            //        // Handle the case where the value is not a valid integer
            //        Console.WriteLine($"Invalid value for key '{key}': {valueStr}");
            //    }
            //}
            //else
            //{
            //    // Handle the case where the input string is not in the expected format
            //    Console.WriteLine($"Invalid input format: {input}");
            //}

            return dictionary;
        }
    }
}

