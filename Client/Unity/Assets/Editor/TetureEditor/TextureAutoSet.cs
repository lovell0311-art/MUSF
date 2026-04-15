using UnityEditor;
using UnityEngine;

namespace ETEditor
{
    public class TextureAutoSet : EditorWindow
    {
        private string m_selectFolder = "";
        private PlatformType_Teture m_platformType = PlatformType_Teture.iPhone;
        private TextureImporterType m_textureImporterType = TextureImporterType.Default;
        private TextureImporterFormat m_importerFormat = TextureImporterFormat.ASTC_6x6;

        [MenuItem("Tools/Texture/Texture Auto Set")]
        private static void ShowPanel()
        {
            GetWindow<TextureAutoSet>().Show();
        }

        private void OnEnable()
        {
            m_selectFolder = PlayerPrefs.GetString("TextureSelectFolder");
            if (Selection.objects.Length > 0)
                m_selectFolder = AssetDatabase.GetAssetPath(Selection.objects[0]);
        }

        private void OnGUI()
        {
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.Label("·��:", GUILayout.Width(80));
            m_selectFolder = GUILayout.TextField(m_selectFolder);
           
            if (GUILayout.Button("ѡ���ļ���", GUILayout.Width(120)))
            {
                var result = EditorUtility.OpenFolderPanel("ѡ���ļ���", m_selectFolder, "");
                if (string.IsNullOrEmpty(result))
                {
                    return;
                }

                m_selectFolder = result.Substring(result.IndexOf("Assets"));
                m_selectFolder = m_selectFolder.Replace('\\', '/');
                PlayerPrefs.SetString("TextureSelectFolder", m_selectFolder);
            }

            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            m_platformType = (PlatformType_Teture)EditorGUILayout.EnumPopup("ƽ̨ : ", m_platformType);
            GUILayout.Space(10);
            m_textureImporterType = (TextureImporterType)EditorGUILayout.EnumPopup("�������Դ���� : ", m_textureImporterType);
            GUILayout.Space(10);
            m_importerFormat = (TextureImporterFormat)EditorGUILayout.EnumPopup("��Դ��ѹ������:", m_importerFormat);
            GUILayout.Space(10);
            if (GUILayout.Button("����ȫ��"))
            {
                SetAllTextureType();
            }
        }

        private void SetAllTextureType()
        {
            var index = 0;
            var assets = AssetDatabase.FindAssets("t:Texture", new string[] { m_selectFolder });
            foreach (var item in assets)
            {
                index++;
                var path = AssetDatabase.GUIDToAssetPath(item);
                // ��ʾ����
                EditorUtility.DisplayProgressBar($"��ǰ��Դ��{index}/{assets.Length}", path, (float)index / assets.Length);
                // IOSƽ̨����
                var importer = AssetImporter.GetAtPath(path) as TextureImporter;
                if (importer == null || importer.textureType != m_textureImporterType)
                {
                    continue;
                }

                if (m_platformType == PlatformType_Teture.All)
                {
                    string[] values = System.Enum.GetNames(typeof(PlatformType));
                    for (int i = 1; i < values.Length; i++)
                    {
                        var setting = importer.GetPlatformTextureSettings(values[i]);
                        // ���ó�ѡ������
                        setting.format = m_importerFormat;
                        setting.overridden = true;
                        importer.SetPlatformTextureSettings(setting);
                    }
                }
                else
                {
                    var setting = importer.GetPlatformTextureSettings(m_platformType.ToString());
                    // ���ó�ѡ������
                    setting.format = m_importerFormat;
                    setting.overridden = true;
                    importer.SetPlatformTextureSettings(setting);
                }

                // ��������
                AssetDatabase.ImportAsset(path);
            }

            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
        }
    }

    /// <summary>
    ///  The options for the platform string are "Standalone", "Web", "iPhone", "Android", "WebGL", "Windows Store Apps", "PS4", "XboxOne", "Nintendo Switch" and "tvOS".
    /// </summary>
    public enum PlatformType_Teture
    {
        All,
        Android,
        iPhone,
    }
}