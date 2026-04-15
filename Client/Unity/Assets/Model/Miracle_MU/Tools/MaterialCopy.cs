using UnityEngine;

namespace ETModel
{
    /// <summary>
    /// 修改材质球属性（相同材质球不同属性 非共享材质球）
    /// </summary>
    public class MaterialCopy : MonoBehaviour
    {
        MaterialPropertyBlock propertyBlock;
        public MeshRenderer[] meshs;
        public SkinnedMeshRenderer[] skinnedMeshRenderers;
       
        private void Awake()
        {
            if (propertyBlock == null)
            { 
             propertyBlock = new MaterialPropertyBlock();
            }
            meshs = this.gameObject.GetComponentsInChildren<MeshRenderer>();
            skinnedMeshRenderers = this.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
            
        }

      
        private void OnDestroy()
        {
            propertyBlock = null;
            meshs = null;
            skinnedMeshRenderers = null;
        }

        public void ChangeWeapon(int lev)
        {
            for (int i = 0; i < meshs.Length; i++)
            {
                propertyBlock.SetFloat("_EffectIntensity", lev);
                meshs[i].SetPropertyBlock(propertyBlock);
            }
           
        }

        public void ChangeSuitAtr(int stage_1,int stage_2,int stage_3) 
        {
            for (int i = 0, length = meshs.Length; i < length; i++)
            {
                propertyBlock.SetFloat("_Stage_1", stage_1);
                propertyBlock.SetFloat("_Stage_2", stage_2);
                propertyBlock.SetFloat("_Stage_3", stage_3);
                meshs[i].SetPropertyBlock(propertyBlock);
            }
            for (int i = 0, length = skinnedMeshRenderers.Length; i < length; i++)
            {
                propertyBlock.SetFloat("_Stage_1", stage_1);
                propertyBlock.SetFloat("_Stage_2", stage_2);
                propertyBlock.SetFloat("_Stage_3", stage_3);
                skinnedMeshRenderers[i].SetPropertyBlock(propertyBlock);
            }
        }
    }
}
