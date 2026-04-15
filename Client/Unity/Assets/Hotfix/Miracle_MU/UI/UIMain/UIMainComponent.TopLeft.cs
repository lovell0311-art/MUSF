using ETModel;

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{


    /// <summary>
    /// 面板左上角模块
    /// </summary>
    public partial class UIMainComponent
    {
        public ReferenceCollector referenceCollector_topleft;
        public Text rolePosTxt;
        public RoleEntity roleEntity;

        public Dropdown PK_dropdown;
        readonly List<string> PKList = new List<string>() {"和平","全体","友方" };
        public Toggle HookTog;
        Toggle HideTog;


        public Button pkbtn, speacebtn, friendbtn, allbtn;
        public GameObject pk;
        public Sprite Peace, Friend, All;
        //玩家当前的坐标位置
        public AstarNode rolecurAstarNode;
        GameSetInfo gameSetInfo;
        private Text lable;

        public void Init_TopLeft() 
        {
            roleEntity = UnitEntityComponent.Instance.LocalRole;
            referenceCollector_topleft = ReferenceCollector_Main.GetGameObject("TopLeft").GetReferenceCollector();
            rolePosTxt = referenceCollector_topleft.GetText("rolePosTxt");//玩家在场景中的坐标信息
            //rolePosTxt = ReferenceCollector_Main.GetText("rolePosTxt");//玩家在场景中的坐标信息

            //挂机设置
            referenceCollector_topleft.GetButton("SetBtn").onClick.AddSingleListener(OpenOnHookSet);
            //相机
            referenceCollector_topleft.GetButton("camesetBtn").onClick.AddSingleListener(OpenCameraSettings);
            //邀请
            referenceCollector_topleft.GetButton("tuiguangBtn").onClick.AddSingleListener(OpenInviteCode);
            //挂机
            HookTog = referenceCollector_topleft.GetToggle("hookTog");
            lable = referenceCollector_topleft.GetText("lable");
           // lable.text = "挂机";


            HookTog.onValueChanged.AddSingleListener(OnHook);
            HookTog.isOn = false;
            //地图场景切换
            referenceCollector_topleft.GetButton("mapBtn").onClick.AddSingleListener(OpenSceneTranslate);
            //功能引导
            referenceCollector_topleft.GetButton("GuidBtn").onClick.AddSingleListener(OpenFeatureGuide);

            //PK 模式选择
            //0:和平->PVE模式 开启后不能攻击除红名以外的任何人、可以被开启PVP模式的其他人攻击
            //1：全体->PVP模式 开启后能无差别攻击任何模式下的任何玩家 可被开启PVP模式的其他人攻击
            //2：友方 ->PVP模式 开启后不能攻击组队、好友、战盟中的玩家、可攻击除这三者以外饿任何玩家、可被开启PVP模式的其他人攻击
            //注意:目前恶魔广场、血色城堡副本地图两张图中不可开启PVP模式 （即全体、友方两种模式）
            //PK_dropdown = referenceCollector_topleft.GetImage("PK_Dropdown").GetComponent<Dropdown>();
            //PK_dropdown.ClearOptions();//清理所有
            //PK_dropdown.AddOptions(PKList);//添加列表
                                           //监听事件
            /*PK_dropdown.onValueChanged.AddListener(async (value) =>

             //PK 模式选择
              //0:和平->PVE模式 开启后不能攻击除红名以外的任何人、可以被开启PVP模式的其他人攻击
              //1：全体->PVP模式 开启后能无差别攻击任何模式下的任何玩家 可被开启PVP模式的其他人攻击
              //2：友方 ->PVP模式 开启后不能攻击组队、好友、战盟中的玩家、可攻击除这三者以外饿任何玩家、可被开启PVP模式的其他人攻击
              //注意:目前恶魔广场、血色城堡副本地图两张图中不可开启PVP模式 （即全体、友方两种模式）
              PK_dropdown = referenceCollector_topleft.GetImage("PK_Dropdown").GetComponent<Dropdown>());
             // PK_dropdown.ClearOptions();//清理所有
              //PK_dropdown.AddOptions(PKList);//添加列表
              //监听事件
              PK_dropdown.onValueChanged.AddListener(async(value) => 
              {
                  if (roleEntity.Property.GetProperValue(E_GameProperty.Level) < 50)
                  {
                      UIComponent.Instance.VisibleUI(UIType.UIHint,"角色等级达到50级后才可切换PK模式");
                      PK_dropdown.value = (int)GlobalDataManager.BattleModel;//默认和平模式
                      return;
                  }
                  //请求设置 PK 模式
                  G2C_SetPlayerKillingResponse g2C_SetPlayerKilling = (G2C_SetPlayerKillingResponse)await SessionComponent.Instance.Session.Call(new C2G_SetPlayerKillingRequest {Model=value});
                  if (g2C_SetPlayerKilling.Error != 0)
                  {
                      UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_SetPlayerKilling.Error.GetTipInfo());
                  }
                  else
                  {
                      GlobalDataManager.BattleModel = (E_BattleType)value;
                    //  Log.DebugGreen($"当前PK模式：{value}");
                  }

              });
              PK_dropdown.value = (int)GlobalDataManager.BattleModel;//默认和平模式
            */


            Peace = referenceCollector_topleft.GetSprite("Peace");
            Friend = referenceCollector_topleft.GetSprite("Friend");
            All = referenceCollector_topleft.GetSprite("All");
            pkbtn = referenceCollector_topleft.GetButton("Pk_Btn");
            pk = referenceCollector_topleft.GetGameObject("PK");
            pkbtn.onClick.AddSingleListener(TogglePkPanel);

            speacebtn = referenceCollector_topleft.GetButton("peace");
            friendbtn = referenceCollector_topleft.GetButton("friend");
            allbtn = referenceCollector_topleft.GetButton("all");
            speacebtn.onClick.AddSingleListener(OnPeacePkClick);
            friendbtn.onClick.AddSingleListener(OnFriendlyPkClick);
            allbtn.onClick.AddSingleListener(OnWholePkClick);
            pkbtn.GetComponent<Image>().sprite = GetSp(GlobalDataManager.BattleModel);





            //一键隐藏
            gameSetInfo = LocalDataJsonComponent.Instance.LoadData<GameSetInfo>(LocalJsonDataKeys.GameSetInfo);
            HideTog = referenceCollector_topleft.GetToggle("HideTog");
            HideTog.isOn = false;
            HideTog.onValueChanged.AddSingleListener(OnHideToggleChanged);
           
            //测试
            referenceCollector_topleft.GetButton("Button").onClick.AddSingleListener(OpenAddEquipments);
            referenceCollector_topleft.GetButton("Button").gameObject.SetActive(false);

        }
        //改变PK状态
        public void ChangePkState(int pkstate)
        {
            E_BattleType e_Battle = (E_BattleType)pkstate;
            pkbtn.GetComponent<Image>().sprite = GetSp(e_Battle);
            GlobalDataManager.BattleModel = e_Battle;
            pk.SetActive(false);

            //PK_dropdown.value = pkstate;//默认和平模式
        }
        Sprite GetSp(E_BattleType type)
        {
            if (type == E_BattleType.Peace) return Peace;
            else if (type == E_BattleType.Friendly) return Friend;
            else if (type == E_BattleType.Whole) return All;
            else return Peace;
        }
        //玩家坐标
        public void ChangeRolePosTxt(AstarNode node)
        {
            if (rolePosTxt == null)
            {
                return;
            }

            string sceneName = SceneComponent.Instance.CurrentSceneName.ToEnum<SceneName>().GetSceneName();
            if (node == null)
            {
                rolePosTxt.text = sceneName;
                rolecurAstarNode = null;
                return;
            }

            rolePosTxt.text = sceneName + "（" + node.x + "," + node.z + "）";
            rolecurAstarNode = node;
            OnLocalRoleMoveInDetail(node);
        }
        //挂机
        private void OnHook(bool isOn)
        {
            if (sitDownBtn.gameObject.activeSelf)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "坐在宝座上无法使用挂机");
                return;
            }
            if (isOn && roleEntity.IsSafetyZone)
            {
                HookTog.isOn = !isOn;
                UIComponent.Instance.VisibleUI(UIType.UIHint, "安全区 不能挂机");
                return;
            }
            if (SiegeWarfareData.CurroleId == roleEntity.Id && SiegeWarfareData.SiegeWarfareIsStart)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint,"坐在宝座上无法使用挂机");
                return;
            }
           // Log.DebugGreen($"roleEntity.GetComponent<RoleOnHookComponent>():{roleEntity.GetComponent<RoleOnHookComponent>()==null}");
            //if (BeginnerGuideData.IsComplete(30))
            //{
            //    SetMask(true);
            //    BeginnerGuideData.SetBeginnerGuide(30);
            //}
            if (isOn)
            {

               // lable.text = "暂停";
                ShowHuntLog();
                roleEntity.GetComponent<RoleOnHookComponent>()?.StartOnHook();
            }
            else
            {
             //   lable.text = "挂机";
                HideHuntLog();
                roleEntity.GetComponent<RoleOnHookComponent>()?.StopOnHook();
            }
        }

        public void StopOnHook() 
        {
            HookTog.isOn = false;
        }

        private void OpenOnHookSet()
        {
            UIComponent.Instance.VisibleUI(UIType.UIOnHookSet);
        }

        private void OpenCameraSettings()
        {
            UIComponent.Instance.VisibleUI(UIType.UISetCameraAtr);
        }

        private void OpenInviteCode()
        {
            UIComponent.Instance.VisibleUI(UIType.UIYaoQingMa);
        }

        private void OpenSceneTranslate()
        {
            UIComponent.Instance.VisibleUI(UIType.UISceneTranslate);
        }

        private void OpenFeatureGuide()
        {
            UIComponent.Instance.VisibleUI(UIType.UIFeatureGuide);
        }

        private void TogglePkPanel()
        {
            if (pk != null)
            {
                pk.SetActive(!pk.activeSelf);
            }
        }

        private void OnPeacePkClick()
        {
            ChangePkStates(E_BattleType.Peace).Coroutine();
        }

        private void OnFriendlyPkClick()
        {
            ChangePkStates(E_BattleType.Friendly).Coroutine();
        }

        private void OnWholePkClick()
        {
            ChangePkStates(E_BattleType.Whole).Coroutine();
        }

        private async ETVoid ChangePkStates(E_BattleType state)
        {
            if (roleEntity == null || roleEntity.Property == null)
            {
                return;
            }

            if (roleEntity.Property.GetProperValue(E_GameProperty.Level) < 50)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "角色等级达到50级后才可切换PK模式");
                if (PK_dropdown != null)
                {
                    PK_dropdown.value = (int)GlobalDataManager.BattleModel;
                }

                return;
            }

            G2C_SetPlayerKillingResponse response = (G2C_SetPlayerKillingResponse)await SessionComponent.Instance.Session.Call(
                new C2G_SetPlayerKillingRequest { Model = (int)state });
            if (response.Error != 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, response.Error.GetTipInfo());
                return;
            }

            GlobalDataManager.BattleModel = state;
            Image pkImage = pkbtn != null ? pkbtn.GetComponent<Image>() : null;
            if (pkImage != null)
            {
                pkImage.sprite = GetSp(state);
            }

            if (pk != null)
            {
                pk.SetActive(false);
            }
        }

        private void OnHideToggleChanged(bool value)
        {
            gameSetInfo.HideRole = value;
            LocalDataJsonComponent.Instance.gameSetInfo.HideRole = value;
            GlobalDataManager.IsHideRole = !value;
            if (value)
            {
                CameraComponent.Instance.MainCamera.cullingMask &= ~(1 << 12);
                foreach (var item in UnitEntityComponent.Instance.RoleEntityDic.Values)
                {
                    if (item.Id == UnitEntityComponent.Instance.LocaRoleUUID)
                    {
                        continue;
                    }

                    item.GetComponent<UIUnitEntityHpBarComponent>().RefreshTitle(false);
                }
            }
            else
            {
                CameraComponent.Instance.MainCamera.cullingMask |= (1 << 12);
                foreach (var item in UnitEntityComponent.Instance.RoleEntityDic.Values)
                {
                    if (item.Id == UnitEntityComponent.Instance.LocaRoleUUID)
                    {
                        continue;
                    }

                    item.GetComponent<UIUnitEntityHpBarComponent>().RefreshTitle(true);
                }
            }
        }

        private void OpenAddEquipments()
        {
            UIComponent.Instance.VisibleUI(UIType.UIAddEquipMents);
        }
    }
}
