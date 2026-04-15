using System.Text.RegularExpressions;
using System;
using UnityEditor;
using UnityEngine;
namespace ETEditor
{

#if UNITY_ANDROID
    /// <summary>
    /// 自动设置导入资源属性选项（模型、图片、音效）
    /// </summary>
    public class EditorResourceSetting : AssetPostprocessor
    {
        #region 模型处理
        /// <summary>
        /// 模型导入之前调用
        /// </summary>
        private void OnPreprocessModel_()
        {
            Debug.Log("模型导入之前调用=" + this.assetPath);
            ModelImporter modelImporter = (ModelImporter)assetImporter;
            //模型优化
            modelImporter.isReadable = false;
            modelImporter.optimizeMeshVertices = true;
            modelImporter.optimizeGameObjects = true;
            modelImporter.meshCompression = ModelImporterMeshCompression.High;
            modelImporter.animationCompression = ModelImporterAnimationCompression.Optimal;
            modelImporter.animationRotationError = 1.0f;
            modelImporter.animationScaleError = 1.0f;
            modelImporter.animationPositionError = 1.0f;

        }
        /// <summary>
        /// 模型导入之后 调用
        /// </summary>
        /// <param name="gameObject"></param>

        private void OnPostprocessModel_(GameObject gameObject)
        {
            Debug.Log("模型导入之后调用  ");

            //关闭fbx的Skinned Motion Vetor
            SkinnedMeshRenderer[] skinnedMeshRenderers = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
            for (int i = 0; i < skinnedMeshRenderers.Length; i++)
            {
                skinnedMeshRenderers[i].skinnedMotionVectors = false;
            }

        }
        #endregion
        public void OnPreprocessAudio_()
        {
            Debug.Log("音频导前预处理");

            AudioImporterSampleSettings AudioSetting = new AudioImporterSampleSettings();

            //加载方式选择
            AudioSetting.loadType = AudioClipLoadType.CompressedInMemory;
            //压缩方式选择
            AudioSetting.compressionFormat = AudioCompressionFormat.Vorbis;
            //设置播放质量
            AudioSetting.quality = 0.1f;
            //优化采样率
            AudioSetting.sampleRateSetting = AudioSampleRateSetting.OptimizeSampleRate;


            AudioImporter audio = assetImporter as AudioImporter;
            //开启单声道 
            audio.forceToMono = true;
            audio.preloadAudioData = true;
            audio.defaultSampleSettings = AudioSetting;

        }
        public void OnPostprocessAudio_(AudioClip clip)
        {
            Debug.Log("音频导后处理");
        }
        /// <summary>
        /// 检索 Teture的类型是否为Sprite
        /// </summary>
        private string RetrivalTextureType = "[_][Uu][Ii]";
        /// <summary>
        /// 检索Texture的MaxSize大小
        /// </summary>
        private string RetrivalTextureMaxSize = @"[&]\d{2,4}";
        /// <summary>
        /// 检索Texture是否为带Alpha通道
        /// </summary>
        private string RetrivalTextureIsAlpha = @"[Aa][Ll][Pp][Hh][Aa]";

        
        public void OnPostprocessTexture_(Texture2D texture)
        {
            
            TextureImporter textureImporter = (TextureImporter)assetImporter;
            string dirName = System.IO.Path.GetDirectoryName(assetPath);
            Debug.Log(dirName);//从Asset开始的目录信息
            if (!dirName.Contains("UI"))
            {

                textureImporter.isReadable = false;
                textureImporter.mipmapEnabled = true;
                textureImporter.streamingMipmaps = true;
                textureImporter.textureType = TextureImporterType.Default;
            }
           



        }
        /// <summary>
        /// 纹理导入之前调用 对纹理进行设置
        /// </summary>
        public void OnPostprocessTexture_()
        {

            TextureImporter textureImporter = (TextureImporter)assetImporter;
            textureImporter.isReadable = false;
            textureImporter.mipmapEnabled = true;
            textureImporter.streamingMipmaps = true;

            /* TextureImporter textureImporter = (TextureImporter)assetImporter;
             textureImporter.isReadable = false;

             string dirName = System.IO.Path.GetDirectoryName(assetPath);
             Debug.Log(dirName);//从Asset开始的目录信息
             string folderStr = System.IO.Path.GetFileName(dirName);
             Debug.Log(folderStr);//最近文件夹信息
             Debug.Log(assetPath);//从Asset开始全路径
             string FileName = System.IO.Path.GetFileName(assetPath);
             Debug.Log("导入文件名称：" + FileName);
             string[] FileNameArray = FileName.Split(new string[] { "_" }, System.StringSplitOptions.RemoveEmptyEntries);

             //含有UI指定字符，此图片是的类型为Sprite
             if (Regex.IsMatch(FileName, RetrivalTextureType))
             {
                 Debug.Log("含有指定字符");
                 textureImporter.textureType = TextureImporterType.Sprite;
                // textureImporter.mipmapEnabled = false;
             }
             else
             {
                 textureImporter.textureType = TextureImporterType.Default;
                // textureImporter.mipmapEnabled = true;
                // textureImporter.streamingMipmaps = true;
             }

             //判断是否使用Alpha通道
             if (Regex.IsMatch(FileName, RetrivalTextureIsAlpha))
             {
                 textureImporter.alphaIsTransparency = true;
             }
             else
             {
                 textureImporter.alphaIsTransparency = false;
             }
            */

            // 设置MaxSize尺寸
            string FileName = System.IO.Path.GetFileName(assetPath);
            Regex tempRegex = new Regex(RetrivalTextureMaxSize);
            if (tempRegex.IsMatch(FileName))
            {
                string MaxSizeStr = tempRegex.Match(FileName).Value.Replace("&", "");
                int TempMaxSize = Convert.ToInt32(MaxSizeStr);
                if (TempMaxSize < 50)
                {
                    TempMaxSize = 32;
                }
                else if (TempMaxSize < 100)
                {
                    TempMaxSize = 64;
                }
                else if (TempMaxSize < 150)
                {
                    TempMaxSize = 128;
                }
                else if (TempMaxSize < 300)
                {
                    TempMaxSize = 256;
                }
                else if (TempMaxSize < 600)
                {
                    TempMaxSize = 512;
                }
                else
                {
                    TempMaxSize = 1024;
                }
                textureImporter.maxTextureSize = TempMaxSize;
                Debug.Log("设置的Texture尺寸为：" + TempMaxSize);
            }


        }
        //所有的资源的导入，删除，移动，都会调用此方法，注意，这个方法是static的  （这个是在对应资源的导入前后函数执行后触发）
        /* public static void OnPostprocessAllAssets(string[] importedAsset, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
         {
             Debug.Log("EditorResourceSetting 有文件处理");

             foreach (string str in importedAsset)
             {

                 Debug.Log("导入文件 = " + str);

             }
             //foreach (string str in deletedAssets)
             //{
             //    Debug.Log("删除文件 = " + str);
             //}
             //foreach (string str in movedAssets)
             //{
             //    Debug.Log("移动后文件 = " + str);
             //}
             //foreach (string str in movedFromAssetPaths)
             //{
             //    Debug.Log("移动前文件 = " + str);
             //}
         }
        */


    }
#endif
}
