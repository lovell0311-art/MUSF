using System.Collections.Generic;
using UnityEngine;
using ETModel;
using System.Linq;
using System.Threading.Tasks;

namespace ETHotfix
{


    [ObjectSystem]
    public class BufferComponentAwake : AwakeSystem<BufferComponent>
    {
        public override void Awake(BufferComponent self)
        {

            BufferComponent.Instance = self;
            self.roleEntity = self.GetParent<UnitEntity>();

            self.BufferDic = new Dictionary<int, GameObject>();
            self.BufferList = new Dictionary<int, List<GameObject>>() { { 23, new List<GameObject>() } };

            self.InitBuffer();
        }
    }

    /// <summary>
    /// Buffer组件
    /// </summary>
    public class BufferComponent : Component
    {

        public static BufferComponent Instance;
        public UnitEntity roleEntity;

        public UIMainComponent mainComponent_Bufer;

        public Dictionary<int, GameObject> BufferDic = new Dictionary<int, GameObject>();
        public List<long> BufferIdLists = new List<long>()
        {
            7,//冰封
            8,//毒咒
        14,//守护之魂
        23,//法神附体
        102,//圣盾防御
        110,//生命之光
        112,//剑之愤怒
        114,//坚强的信念
        115,//坚强的庇护
        203,//弓箭手 守护之光
        204,//战神之力
        214,//冰封箭
        216,//无影箭
        321,//玄月斩
        410,//致命圣印
        502,//昏睡术
        503,//爆裂击
        505,//安魂弥撒
        506,//伤害反射
        509,//狂暴术
        510,//虚弱阵
        511,//破御阵

        };

        /// <summary>
        /// 挂在玩家节点上的Buffer、技能特效
        /// </summary>
        public Dictionary<int, List<GameObject>> BufferList;

        public void InitBuffer()
        {
            if (roleEntity is RoleEntity role)
            {
                if (role.Game_Object == null) return;

                switch (role.RoleType)
                {
                    case E_RoleType.Magician:
                        var ob = role.Game_Object.GetReferenceCollector().GetAssets(MonoReferenceType.Object);
                        foreach (GameObject item in ob.Cast<GameObject>())
                        {
                            if (item.name.Contains("FaShenFuTI"))//法神附体
                            {
                                BufferList[23].Add(item);
                                item.SetActive(false);
                            }
                        }
                        break;
                    case E_RoleType.Swordsman:
                        break;
                    case E_RoleType.Archer:
                        break;
                    case E_RoleType.Magicswordsman:
                        break;
                    case E_RoleType.Holymentor:
                        break;
                    case E_RoleType.Summoner:
                        break;
                    case E_RoleType.Gladiator:
                        break;
                    case E_RoleType.GrowLancer:
                        break;
                    case E_RoleType.Runemage:
                        break;
                    case E_RoleType.StrongWind:
                        break;
                    case E_RoleType.Gunners:
                        break;
                    case E_RoleType.WhiteMagician:
                        break;
                    case E_RoleType.WomanMagician:
                        break;
                    default:
                        break;
                }
            }
        }

        public void PlayEffect(int effectId)
        {

            if (BufferList.TryGetValue(effectId, out List<GameObject> list))
            {
                foreach (GameObject item in list)
                {

                    if (item.GetComponent<ParticleSystem>() is ParticleSystem particleSystem)
                    {
                        particleSystem.Play();
                    }
                }

            }

        }

        public void StopEffect(int effectId)
        {
            if (BufferList.TryGetValue(effectId, out List<GameObject> list))
            {
                foreach (GameObject item in list)
                {
                    if (item.GetComponent<ParticleSystem>() is ParticleSystem particleSystem)
                    {
                        particleSystem.Stop();
                    }
                }

            }

        }

        /// <summary>
        /// 显示Buffer
        /// </summary>
        /// <param name="bufferIndex">Buffer Id</param>
        public async ETTask ShowBuffer(int bufferIndex)
        {
            //await ETTask.CompletedTask;
             Log.Info("ShowBuffer ---------- " + bufferIndex);
            if (UIComponent.Instance.Get(UIType.UIMainCanvas) == null)
            {
                Log.Info("ShowBuffer ---------- UIType.UIMainCanvas 主页面资源不存在");
                return;
            }

            if (mainComponent_Bufer == null)
            {
                mainComponent_Bufer = UIMainComponent.Instance;
            }

            Buff_UnitConfig buff_UnitConfig = ConfigComponent.Instance.GetItem<Buff_UnitConfig>(bufferIndex);

            if (buff_UnitConfig == null)
            {
                Log.Info("ShowBuffer ---------- buff_UnitConfig 为查找到资源");
                return;
            }
            // Log.Info("添加BuffShowBuffer  " + buff_UnitConfig.AttackEffect.StringToAB());

            //  Log.DebugBrown($"buffer:{buff_UnitConfig.AttackEffect} skillInfos.Icon:{buff_UnitConfig.Icon} string.IsNullOrEmpty(skillInfos.Icon):{string.IsNullOrEmpty(buff_UnitConfig.Icon)}");

            if (!string.IsNullOrEmpty(buff_UnitConfig.Icon) && roleEntity?.Id == UnitEntityComponent.Instance.LocaRoleUUID)
            {
                //Log.DebugBrown("主界面buff" + bufferIndex + ":::icon" + buff_UnitConfig.Icon + "::::" + buff_UnitConfig.Name);
                //显示buff图标
               // mainComponent_Bufer.buffContent.gameObject.SetActive(true);
                mainComponent_Bufer.ShowBuff((long)bufferIndex, buff_UnitConfig.Icon, buff_UnitConfig.Name);
            }

            if (buff_UnitConfig == null || string.IsNullOrEmpty(buff_UnitConfig.AttackEffect)) return;
            await ResourcesComponent.Instance.LoadGameObjectAsync(buff_UnitConfig.AttackEffect.StringToAB(), buff_UnitConfig.AttackEffect, () =>
            {
                if (roleEntity.Game_Object)
                {
                    BufferDic.TryGetValue(bufferIndex, out GameObject buffer);
                    if (buffer == null)
                    {
                        buffer = ResourcesComponent.Instance.LoadGameObject(buff_UnitConfig.AttackEffect.StringToAB(), buff_UnitConfig.AttackEffect);
                    }

                    PlayEffect(bufferIndex);
                    if (buffer == null) return;
                    buffer.transform.SetParent(roleEntity.Game_Object.transform);

                    if (roleEntity.Id == UnitEntityComponent.Instance.LocaRoleUUID)
                    {
                        buffer.SetLayer(LayerNames.LOCALROLE);
                    }
                    else
                    {
                        buffer.SetLayer(LayerNames.ROLE);
                    }

                    buffer.transform.localPosition = Vector3.zero;
                    buffer.transform.localRotation = Quaternion.identity;
                    buffer.transform.localScale = Vector3.one;
                    BufferDic[bufferIndex] = buffer;//缓存起来
                }
            });
        }

        /// <summary>
        /// 移除 Buffer
        /// </summary>
        /// <param name="bufferIndex"></param>

        public void HideBuffer(int bufferIndex)
        {
            if (BufferDic.TryGetValue(bufferIndex, out GameObject buffer))
            {
                ResourcesComponent.Instance.RecycleGameObject(buffer);
                StopEffect(bufferIndex);
                BufferDic.Remove(bufferIndex);
            }

            if (roleEntity.Id == UnitEntityComponent.Instance.LocaRoleUUID)
                mainComponent_Bufer?.HideBuff(bufferIndex);//隐藏Buffer图标
        }


        public void RemoveAllBuffer()
        {
            List<int> bufferKeys = new List<int>(this.BufferDic.Count);
            foreach (int key in this.BufferDic.Keys)
            {
                bufferKeys.Add(key);
            }
            for (int i = 0; i < bufferKeys.Count; i++)
            {
                HideBuffer(bufferKeys[i]);
            }

        }

        public void ClearBuffer()
        {
            foreach (KeyValuePair<int, GameObject> item in this.BufferDic)
            {

                if (item.Value != null)
                {
                    ResourcesComponent.Instance.RecycleGameObject(item.Value);
                }
                StopEffect(item.Key);
                mainComponent_Bufer?.HideBuff(item.Key);//隐藏Buffer图标
            }
            ClearBufferIcon();
            BufferDic.Clear();


        }

        public void ClearBufferIcon()
        {
            mainComponent_Bufer?.ClearBuffer();//隐藏Buffer图标

        }


        public override void Dispose()
        {
            if (this.IsDisposed) return;
            base.Dispose();

            if (Instance == this)
            {
                Instance = null;
            }

            foreach (KeyValuePair<int, GameObject> item in this.BufferDic)
            {
                if (item.Value != null)
                {
                    ResourcesComponent.Instance.RecycleGameObject(item.Value);
                }
                StopEffect(item.Key);
            }

            ClearBufferIcon();
            BufferDic.Clear();

        }

    }
}
