using ETModel;
using LitJson;

using System.Collections.Generic;
using System.Linq;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;
using Codice.CM.Common;
using Codice.Client.BaseCommands.BranchExplorer;
using System.Text.RegularExpressions;

namespace ETHotfix
{

    /// <summary>
    /// 闁诲繐绻愮换鎰耿閹绢喖鐐婇柣鎰靛劔娓氣偓瀹?
    /// </summary>
    public partial class UIMainComponent
    {
 
        //闂佸搫鍊瑰姗€鎯冮鍕嵍闁哄瀵х粋?
        ReferenceCollector minmapRefrence;
        Dictionary<string, Sprite> MinMapDic = new Dictionary<string, Sprite>();
        string minmapres = "Minmap";
        //GuZhanChang_MonsterSpawnPoints_text
        GameObject BigMap;
        readonly string NPCRes = "NPC";//NPC闂備焦婢樼粔鍫曟偪?
        readonly string MonsterRes = "Monster";//Monster闂備焦婢樼粔鍫曟偪?
        readonly string TransferPointsRes = "TransferPoints";//TransferPoints闂備焦婢樼粔鍫曟偪?
        private Dictionary<string, string> NPCInfoDic = new Dictionary<string, string>();
        private Dictionary<Vector3, GameObject> npcPoint = new Dictionary<Vector3, GameObject>();
        public Dictionary<string, string> MonsterInfoDic = new Dictionary<string, string>();
        private Dictionary<long, GameObject> monsterPoint = new Dictionary<long, GameObject>();
        private Dictionary<string, string> TransferPointsInfoDic = new Dictionary<string, string>();

        //NPC闂佸憡鍨煎▍锝夊极閹剧粯鍊风憸鐗堝笚閼茬娀鏌?
        public List<SpawnPoint> NPCSpawnPointLists=new List<SpawnPoint>();
         

        UnitEntityPathComponent unitEntityPathComponent;
        AstarNode navtarget;

        int curSceneId =-1;
        int lastSceneId=-1;


        public MiniMap miniMap;
        /// <summary>闂佸搫鐗滈崜娆忥耿鐎靛憡鍠嗛柟鐑樻礀椤?闁诲繐绻愮换鎰耿閹绢喖鐐婄紓宥庢櫑on闁诲海鏁搁、濠囨寘?/summary>
        private MiniMap.IconRole miniMap_RoleIcon;

        private GameObject miniMap_Icon;
        private ReferenceCollector iconRefrence;
        private Vector3 lastRolePosition;
        private const float LegacyMinimapExpectedSize = 1024f;
        private const float LegacyMinimapPackedSpriteSize = 752f;
        private const string LegacyMinimapManifestHash = "Hash: 755c1b89b69fe1479a70d9d7c7efd516";
        private const float MainUiMiniMapRightOffset = 0f;
        private static readonly Vector2 MainUiMiniMapViewportOffset = Vector2.zero;
        private static readonly Vector2 MainUiMiniMapTrackCenterOffset = new Vector2(0f, 10f);
        private bool legacyMinimapBundleChecked;
        private float legacyMinimapBundleFactor = 1f;

        private string minimap_icon = "MiniMap_Icon";

        private ReferenceCollector LoadMinimapAtlasCollector()
        {
            string bundleName = minmapres.StringToAB();
            try
            {
                AssetBundleComponent.Instance.LoadBundle(bundleName);
                GameObject minmapPrefab = AssetBundleComponent.Instance.GetAsset(bundleName, minmapres) as GameObject;
                return minmapPrefab != null ? minmapPrefab.GetReferenceCollector() : null;
            }
            catch (Exception e)
            {
                Log.Error($"InitMiniMap load atlas collector failed bundle:{bundleName} error:{e.Message}");
                try
                {
                    AssetBundleComponent.Instance.UnloadBundle(bundleName);
                }
                catch
                {
                }
                return null;
            }
        }

        private void TryDeleteMinimapHotfixFile(string path)
        {
            try
            {
                if (!File.Exists(path))
                {
                    return;
                }

                File.Delete(path);
                Log.Warning($"InitMiniMap deleted bad hotfix atlas file: {path}");
            }
            catch (Exception e)
            {
                Log.Error($"InitMiniMap failed to delete bad hotfix atlas file: {path} error:{e.Message}");
            }
        }

        private bool TryReloadMinimapAtlasFromStreamingAssets(out ReferenceCollector atlasCollector)
        {
            atlasCollector = null;

            string bundleName = minmapres.StringToAB();
            string hotfixBundlePath = Path.Combine(PathHelper.AppHotfixResPath, bundleName);
            if (!File.Exists(hotfixBundlePath))
            {
                return false;
            }

            Log.Warning($"InitMiniMap fallback to streaming atlas after hotfix load failed: {hotfixBundlePath}");
            TryDeleteMinimapHotfixFile(hotfixBundlePath);
            TryDeleteMinimapHotfixFile($"{hotfixBundlePath}.manifest");
            AssetBundleComponent.Instance.UnloadBundle(bundleName);

            atlasCollector = LoadMinimapAtlasCollector();
            return atlasCollector != null;
        }

        private bool TryReadMinimapManifest(string manifestPath, out string manifestText)
        {
            manifestText = string.Empty;

            if (!File.Exists(manifestPath))
            {
                return false;
            }

            try
            {
                manifestText = File.ReadAllText(manifestPath);
                return !string.IsNullOrEmpty(manifestText);
            }
            catch (Exception e)
            {
                Log.Error($"InitMiniMap failed to inspect manifest path:{manifestPath} error:{e.Message}");
                return false;
            }
        }

        private float GetLegacyMinimapBundleFactor()
        {
            if (legacyMinimapBundleChecked)
            {
                return legacyMinimapBundleFactor;
            }

            legacyMinimapBundleChecked = true;
            legacyMinimapBundleFactor = 1f;

            string bundleName = minmapres.StringToAB();
            string[] manifestPaths =
            {
                Path.Combine(PathHelper.AppHotfixResPath, $"{bundleName}.manifest")
            };

            for (int i = 0; i < manifestPaths.Length; ++i)
            {
                string manifestPath = manifestPaths[i];
                if (!TryReadMinimapManifest(manifestPath, out string manifestText))
                {
                    continue;
                }

                if (manifestText.Contains(LegacyMinimapManifestHash))
                {
                    legacyMinimapBundleFactor = LegacyMinimapExpectedSize / LegacyMinimapPackedSpriteSize;
                }

                break;
            }

            return legacyMinimapBundleFactor;
        }

        public void InitMiniMap()
        {
            curSceneId = -1;
            legacyMinimapBundleChecked = false;
            legacyMinimapBundleFactor = 1f;
            Image miniMapImage = ReferenceCollector_Main.GetImage("MiniMap");
            GameObject miniMapRoot = miniMapImage != null ? miniMapImage.gameObject : null;
            if (miniMapRoot == null)
            {
                Log.Error("InitMiniMap missing MiniMap root image");
                return;
            }

            ApplyMiniMapRightEdgeLayout(miniMapImage.rectTransform);
            minmapRefrence = miniMapRoot.GetReferenceCollector();
            if (minmapRefrence == null)
            {
                Log.Error("InitMiniMap missing ReferenceCollector on MiniMap root");
                return;
            }

            BigMap = minmapRefrence.GetGameObject("BigMap");
            canvas = null;
            LimitContainer = null;
            if (BigMap != null)
            {
                BigMap.SetActive(false);
            }
            Log.DebugBrown($"InitMiniMap load atlas: {minmapres}");
            // 地图底图只是一个引用表，直接从 AB 里的 prefab 读取，避免实例化路径影响 ReferenceCollector。
            ReferenceCollector atlasCollector = LoadMinimapAtlasCollector();
            if (atlasCollector == null && !TryReloadMinimapAtlasFromStreamingAssets(out atlasCollector))
            {
                Log.Error($"InitMiniMap missing atlas prefab: {minmapres}");
            }

            if (atlasCollector != null)
            {
                MinMapDic.Clear();
                var allsprite = atlasCollector.GetKVAssets(MonoReferenceType.Sprite);
                if (allsprite == null || allsprite.Length == 0)
                {
                    Log.Error($"InitMiniMap missing atlas sprites: {minmapres}");
                }
                else
                {
                    for (int i = 0, length = allsprite.Length; i < length; i++)
                    {
                        var sprite = allsprite[i];
                        MinMapDic[sprite.Key] = (Sprite)sprite.Value;
                    }
                    Log.DebugBrown($"InitMiniMap atlas sprite count: {MinMapDic.Count}");
                }
            }

            // 闂佸憡姊绘慨鎯归崲鏀僶n
            miniMap_Icon = ResourcesComponent.Instance.LoadGameObject(minimap_icon.StringToAB(), minimap_icon);
            if (miniMap_Icon != null)
            {
                miniMap_Icon.SetActive(false);
                iconRefrence = miniMap_Icon.GetReferenceCollector();
                if (iconRefrence == null)
                {
                    Log.Error($"InitMiniMap missing icon collector: {minimap_icon}");
                }
            }
            else
            {
                iconRefrence = null;
                Log.Error($"InitMiniMap missing icon prefab: {minimap_icon}");
            }

            // 闂佸憡甯楃粙鎴犵磽閹绘niMap
            miniMap = new MiniMap();
            miniMap.Init(miniMapImage.transform);
            miniMap.SetViewportCenterOffset(MainUiMiniMapViewportOffset);
            miniMap.SetTrackCenterOffset(MainUiMiniMapTrackCenterOffset);
            miniMap.CanDrag = false;
            miniMap.CanZoom = false;
            miniMap.MiniMap_Icon = miniMap_Icon;
            miniMap.MapScale = new Vector3(3f, 3f, 3f);

            Button openBigMapBtn = minmapRefrence.GetButton("OpenBigMapBtn");
            if (openBigMapBtn != null)
            {
                openBigMapBtn.onClick.AddSingleListener(() =>
                {
                    ShowBigMap();
                });
            }

            Button closeBtn = minmapRefrence.GetButton("CloseBtn");
            if (closeBtn != null)
            {
                closeBtn.onClick.AddSingleListener(() =>
                {
                    //闂佺绻戞繛濠偽涚€涙ê绶炵憸宥咃耿閹绢喖鐐?
                    HideBigMap();
                });
            }
        }


        private void ApplyMiniMapRightEdgeLayout(RectTransform miniMapRootRect)
        {
            ApplyMiniMapRightEdgeOffset(miniMapRootRect, MainUiMiniMapRightOffset);
            RectTransform cameraRect = miniMapRootRect != null && miniMapRootRect.parent != null
                ? miniMapRootRect.parent.Find("camera") as RectTransform
                : null;
            ApplyMiniMapRightEdgeOffset(cameraRect, MainUiMiniMapRightOffset);
        }
        private static void ApplyMiniMapRightEdgeOffset(RectTransform rectTransform, float rightOffset)
        {
            if (rectTransform == null)
            {
                return;
            }
            if (rectTransform.anchorMax.x < 0.999f || rectTransform.pivot.x < 0.999f)
            {
                return;
            }
            if (rectTransform.anchorMin.x < 0.84f)
            {
                return;
            }
            Vector2 anchoredPosition = rectTransform.anchoredPosition;
            if (Mathf.Abs(anchoredPosition.x - rightOffset) < 0.01f)
            {
                return;
            }
            anchoredPosition.x = rightOffset;
            rectTransform.anchoredPosition = anchoredPosition;
        }
        void InitNPCInfo()
        {
            
            ETModel.Game.Scene.GetComponent<AssetBundleComponent>().LoadBundle(NPCRes.StringToAB());
            var npc = AssetBundleComponent.Instance.GetAsset(NPCRes.StringToAB(), NPCRes) as GameObject;
            var allnpcinfo = npc.GetReferenceCollector().GetKVAssets(MonoReferenceType.TextAsset);
           // Log.DebugBrown("闂佺懓鐏氶幐绋跨暤閸嶇c" + NPCRes);
            for (int i = 0, length = allnpcinfo.Length; i < length; i++)
            {
                var info = allnpcinfo[i];
                string str = (info.Value as TextAsset).text;
                NPCInfoDic[info.Key] = str;
            }
            ETModel.Game.Scene.GetComponent<AssetBundleComponent>().UnloadBundle(NPCRes.StringToAB());
         
        }

        void InitMonsterInfo() 
        {
            ETModel.Game.Scene.GetComponent<AssetBundleComponent>().LoadBundle(MonsterRes.StringToAB());
            var monster = AssetBundleComponent.Instance.GetAsset(MonsterRes.StringToAB(), MonsterRes) as GameObject;
            var allmonsterinfo = monster.GetReferenceCollector().GetKVAssets(MonoReferenceType.TextAsset);
            
            for (int i = 0, length = allmonsterinfo.Length; i < length; i++)
            {
                var info = allmonsterinfo[i];
                string str = (info.Value as TextAsset).text;
                MonsterInfoDic[info.Key] = str;
            }
            ETModel.Game.Scene.GetComponent<AssetBundleComponent>().UnloadBundle(MonsterRes.StringToAB());
        }

        void InitTransferPointsInfo()
        {
            ETModel.Game.Scene.GetComponent<AssetBundleComponent>().LoadBundle(TransferPointsRes.StringToAB());
            var transfer = AssetBundleComponent.Instance.GetAsset(TransferPointsRes.StringToAB(), TransferPointsRes) as GameObject;
            var alltransferinfo = transfer.GetReferenceCollector().GetKVAssets(MonoReferenceType.TextAsset);

            for (int i = 0, length = alltransferinfo.Length; i < length; i++)
            {
                var info = alltransferinfo[i];
                string str = (info.Value as TextAsset).text;
                TransferPointsInfoDic[info.Key] = str;
            }
            ETModel.Game.Scene.GetComponent<AssetBundleComponent>().UnloadBundle(TransferPointsRes.StringToAB());

        }

        private void UpdateMiniMapRoleIcon(bool force)
        {
            if (roleEntity == null || roleEntity.Game_Object == null || miniMap_RoleIcon == null)
            {
                return;
            }

            Vector3 rolePosition = roleEntity.Position;
            if (!force && (lastRolePosition - rolePosition).sqrMagnitude <= 0.1f)
            {
                return;
            }

            lastRolePosition = rolePosition;
            Vector2 iconPosition = new Vector2(rolePosition.x + 1f, rolePosition.z + 1f);
            Quaternion iconRotation = Quaternion.Euler(0, 0, -(this.roleEntity.Game_Object.transform.parent.rotation.eulerAngles.y + 135));

            miniMap_RoleIcon.Position = iconPosition;
            miniMap_RoleIcon.SetRotation(iconRotation);

            if (bigMap_RoleIcon != null)
            {
                bigMap_RoleIcon.Position = iconPosition;
                bigMap_RoleIcon.Rotation = iconRotation;
            }
        }

        public void UpdateMiniMap()
        {
            UpdateMiniMapRoleIcon(false);

            miniMap.Update();
            if(isOpenBigMap)
            {
                bigMap.Update();
            }
        }

        public void UpdateMiniMap(MiniMap map,int sceneId)
        {
            if (curSceneId == sceneId) return;
            lastSceneId = curSceneId;
            curSceneId = sceneId;

            Log.Debug($"MiniMap update begin sceneId:{sceneId} lastSceneId:{lastSceneId}");
            LoginStageTrace.Append($"MiniMap update begin sceneId={sceneId} lastSceneId={lastSceneId}");

            map.Clear();
            miniMap_RoleIcon = null;

            map.Init(ReferenceEquals(map, bigMap) && BigMap != null ? BigMap.transform : ReferenceCollector_Main.GetImage("MiniMap").transform);
            map.SetViewportCenterOffset(ReferenceEquals(map, miniMap) ? MainUiMiniMapViewportOffset : Vector2.zero);
            map.SetTrackCenterOffset(ReferenceEquals(map, miniMap) ? MainUiMiniMapTrackCenterOffset : Vector2.zero);
            SwitchMiniMap(map, sceneId);
            map.MapScale = new Vector3(3f, 3f, 3f);

            miniMap_RoleIcon = map.Create<MiniMap.IconRole>("role", 5);
            lastRolePosition = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            UpdateMiniMapRoleIcon(true);
            // 闁荤姾娅ｉ崰鏇㈡煂濞ｆ瓭on
            map.TrackIcon = miniMap_RoleIcon;
            map.SetCenterPosition(miniMap_RoleIcon.Position);

            map.OffsetTranform.SetSiblingIndex(1);

            map.Update();
        }

        #region pair
        class Pair<T1, T2>
        {
            public T1 Item1;
            public T2 Item2;
        }
        #endregion
        private void SwitchMiniMap(MiniMap map,int sceneId)
        {
            Sprite mapSprite=null;
           //  Log.DebugBrown("SwitchMiniMap" + map + ":::sceneid" + sceneId);
            Map_InfoConfig map_Info = ConfigComponent.Instance.GetItem<Map_InfoConfig>(sceneId);
            if (map_Info == null)
            {
                Log.Error($"SwitchMiniMap missing Map_InfoConfig sceneId:{sceneId}");
                LoginStageTrace.Append($"MiniMap config missing sceneId={sceneId}");
                map.UpdateMiniMap(null, Vector2Int.zero);
                return;
            }

            float effectiveScaleOffset =
                (map_Info.ScaleOffset > 0f && !float.IsNaN(map_Info.ScaleOffset) && !float.IsInfinity(map_Info.ScaleOffset))
                ? map_Info.ScaleOffset
                : 1f;

            Log.Debug(
                $"SwitchMiniMap sceneId:{sceneId} sceneName:{map_Info.SceneName} " +
                $"minimap:{map_Info.Minimap} offset:({map_Info.PosOffsetX},{map_Info.PosOffsetY}) scale:{map_Info.ScaleOffset}");
            LoginStageTrace.Append(
                $"MiniMap switch sceneId={sceneId} sceneName={map_Info.SceneName} " +
                $"minimap={map_Info.Minimap} offset=({map_Info.PosOffsetX},{map_Info.PosOffsetY}) " +
                $"scaleRaw={map_Info.ScaleOffset:0.###} scaleEffective={effectiveScaleOffset:0.###}");

            if (string.IsNullOrWhiteSpace(map_Info.Minimap))
            {
                Log.Error($"SwitchMiniMap invalid minimap key sceneId:{sceneId} sceneName:{map_Info.SceneName}");
                LoginStageTrace.Append($"MiniMap invalid minimap key sceneId={sceneId} sceneName={map_Info.SceneName}");
            }

            if (map_Info.ScaleOffset <= 0f || float.IsNaN(map_Info.ScaleOffset) || float.IsInfinity(map_Info.ScaleOffset))
            {
                Log.Error(
                    $"SwitchMiniMap invalid calibration scale sceneId:{sceneId} sceneName:{map_Info.SceneName} " +
                    $"scale:{map_Info.ScaleOffset}");
                LoginStageTrace.Append(
                    $"MiniMap invalid calibration scale sceneId={sceneId} sceneName={map_Info.SceneName} " +
                    $"scaleRaw={map_Info.ScaleOffset:0.###} fallback={effectiveScaleOffset:0.###}");
            }

            if (Mathf.Abs(map_Info.PosOffsetX) > 1024 || Mathf.Abs(map_Info.PosOffsetY) > 1024)
            {
                Log.Warning(
                    $"SwitchMiniMap suspicious calibration offset sceneId:{sceneId} sceneName:{map_Info.SceneName} " +
                    $"offset:({map_Info.PosOffsetX},{map_Info.PosOffsetY})");
                LoginStageTrace.Append(
                    $"MiniMap suspicious calibration offset sceneId={sceneId} sceneName={map_Info.SceneName} " +
                    $"offset=({map_Info.PosOffsetX},{map_Info.PosOffsetY})");
            }

            if (MinMapDic.TryGetValue(map_Info.Minimap, out Sprite sprite))
            {
                mapSprite = sprite;
            }
            else
            {
                //  Log.DebugRed($"map_Info.SceneName:{map_Info.Minimap}  婵炴垶鎸哥粔鎾偤閵娾晛鎹?);
             //   mapSprite = MinMapDic.ElementAt(0).Value;
            }

            if (mapSprite == null)
            {
                Log.Error(
                    $"SwitchMiniMap missing minimap sprite:{map_Info.Minimap} sceneId:{sceneId} " +
                    $"availableCount:{MinMapDic.Count}");
                LoginStageTrace.Append(
                    $"MiniMap sprite missing sceneId={sceneId} sceneName={map_Info.SceneName} " +
                    $"minimap={map_Info.Minimap} availableCount={MinMapDic.Count}");
            }

            Vector2 effectiveCalibrationOffset = new Vector2(map_Info.PosOffsetX, map_Info.PosOffsetY);
            float legacyBundleFactor = GetLegacyMinimapBundleFactor();
            if (legacyBundleFactor > 1.001f)
            {
                effectiveCalibrationOffset *= legacyBundleFactor;
                effectiveScaleOffset *= legacyBundleFactor;
            }

            map.ConfigureMapCalibration(effectiveCalibrationOffset, effectiveScaleOffset);
            map.UpdateMiniMap(mapSprite, Vector2Int.zero);


            //闂佸搫瀚晶浠嬪Φ濮濅揪C
            ClearNpcPoint();
            if (NPCInfoDic.Count == 0)
            {
                InitNPCInfo();
            }
            if (NPCInfoDic.TryGetValue($"{map_Info.Minimap}_NPCSpawnPoints", out string npcinfos))
            {
                NPCSpawnPointLists.Clear();
                SpawnPoint[] spawnPoint = JsonMapper.ToObject<SpawnPoint[]>(npcinfos);
                for (int i = 0, length = spawnPoint.Length; i < length; i++)
                {
                    ShowNpcPoint(map, spawnPoint[i]);
                }

            }
            else
            {
                NPCSpawnPointLists.Clear();

            }

            //闂佸搫瀚晶浠嬪Φ濮濅狗nster
            CleanMonster();
            if (MonsterInfoDic.Count == 0)
            {
                InitMonsterInfo();
            }
            if (MonsterInfoDic.TryGetValue($"{map_Info.Minimap}_MonsterSpawnPoints_text", out string monsterinfos))//YongZheDaLu_MonsterSpawnPoints
            {
                // 闂佽鍓︽禍鐐存櫠缁侯槉 2 闂佺鍕闁?
                MultiMap<int, Vector2> createMonsterDict = new MultiMap<int, Vector2>();
                var dict = createMonsterDict.GetDictionary();
                float sqrDistance = 90f * 90f;  // 闁荤姷鍎ょ换鍕€?90 闂佸搫绉烽崑鎰枔閹达附鍎庣紒瀣閸婇亶鏌熼澶夌敖濠⒀勫缁嬪顓奸崟顓犵崶闂備焦褰冪粔鎾囬弻銉ュ強闁告挆浣风驳

                // 闂佺鍕闁?2 闂佽鍓︽禍鐐存櫠?
                MultiMap<(float, float), SpawnPoint> showMonsterPointDict = new MultiMap<(float, float), SpawnPoint>();
                List<Vector2> allShowPoint = new List<Vector2>();   // 闂佸湱顣介崑鎾绘煛閸繍妯€婵炴挸澧庨幉鐗堟媴鐟欏嫧鏋栫紓浣插亾闁惧繗顫夐悾閬嶆煕瑜庨崝鏍偉閿濆鍊?
                float groupSqrDistance = 5f * 5f;   // 闁荤姷鍎ょ换鍕€?5 闂佸搫绉烽崑鎰枔閹达箑绠戞い蹇撴閳锋牕霉閸忚壈澹橀柟顔奸叄閻涱喚鎹勯崫鍕帓婵炴垶鎸撮崑鎾斥槈閹垮啩绨介柛瀣剁秮瀵即宕滆娴?


                SpawnPoint[] spawnPoint = JsonMapper.ToObject<SpawnPoint[]>(monsterinfos);
                for (int i = 0, length = spawnPoint.Length; i < length; i++)
                {
                    Vector2 pos = new Vector2(spawnPoint[i].PositionX, spawnPoint[i].PositionY);
                    bool canShow = true;
                    if(dict.TryGetValue((int)spawnPoint[i].Index,out var list))
                    {
                        for(int n = 0;n< list.Count;++n)
                        {
                            if((list[n] - pos).sqrMagnitude < sqrDistance)
                            {
                                canShow = false;
                                break;
                            }
                        }
                    }
                    if(canShow)
                    {
                        createMonsterDict.Add((int)spawnPoint[i].Index, pos);
                        foreach(Vector2 v2 in allShowPoint)
                        {
                            if((pos-v2).sqrMagnitude < groupSqrDistance)
                            {
                                showMonsterPointDict.Add((v2.x,v2.y), spawnPoint[i]);
                                canShow = false;
                                break;
                            }
                        }
                        if(canShow)
                        {
                            allShowPoint.Add(pos);
                            showMonsterPointDict.Add((pos.x, pos.y), spawnPoint[i]);
                        }
                    }
                }
                // 闂佸搫瀚晶浠嬪Φ濮樿泛绠戞い蹇撴閳?
                foreach(var kv in showMonsterPointDict.GetDictionary())
                {
                    ShowMonsterPoint(map, kv.Value);
                }

                //  PreLoadComponent.Instance.UnLoad(lastSceneId);
            }
            // 闂佸搫瀚晶浠嬪Φ濮橆厼顕遍柣妯煎劦閸嬫挻鎷呴崫鍕寲
            if(TransferPointsInfoDic.Count == 0)
            {
                InitTransferPointsInfo();
            }
            if(TransferPointsInfoDic.TryGetValue($"{map_Info.Minimap}_TransferPoint",out string transferpointinfos))
            {
                SpawnPoint[] spawnPoint = JsonMapper.ToObject<SpawnPoint[]>(transferpointinfos);
                Dictionary<long, Pair<Vector2, int>> alltransferPoint = new Dictionary<long, Pair<Vector2, int>> ();
                for (int i = 0, length = spawnPoint.Length; i < length; i++)
                {
                    if (alltransferPoint.TryGetValue(spawnPoint[i].Index,out var value))
                    {
                        value.Item2++;
                        value.Item1.x += spawnPoint[i].PositionX;
                        value.Item1.y += spawnPoint[i].PositionY;
                    }
                    else
                    {
                        Pair<Vector2, int> pair = new Pair<Vector2, int>();
                        pair.Item1 = new Vector2(spawnPoint[i].PositionX, spawnPoint[i].PositionY);
                        pair.Item2 = 1;
                        alltransferPoint[spawnPoint[i].Index] = pair;
                    }
                }
                foreach(var kv in alltransferPoint)
                {
                    kv.Value.Item1.x /= kv.Value.Item2;
                    kv.Value.Item1.y /= kv.Value.Item2;
                    ShowTransterPoint(map, kv.Key, kv.Value.Item1);
                }
            }


            map.Update();
        }

        /// <summary>
        /// 闂佸憡姊绘慨鎯归崶鈹惧亾鐟欏嫮绠氭俊顐㈢焸瀹曟悂骞囬鐘茬伇
        /// </summary>
        /// <param name="map"></param>
        /// <param name="sceneId"></param>
        private void LoadTreasurePoint(MiniMap map,int sceneId)
        {
            foreach(var kv in TreasureMapComponent.Instance.AllPoint)
            {
                if(kv.Value.MapId == sceneId)
                {
                    MiniMap.IconTreasurePoint treasureIcon = map.Create<MiniMap.IconTreasurePoint>($"Treasure_{kv.Value.TreasureType}", 6);
                    treasureIcon.Position = Grid2MiniMapPosition(kv.Value.PosX, kv.Value.PosY);
                    switch (kv.Value.TreasureType)
                    {
                        case ETreasureType.TreasureMap:
                            {
                                Npc_InfoConfig npc_Info = ConfigComponent.Instance.GetItem<Npc_InfoConfig>(kv.Value.NpcConfigId);
                                treasureIcon.image.sprite = iconRefrence.GetSprite("treasure_map");
                                treasureIcon.text.text = $"\u85cf\u5b9d\u56fe [<color=red>{(npc_Info != null ? npc_Info.Name : "\u672a\u77e5\u7684")}</color>]";
                            }
                            break;
                        case ETreasureType.LongWangBaoZang:
                            {
                                EnemyConfig_InfoConfig enemy_Info = ConfigComponent.Instance.GetItem<EnemyConfig_InfoConfig>(kv.Value.NpcConfigId);
                                treasureIcon.image.sprite = iconRefrence.GetSprite("dragon_2");
                                treasureIcon.text.text = $"\u9f99\u738b\u5b9d\u85cf [<color=red>{(enemy_Info != null ? enemy_Info.Name : "\u672a\u77e5\u7684")}</color>]";
                            }
                            break;
                        case ETreasureType.FuHuaDan:
                            {
                                EnemyConfig_InfoConfig enemy_Info = ConfigComponent.Instance.GetItem<EnemyConfig_InfoConfig>(kv.Value.NpcConfigId);
                                treasureIcon.image.sprite = iconRefrence.GetSprite("egg_2");
                                treasureIcon.text.text = $"\u8d64\u7130\u517d\u5b75\u5316\u86cb [<color=red>{(enemy_Info != null ? enemy_Info.Name : "\u672a\u77e5\u7684")}</color>]";
                            }
                            break;
                        case ETreasureType.NaJie:
                            {
                                EnemyConfig_InfoConfig enemy_Info = ConfigComponent.Instance.GetItem<EnemyConfig_InfoConfig>(kv.Value.NpcConfigId);
                                treasureIcon.image.sprite = iconRefrence.GetSprite("monster_1");
                                treasureIcon.text.text = $"\u7eb3\u6212 [<color=red>{(enemy_Info != null ? enemy_Info.Name : "\u672a\u77e5\u7684")}</color>]";
                            }
                            break;
                        case ETreasureType.XiaoTianShi:
                            {
                                EnemyConfig_InfoConfig enemy_Info = ConfigComponent.Instance.GetItem<EnemyConfig_InfoConfig>(kv.Value.NpcConfigId);
                                treasureIcon.image.sprite = iconRefrence.GetSprite("monster_1");
                                treasureIcon.text.text = $"\u5c0f\u5929\u4f7f [<color=red>{(enemy_Info != null ? enemy_Info.Name : "\u672a\u77e5\u7684")}</color>]";
                            }
                            break;
                        default:
                            break;
                    
                    }

                }

            }

            
        }

        public void OnLocalRoleMoveInDetail(AstarNode astar)
        {
            if(bigMap_NavList.Count != 0)
            {
                Vector2 pos = AstarNode2MiniMapPosition(astar);
                Vector2 pos2 = bigMap_NavList.First();
                if ((pos2 - pos).sqrMagnitude > 0.5f)
                {
                    bigMap_NavList.RemoveFirst();
                    if(isOpenBigMap)
                    {
                        MiniMap.IconNavPoint nav = bigMap_NavIconList.First();
                        bigMap_NavIconList.RemoveFirst();
                        nav.Dispose();
                    }
                }
            }
        }


        /// <summary>
        /// 闂佸搫瀚晶浠嬪Φ濮樺彉鐒婇煫鍥ㄦ尰閸曢箖鏌涢妷锕€绀冮柣顓熷劤椤曘儵宕熼鐐板寲
        /// </summary>
        /// <param name="astars"></param>

        private void UpdateBigMapNavigationIcons(List<AstarNode> astars)
        {
            if (astars == null || astars.Count == 0 || isOpenBigMap == false || bigMap == null)
            {
                return;
            }

            foreach (var icon in bigMap_NavIconList)
            {
                icon.Dispose();
            }
            bigMap_NavIconList.Clear();
            bigMap_NavList.Clear();

            foreach (AstarNode node in astars)
            {
                MiniMap.IconNavPoint navIcon = bigMap.Create<MiniMap.IconNavPoint>("nav_point", 1);
                navIcon.Position = AstarNode2MiniMapPosition(node);
                bigMap_NavIconList.AddLast(navIcon);
                bigMap_NavList.AddLast(navIcon.Position);
            }
            if (bigMap_TargetPointIcon == null)
            {
                bigMap_TargetPointIcon = bigMap.Create<MiniMap.IconTargetPoint>("target_point", 1);
            }
            bigMap_TargetPointIcon.Position = bigMap_NavList.Last();
        }

        private void ClearBigMapNavigationIcons()
        {
            bigMap_TargetPointIcon?.Dispose();
            bigMap_TargetPointIcon = null;
            foreach (var icon in bigMap_NavIconList)
            {
                icon.Dispose();
            }

            bigMap_NavIconList.Clear();
            bigMap_NavList.Clear();
        }        private void AstarNavCallback(List<AstarNode> astars)
        {
            if (astars.Count == 0) return;
            unitEntityPathComponent.StopMove();
            if (RoleOnHookComponent.Instance.IsOnHooking) 
            {
                HookTog.isOn = false;
            }
                async ETVoid StartMove()
            {

                roleEntity.GetComponent<RoleMoveControlComponent>().IsNavigation = true;
                await unitEntityPathComponent.StartMove(astars);
                roleEntity.GetComponent<RoleMoveControlComponent>().IsNavigation = false;
                // 闂佸搫鐗滈崜姘额敃閼测晝鐭撻悹鍥ㄥ絻琚熺紓鍌欑劍閹歌顭?
                // 濠电偞鎸搁幊妯衡枍鎼淬劍鍊?
                bigMap_TargetPointIcon?.Dispose();
                bigMap_TargetPointIcon = null;
                foreach (var icon in bigMap_NavIconList) icon.Dispose();
                bigMap_NavIconList.Clear();
                bigMap_NavList.Clear();
            }
            StartMove().Coroutine();
            if (isOpenBigMap == false) return;
            foreach (var icon in bigMap_NavIconList)
            {
                icon.Dispose();
            }
            bigMap_NavIconList.Clear();
            bigMap_NavList.Clear();

            foreach (AstarNode node in astars)
            {
                MiniMap.IconNavPoint navIcon = bigMap.Create<MiniMap.IconNavPoint>("nav_point", 1);
                navIcon.Position = AstarNode2MiniMapPosition(node);
                bigMap_NavIconList.AddLast(navIcon);
                bigMap_NavList.AddLast(navIcon.Position);
            }
            // 闂佸搫瀚晶浠嬪Φ濮樿埖鍎庢い鏃傛櫕閸ㄥジ鏌?
            if(bigMap_TargetPointIcon == null)
            {
                bigMap_TargetPointIcon = bigMap.Create<MiniMap.IconTargetPoint>("target_point", 1);
            }
            bigMap_TargetPointIcon.Position = bigMap_NavList.Last();
        }

        public void ShowNpcPoint(MiniMap map,SpawnPoint spawnPoint)
        {
           // Log.DebugBrown("ShowNpcPoint" + spawnPoint.Index);
            var nocinfo = ConfigComponent.Instance.GetItem<Npc_InfoConfig>((int)spawnPoint.Index);
            if(string.IsNullOrEmpty(nocinfo.Icon) == false)
            {
                MiniMap.IconNpc npc = map.Create<MiniMap.IconNpc>($"NPC_{nocinfo.Id}", 6);
                npc.Position = Grid2MiniMapPosition(spawnPoint.PositionX, spawnPoint.PositionY);
                npc.image.sprite = iconRefrence.GetSprite(nocinfo.Icon);
                npc.text.text = nocinfo.Name;
                NPCSpawnPointLists.Add(spawnPoint);
               // Log.DebugBrown("name" + nocinfo.Name + ":::" + spawnPoint.PositionX + ":::" + spawnPoint.PositionY+":::icon"+ nocinfo.Icon);
            }
           

        }


        public void ShowMonsterPoint(MiniMap map,List<SpawnPoint> spawnPointList)
        {

            float Remap(float x, float t1, float t2, float s1, float s2)
            {
                return (s2 - s1) / (t2 - t1) * (x - t1) + s1;
            }

            string text = "";
            for(int i=0;i<spawnPointList.Count;++i)
            {
                SpawnPoint spawnPoint = spawnPointList[i];
                var monsterinfo = ConfigComponent.Instance.GetItem<EnemyConfig_InfoConfig>((int)spawnPoint.Index);
                if (monsterinfo != null)
                {
                    int level = (int)(Remap(monsterinfo.Lvl, 0, 150, 0, 500));
                    //int minLevel = level - 25;
                   // int maxLevel = level + 25;
                   // if (minLevel < 1) minLevel = 1;
                  //  if (i != 0) text += "\n";
                    text += $"<color=red>{monsterinfo.Name}</color> [闂佽鍓︽禍鐐存櫠鐠恒劎椹冲璺虹焸閻?<color=#00c000>Lv.{monsterinfo.Lvl}</color>]";
                }
            }
            //Log.Info($"spawnPointList.Count  = {spawnPointList.Count} {text}");

        

            MiniMap.IconMonster monster = map.Create<MiniMap.IconMonster>($"monster_{spawnPointList[0].Index}", 6);
            monster.Position = Grid2MiniMapPosition(spawnPointList[0].PositionX, spawnPointList[0].PositionY);
            monster.text.text = text;

            //PreLoadComponent.Instance.PreLoad(curSceneId, monsterinfo.ResName);
        }

        public void ShowTransterPoint(MiniMap map, long configId, Vector2 pos)
        {
            //if (monsterPoint.ContainsKey(spawnPoint.Index)) return;
            var transferinfo = ConfigComponent.Instance.GetItem<Map_TransferPointConfig>((int)configId);
            if(transferinfo != null && transferinfo.TargetIndex != 0)
            {
                MiniMap.IconTransterPoint transferpoint = map.Create<MiniMap.IconTransterPoint>($"transterpoint_{configId}", 7);
                transferpoint.Position = Grid2MiniMapPosition(pos);
                transferpoint.text.text = transferinfo.NameInMinimap;
            }
        }

        public void ClearNpcPoint()
        {
            for (int i = 0, length = npcPoint.Count; i < length; i++)
            {
                GameObject.Destroy(npcPoint.ElementAt(i).Value);
            }
            npcPoint.Clear();
            NPCSpawnPointLists.Clear();
            unitEntityPathComponent = null;
        }
        public void CleanMonster()
        {
            for (int i = 0; i < monsterPoint.Count; i++)
            {
                GameObject.Destroy(monsterPoint.ElementAt(i).Value);
            }
            monsterPoint.Clear();
        }


        public Vector2 Grid2MiniMapPosition(Vector2 pos)
        {
            return new Vector2(pos.x * 2 + 1, pos.y * 2 + 1);
        }
        public Vector2 Grid2MiniMapPosition(int x,int y)
        {
            return new Vector2(x * 2 + 1, y * 2 + 1);
        }
        public Vector2 AstarNode2MiniMapPosition(AstarNode node)
        {
            return new Vector2(node.x * 2 + 1, node.z * 2 + 1);
        }
    }
}
