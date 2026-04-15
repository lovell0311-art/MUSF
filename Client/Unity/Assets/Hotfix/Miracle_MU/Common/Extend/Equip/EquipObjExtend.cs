using UnityEngine;
using ETModel;


namespace ETHotfix
{
    /*
     0~6级 不显示特效
     7~8级 开启Stage_1流光特效
     9~12级 开启 Stage_1 和 Stage_2 流光特效
     13~14 开启 角 
     +15 开启 Stage_1、2、3 和角
     */


    /// <summary>
    /// 装备拓展类
    /// </summary>
    public static class EquipObjExtend
    {
       static  MaterialPropertyBlock  propertyBlock=new MaterialPropertyBlock();

        /// <summary>
        /// 设置背部 装备
        /// </summary>
        /// <param name="self"></param>
        /// <param name="lev"></param>
        public static void SetBack(this GameObject self, int lev = 0,bool isleft=false)
        {
          
            if (isleft)
            {
                ShowMode(self, E_ModeType.LeftBack);
                if (self.transform.Find("LeftBack") is Transform leftback)
                {
                    if (self.name.Contains("Weapon"))
                    {
                        //武器
                        ShowWeaponEffect(leftback.gameObject, lev);
                    }
                    else
                    {
                        ShowEffect(leftback.gameObject, lev);
                        ShowEffect_Lev(leftback.gameObject, lev);
                    }
                }
            }
            else
            {
                ShowMode(self, E_ModeType.Back);
                if (self.transform.Find("Back") is Transform back)
                {

                    //ShowEffect(back.gameObject, lev);
                    if (self.name.Contains("Weapon"))
                    {
                        //武器
                        ShowWeaponEffect(back.gameObject, lev);
                    }
                    else
                    {
                        ShowEffect(back.gameObject, lev);
                        ShowEffect_Lev(back.gameObject, lev);
                    }
                }
            }

        }


        public static void SetWorld(this GameObject self, int lev = 0)
        {

            ShowMode(self, E_ModeType.World);
            //根据等级 设置流光特效

            if (self.transform.Find("World") is Transform world)
            {
                if (self.name.Contains("Weapon"))
                {
                    ShowWeaponEffect(world.gameObject, lev);
                }
                else if (self.name.Contains("Guard") || self.name.Contains("Wing") || self.name.Contains("Flag"))//守护
                {
                    ShowGuardEffect(world.gameObject, lev);
                    //world.gameObject.SetLayer(LayerNames.HIDDEN);
                }
                else
                {
                    ShowEffect(world.gameObject, lev);
                    ShowEffect_Lev(world.gameObject, lev);
                }
            }


        }

        public static void SetUI(this GameObject self, int lev = 0)
        {

            ShowMode(self, E_ModeType.UI);
            //根据等级 设置流光特效
            if (self.transform.Find("UI") is Transform ui)
            {
                if (self.name.Contains("Weapon"))
                {
                    ShowWeaponEffect(ui.gameObject, lev, E_ModeType.UI);
                }
                else if (self.name.Contains("Guard") || self.name.Contains("Wing"))//守护
                {
                    ShowGuardEffect(ui.gameObject, lev);
                }
                else
                {
                    ShowEffect(ui.gameObject, lev, E_ModeType.UI);
                    ShowEffect_Lev(ui.gameObject, lev);
                }
                // 
            }

        }


        public static void ShowMode(GameObject obj, E_ModeType type)
        {
            if (obj.transform.Find("World") is Transform world)
            {
                world.gameObject.SetActive(type == E_ModeType.World);

                ReferenceCollector collector = world.GetReferenceCollector();
                if (collector != null && type != E_ModeType.World)
                {
                    var objs = collector.GetAssets(MonoReferenceType.Object);
                    if (objs != null)
                    {
                        foreach (GameObject item in objs)
                        {
                            if (item == null) continue;
                            item.SetActive(type == E_ModeType.World);
                        }
                    }
                }

                if (world.gameObject.layer != LayerNames.GetLayerInt(LayerNames.ROLE))
                    world.gameObject.SetLayer(LayerNames.ROLE);
            }
            if (obj.transform.Find("UI") is Transform ui)
            {
                ui.gameObject.SetActive(type == E_ModeType.UI);
                if (ui.gameObject.layer != LayerNames.GetLayerInt(LayerNames.UI))
                    ui.gameObject.SetLayer(LayerNames.UI);
            }
            if (obj.transform.Find("Back") is Transform back)
            {
                back.gameObject.SetActive(type == E_ModeType.Back);

                if (back.gameObject.layer != LayerNames.GetLayerInt(LayerNames.ROLE))
                    back.gameObject.SetLayer(LayerNames.ROLE);

            }
            if (obj.transform.Find("LeftBack") is Transform leftback)
            {
                leftback.gameObject.SetActive(type == E_ModeType.LeftBack);

                if (leftback.gameObject.layer != LayerNames.GetLayerInt(LayerNames.ROLE))
                    leftback.gameObject.SetLayer(LayerNames.ROLE);

            }
        }
        /// <summary>
        /// 设置守护的特效
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="lev"></param>
        public static void ShowGuardEffect(GameObject obj, int lev = 1)
        {
            ReferenceCollector collector = obj.GetReferenceCollector();
            if (collector == null) return;
            if (collector.GetGameObject("Stage_1") is GameObject stage_1)
            {
                //守护等级大于 10 显示特效
                stage_1.SetActive(lev >= 10);
            }
        }

        /// <summary>
        /// 设置 武器特效
        /// </summary>
        public static void ShowWeaponEffect(GameObject obj, int lev = 1, E_ModeType modeType = E_ModeType.World)
        {

            ReferenceCollector collector = obj.GetReferenceCollector();
            if (modeType == E_ModeType.World)
            {
                if (collector.GetGameObject("Stage_1") is GameObject stage_1)
                {
                    SkinnedMeshRenderer[] WorldskinnedMeshRenderers = stage_1.GetComponentsInChildren<SkinnedMeshRenderer>();
                    foreach (var skinnedMesh in WorldskinnedMeshRenderers)
                    {
                        propertyBlock.SetFloat("_EffectIntensity", lev);
                        skinnedMesh.SetPropertyBlock(propertyBlock);
                    }
                    MeshRenderer[] WorldMeshRenderers = stage_1.GetComponentsInChildren<MeshRenderer>();
                    foreach (var Mesh in WorldMeshRenderers)
                    {
                        propertyBlock.SetFloat("_EffectIntensity", lev);
                        Mesh.SetPropertyBlock(propertyBlock);
                    }
                }
            }
            else
            {
                SkinnedMeshRenderer[] skinnedMeshRenderers = obj.GetComponentsInChildren<SkinnedMeshRenderer>();
                foreach (var skinnedMesh in skinnedMeshRenderers)
                {
                    propertyBlock.SetFloat("_EffectIntensity", lev);

                    skinnedMesh.SetPropertyBlock(propertyBlock);
                }
                MeshRenderer[] MeshRenderers = obj.GetComponentsInChildren<MeshRenderer>();
                foreach (var Mesh in MeshRenderers)
                {
                    propertyBlock.SetFloat("_EffectIntensity", lev);
                    Mesh.SetPropertyBlock(propertyBlock);
                }
            }
            //满级十五级 才开启粒子特效
            if (collector.GetGameObject("Stage_2") is GameObject stage_2)
            {
                if (modeType == E_ModeType.World)
                {
                    stage_2.SetLayer(LayerNames.LOCALROLE);
                }
                stage_2.SetActive(lev >= 15);
            }
        }

        /// <summary>
        /// 设置特效
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="lev"></param>
        /// <param name="modeType"></param>
        public static void ShowEffect(GameObject obj, int lev = 0, E_ModeType modeType = E_ModeType.World)
        {
            
                OpenAngleEffect(isOpen: false);
                int stage_1 = 0;
                int stage_2 = 0;
                int stage_3 = 0;
            try
            {
                if (lev >= 15) //+15 特效全部开启
                {
                    stage_1 = 1;
                    stage_2 = 1;
                    stage_3 = 1;
                    OpenAngleEffect(isOpen: modeType != E_ModeType.UI, scale: 1f);

                }
                else if (lev >= 13)//开启 角 
                {
                    stage_1 = 1;
                    stage_2 = 1;
                    stage_3 = 1;
                    OpenAngleEffect(isOpen: modeType != E_ModeType.UI, scale: .5f);
                }
                else if (lev >= 9)//开启Stage_1 、Stage_2、流光特效
                {
                    stage_1 = 1;
                    stage_2 = 1;
                    stage_3 = 0;
                }
                else if (lev >= 7)//开启 Stage_1
                {
                    stage_1 = 1;
                    stage_2 = 0;
                    stage_3 = 0;
                }
                else //关闭 所有特效
                {
                    stage_1 = 0;
                    stage_2 = 0;
                    stage_3 = 0;
                    OpenAngleEffect(isOpen: false);
                }
            }
            catch (System.Exception e)
            {

                
            }
           

            try
            {
                SkinnedMeshRenderer[] skinnedMeshRenderers = obj.GetComponentsInChildren<SkinnedMeshRenderer>();
                foreach (var skinnedMesh in skinnedMeshRenderers)
                {
                    propertyBlock.SetFloat("_Stage_1", stage_1);
                    propertyBlock.SetFloat("_Stage_2", stage_2);
                    propertyBlock.SetFloat("_Stage_3", stage_3);

                    skinnedMesh.SetPropertyBlock(propertyBlock);
                }
                MeshRenderer[] MeshRenderers = obj.GetComponentsInChildren<MeshRenderer>();
                
                foreach (var Mesh in MeshRenderers)
                {
                    propertyBlock.SetFloat("_Stage_1", stage_1);
                    propertyBlock.SetFloat("_Stage_2", stage_2);
                    propertyBlock.SetFloat("_Stage_3", stage_3);
                    Mesh.SetPropertyBlock(propertyBlock);
                }
            }
            catch (System.Exception E)
            {

               
            }

            void OpenAngleEffect(bool isOpen = true, float scale = 1)
            {
                try
                {
                    ReferenceCollector collector = obj.GetReferenceCollector();
                    if (collector == null)
                        return;
                    //显示光圈角
                    if (collector.GetGameObject("Angle_L") is GameObject Angle_L)
                    {
                        Angle_L.SetActive(isOpen);
                        Angle_L.transform.localScale = Vector3.one * scale;
                    }
                    if (collector.GetGameObject("Angle_R") is GameObject Angle_R)
                    {
                        Angle_R.SetActive(isOpen);
                        Angle_R.transform.localScale = Vector3.one * scale;
                    }
                    if (collector.GetGameObject("Angle_Head") is GameObject Angle_Head)
                    {
                        Angle_Head.SetActive(isOpen);
                        Angle_Head.transform.localScale = Vector3.one * scale;
                    }
                }
                catch (System.Exception e)
                {

                }
               
            }
        }

        /// <summary>
        /// 显示特效（弃用）
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="lev"></param>
        public static void ShowEffect_Lev(GameObject obj, int lev = 0)
        {
            ReferenceCollector collector = obj.GetReferenceCollector();
            if (collector == null)
                return;

            CloseAllEffect(collector);
            if (lev >= 15)// +15 特效 全部开启
            {
                if (collector.GetGameObject("Stage_1") is GameObject stage_1 && !stage_1.activeSelf)
                {
                    stage_1.SetActive(true);
                }
                if (collector.GetGameObject("Stage_2") is GameObject stage_2 && !stage_2.activeSelf)
                {
                    stage_2.SetActive(true);
                }
                if (collector.GetGameObject("Stage_3") is GameObject stage_3)
                {
                    stage_3.SetActive(true);
                }
                else
                {
                    Log.DebugRed("+15 特效不存在");
                }
                //显示光圈角
                if (collector.GetGameObject("Angle_L") is GameObject Angle_L)
                {
                    Angle_L.SetActive(true);
                    Angle_L.transform.localScale = Vector3.one * -1.5f;
                }
                if (collector.GetGameObject("Angle_R") is GameObject Angle_R)
                {
                    Angle_R.SetActive(true);
                    Angle_R.transform.localScale = Vector3.one * -1.5f;
                }
                if (collector.GetGameObject("Angle_Head") is GameObject Angle_Head)
                {
                    Angle_Head.SetActive(true);
                    Angle_Head.transform.localScale = Vector3.one * -1.5f;
                }
            }
            else if (lev >= 14)// 开启 显示光圈角
            {
                if (collector.GetGameObject("Stage_1") is GameObject stage_1)
                {
                    stage_1.SetActive(true);
                }
                if (collector.GetGameObject("Stage_2") is GameObject stage_2)
                {
                    stage_2.SetActive(true);
                }
                if (collector.GetGameObject("Stage_3") is GameObject stage_3)
                {
                    stage_3.SetActive(false);
                }
                if (collector.GetGameObject("Angle_L") is GameObject Angle_L)
                {
                    Angle_L.SetActive(true);
                    Angle_L.transform.localScale = Vector3.one * -1f;
                }
                if (collector.GetGameObject("Angle_R") is GameObject Angle_R)
                {
                    Angle_R.SetActive(true);
                    Angle_R.transform.localScale = Vector3.one * -1f;
                }
                if (collector.GetGameObject("Angle_Head") is GameObject Angle_Head)
                {
                    Angle_Head.SetActive(true);
                    Angle_Head.transform.localScale = Vector3.one * -1f;
                }
            }
            else if (lev >= 13)// 开启 显示光圈角
            {
                if (collector.GetGameObject("Stage_1") is GameObject stage_1)
                {
                    stage_1.SetActive(true);
                }
                if (collector.GetGameObject("Stage_2") is GameObject stage_2)
                {
                    stage_2.SetActive(true);
                }
                if (collector.GetGameObject("Stage_3") is GameObject stage_3)
                {
                    stage_3.SetActive(false);
                }

                if (collector.GetGameObject("Angle_L") is GameObject Angle_L)
                {
                    Angle_L.SetActive(true);
                    Angle_L.transform.localScale = Vector3.one * -.5f;
                }
                if (collector.GetGameObject("Angle_R") is GameObject Angle_R)
                {
                    Angle_R.SetActive(true);
                    Angle_R.transform.localScale = Vector3.one * -.5f;
                }
                if (collector.GetGameObject("Angle_Head") is GameObject Angle_Head)
                {
                    Angle_Head.SetActive(true);
                    Angle_Head.transform.localScale = Vector3.one * -.5f;
                }
                if (collector.GetGameObject("Hand_R") is GameObject hand_R)
                {
                    hand_R.SetActive(true);
                }
                if (collector.GetGameObject("Hand_L") is GameObject Hand_L)
                {
                    Hand_L.SetActive(true);
                }
            }
            else if (lev >= 9)// 开启 Stage_1 Stage_2 流光特效
            {

                if (collector.GetGameObject("Stage_1") is GameObject stage_1 && !stage_1.activeSelf)
                {
                    stage_1.SetActive(true);
                }
                if (collector.GetGameObject("Stage_2") is GameObject stage_2)
                {
                    stage_2.SetActive(true);
                }
                if (collector.GetGameObject("Stage_3") is GameObject stage_3 && stage_3.activeSelf)
                {
                    stage_3.SetActive(false);
                }
            }
            else if (lev >= 7)//开启 Stage_1 流光 特效
            {
                if (collector.GetGameObject("Stage_1") is GameObject stage_1)
                {
                    stage_1.SetActive(true);
                }
                if (collector.GetGameObject("Stage_2") is GameObject stage_2 && stage_2.activeSelf)
                {
                    stage_2.SetActive(false);
                }
                if (collector.GetGameObject("Stage_3") is GameObject stage_3 && stage_3.activeSelf)
                {
                    stage_3.SetActive(false);
                }
            }
            else
            {

            }

        }
        /// <summary>
        /// 关闭所有特效
        /// </summary>
        /// <param name="collector"></param>
        static void CloseAllEffect(ReferenceCollector collector)
        {
            #region CloseAllEffect
            if (collector.GetGameObject("Stage_1") is GameObject stage_1 && stage_1.activeSelf)
            {
                stage_1.SetActive(false);
            }
            if (collector.GetGameObject("Stage_2") is GameObject stage_2 && stage_2.activeSelf)
            {
                stage_2.SetActive(false);
            }
            if (collector.GetGameObject("Stage_3") is GameObject stage_3 && stage_3.activeSelf)
            {
                stage_3.SetActive(false);
            }
            if (collector.GetGameObject("Angle_R") is GameObject Angle_R)
            {
                Angle_R.SetActive(false);
            }
            if (collector.GetGameObject("Angle_L") is GameObject Angle_L)
            {
                Angle_L.SetActive(false);
            }
            if (collector.GetGameObject("Angle_Head") is GameObject Angle_Head)
            {
                Angle_Head.SetActive(false);
            }
            if (collector.GetGameObject("Guang_Head") is GameObject Guang_Head)
            {
                Guang_Head.SetActive(false);
            }
            if (collector.GetGameObject("CaiDai") is GameObject CaiDai)
            {
                CaiDai.SetActive(false);
            }
            if (collector.GetGameObject("huo") is GameObject huo)
            {
                huo.SetActive(false);
            }
            if (collector.GetGameObject("Hand_R") is GameObject hand_R)
            {
                hand_R.SetActive(false);
            }
            if (collector.GetGameObject("Hand_L") is GameObject Hand_L)
            {
                Hand_L.SetActive(false);
            }
            #endregion
        }
        /// <summary>
        /// 显示套装彩带（穿戴一整套套装 才显示）
        /// </summary>
        /// <param name="self"></param>
        public static void ShowCaiDai(this GameObject self)
        {
            ReferenceCollector collector = self.transform.Find("World")?.GetReferenceCollector();
            if (collector == null) return;
            if (collector.GetGameObject("CaiDai") is GameObject CaiDai)
            {
                CaiDai.SetActive(true);
            }
            if (collector.GetGameObject("huo") is GameObject huo)
            {
                huo.SetActive(true);
            }

        }
        /// <summary>
        /// 隐藏彩带
        /// </summary>
        /// <param name="self"></param>
        public static void HideCaiDai(this GameObject self)
        {
            ReferenceCollector collector = self.transform.Find("World")?.GetReferenceCollector();
            if (collector == null) return;
            if (collector.GetGameObject("CaiDai") is GameObject CaiDai)
            {
                CaiDai.SetActive(false);
            }
            if (collector.GetGameObject("huo") is GameObject huo)
            {
                huo.SetActive(false);
            }

        }



    }

    public enum E_ModeType
    {
        World,
        Back,
        LeftBack,
        UI
    }
}