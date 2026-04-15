using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.SceneManagement;

namespace ETModel
{
    [ObjectSystem]
    public class SoundComponentAwakeSystem : AwakeSystem<SoundComponent>
    {
        public override void Awake(SoundComponent self)
        {
            self.AudioGameObj = Component.Global.transform.Find("AudioSource").gameObject;
            self.bgAudioSource = self.AudioGameObj.AddComponent<AudioSource>();
            self.bgAudioSource.loop = true;//背景音乐 可循环播放
            self.bgAudioSource.clip = null;
            self.bgAudioSource.playOnAwake = false;

            self.audioSource = self.AudioGameObj.AddComponent<AudioSource>();
            self.audioSource.loop = false;
            self.audioSource.clip = null;
            self.audioSource.playOnAwake = false;

            self.Awake();
        }

    }

    /// <summary>
    /// 游戏音效管理组件
    /// </summary>
    public class SoundComponent : Component
    {
        public static SoundComponent Instance;
        public GameObject AudioGameObj;
        public AudioSource bgAudioSource = null;//背景音乐
        public AudioSource audioSource = null;//技能、特效 点击等音效

        public ReferenceCollector sceneAudioReference;//场景中的怪物
        public ReferenceCollector skillAudioReference;//技能音效
        private bool soundMute = false;//是否为静音

        public long intervalTime = 5000;//背景音乐循环播放的间隔时间（时间戳 毫秒）
        public float delayTime = 3;//背景音乐开始播放的延时时间（时间戳 秒）
        //所有音效
        private Dictionary<string, SoundData> m_clips = new Dictionary<string, SoundData>();


        GameSetInfo gameSetInfo;



        public void Awake()
        {
            Instance = this;
            GetInitMusic();

            gameSetInfo = LocalDataJsonComponent.Instance.LoadData<GameSetInfo>(LocalJsonDataKeys.GameSetInfo) ?? new GameSetInfo();
        }


        /// <summary>
        /// 加载技能音效
        /// </summary>
        /// <param name="assetBundlName"></param>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public async ETVoid LoadSkillAudioRefrence(string assetBundlName, string assetName)
        {
            if (skillAudioReference != null) return;

            // Log.Info("关闭音效加载 " + assetBundlName + " assetName = " + assetName);
            ResourcesComponent.Instance.LoadGameObjectAsync(assetBundlName, assetName, () =>
            {
                GameObject obj = ResourcesComponent.Instance.LoadGameObject(assetBundlName, assetName);
                skillAudioReference = obj.GetReferenceCollector();

            }).Coroutine();


            await ETTask.CompletedTask;

        }
        /// <summary>
        /// 加载场景音效
        /// </summary>
        /// <param name="assetBundleName"></param>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public async ETVoid LoadSceneAudioRefrence(string assetBundleName, string assetName)
        {

            if (sceneAudioReference != null)
            {
                if (sceneAudioReference.gameObject.name.Equals(assetName))//当前场景的音效资源 已经加载了
                {
                    return;
                }
                //卸载上一个场景的的音效资源
                AssetBundleComponent.Instance.UnloadBundle(sceneAudioReference.gameObject.name.StringToAB());
            }
            await AssetBundleComponent.Instance.LHLoadBundleAsync(assetBundleName);
            GameObject obj = AssetBundleComponent.Instance.GetAsset(assetBundleName, assetName) as GameObject;
            if (obj != null)
                sceneAudioReference = obj.GetReferenceCollector();


        }
        /// <summary>
        /// 背景音乐 静音
        /// </summary>
        /// <param name="isMute"></param>
        public void SetBgSoundMute(bool isMute)
        {
            bgAudioSource.mute = isMute;

            //设置场景中的音乐 为静音
            foreach (SoundData clip in m_clips.Values)
            {
                clip.Mute = isMute;
            }
        }
        /// <summary>
        /// 音效静音
        /// </summary>
        /// <param name="isMute"></param>
        public void SetAudioSoundMute(bool isMute)
        {
            audioSource.mute = isMute;
            soundMute = isMute;

        }
        /// <summary>
        /// 设置背景音效音量
        /// </summary>
        /// <param name="volume"></param>
        public void SetBgVolume(float volume)
        {
            volume = Mathf.Clamp01(volume);
            bgAudioSource.volume = volume;
            foreach (SoundData clip in m_clips.Values)
            {
                clip.Volume = volume * clip.volume;
            }
        }
        /// <summary>
        /// 设置技能、怪物音效音量
        /// </summary>
        /// <param name="volume"></param>
        public void SetAudioVolume(float volume)
        {
            volume = Mathf.Clamp01(volume);
            audioSource.volume = volume;
        }
        /// <summary>
        /// 播放技能音效
        /// </summary>
        /// <param name="skillClipName">当前技能的音效名</param>
        public void PlaySkill(string skillClipName)
        {
            if (gameSetInfo.CloseSound) return;
            if (skillAudioReference == null) return;
            AudioClip clip = skillAudioReference.GetAudioClip(skillClipName);
            if (clip == null)
            {

                return;
            }

            audioSource.pitch = 1;
            audioSource.PlayOneShot(clip, 0.3f);

        }
        /// <summary>
        /// 播放怪物的音效
        /// </summary>
        /// <param name="clipName"></param>
        /// <param name="pos"></param>
        public void PlayClip(string clipName, Vector3 pos)
        {
            if (gameSetInfo.CloseSound) return;
            if (sceneAudioReference == null) return;

            AudioClip clip = sceneAudioReference.GetAudioClip(clipName);

            if (clip == null)
            {

                return;
            }

            PlayClipAtPoint(clip, pos, soundMute ? 0 : 1);


        }
        public static void PlayClipAtPoint(AudioClip clip, Vector3 position, [UnityEngine.Internal.DefaultValue("1.0F")] float volume)
        {
            // GameObject gameObject = new GameObject("OneShotAudio");
            GameObject gameObject = ResourcesComponent.Instance.LoadEffectObject("OneShotAudio".StringToAB(), "OneShotAudio", 700);
            gameObject.transform.position = position;
            AudioSource audioSource = (AudioSource)gameObject.GetComponent(typeof(AudioSource));
            audioSource.clip = clip;
            audioSource.spatialBlend = 1f;
            audioSource.volume = volume;
            audioSource.Play();
            //Log.DebugGreen($"(int)(clip.length * ((Time.timeScale < 0.01f) ? 0.01f : Time.timeScale)):{(int)(clip.length * ((Time.timeScale < 0.01f) ? 0.01f : Time.timeScale))}");
            // ResourcesComponent.Instance.RecycleGameObject(gameObject, (int)(clip.length * ((Time.timeScale < 0.01f) ? 0.01f : Time.timeScale)));
            // gameObject.GetComponent<ResourcesRecycle>().RecycleTime = clip.length * ((Time.timeScale < 0.01f) ? 0.01f : Time.timeScale);

        }
        /// <summary>
        /// 播放背景音乐
        /// </summary>
        /// <param name="SceneAudioName">当前场景的名字</param>
        public void PlayBgSound(string SceneAudioName)
        {

            if (gameSetInfo.CloseMusic) return;
            if (sceneAudioReference == null) return;
            AudioClip clip = sceneAudioReference.GetAudioClip(SceneAudioName);
            if (clip == null)
            {
                bgAudioSource.clip = null;

                return;
            }
            bgAudioSource.clip = clip;
            PlayBGAudio();

        }
        /// <summary>
        /// 暂停背景音乐
        /// </summary>
        public void Pause()
        {
            bgAudioSource.Pause();

        }

        /// <summary>
        /// 继续播放
        /// </summary>
        public void Resume()
        {
            bgAudioSource.UnPause();
        }
        /// <summary>
        /// 播放背景音乐
        /// </summary>
        public void PlayBGAudio()
        {
            bgAudioSource.PlayDelayed(delayTime);
            // LoopPlayBgAudio().Coroutine();
        }
        /// <summary>
        /// 循环播放背景音效
        /// </summary>
        /// <returns></returns>
        public async ETVoid LoopPlayBgAudio()
        {
            await TimerComponent.Instance.WaitAsync((long)bgAudioSource.clip.length + intervalTime);
            PlayBGAudio();
        }

        /// <summary>
        /// 得到当前场景中的音效
        /// </summary>
        public void GetCurSceneSounds()
        {

            UnityEngine.SceneManagement.Scene scene = SceneManager.GetActiveScene();
            GameObject[] gos = scene.GetRootGameObjects();
            m_clips.Clear();
            foreach (GameObject item in gos)
            {
                if (item.name == "Sounds")
                {
                    for (int i = 0; i < item.transform.childCount; i++)
                    {
                        SoundData sound_ = item.transform.GetChild(i).GetComponent<SoundData>();
                        if (sound_ != null)
                        {
                            sound_.Mute = gameSetInfo.CloseMusic;
                            m_clips[item.transform.GetChild(i).name] = sound_;
                        }
                    }
                }
                else
                {
                    SoundData sound = item.GetComponent<SoundData>();
                    if (sound != null)
                    {
                        sound.Mute = gameSetInfo.CloseMusic;
                        m_clips[item.name] = sound;
                    }
                }

            }
        }

        /// <summary>
        /// 初始场景背景音乐
        /// </summary>
        private void GetInitMusic()
        {
            AssetBundleComponent.Instance.LoadBundle("Audio_InitMusic".StringToAB());
            AudioClip initClip = (AudioClip)AssetBundleComponent.Instance.GetAsset("Audio_InitMusic".StringToAB(), "init_beijing_2");
            bgAudioSource.clip = initClip;
            PlayBGAudio();

        }
        public void Clear()
        {
            m_clips.Clear();
        }
        public override void Dispose()
        {
            if (this.IsDisposed) return;
            base.Dispose();
            sceneAudioReference = null;
            m_clips.Clear();
        }

    }
}