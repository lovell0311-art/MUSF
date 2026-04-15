using System.Collections;
using UnityEditor;
using UnityEngine;

public class EditorCoroutineExample : EditorWindow
{
    private float startTime;
    private float duration = 2f;

    [MenuItem("Window/Editor Coroutine Example")]
    static void Init()
    {
        EditorCoroutineExample window = GetWindow<EditorCoroutineExample>();
        window.Show();
    }

    void OnGUI()
    {
        if (GUILayout.Button("Start Coroutine"))
        {
            // 记录开始时间
            startTime = (float)EditorApplication.timeSinceStartup;
        }

        // 在 OnGUI 中手动模拟 Update 循环
        float elapsedTime = (float)EditorApplication.timeSinceStartup - startTime;
        if (elapsedTime < duration)
        {
            Repaint(); // 强制重新绘制窗口
        }
        else
        {
            Debug.Log("Coroutine Finished");
        }
    }
}

