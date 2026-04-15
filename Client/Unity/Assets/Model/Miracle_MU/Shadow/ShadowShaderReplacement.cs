using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETModel
{
    [ExecuteInEditMode]
    public class ShadowShaderReplacement : MonoBehaviour
    {
        public Shader replacementShader;
        private Camera mcamera;

        private void OnEnable()
        {
            mcamera = GetComponent<Camera>();
            replacementShader = Shader.Find("ShadowSystem/ReplacementShadow");
            if (replacementShader != null)
            {
                //mcamera.SetReplacementShader(replacementShader, "RenderType");
                mcamera.SetReplacementShader(replacementShader, "");
            }
        }
        private void OnDisable()
        {
            mcamera.ResetReplacementShader();
        }
    }
}