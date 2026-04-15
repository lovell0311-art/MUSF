using UnityEngine;
using System.IO;
using System.Linq;
using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;

/// <summary>
/// 功能：通用静态方法
/// </summary>
namespace ETModel
{
    public class GameUtility
    {
        public const string AssetsFolderName = "Assets";
        private static Vector3 lastPrimaryPointerScreenPosition = Vector3.zero;
        private static bool hasActivePrimaryTouch = false;

        public static string FormatToUnityPath(string path)
        {
            return path.Replace("\\", "/");
        }

        public static string FormatToSysFilePath(string path)
        {
            return path.Replace("/", "\\");
        }

        /// <summary>
        /// 获取当前平台
        /// </summary>
        /// <returns></returns>
        public static string GetPlatform()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    return "Android";
                case RuntimePlatform.IPhonePlayer:
                    return "IOS";
                default:
                    return "Windows";
            }
        }

        /// <summary>
        /// 是否是移动端
        /// </summary>
        /// <returns></returns>
        public static bool IsMobile()
        {
            return Application.isMobilePlatform;
        }

        /// <summary>
        /// 获取当前网络
        /// </summary>
        /// <returns>0：无网，1：移动网，2：wifi</returns>
        public static int GetInternet()
        {
            return (int)Application.internetReachability;
        }

        /// <summary>
        /// 手机永不休眠
        /// </summary>
        public static void NeverSleep()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        /// <summary>
        /// 获取时间戳
        /// </summary>
        /// <returns></returns>
        public static string GetTimeStamp()
        {
            return new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString();
        }

        /// <summary>
        /// 判断手指是否点击UI，用于UI遮挡
        /// true 是点在了UI上
        /// </summary>
        public static bool IsPointerOverGameObject()
        {
            return IsPointerOverGameObject(-1);
        }

        public static bool IsPointerOverGameObject(int pointerId)
        {
            if (UnityEngine.EventSystems.EventSystem.current == null)
            {
                return false;
            }

#if UNITY_STANDALONE_WIN||UNITY_EDITOR_WIN
            return UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();
#else
            if (pointerId >= 0)
            {
                return UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(pointerId);
            }

            if (Input.touchCount > 0)
            {
                return UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
            }

            return UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();
#endif
        }

        public static bool TryGetPrimaryPointerDown(out Vector3 screenPosition, out int pointerId)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);
                if (touch.phase == TouchPhase.Began)
                {
                    screenPosition = touch.position;
                    lastPrimaryPointerScreenPosition = screenPosition;
                    hasActivePrimaryTouch = true;
                    pointerId = touch.fingerId;
                    return true;
                }
            }

            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                lastPrimaryPointerScreenPosition = touch.position;
                if (!hasActivePrimaryTouch)
                {
                    screenPosition = touch.position;
                    hasActivePrimaryTouch = true;
                    pointerId = touch.fingerId;
                    return true;
                }
            }
            else
            {
                hasActivePrimaryTouch = false;
            }

            if (Input.GetMouseButtonDown(0))
            {
                screenPosition = Input.mousePosition;
                lastPrimaryPointerScreenPosition = screenPosition;
                pointerId = -1;
                return true;
            }

            screenPosition = Vector3.zero;
            pointerId = -1;
            return false;
        }

        public static Vector3 GetPrimaryPointerScreenPosition()
        {
            if (Input.touchCount > 0)
            {
                Vector3 touchPosition = Input.GetTouch(0).position;
                lastPrimaryPointerScreenPosition = touchPosition;
                return touchPosition;
            }

            if (Input.GetMouseButton(0) ||
                Input.GetMouseButtonDown(0) ||
                Input.GetMouseButtonUp(0))
            {
                return Input.mousePosition;
            }

            return lastPrimaryPointerScreenPosition;
        }

        /// <summary>
        /// 获取手机点击的UI，用于判断指定UI遮挡
        /// </summary>
        public static bool GetPointerOverGameObject(out GameObject pressObject)
        {
            pressObject = null;

            if (UnityEngine.EventSystems.EventSystem.current == null || !IsPointerOverGameObject())
            {
                return false;
            }

            Vector3 pointerPosition = GetPrimaryPointerScreenPosition();
            PointerEventData eventDataCurrentPosition = new PointerEventData(UnityEngine.EventSystems.EventSystem.current)
            {
                position = new Vector2(pointerPosition.x, pointerPosition.y)
            };

            List<RaycastResult> results = new List<RaycastResult>();
            UnityEngine.EventSystems.EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            if (results.Count > 0)
            {
                pressObject = results[0].gameObject;
                return true;
            }

            return false;
        }
        // 通用变焦：鼠标滚动,鼠标两个手指不按
        public static float GetZoomUniversal()
        {
            if (Input.mousePresent)
                return GetAxisRawScrollUniversal();
            else if (Input.touchSupported)
                return GetPinch();
            return 0;
        }
        // 所有平台之间一致的硬鼠标滚动
        // 输入.GetAxis（“鼠标滚轮”）和
        // 输入.GetAxisRaw（“鼠标滚轮”）
        // 这两个返回值都是独立的0.01和WebGL上的0.5，这
        // 导致在WebGL上缩放过快等。
        // 通常GetAxisRaw应该返回-1,0,1，但对于滚动则不返回
        public static float GetAxisRawScrollUniversal()
        {
            float scroll = Input.GetAxisRaw("Mouse ScrollWheel");
            if (scroll < 0) return -1;
            if (scroll > 0) return 1;
            return 0;
        }
        // two finger pinch detection
        // source: https://docs.unity3d.com/Manual/PlatformDependentCompilation.html
        public static float GetPinch()
        {
            if (Input.touchCount == 2)
            {
                // Store both touches.
                Touch touchZero = Input.GetTouch(0);
                Touch touchOne = Input.GetTouch(1);

                // Find the position in the previous frame of each touch.
                Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

                // Find the magnitude of the vector (the distance) between the touches in each frame.
                float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

                // Find the difference in the distances between each frame.
                return touchDeltaMag - prevTouchDeltaMag;
            }
            return 0;
        }
        /// <summary>
        /// 漂亮的打印秒数小时：分钟：秒（.毫秒/100）秒
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static string PrettySeconds(float seconds)
        {
            TimeSpan t = TimeSpan.FromSeconds(seconds);
            string res = "";
            if (t.Days > 0) res += t.Days + "d";
            if (t.Hours > 0) res += " " + t.Hours + "h";
            if (t.Minutes > 0) res += " " + t.Minutes + "m";
            // 0.5s, 1.5s etc. if any milliseconds. 1s, 2s etc. if any seconds
            if (t.Milliseconds > 0) res += " " + t.Seconds + "." + (t.Milliseconds / 100) + "s";
            else if (t.Seconds > 0) res += " " + t.Seconds + "s";
            // 如果字符串仍然是空的，因为值是“0”，那么至少
            // 返回秒数，而不是返回空字符串
            return res != "" ? res : "0s";
        }
        /// <summary>
        /// 递归查找子节点
        /// </summary>
        /// <param name="parent">父节点</param>
        /// <param name="name">子节点名字</param>
        /// <returns>子节点Transform</returns>
        public static Transform FindChildRecursion(Transform parent, string name)
        {
            foreach (Transform child in parent)
            {
                if (child.name == name)
                {
                    return child;
                }
                else
                {
                    Transform ret = FindChildRecursion(child, name);
                    if (ret != null)
                        return ret;
                }
            }
            return null;
        }

        /// <summary>
        /// 将文件夹全名前缀改成Assets/
        /// </summary>
        /// <param name="full_path"></param>
        /// <returns></returns>
        public static string FullPathToAssetPath(string full_path)
        {
            full_path = FormatToUnityPath(full_path);
            if (!full_path.StartsWith(Application.dataPath))
            {
                return null;
            }
            string ret_path = full_path.Replace(Application.dataPath, "");
            return AssetsFolderName + ret_path;
        }

        /// <summary>
        /// 获得文件的后缀名
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetFileExtension(string path)
        {
            return Path.GetExtension(path).ToLower();
        }

        /// <summary>
        /// 获取文件夹下包括（不包括）的后缀名的文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="extensions"></param>
        /// <param name="exclude"></param>
        /// <returns></returns>
        public static string[] GetSpecifyFilesInFolder(string path, string[] extensions = null, bool exclude = false)
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }

            if (extensions == null)
            {
                return Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
            }
            else if (exclude)
            {
                return Directory.GetFiles(path, "*.*", SearchOption.AllDirectories)
                    .Where(f => !extensions.Contains(GetFileExtension(f))).ToArray();
            }
            else
            {
                return Directory.GetFiles(path, "*.*", SearchOption.AllDirectories)
                    .Where(f => extensions.Contains(GetFileExtension(f))).ToArray();
            }
        }

        /// <summary>
        /// 获取文件夹下指定的后缀名文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static string[] GetSpecifyFilesInFolder(string path, string pattern)
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }

            return Directory.GetFiles(path, pattern, SearchOption.AllDirectories);
        }

        /// <summary>
        /// 获取文件夹下的所有文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string[] GetAllFilesInFolder(string path)
        {
            return GetSpecifyFilesInFolder(path);
        }

        /// <summary>
        /// 返回文件夹下的所有子文件夹
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string[] GetAllDirsInFolder(string path)
        {
            return Directory.GetDirectories(path, "*", SearchOption.AllDirectories);
        }

        /// <summary>
        /// 如果路径的文件的文件夹不存在则创建文件夹
        /// </summary>
        /// <param name="filePath"></param>
        public static void CheckFileAndCreateDirWhenNeeded(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return;
            }

            FileInfo file_info = new FileInfo(filePath);
            DirectoryInfo dir_info = file_info.Directory;
            if (!dir_info.Exists)
            {
                Directory.CreateDirectory(dir_info.FullName);
            }
        }

        /// <summary>
        /// 如果路径文件夹不存在则创建
        /// </summary>
        /// <param name="folderPath"></param>
        public static void CheckDirAndCreateWhenNeeded(string folderPath)
        {
            if (string.IsNullOrEmpty(folderPath))
            {
                return;
            }

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
        }

        /// <summary>
        /// 将二进制数据写入文件
        /// </summary>
        /// <param name="outFile"></param>
        /// <param name="outBytes"></param>
        /// <returns></returns>
        public static bool SafeWriteAllBytes(string outFile, byte[] outBytes)
        {
            try
            {
                if (string.IsNullOrEmpty(outFile))
                {
                    return false;
                }

                CheckFileAndCreateDirWhenNeeded(outFile);
                if (File.Exists(outFile))
                {
                    File.SetAttributes(outFile, FileAttributes.Normal);
                }
                File.WriteAllBytes(outFile, outBytes);
                return true;
            }
            catch (System.Exception ex)
            {
                Debug.LogError(string.Format("SafeWriteAllBytes failed! path = {0} with err = {1}", outFile, ex.Message));
                return false;
            }
        }

        /// <summary>
        /// 将字符串数组写入文件
        /// </summary>
        /// <param name="outFile"></param>
        /// <param name="outBytes"></param>
        /// <returns></returns>
        public static bool SafeWriteAllLines(string outFile, string[] outLines)
        {
            try
            {
                if (string.IsNullOrEmpty(outFile))
                {
                    return false;
                }

                CheckFileAndCreateDirWhenNeeded(outFile);
                if (File.Exists(outFile))
                {
                    File.SetAttributes(outFile, FileAttributes.Normal);
                }
                File.WriteAllLines(outFile, outLines);
                return true;
            }
            catch (System.Exception ex)
            {
                Debug.LogError(string.Format("SafeWriteAllLines failed! path = {0} with err = {1}", outFile, ex.Message));
                return false;
            }
        }

        /// <summary>
        /// 将字符串写入文件
        /// </summary>
        /// <param name="outFile"></param>
        /// <param name="outBytes"></param>
        /// <returns></returns>
        public static bool SafeWriteAllText(string outFile, string text)
        {
            try
            {
                if (string.IsNullOrEmpty(outFile))
                {
                    return false;
                }

                CheckFileAndCreateDirWhenNeeded(outFile);
                if (File.Exists(outFile))
                {
                    File.SetAttributes(outFile, FileAttributes.Normal);
                }
                File.WriteAllText(outFile, text);
                return true;
            }
            catch (System.Exception ex)
            {
                Debug.LogError(string.Format("SafeWriteAllText failed! path = {0} with err = {1}", outFile, ex.Message));
                return false;
            }
        }

        /// <summary>
        /// 读取文件的数据返回二进制数组
        /// </summary>
        /// <param name="inFile"></param>
        /// <returns></returns>
        public static byte[] SafeReadAllBytes(string inFile)
        {
            try
            {
                if (string.IsNullOrEmpty(inFile))
                {
                    return null;
                }

                if (!File.Exists(inFile))
                {
                    return null;
                }

                File.SetAttributes(inFile, FileAttributes.Normal);
                return File.ReadAllBytes(inFile);
            }
            catch (System.Exception ex)
            {
                Debug.LogError(string.Format("SafeReadAllBytes failed! path = {0} with err = {1}", inFile, ex.Message));
                return null;
            }
        }

        /// <summary>
        /// 读取文件返回字符串数组
        /// </summary>
        /// <param name="inFile"></param>
        /// <returns></returns>
        public static string[] SafeReadAllLines(string inFile)
        {
            try
            {
                if (string.IsNullOrEmpty(inFile))
                {
                    return null;
                }

                if (!File.Exists(inFile))
                {
                    return null;
                }

                File.SetAttributes(inFile, FileAttributes.Normal);
                return File.ReadAllLines(inFile);
            }
            catch (System.Exception ex)
            {
                Debug.LogError(string.Format("SafeReadAllLines failed! path = {0} with err = {1}", inFile, ex.Message));
                return null;
            }
        }

        /// <summary>
        /// 读取文件返回字符串
        /// </summary>
        /// <param name="inFile"></param>
        /// <returns></returns>
        public static string SafeReadAllText(string inFile)
        {
            try
            {
                if (string.IsNullOrEmpty(inFile))
                {
                    return null;
                }

                if (!File.Exists(inFile))
                {
                    return null;
                }

                File.SetAttributes(inFile, FileAttributes.Normal);
                return File.ReadAllText(inFile);
            }
            catch (System.Exception ex)
            {
                Debug.LogError(string.Format("SafeReadAllText failed! path = {0} with err = {1}", inFile, ex.Message));
                return null;
            }
        }

        /// <summary>
        /// 删除路径文件夹下的所有文件及文件夹
        /// </summary>
        /// <param name="dirPath"></param>
        public static void DeleteDirectory(string dirPath)
        {
            string[] files = Directory.GetFiles(dirPath);
            string[] dirs = Directory.GetDirectories(dirPath);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string dir in dirs)
            {
                DeleteDirectory(dir);
            }

            Directory.Delete(dirPath, false);
        }

        /// <summary>
        /// 清空文件夹
        /// </summary>
        /// <param name="folderPath"></param>
        /// <returns></returns>
        public static bool SafeClearDir(string folderPath)
        {
            try
            {
                if (string.IsNullOrEmpty(folderPath))
                {
                    return true;
                }

                if (Directory.Exists(folderPath))
                {
                    DeleteDirectory(folderPath);
                }
                Directory.CreateDirectory(folderPath);
                return true;
            }
            catch (System.Exception ex)
            {
                Debug.LogError(string.Format("SafeClearDir failed! path = {0} with err = {1}", folderPath, ex.Message));
                return false;
            }
        }

        /// <summary>
        /// 删除文件夹
        /// </summary>
        /// <param name="folderPath"></param>
        /// <returns></returns>
        public static bool SafeDeleteDir(string folderPath)
        {
            try
            {
                if (string.IsNullOrEmpty(folderPath))
                {
                    return true;
                }

                if (Directory.Exists(folderPath))
                {
                    DeleteDirectory(folderPath);
                }
                return true;
            }
            catch (System.Exception ex)
            {
                Debug.LogError(string.Format("SafeDeleteDir failed! path = {0} with err: {1}", folderPath, ex.Message));
                return false;
            }
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static bool SafeDeleteFile(string filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath))
                {
                    return true;
                }

                if (!File.Exists(filePath))
                {
                    return true;
                }
                File.SetAttributes(filePath, FileAttributes.Normal);
                File.Delete(filePath);
                return true;
            }
            catch (System.Exception ex)
            {
                Debug.LogError(string.Format("SafeDeleteFile failed! path = {0} with err: {1}", filePath, ex.Message));
                return false;
            }
        }

        /// <summary>
        /// 将source文件移动到dest文件并改名
        /// </summary>
        /// <param name="sourceFileName"></param>
        /// <param name="destFileName"></param>
        /// <returns></returns>
        public static bool SafeRenameFile(string sourceFileName, string destFileName)
        {
            try
            {
                if (string.IsNullOrEmpty(sourceFileName))
                {
                    return false;
                }

                if (!File.Exists(sourceFileName))
                {
                    return true;
                }
                SafeDeleteFile(destFileName);
                File.SetAttributes(sourceFileName, FileAttributes.Normal);
                File.Move(sourceFileName, destFileName);
                return true;
            }
            catch (System.Exception ex)
            {
                Debug.LogError(string.Format("SafeRenameFile failed! path = {0} with err: {1}", sourceFileName, ex.Message));
                return false;
            }
        }

        /// <summary>
        /// 将from文件复制到to文件，允许覆盖
        /// </summary>
        /// <param name="fromFile"></param>
        /// <param name="toFile"></param>
        /// <returns></returns>
        public static bool SafeCopyFile(string fromFile, string toFile)
        {
            try
            {
                if (string.IsNullOrEmpty(fromFile))
                {
                    return false;
                }

                if (!File.Exists(fromFile))
                {
                    return false;
                }
                CheckFileAndCreateDirWhenNeeded(toFile);
                SafeDeleteFile(toFile);
                File.Copy(fromFile, toFile, true);
                return true;
            }
            catch (System.Exception ex)
            {
                Debug.LogError(string.Format("SafeCopyFile failed! formFile = {0}, toFile = {1}, with err = {2}",
                    fromFile, toFile, ex.Message));
                return false;
            }
        }
    }
}
