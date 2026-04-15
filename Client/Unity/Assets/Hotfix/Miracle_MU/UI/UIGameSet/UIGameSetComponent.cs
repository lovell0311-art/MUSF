using ETModel;
using UnityEngine.UI;
using System.Linq;
using System;
using UnityEngine;


namespace ETHotfix
{
    [ObjectSystem]
    public class UIGameSetComponentAwake : AwakeSystem<UIGameSetComponent>
    {
        public override void Awake(UIGameSetComponent self)
        {
            self.Awake();
        }
    }
    public enum E_TogType 
    {
     CloseEffect,//鍏抽棴鐗规晥
     CloseSound,//鍏抽棴闊虫晥
     CloseMusic,//鍏抽棴闊充箰
     RefrenceTeam,//鎷掔粷缁勯槦
     HideRole//闅愯棌鐜╁妯″瀷
    }
    public class UIGameSetComponent : Component
    {
        Toggle closeEffectTog, soundTog, MusicTog, cancleTeamTog, hideroleTog;
        InputField duihuanma;
      
        public void Awake()
        {
            ReferenceCollector collector = GetParent<UI>().GameObject.GetReferenceCollector();
            closeEffectTog = collector.GetToggle("closeEffectTog");
            soundTog = collector.GetToggle("soundTog");
            MusicTog = collector.GetToggle("MusicTog");
            cancleTeamTog = collector.GetToggle("cancleTeamTog");
            hideroleTog = collector.GetToggle("cancleFriendTog");

            duihuanma = collector.GetInputField("Account");
            collector.GetButton("CloseBtn").onClick.AddSingleListener(Save);
            collector.GetButton("ChangeRoleBtn").onClick.AddSingleListener(() => ChangeRole().Coroutine());
            collector.GetButton("DuiHuanBtn").onClick.AddSingleListener(DuihuanOnClick);


           // hideroleTog.gameObject.SetActive(false);

            closeEffectTog.onValueChanged.AddSingleListener((value) => { ChangTogState(value, E_TogType.CloseEffect); });
            soundTog.onValueChanged.AddSingleListener((value) => { ChangTogState(value, E_TogType.CloseSound); });
            MusicTog.onValueChanged.AddSingleListener((value) => { ChangTogState(value, E_TogType.CloseMusic); });
            cancleTeamTog.onValueChanged.AddSingleListener((value) => { ChangTogState(value, E_TogType.RefrenceTeam); });
            hideroleTog.onValueChanged.AddSingleListener((value) => { ChangTogState(value, E_TogType.HideRole); });

            GameSetInfo gameSetInfo = LocalDataJsonComponent.Instance.LoadData<GameSetInfo>(LocalJsonDataKeys.GameSetInfo);

            if (gameSetInfo != null)
            {
                closeEffectTog.isOn = gameSetInfo.CloseEffect;
                soundTog.isOn = gameSetInfo.CloseSound;
                MusicTog.isOn = gameSetInfo.CloseMusic;
                cancleTeamTog.isOn = gameSetInfo.RefuseTream;
                hideroleTog.isOn = gameSetInfo.HideRole;
            }
        }
        public void ChangTogState(bool ison, E_TogType togType)
        {
            GameSetInfo gameSetInfo = LocalDataJsonComponent.Instance.gameSetInfo;
            switch (togType)
            {
                case E_TogType.CloseEffect:
                   gameSetInfo.CloseEffect = ison;
                    break;
                case E_TogType.CloseSound:
                    gameSetInfo.CloseSound = ison;
                    break;
                case E_TogType.CloseMusic:
                   gameSetInfo.CloseMusic = ison;
                    break;
                case E_TogType.RefrenceTeam:
                   gameSetInfo.RefuseTream = ison;
                    break;
                case E_TogType.HideRole:
                    gameSetInfo.HideRole = ison;
                    UnitEntityComponent.Instance.RefreshModel(!ison);
                    break;
            }
        }
        public void DuihuanOnClick() {
            string duihuanmaStr = duihuanma.text;
            //Log.Debug(duihuanmaStr);
            //鏄惁涓虹┖瀛楃涓?
            if (duihuanmaStr == "")
            {
                return;
            }
            //鍒ゆ柇鏄惁鍦ㄥ厬鎹㈢爜鍦ㄧ姝㈡椂闂村唴
            string contentStr = PlayerPrefs.GetString("duihuanmaTime"+ UnitEntityComponent.Instance.LocaRoleUUID.ToString());
            if (contentStr != "")
            { 
                if (TimeHelper.GetNow() < Convert.ToInt64(contentStr))
                {
                    UIConfirmComponent uIConfirmComponent = UIConfirmComponentExtend.GetUIConfirmComponent();
                    uIConfirmComponent.SetTipText($"鍏戞崲鐮佽緭鍏ラ敊璇?娆★紝杩炵画5娆¤緭閿欏垯12灏忔椂鍐呮棤娉曞啀娆¤緭鍏ワ紒");
                    return;
                }
            }
            SendDuiHuanCode().Coroutine();
            async ETVoid SendDuiHuanCode()
            {
                G2C_UseRedemptionCodeResponse g2C_useRedemptionCodeResponse = (G2C_UseRedemptionCodeResponse)await SessionComponent.Instance.Session.Call(new C2G_UseRedemptionCodeRequest
                {
                    RedemptionCode = duihuanmaStr,
                });
                if (g2C_useRedemptionCodeResponse.Error != 0)
                {
                    //Debug.Log(g2C_useRedemptionCodeResponse.Message);
                    if (g2C_useRedemptionCodeResponse.Error == 3204)
                    {
                        long useTime = g2C_useRedemptionCodeResponse.TimeTick;
                        //UIComponent.Instance.VisibleUI(UIType.UIHint, $"鍏戞崲鐮佽緭鍏ラ敊璇?娆★紝杩炵画5娆¤緭閿欏垯12灏忔椂鍐呮棤娉曞啀娆¤緭鍏ワ紒");
                        UIConfirmComponent uIConfirmComponent = UIConfirmComponentExtend.GetUIConfirmComponent();
                        uIConfirmComponent.SetTipText($"鍏戞崲鐮佽緭鍏ラ敊璇?娆★紝杩炵画5娆¤緭閿欏垯12灏忔椂鍐呮棤娉曞啀娆¤緭鍏ワ紒");
                        PlayerPrefs.SetString("duihuanmaTime" + UnitEntityComponent.Instance.LocaRoleUUID.ToString(), useTime.ToString());
                    }
                    else
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_useRedemptionCodeResponse.Error.GetTipInfo());
                    }
                }
                else
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "鍏戞崲鎴愬姛");
                }
            }
        }
        /// <summary>
        /// 淇濆瓨璁剧疆
        /// </summary>
        public void Save() 
        {
            GameSetInfo gameSet = new GameSetInfo
            {
                CloseEffect = closeEffectTog.isOn,
                CloseSound = soundTog.isOn,
                CloseMusic = MusicTog.isOn,
                RefuseTream = cancleTeamTog.isOn,
                HideRole = hideroleTog.isOn
            };
            LocalDataJsonComponent.Instance.gameSetInfo = gameSet;
            LocalDataJsonComponent.Instance.SavaData(gameSet, LocalJsonDataKeys.GameSetInfo);
            if (UnitEntityComponent.Instance.LocalRole != null && UnitEntityComponent.Instance.LocalRole.GetComponent<RoleSkillComponent>() is RoleSkillComponent roleSkillComponent)
            {
                roleSkillComponent.gameSetInfo = gameSet;
            }
            
            UIComponent.Instance.Remove(UIType.UIGameSet);
        }

        /// <summary>
        /// 鍒囨崲瑙掕壊
        /// </summary>
        private async ETVoid ChangeRole() 
        {
            SceneName sceneName = SceneComponent.Instance.CurrentSceneName.ToEnum<SceneName>();
            string scenceName = SceneNameExtension.GetSceneName(sceneName);
            if (sceneName == SceneName.XueSeChengBao || sceneName == SceneName.EMoGuangChang || sceneName == SceneName.kalima_map)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "请退出副本后切换角色！");
                return;
            }

            GlobalDataManager.ChangeSceneIsChooseRole = true;//鍒囨崲瑙掕壊涓?
            LogCollectionComponent.Instance.Info("#切换角色# 开始切换角色");

            Gate2C_KickRole gate2C_KickRole = (Gate2C_KickRole)await SessionComponent.Instance.Session.Call(new C2Gate_KickRole { });
            if (gate2C_KickRole.Error != 0)
            {
                GlobalDataManager.ChangeSceneIsChooseRole = false;//鍒囨崲瑙掕壊涓?
                UIComponent.Instance.VisibleUI(UIType.UIHint, gate2C_KickRole.Error.GetTipInfo());
                LogCollectionComponent.Instance.Info("开始切换角色");
                LogCollectionComponent.Instance.Warn($"#鍒囨崲瑙掕壊# 鍒囨崲瑙掕壊澶辫触 Error:{gate2C_KickRole.Error}");
            }
            else
            {
                UIMainComponent mainComponent = UIMainComponent.Instance;
                if (mainComponent != null && !mainComponent.IsDisposed)
                {
                    mainComponent.StopOnHook();
                    mainComponent.ClearTask();

                    if (mainComponent.medicineEntity_Hp != null)
                    {
                        mainComponent.medicineEntity_Hp.Num = 0;
                    }

                    if (mainComponent.medicineEntity_Mp != null)
                    {
                        mainComponent.medicineEntity_Mp.Num = 0;
                    }
                }
                CloseGameplayWindowsForRoleSwitch(removeMainCanvas: false);
                UIComponent.Instance.VisibleUI(UIType.UISceneLoading);//鍦烘櫙鍔犺浇闈㈡澘
                FriendListData.Clear();//娓呯悊濂藉弸缂撳瓨鏁版嵁
               
                TeamDatas.Clear();//娓呯悊闃熶紞鏁版嵁
                TaskDatas.ClearTask();//娓呯悊浠诲姟淇℃伅
                KnapsackItemsManager.ClearKnapsackItems();//娓呯悊鑳屽寘鏁版嵁
                SoundComponent.Instance.Clear();//娓呯悊褰撳墠鍦烘櫙鐨勯煶鏁?
                UIMainComponent.Instance.ClearTask();//娓呯悊褰撳墠瑙掕壊鐨勪换鍔?
                UIMainComponent.Instance.medicineEntity_Hp.Num = 0;
                UIMainComponent.Instance.medicineEntity_Mp.Num = 0;
                TreasureMapComponent.Instance.Clear();// 瀹濊棌灏忓湴鍥緄con

                UIE_MailData.uIE_MailInfos.Clear();//娓呯┖閭欢
                //UIE_MailData.lastClickEmail = null;
                ChatMessageDataManager.ClearChatMeesage();//娓呯┖鑱婂ぉ鏁版嵁
                CameraFollowComponent.Instance.followTarget = null;
                WarAllianceDatas.Clear();//娓呯悊鎴樼洘鏁版嵁
                //娓呯悊瀹炰綋鏁版嵁

                UnitEntityComponent.Instance.Clear();
                Save();
                
                G2C_LoginSystemEnterGameAreaMessage g2C_EnterGameAreaMessage = (G2C_LoginSystemEnterGameAreaMessage)await SessionComponent.Instance.Session.Call(new C2G_LoginSystemEnterGameAreaMessage
                {
                    GameAreaId = GlobalDataManager.EnterZoneID,//澶у尯id
                    LineId = GlobalDataManager.EnterLineID//绾胯矾id
                });
                //鎻愮ず閿欒淇℃伅
                if (g2C_EnterGameAreaMessage.Error != 0)
                {
                    GlobalDataManager.ChangeSceneIsChooseRole = false;
                    UIComponent.Instance.Remove(UIType.UISceneLoading);
                    UIConfirmComponent uIConfirm = UIConfirmComponentExtend.GetUIConfirmComponent();
                    uIConfirm.SetTipText(g2C_EnterGameAreaMessage.Error.GetTipInfo(), true);
                    return;
                }
                else
                {
                    RoleArchiveInfoManager.Instance.CanCreatRoleList = g2C_EnterGameAreaMessage.GameOccupation.ToList();
                }

                //鑾峰彇瑙掕壊淇℃伅
                G2C_LoginSystemGetGamePlayerInfoResponse g2C_GamePlayerGetInfoResponse = (G2C_LoginSystemGetGamePlayerInfoResponse)await SessionComponent.Instance.Session.Call(new C2G_LoginSystemGetGamePlayerInfoRequest
                {
                    GameId = g2C_EnterGameAreaMessage.GameIds
                });
                //鎻愮ず閿欒淇℃伅
                if (g2C_GamePlayerGetInfoResponse.Error != 0)
                {
                    GlobalDataManager.ChangeSceneIsChooseRole = false;
                    UIComponent.Instance.Remove(UIType.UISceneLoading);
                    UIConfirmComponent uIConfirm = UIConfirmComponentExtend.GetUIConfirmComponent();
                    uIConfirm.SetTipText(g2C_GamePlayerGetInfoResponse.Error.GetTipInfo(), true);
                    return;
                }
                RoleArchiveInfoManager.Instance.roleArchiveInfosDic.Clear();
                RoleArchiveInfoManager.Instance.CanCreatRoleList.Clear();
                //缂撳瓨瑙掕壊淇℃伅 鏄剧ず瑙掕壊浣跨敤
                int length = g2C_GamePlayerGetInfoResponse.GameInfos.count;
                for (int i = 0; i < length; i++)
                {
                    //缂撳瓨 瑙掕壊淇℃伅 鏂逛究鍒涘缓瑙掕壊浣跨敤
                    G2C_LoginSystemGetGamePlayerInfoMessage roleInfos = g2C_GamePlayerGetInfoResponse.GameInfos[i];
                    RoleArchiveInfo roleArchive = new RoleArchiveInfo
                    {
                        UUID = roleInfos.GameId,
                        Name = roleInfos.NickName,
                        Level = roleInfos.Level,
                        RoleType = roleInfos.PlayerType,
                        struct_ItemIns = roleInfos.AllEquipStatus.ToList(),
                        ClassLev = roleInfos.OccupationLevel
                    };
                    RoleArchiveInfoManager.Instance.Add(roleInfos.GameId, roleArchive);
                }
                GlobalDataManager.GCClear();
                
                SceneComponent.Instance.LoadScene(SceneName.ChooseRole.ToString());
                UIComponent.Instance.Remove(UIType.UIMainCanvas);
                UIComponent.Instance.VisibleUI(UIType.UIChooseRole);//显示角色选择面板
            }
            GlobalDataManager.IsHideRole = true;
            LogCollectionComponent.Instance.Info("#切换角色# 切换角色流程结束");




        }

        private void CloseGameplayWindowsForRoleSwitch(bool removeMainCanvas = true)
        {
            UIComponent.Instance.Remove(UIType.UIOnHookSet);
            UIComponent.Instance.Remove(UIType.UIRoleInfo);
            UIComponent.Instance.Remove(UIType.UIKnapsack);
            UIComponent.Instance.Remove(UIType.UIKnapsackNew);
            UIComponent.Instance.Remove(UIType.UIFirendList);
            UIComponent.Instance.Remove(UIType.UISkill);
            UIComponent.Instance.Remove(UIType.UIPet);
            UIComponent.Instance.Remove(UIType.UIMount);
            UIComponent.Instance.Remove(UIType.UIShop);
            UIComponent.Instance.Remove(UIType.UISelectOtherPlayer);
            UIComponent.Instance.Remove(UIType.UISceneTranslate);
            if (removeMainCanvas)
            {
                UIComponent.Instance.Remove(UIType.UIMainCanvas);
            }
        }
    }
}
