using System;//Math
using UnityEngine;
using System.Collections;

public class ShowStats : MonoBehaviour
{
    public int MaxFrameRate = 1000;
    public float TimmingPeriod = 5.0f;//多长时间一个计时周期，单位：秒
    public Color TextColor = Color.red;

    float AccTime = 0.0f;//单位：秒
    float FrameCount = 0.0f;
    float SmoothFPS = 0.0f;

    void Update()
    {
        Application.targetFrameRate = MaxFrameRate;
        //deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
    }
    
    void OnGUI()
    {
        int w = Screen.width, h = Screen.height;

        GUIStyle style = new GUIStyle();

        Rect rect = new Rect(60, 100, w, h * 2 / 100);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 2 / 100;
        //new Color (0.0f, 0.0f, 0.5f, 1.0f);
        style.normal.textColor = TextColor;
        //float msec = deltaTime * 1000.0f;
        //float SmoothFPS = 1.0f / Mathf.Max(Time.smoothDeltaTime, 0.00001f);

        FrameCount += 1.0f;
        AccTime += Mathf.Clamp(Time.unscaledDeltaTime, 0.00001f, 100.0f);
        if(AccTime > TimmingPeriod)
        {
            SmoothFPS = FrameCount / AccTime;

            FrameCount = 0.0f;
            AccTime -= TimmingPeriod;
        }

        string text = "CrazyEngine Rocks(DavidBillLi@qq.com).\n";// = "Device Name: " + SystemInfo.deviceName + "\n";
#if UNITY_IOS
        string PhoneName = GetIPhoneType(SystemInfo.deviceModel);
        if (0 != PhoneName.Length)
            text += "Phone Name: " + PhoneName + "\n";
        else
            text += "Device Model: " + SystemInfo.deviceModel + "\n";
#else
        text += "Device Model: " + SystemInfo.deviceModel + "\n";
#endif

        text += "Graphics Device Name: " + SystemInfo.graphicsDeviceName + "\n";
        text += string.Format("Screen resolution: {0:0}, {1:0}\n", Screen.width, Screen.height);
        text += string.Format("Average FPS: {0:0.0}; Realtime delta time: {1:0.0} ms\n",
            SmoothFPS, Time.smoothDeltaTime * 1000.0f);
        GUI.Label(rect, text, style);
    }
    

    string GetIPhoneType(string DeviceModel)
    {
        string Type;
        if ("iPhone3,1" == DeviceModel)
            Type = "iPhone 4";
        else if ("iPhone3,2" == DeviceModel)
            Type = "iPhone 4";
        else if ("iPhone3,3" == DeviceModel)
            Type = "iPhone 4";
        else if ("iPhone4,1" == DeviceModel)
            Type = "iPhone 4S";
        else if ("iPhone5,1" == DeviceModel)
            Type = "iPhone 5";
        else if ("iPhone5,2" == DeviceModel)
            Type = "iPhone 5";
        else if ("iPhone5,3" == DeviceModel)
            Type = "iPhone 5c";
        else if ("iPhone5,4" == DeviceModel)
            Type = "iPhone 5c";
        else if ("iPhone6,1" == DeviceModel)
            Type = "iPhone 5s";
        else if ("iPhone6,2" == DeviceModel)
            Type = "iPhone 5s";
        else if ("iPhone7,1" == DeviceModel)
            Type = "iPhone 6 Plus";
        else if ("iPhone7,2" == DeviceModel)
            Type = "iPhone 6";
        else if ("iPhone8,1" == DeviceModel)
            Type = "iPhone 6s";
        else if ("iPhone8,2" == DeviceModel)
            Type = "iPhone 6s Plus";
        else if ("iPhone8,4" == DeviceModel)
            Type = "iPhone SE";
        else if ("iPhone9,1" == DeviceModel)
            Type = "iPhone 7";
        else if ("iPhone9,2" == DeviceModel)
            Type = "iPhone 7 Plus";
        else if ("iPhone9,3" == DeviceModel)
            Type = "iPhone 7";
        else if ("iPhone9,4" == DeviceModel)
            Type = "iPhone 7 Plus";
        else if ("iPhone10,1" == DeviceModel)
            Type = "iPhone 8";
        else if ("iPhone10,2" == DeviceModel)
            Type = "iPhone 8 Plus";
        else if ("iPhone10,4" == DeviceModel)
            Type = "iPhone 8";
        else if ("iPhone10,5" == DeviceModel)
            Type = "iPhone 8 Plus";
        else if ("iPhone10,3" == DeviceModel)
            Type = "iPhone X";
        else if ("iPhone10,6" == DeviceModel)
            Type = "iPhone X";
        else if ("iPhone11,2" == DeviceModel)
            Type = "iPhone XS";
        else if ("iPhone11,4" == DeviceModel)
            Type = "iPhone XS Max";
        else if ("iPhone11,6" == DeviceModel)
            Type = "iPhone XS Max";
        else if ("iPhone11,8" == DeviceModel)
            Type = "iPhone XR";
        else if ("iPhone12,1" == DeviceModel)
            Type = "iPhone 11";
        else if ("iPhone12,3" == DeviceModel)
            Type = "iPhone 11 Pro";
        else if ("iPhone12,5" == DeviceModel)
            Type = "iPhone 11 Pro Max";
        else if ("iPhone12,8" == DeviceModel)
            Type = "iPhone SE 2";
        else if ("iPhone13,1" == DeviceModel)
            Type = "iPhone 12 mini";
        else if ("iPhone13,2" == DeviceModel)
            Type = "iPhone 12";
        else if ("iPhone13,3" == DeviceModel)
            Type = "iPhone 12 Pro";
        else if ("iPhone13,4" == DeviceModel)
            Type = "iPhone 12 Pro Max";
        else if ("i386" == DeviceModel)
            Type = "Simulator";
        else if ("x86_64" == DeviceModel)
            Type = "Simulator";
        else if ("iPod1,1" == DeviceModel)
            Type = "iPod Touch 1G";
        else if ("iPod2,1" == DeviceModel)
            Type = "iPod Touch 2G";
        else if ("iPod3,1" == DeviceModel)
            Type = "iPod Touch 3G";
        else if ("iPod4,1" == DeviceModel)
            Type = "iPod Touch 4G";
        else if ("iPod5,1" == DeviceModel)
            Type = "iPod Touch 5G";
        else if ("iPod7,1" == DeviceModel)
            Type = "iPod Touch 6G";
        else if ("iPod9,1" == DeviceModel)
            Type = "iPod Touch 7G";
        else if ("iPad1,1" == DeviceModel)
            Type = "iPad";
        else if ("iPad1,2" == DeviceModel)
            Type = "iPad 3G";
        else if ("iPad2,1" == DeviceModel)
            Type = "iPad 2";
        else if ("iPad2,2" == DeviceModel)
            Type = "iPad 2";
        else if ("iPad2,3" == DeviceModel)
            Type = "iPad 2";
        else if ("iPad2,4" == DeviceModel)
            Type = "iPad 2";
        else if ("iPad2,5" == DeviceModel)
            Type = "iPad Mini";
        else if ("iPad2,6" == DeviceModel)
            Type = "iPad Mini";
        else if ("iPad2,7" == DeviceModel)
            Type = "iPad Mini";
        else if ("iPad3,1" == DeviceModel)
            Type = "iPad 3";
        else if ("iPad3,2" == DeviceModel)
            Type = "iPad 3";
        else if ("iPad3,3" == DeviceModel)
            Type = "iPad 3";
        else if ("iPad3,4" == DeviceModel)
            Type = "iPad 4";
        else if ("iPad3,5" == DeviceModel)
            Type = "iPad 4";
        else if ("iPad3,6" == DeviceModel)
            Type = "iPad 4";
        else if ("iPad4,1" == DeviceModel)
            Type = "iPad Air";
        else if ("iPad4,2" == DeviceModel)
            Type = "iPad Air";
        else if ("iPad4,3" == DeviceModel)
            Type = "iPad Air";
        else if ("iPad4,4" == DeviceModel)
            Type = "iPad Mini 2";
        else if ("iPad4,5" == DeviceModel)
            Type = "iPad Mini 2";
        else if ("iPad4,6" == DeviceModel)
            Type = "iPad Mini 2";
        else if ("iPad4,7" == DeviceModel)
            Type = "iPad Mini 3";
        else if ("iPad4,8" == DeviceModel)
            Type = "iPad Mini 3";
        else if ("iPad4,9" == DeviceModel)
            Type = "iPad Mini 3";
        else if ("iPad5,1" == DeviceModel)
            Type = "iPad Mini 4";
        else if ("iPad5,2" == DeviceModel)
            Type = "iPad Mini 4";
        else if ("iPad5,3" == DeviceModel)
            Type = "iPad Air 2";
        else if ("iPad5,4" == DeviceModel)
            Type = "iPad Air 2";
        else if ("iPad6,3" == DeviceModel)
            Type = "iPad Pro 9.7";
        else if ("iPad6,4" == DeviceModel)
            Type = "iPad Pro 9.7";
        else if ("iPad6,7" == DeviceModel)
            Type = "iPad Pro 12.9";
        else if ("iPad6,8" == DeviceModel)
            Type = "iPad Pro 12.9";
        else if ("iPad6,11" == DeviceModel)
            Type = "iPad 5";
        else if ("iPad6,12" == DeviceModel)
            Type = "iPad 5";
        else if ("iPad7,1" == DeviceModel)
            Type = "iPad Pro 12.9 inch 2nd gen";
        else if ("iPad7,2" == DeviceModel)
            Type = "iPad Pro 12.9 inch 2nd gen";
        else if ("iPad7,3" == DeviceModel)
            Type = "iPad Pro 10.5 inch";
        else if ("iPad7,4" == DeviceModel)
            Type = "iPad Pro 10.5 inch";
        else if ("iPad7,5" == DeviceModel)
            Type = "iPad 6";
        else if ("iPad7,6" == DeviceModel)
            Type = "iPad 6";
        else if ("iPad7,11" == DeviceModel)
            Type = "iPad 7";
        else if ("iPad7,12" == DeviceModel)
            Type = "iPad 7";
        else if ("iPad8,1" == DeviceModel)
            Type = "iPad Pro 11 - inch";
        else if ("iPad8,2" == DeviceModel)
            Type = "iPad Pro 11 - inch";
        else if ("iPad8,3" == DeviceModel)
            Type = "iPad Pro 11 - inch";
        else if ("iPad8,4" == DeviceModel)
            Type = "iPad Pro 11 - inch";
        else if ("iPad8,5" == DeviceModel)
            Type = "iPad Pro 12.9 - inch 3rd gen";
        else if ("iPad8,6" == DeviceModel)
            Type = "iPad Pro 12.9 - inch 3rd gen";
        else if ("iPad8,7" == DeviceModel)
            Type = "iPad Pro 12.9 - inch 3rd gen";
        else if ("iPad8,8" == DeviceModel)
            Type = "iPad Pro 12.9 - inch 3rd gen";
        else if ("iPad8,9" == DeviceModel)
            Type = "iPad Pro 11 - inch 2nd gen";
        else if ("iPad8,10" == DeviceModel)
            Type = "iPad Pro 11 - inch 2nd gen";
        else if ("iPad8,11" == DeviceModel)
            Type = "iPad Pro 12.9 - inch 4th gen";
        else if ("iPad8,12" == DeviceModel)
            Type = "iPad Pro 12.9 - inch 4th gen";
        else if ("iPad11,1" == DeviceModel)
            Type = "iPad Mini 5";
        else if ("iPad11,2" == DeviceModel)
            Type = "iPad Mini 5";
        else if ("iPad11,3" == DeviceModel)
            Type = "iPad Air 3";
        else if ("iPad11,4" == DeviceModel)
            Type = "iPad Air 3";
        else if ("iPad11,6" == DeviceModel)
            Type = "iPad 8";
        else if ("iPad11,7" == DeviceModel)
            Type = "iPad 8";
        else if ("iPad13,1" == DeviceModel)
            Type = "iPad Air 4";
        else if ("iPad13,2" == DeviceModel)
            Type = "iPad Air 4";
        else
        {
            Type = "";
        }
        return Type;
    }
}
