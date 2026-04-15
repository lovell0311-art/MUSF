using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETModel
{
    /// <summary>
    ///  屏幕分辨率控制类
    /// </summary>
    public class CamreaController : MonoBehaviour
    {
        //想要的 宽比
        float ScaleWithWidth = 16f;
        //想要的 高比
        float ScaleWithHight = 9f;

        private void Start()
        {
            ScreeneResolution();
        }

        private Camera MAIN_CAMERA;
        private float rectHight;
        private float rectwidth;
        private float widthShoudSize;
        private float heightShoudSize;

        private void ScreeneResolution()
        {
            MAIN_CAMERA = GetComponent<Camera>();

            float screenWidth = Screen.width;
            float screenheight = Screen.height;

            widthShoudSize = screenheight / ScaleWithHight * ScaleWithWidth;
            heightShoudSize = screenWidth / ScaleWithWidth * ScaleWithHight;

            rectwidth = widthShoudSize / screenWidth;
            rectHight = heightShoudSize / screenheight;

            if (Screen.width <= Screen.height)
                MAIN_CAMERA.rect = new Rect(0, (1f - rectHight) / 2f, 1, rectHight);
            else
                MAIN_CAMERA.rect = new Rect((1f - rectwidth) / 2f, 0, rectwidth, 1);
        }   

    }
}
