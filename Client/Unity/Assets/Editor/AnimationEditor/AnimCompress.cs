using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ETEditor
{
    public class AnimCompress : AssetPostprocessor
    {
        public const string ScaleKeyName = "localscale";
        public const string RotationKeyName = "localrotation";

        void OnPostprocessModel(GameObject go)
        {
            List<AnimationClip> clips = new List<AnimationClip>(AnimationUtility.GetAnimationClips(go));

            if (clips.Count == 0)
            {
                AnimationClip[] objectList = Object.FindObjectsOfType(typeof(AnimationClip)) as AnimationClip[];
                if (objectList != null)
                {
                    clips.AddRange(objectList);
                }
            }

            for (int i = 0; i < clips.Count; i++)
            {
                AnimationEditor.CompressAnim(clips[i]);
            }
        }


    }
}