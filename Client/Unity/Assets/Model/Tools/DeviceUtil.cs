using System.Net.NetworkInformation;
using System.Net.Sockets;
using UnityEngine;
#if UNITY_IPHONE
using UnityEngine.iOS;
#endif
namespace ETModel
{
    public static class DeviceUtil
    {
        /// <summary>
        /// 获取设备唯一标识符
        /// </summary>
        public static string DeviceIdentifier
        {
            get
            {
                return SystemInfo.deviceUniqueIdentifier;

            }
        }

        /// <summary>
        /// 获取设备型号
        /// </summary>
        public static string DeviceModel
        {
            get
            {
#if UNITY_IPHONE
            return Device.generation.ToString();;
#else
                return SystemInfo.deviceModel;
#endif
            }
        }
        /// <summary>
        /// 获取系统类型
        /// </summary>
        public static string DeviceSystemType
        {
            get 
            {
                return SystemInfo.operatingSystem;
            }
        }
        /// <summary>
        /// 获取系统类型
        /// </summary>
        public static int DeviceVersionType
        {
            get
            {
#if UNITY_STANDALONE_WIN
                return (int)DeviceType.PC;

#elif UNITY_ANDROID
       return (int)DeviceType.ANDROID;
#else 
return (int)DeviceType.IOS;
#endif
            }
        }
        public enum DeviceType
        {
            Unknown = 0,
            IOS = 1, //苹果
            ANDROID = 2, //安卓
            PC = 3 //PC
        }
        /// <summary>
        /// 获取IP
        /// </summary>
        /// <returns></returns>
        public static string GetIP()
        {
            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adater in adapters)
            {
                if (adater.Supports(NetworkInterfaceComponent.IPv4))
                {
                    UnicastIPAddressInformationCollection UniCast = adater.GetIPProperties().UnicastAddresses;
                    if (UniCast.Count > 0)
                    {
                        foreach (UnicastIPAddressInformation uni in UniCast)
                        {
                            if (uni.Address.AddressFamily == AddressFamily.InterNetwork)
                            {
                                return uni.Address.ToString();
                            }
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 网络可达性
        /// </summary> 
        /// <returns></returns>
        public static bool IsNetworkReachability()
        {
            switch (Application.internetReachability)
            {
                case NetworkReachability.ReachableViaLocalAreaNetwork:
                    //Log.DebugRed("当前使用的是：WiFi，请放心更新！");
                    return true;
                case NetworkReachability.ReachableViaCarrierDataNetwork:
                   // Log.DebugRed("当前使用的是移动网络，是否继续更新？");
                    return true;
                default:
                    //Log.DebugRed("当前没有联网，请您先联网后再进行操作！");
                    return false;
            }
        }
    }
}
