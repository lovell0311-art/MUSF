using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using UnityEngine.U2D;

namespace ETModel
{
    [ObjectSystem]
    public class SpriteUtilityAwake : AwakeSystem<SpriteUtility>
    {
        public override void Awake(SpriteUtility self)
        {
            self.Awake();
        }
    }
    /// <summary>
    /// Sprite 实用工具类
    /// UGUI中使用图集频繁获取切换图片atlas.GetSprite(name)克隆出的资源不会自动卸载
    /// 所以以获取的图片不同要再用这个方法获取
    /// </summary>
    public class SpriteUtility : Component
    {

        public static SpriteUtility Instance;
        /// <summary>
        /// key->sprite名字
        /// value->对应的Sprite
        /// </summary>

        private Dictionary<string, Sprite> LocalSprite;
        /// <summary>
        /// key->SpriteAtlas 图集名
        /// value->对应的图集
        /// </summary>
        private Dictionary<string, SpriteAtlas> LocaAtlas;

        public void Awake()
        {
            Instance = this;
            LocalSprite = new Dictionary<string, Sprite>();
            LocaAtlas = new Dictionary<string, SpriteAtlas>();
        }
        /// <summary>
        /// 获取Sprite
        /// </summary>
        /// <param name="AtlasName">Sprite所在的图集名</param>
        /// <param name="SpriteName">Sprite的名字</param>
        /// <returns>Sprite</returns>
        public Sprite GetAtlasSprite(string AtlasName,string SpriteName)
        {
            string Sprite_Name = $"{AtlasName}_{SpriteName}";//缓存Sprite的名字为 图集_精灵 （不同图集下有相同名字的精灵时 不会加载错）
            if (LocalSprite.TryGetValue(Sprite_Name, out Sprite sprite))
            {
                return sprite;
            }
            else
            {
                sprite = GetSpriteAtlas(AtlasName)?.GetSprite(SpriteName);
                LocalSprite[Sprite_Name] = sprite;
            }
           return sprite;
        }
        public Sprite GetAtlasSprite(string SpriteName)
        {
            if (LocalSprite.TryGetValue(SpriteName, out Sprite sprite))
            {
                return sprite;
            }
            return sprite ;
        }

        /// <summary>
        /// 加载图集
        /// </summary>
        /// <param name="AtlasName">图集的名字</param>
        /// <returns>Atlas</returns>
        public SpriteAtlas GetSpriteAtlas(string AtlasName) 
        {
            SpriteAtlas atlas;
            if (!LocaAtlas.ContainsKey(AtlasName))//图集是否已经加载
            {
                //加载图集
                AssetBundleComponent.Instance.LoadBundle(AtlasName.StringToAB());
                atlas = (SpriteAtlas)AssetBundleComponent.Instance.GetAsset(AtlasName.StringToAB(), AtlasName);
                LocaAtlas[AtlasName] = atlas;
              //  AssetBundleComponent.Instance.UnloadBundle(AtlasName.StringToAB());
            }
            else
            {
                atlas = LocaAtlas[AtlasName];
            }
            return atlas;
        }
        /// <summary>
        /// 清理未使用的图集
        /// </summary>
        /// <param name="atalName"></param>
        public void ClearAtals(string atalName) 
        {
            if (LocaAtlas.ContainsKey(atalName))
            {
                LocaAtlas.Remove(atalName);
                AssetBundleComponent.Instance.UnloadBundle(atalName.StringToAB());
            }
        }


        public override void Dispose()
        {
            if (this.IsDisposed)
                return;
            base.Dispose();
            LocaAtlas.Clear();
            LocaAtlas = null;
            LocalSprite.Clear();
            LocalSprite = null;
        }
    }
}
