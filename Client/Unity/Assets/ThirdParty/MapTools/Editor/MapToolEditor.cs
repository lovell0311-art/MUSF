using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR_WIN
public class MapToolEditor 
{
    [MenuItem("Tools/地图工具/刷可行走区域", false)]
    static void CanMoveArea() 
    {
        GameObject.Instantiate<GameObject>(AssetDatabase.LoadAssetAtPath<GameObject>(@"Assets\ThirdParty\MapTools\HMMapCreater\MapJsonPrefab.prefab"));
       
    }
    [MenuItem("Tools/地图工具/刷怪物", false)]
    static void Monster()
    {

        
        GameObject.Instantiate<GameObject>(AssetDatabase.LoadAssetAtPath<GameObject>(@"Assets\ThirdParty\MapTools\MonsterSpanw\CreatMonsterPoints.prefab"));
    }
    [MenuItem("Tools/地图工具/刷NPC", false)]
    static void NPC()
    {

       
        GameObject.Instantiate<GameObject>(AssetDatabase.LoadAssetAtPath<GameObject>(@"Assets\ThirdParty\MapTools\NPCSpanw\CreatNPCPoints.prefab"));
    }
    [MenuItem("Tools/地图工具/玩家出生区域", false)]
    static void RoleSpawn()
    {

       
        GameObject.Instantiate<GameObject>(AssetDatabase.LoadAssetAtPath<GameObject>(@"Assets\ThirdParty\MapTools\CreatMapSpawnPoints\CreatMapSpawnPoints.prefab"));
    } 
    [MenuItem("Tools/地图工具/安全区", false)]
    static void RoleSafeAreas()
    {

       
        GameObject.Instantiate<GameObject>(AssetDatabase.LoadAssetAtPath<GameObject>(@"Assets\ThirdParty\MapTools\SafeAreas\SafeAreas.prefab"));
    }
    [MenuItem("Tools/地图工具/传送区域", false)]
    static void TransferPoints()
    {

       
        GameObject.Instantiate<GameObject>(AssetDatabase.LoadAssetAtPath<GameObject>(@"Assets\ThirdParty\MapTools\TransferPoints\TransferPoints.prefab"));
    } 
    [MenuItem("Tools/地图工具/任务", false)]
    static void TaskPoints()
    {
        GameObject.Instantiate<GameObject>(AssetDatabase.LoadAssetAtPath<GameObject>(@"Assets\ThirdParty\MapTools\TaskPoint\TaskPoint.prefab"));
    }
}
#endif
