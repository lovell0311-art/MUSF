using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using LitJson;
using UnityEngine.UI;

#if UNITY_EDITOR_WIN
public class MapJsonCreate : MonoBehaviour
{
   
    [Header("地图大小")]
    public int GridX = 350;//格子X
    public int GridZ = 350;//格子Z
    public Text GridText;
    [Header("滚轮缩放比例")]
    public float SizeScale = 0.3f;
    public float SizeScaleDis = 80;
    [Header("笔刷大小 *2")]
    public int brushSize = 1;

    //private
    private Texture2D texture2D;
    private Dictionary<int, int> mapDic = new Dictionary<int, int>();
    private int cameraX, cameraY;
    private Transform quaPlane;
    private Camera mainCamera;
    private bool isMousePress;
    private Vector3 mouseLostPos;
    private float GridHeight = 5;
    private float width = 2f;
    private int maxCount = 0;

    void Start()
    {
        string currSceneName = SceneManager.GetActiveScene().name;
        if (File.Exists(Application.dataPath + "/Res/NavGridData/" + currSceneName + ".txt"))
        {
            string text = File.ReadAllText(Application.dataPath + "/Res/NavGridData/" + currSceneName + ".txt", System.Text.Encoding.UTF8);
            
            if (!string.IsNullOrEmpty(text))
            {
                NavGridData data = JsonMapper.ToObject<NavGridData>(text);
                GridX = data.Width;
                GridZ = data.Height;
                for (int i = 0; i < data.SceneInfos.Count; i++)
                {
                    int nClamp = data.SceneInfos[i];
                    if (nClamp == 1)
                    {
                        mapDic[i] = nClamp;
                    }
                }
            }
        }

        if (mainCamera == null){
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
        texture2D = new Texture2D(GridX, GridZ);
    }
    private void OnDestroy()
    {
        SaveFile();
    }

    public void SaveFile()
    {
        NavGridData data = new NavGridData();
        
        data.Width = GridX;
        data.Height = GridZ;
        data.SceneInfoSize = GridX * GridZ;
        data.SceneInfos = new List<int>();
        for (int y = 0; y < GridZ; y++)
        {
            for (int x = 0; x < GridX; x++)
            {
                int id = y * GridX + x;
                if (mapDic.ContainsKey(id))
                {
                    data.SceneInfos.Add(1);
                }
                else
                {
                    data.SceneInfos.Add(0);
                }
            }
        }
        string currSceneName = SceneManager.GetActiveScene().name;
        string text = JsonMapper.ToJson(data);
        string path = Application.dataPath + "/Res/NavGridData/";
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
       //File.WriteAllText(Application.dataPath + "/Res/NavGridData/" + currSceneName + ".txt", text, System.Text.Encoding.UTF8);
        File.WriteAllText(path + currSceneName + ".txt", text, System.Text.Encoding.UTF8);
        Debug.Log("<color=#00FF00>保存完毕</color>");
    }

    void Update()
    {
        
        if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
        {
            Vector3 mousePos = Input.mousePosition;
            Ray ray = mainCamera.ScreenPointToRay(mousePos);
            RaycastHit hit;
            Physics.Raycast(ray, out hit, 1000);
            if (hit.collider == null) return;
            Vector3 pos = hit.point;
            int currX = (int)(pos.x / width);
            int currZ = (int)(pos.z / width);
            int id = currZ * GridX + currX;
            maxCount = GridX * GridZ;
            if (id < 0 || id >= maxCount) { }
            else
            {
                if (Input.GetMouseButton(0))
                {
                    mapDic[id] = 1;
                    if (brushSize != 1)
                    {
                        for (int i = -brushSize; i < brushSize; i++)
                        {
                            for (int j = -brushSize; j < brushSize; j++)
                            {
                                mapDic[(currZ + i) * GridX + (currX + j)] = 1;
                            }
                        }
                    }
                }
                else if (Input.GetMouseButton(1))
                {
                    if (mapDic.ContainsKey(id))
                    {
                        mapDic.Remove(id);
                        if (brushSize != 1)
                        {
                            for (int i = -brushSize; i < brushSize; i++)
                            {
                                for (int j = -brushSize; j < brushSize; j++)
                                {
                                    mapDic.Remove((currZ + i) * GridX + (currX + j));
                                }
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
            cameraX = (int)(mainCamera.transform.position.x / width);
            cameraY = (int)(mainCamera.transform.position.z / width);
            quaPlane.position -= dir * Time.deltaTime * 2;
        }
    }
    private void OnDrawGizmos()
    {
            for (int y = 0; y < GridZ; y++)
            {
                for (int x = 0; x < GridX; x++)
                {
                    int id = y * GridX + x;
                    if (mapDic.ContainsKey(id))
                    {
                    Gizmos.color = new Color(67, 0, 0, 100 / 255f);
                    
                    Gizmos.DrawCube(new Vector3(width * 0.5f + x * width, GridHeight, width * 0.5f + y * width), new Vector3(width, 0, width));                     }
                    else
                    {
                     Gizmos.color = new Color(1, 1, 1, 10 / 255f);
                    //Gizmos.color = Color.green;
                     Gizmos.DrawWireCube(new Vector3(width * 0.5f + x * width, GridHeight, width * 0.5f + y * width), new Vector3(width, 0, width));
                    }
                }
            }
    }
}
#endif
public class NavGridData
{
    public int Width { get; set; }
    public int Height { get; set; }
    public int SceneInfoSize { get; set; }
    public List<int> SceneInfos { get; set; }
}

#if UNITY_EDITOR
[CustomEditor(typeof(MapJsonCreate))]
public class MapJsonCreateHelper : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("保存导出"))
        {
            var component = target as MapJsonCreate;
            component.SaveFile();
        }
    }
}
#endif