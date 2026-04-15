using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


//#if UNITY_EDITOR_WIN
public class SafeAreas : MonoBehaviour
{ //public
    [Header("地图大小")]
    public int GridX = 350;//格子X
    public int GridZ = 350;//格子Z
    public Text GridText;
    [Header("滚轮缩放比例")]
    public float SizeScale = 0.3f;
    public float SizeScaleDis = 80;
    [Header("笔刷大小 *2")]
    public int brushSize = 1;

    private Transform quaPlane;
    private Camera mainCamera;
    private bool isMousePress;
    private Vector3 mouseLostPos;
    private float GridHeight = 5;
    public float width = 2f;



    private Dictionary<Vector2, SpawnPoint> monsterPointsDic = new Dictionary<Vector2, SpawnPoint>();



    public Button savaBtn;


    void Start()
    {
       

        savaBtn.onClick.AddListener(SaveFile);
        string currSceneName = SceneManager.GetActiveScene().name;
        //加载 该场景的格子信息
        if (File.Exists(Application.dataPath + "/Res/NavGridData/" + currSceneName + ".txt"))
        {
            string text = File.ReadAllText(Application.dataPath + "/Res/NavGridData/" + currSceneName + ".txt", System.Text.Encoding.UTF8);
            if (!string.IsNullOrEmpty(text))
            {
                NavGridData data = JsonMapper.ToObject<NavGridData>(text);
                GridX = data.Width;
                GridZ = data.Height;
            }
        }

        if (mainCamera == null)
        {
            mainCamera = new GameObject("mainCamera").AddComponent<Camera>();
            mainCamera.orthographic = true;
            mainCamera.transform.localEulerAngles = Vector3.right * 90;
            mainCamera.transform.position = Vector3.up * 90;
            mainCamera.orthographicSize = 100;
            GridHeight = 45;
            quaPlane = GameObject.CreatePrimitive(PrimitiveType.Quad).transform;
            quaPlane.localEulerAngles = Vector3.right * 90;
            quaPlane.position = new Vector3(mainCamera.transform.position.x, 90, mainCamera.transform.position.z);
            quaPlane.gameObject.GetComponent<Renderer>().enabled = false;
            quaPlane.localScale = new Vector3(200, 100, 1);
        }

        InitMonsterSpanwPoints();


    }



    /// <summary>
    /// 初始化当前 场景的安全区
    /// </summary>
    private void InitMonsterSpanwPoints()
    {
        string currSceneName = SceneManager.GetActiveScene().name + "_SafeArea";
       // string path = "../SpawnPoints/SafeAreas/" + currSceneName + ".json";//当前场景 怪物出生点的 Json 配置
     //   string path = "E:\\Miracle_Mu\\Client\\Unity\\Assets\\Res\\Safes\\" + currSceneName + ".json";//当前场景 怪物出生点的 Json 配置
        string path = "../Unity/Assets/Res/Safes/" + currSceneName + ".json";//当前场景 怪物出生点的 Json 配置
        if (File.Exists(path))
        {
            monsterPointsDic.Clear();
            string text = File.ReadAllText(path, System.Text.Encoding.UTF8);
            if (!string.IsNullOrEmpty(text))
            {
                SpawnPoint[] data = JsonMapper.ToObject<SpawnPoint[]>(text);
                foreach (var item in data)
                {
                    monsterPointsDic[new Vector2(item.PositionX, item.PositionY)] = item;

                }
            }
        }
    }

  

    private void OnDestroy()
    {
        SaveFile();
    }

   

    public void SaveFile()
    {
       
        string currSceneName = SceneManager.GetActiveScene().name + "_SafeArea";
        string text = JsonMapper.ToJson(monsterPointsDic.Values.ToList());
        string path = "../Unity/Assets/Res/Safes/";
       // string path = "E:\\Miracle_Mu\\Client\\Unity\\Assets\\Res\\Safes\\";
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        //File.WriteAllText(path + currSceneName + ".json", text, System.Text.Encoding.UTF8); //保存 当前的怪物出生点信息
        File.WriteAllText(path + currSceneName + ".json", text, new UTF8Encoding(false)); //保存 当前的怪物出生点信息

        Debug.Log($"<color=#00FF00>保存完毕 {path + currSceneName + ".json"}</color>");
    }

    void Update()
    {
        //判断是否点击到了UI上
        if (EventSystem.current.IsPointerOverGameObject())
        {

            return;
        }

        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
         
            Vector3 mousePos = Input.mousePosition;
            Ray ray = mainCamera.ScreenPointToRay(mousePos);
            RaycastHit hit;
            Physics.Raycast(ray, out hit, 1000);
            if (hit.collider == null)
            {
                Debug.Log("点击目标为空");
                return;

            }
            Vector3 pos = hit.point;
            int currX = (int)(pos.x / width);
            int currZ = (int)(pos.z / width);
            Vector2 vector2 = new Vector2(currX, currZ);

            if (Input.GetMouseButtonDown(0))
            {
              
                monsterPointsDic[vector2] = new SpawnPoint { Index = 1, PositionX = currX, PositionY = currZ };
                if (brushSize != 1)
                {
                    for (int i = -brushSize; i < brushSize; i++)
                    {
                        for (int j = -brushSize; j < brushSize; j++)
                        {
                            monsterPointsDic[new Vector2(currX+i, currZ+j)] = new SpawnPoint { Index = 1, PositionX = currX + i, PositionY = currZ+j };
                        }
                    }
                }

            }
            else if (Input.GetMouseButtonDown(1))
            {
              
                if (monsterPointsDic.ContainsKey(vector2))
                {
                    monsterPointsDic.Remove(vector2);
                }
                if (brushSize != 1)
                {
                    for (int i = -brushSize; i < brushSize; i++)
                    {
                        for (int j = -brushSize; j < brushSize; j++)
                        {
                            if (monsterPointsDic.ContainsKey(new Vector2(currX + i, currZ + j))) 
                            {
                                monsterPointsDic.Remove(new Vector2(currX + i, currZ + j));
                            }
                              
                        }
                    }
                }
            }
            GridText.text = "选择坐标：(" + currX + "," + currZ + ")";
        }
        float xw = Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * SizeScaleDis;
        if (xw != 0)
        {
            mainCamera.orthographicSize -= xw;
            Vector3 pos = mainCamera.gameObject.transform.localPosition;
            pos.y -= xw * SizeScale;
            GridHeight = pos.y * 0.5f;
            mainCamera.gameObject.transform.localPosition = pos;
        }
        if (Input.GetMouseButtonDown(2))
        {
            mouseLostPos = Input.mousePosition;
            isMousePress = true;
        }
        if (Input.GetMouseButtonUp(2))
        {
            isMousePress = false;
        }
        if (isMousePress)
        {
            Vector3 dir = Input.mousePosition - mouseLostPos;
            mouseLostPos = Input.mousePosition;
            dir.z = dir.y;
            dir.y = 0;
            mainCamera.transform.position -= dir * Time.deltaTime * 2;
            quaPlane.position -= dir * Time.deltaTime * 2;
        }
    }
    private void OnDrawGizmos()
    {
        for (int y = 0; y < GridZ; y++)
        {
            for (int x = 0; x < GridX; x++)
            {

                if (monsterPointsDic.ContainsKey(new Vector2(x, y)))
                {
                    Gizmos.color = new Color(67, 0, 0, 100 / 255f);
                    Gizmos.DrawCube(new Vector3(width * 0.5f + x * width, GridHeight, width * 0.5f + y * width), new Vector3(width, 0, width));
                }
                else
                {
                    Gizmos.color = new Color(1, 1, 1, 10 / 255f);
                    Gizmos.DrawWireCube(new Vector3(width * 0.5f + x * width, GridHeight, width * 0.5f + y * width), new Vector3(width, 0, width));
                }

            }
        }
    }



#if UNITY_EDITOR
    [CustomEditor(typeof(NPCCreatPointJson))]
    public class MonsterCreatPointJsonHelper : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("保存导出"))
            {
                var component = target as NPCCreatPointJson;
                component.SaveFile();
            }
        }
    }
#endif
    #region Json
    /// <summary>
    /// 获取到本地的Json文件并且解析返回对应的json字符串
    /// </summary>
    /// <param name="filepath">文件路径</param>
    /// <returns></returns>
    private string GetJsonFile(string filepath)
    {
        string json = string.Empty;
        using (FileStream fs = new FileStream(filepath, FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite, FileShare.ReadWrite))
        {
            using (StreamReader sr = new StreamReader(fs, Encoding.UTF8))
            {
                json = sr.ReadToEnd().ToString();
            }
        }
        return json;
    }
    #endregion
}
//#endif


