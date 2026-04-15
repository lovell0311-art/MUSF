using UnityEngine;
using System.IO;
using ETModel;
namespace ETHotfix
{
    public static class LoginDiagnosticOptions
    {
        // Diagnostic auto-flow should be explicit opt-in so it does not interfere
        // with normal login/choose-role flows on test and release builds.
        private const string AutoFlowKey = "codex.login.autoflow";
        private const string MapDeliveryTargetKey = "codex.mapdelivery.target";
        private const string AutoFlowFlagFileName = "autoflow.flag";

        public static bool AutoFlowEnabled
        {
            get
            {
                if (PlayerPrefs.GetInt(AutoFlowKey, 0) == 1)
                {
                    return true;
                }

                try
                {
                    return File.Exists(Path.Combine(PathHelper.AppHotfixResPath, AutoFlowFlagFileName));
                }
                catch
                {
                    return false;
                }
            }
        }

        public static int AutoMapDeliveryTargetId
        {
            get
            {
                return PlayerPrefs.GetInt(MapDeliveryTargetKey, 0);
            }
        }

        public static void ClearAutoMapDeliveryTarget()
        {
            PlayerPrefs.DeleteKey(MapDeliveryTargetKey);
            PlayerPrefs.Save();
        }
    }
}
