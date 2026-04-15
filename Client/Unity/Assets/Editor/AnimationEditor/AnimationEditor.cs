using ETModel;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ETEditor
{

    public static class AnimationEditor
    {
     //   [MenuItem("Assets/压缩动画精度")]
        public static void OnPostprocessModel() 
        {
            List<string> paths = GetPrefabsAndScenes(AssetDatabase.GetAssetPath(Selection.objects[0]));
            Log.DebugBrown($"{paths}");
            foreach (var path in paths)
            {
                string path1 = path.Replace('\\', '/');
                //当前预制体
                AnimationClip anim = AssetDatabase.LoadAssetAtPath<AnimationClip>(path1);
                Log.DebugBrown($"{path1}->{anim}");
               /* //去除scale曲线
                foreach (EditorCurveBinding theCurveBinding in AnimationUtility.GetCurveBindings(anim))
                {
                    string name = theCurveBinding.propertyName.ToLower();
                    if (name.Contains("scale"))
                    {
                        AnimationUtility.SetEditorCurve(anim, theCurveBinding, null);
                    }

                }*/


                //浮点数精度压缩到f3
                EditorCurveBinding[] bindings = AnimationUtility.GetCurveBindings(anim);

                for (int j = 0; j < bindings.Length; j++)
                {
                    EditorCurveBinding curveBinding = bindings[j];
                    AnimationCurve curve = AnimationUtility.GetEditorCurve(anim, curveBinding);

                    if (curve == null || curve.keys == null)
                    {
                        continue;
                    }

                    Keyframe[] keys = curve.keys;
                    for (int k = 0; k < keys.Length; k++)
                    {
                        Keyframe key = keys[k];
                        key.value = float.Parse(key.value.ToString("f3"));
                        key.inTangent = float.Parse(key.inTangent.ToString("f3"));
                        key.outTangent = float.Parse(key.outTangent.ToString("f3"));
                        keys[k] = key;
                    }
                    curve.keys = keys;

                    AnimationUtility.SetEditorCurve(anim, curveBinding, curve);

                }
            }
        }
        [MenuItem("Assets/压缩动画精度")]
        public static void CompressAniamtion()
        {
            List<string> paths = GetPrefabsAndScenes(AssetDatabase.GetAssetPath(Selection.objects[0]));
            Log.DebugBrown($"{paths}");
            foreach (var path in paths)
            {
                string path1 = path.Replace('\\', '/');
                AnimationClip anim = AssetDatabase.LoadAssetAtPath<AnimationClip>(path1);
                CompressAnim(anim);
            }
            EditorUtility.ClearProgressBar();
        }
        
        [MenuItem("Tools/Animation/CompressAnim &v")]
        public static void CompressAnimMenu()
        {
            try
            {
                var selectGos = Selection.gameObjects;
                for (int i = 0; i < selectGos.Length; i++)
                {
                    EditorUtility.DisplayProgressBar("CompressAnim", selectGos[i].name + " Compressing...", ((float)i / selectGos.Length));
                    AnimationClip[] clips = GetAnimationClips(selectGos[i]);
                    foreach (var c in clips)
                    {
                        CompressAnim(c);
                    }
                }
            }
            catch (Exception)
            {
                EditorUtility.ClearProgressBar();
                throw;
            }
            EditorUtility.ClearProgressBar();
        }
        public static void CompressAnim(AnimationClip clip)
        {
            ReduceScaleKey(clip, "localscale");
            ReduceFloatPrecision(clip);
        }

        /// 获取GameObject上的AnimationClips
        /// </summary>
        public static AnimationClip[] GetAnimationClips(GameObject go)
        {
            List<AnimationClip> clips = new List<AnimationClip>();

            string path = AssetDatabase.GetAssetPath(go);
            Object[] objects = AssetDatabase.LoadAllAssetsAtPath(path);

            foreach (var o in objects)
            {
                if (o is AnimationClip)
                {
                    clips.Add((AnimationClip)o);
                }
            }

            return clips.ToArray();
        }


        public static List<string> GetPrefabsAndScenes(string srcPath)
        {
            List<string> paths = new List<string>();
            FileHelper.GetAllFiles(paths, srcPath);

            List<string> files = new List<string>();
            foreach (string str in paths)
            {
                if (str.EndsWith(".anim"))
                {
                    files.Add(str);
                }
            }
            return files;
        }

        //删减帧
        private static void ReduceScaleKey(AnimationClip clip, string keyName)
        {
            EditorCurveBinding[] curves = AnimationUtility.GetCurveBindings(clip);

            for (int j = 0; j < curves.Length; j++)
            {
                EditorCurveBinding curveBinding = curves[j];

                if (curveBinding.propertyName.ToLower().Contains(keyName))
                {
                    AnimationUtility.SetEditorCurve(clip, curveBinding, null);
                }
            }
        }
        //动画精度压缩
        private static void ReduceFloatPrecision(AnimationClip clip)
        {
            EditorCurveBinding[] bindings = AnimationUtility.GetCurveBindings(clip);

            for (int j = 0; j < bindings.Length; j++)
            {
                EditorCurveBinding curveBinding = bindings[j];
                AnimationCurve curve = AnimationUtility.GetEditorCurve(clip, curveBinding);

                if (curve == null || curve.keys == null)
                {
                    continue;
                }

                Keyframe[] keys = curve.keys;
                for (int k = 0; k < keys.Length; k++)
                {
                    Keyframe key = keys[k];
                    key.value = float.Parse(key.value.ToString("f3"));
                    key.inTangent = float.Parse(key.inTangent.ToString("f3"));
                    key.outTangent = float.Parse(key.outTangent.ToString("f3"));
                    keys[k] = key;
                }
                curve.keys = keys;

                AnimationUtility.SetEditorCurve(clip, curveBinding, curve);
            }
        }
    }
}
