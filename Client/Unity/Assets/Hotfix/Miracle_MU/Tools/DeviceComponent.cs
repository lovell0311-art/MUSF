using ETModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETHotfix
{
    public enum DeviceState
    {
        /// <summary>
        /// 正常
        /// </summary>
        Normal,

        /// <summary>
        /// 提示退出游戏
        /// </summary>
        BanQuitGame,
    }

    [ObjectSystem]
    public class DeviceComponentAwake : AwakeSystem<DeviceComponent>
    {
        public override void Awake(DeviceComponent self)
        {
            self.Awake();
        }
    }

    [ObjectSystem]
    public class DeviceComponentUpdate : UpdateSystem<DeviceComponent>
    {
        public override void Update(DeviceComponent self)
        {
            self.Update();
        }
    }
    public class DeviceComponent : Component
    {
        public static DeviceComponent Instance { get; private set; }

        public DeviceState state = DeviceState.Normal;

        public void Awake()
        {
            Instance = this;
        }

        public void Update()
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                //安卓设备按了返回键
                if (state == DeviceState.Normal)
                {
                    state = DeviceState.BanQuitGame;
                    //退出

                    UIConfirmComponent uIConfirm = UIConfirmComponentExtend.GetUIConfirmComponent();
                    uIConfirm.SetTipText("是否退出游戏?");
                    uIConfirm.AddActionEvent(() =>
                    {
                        LogCollectionComponent.Instance.Info("退出游戏");
                        Application.Quit();
                     //   Component.Global.GetComponent<XySdk>().Exit();
                        Component.Global.GetComponent<SdkUtility>().Exit();

                    });
                }
                else
                {
                    state = DeviceState.Normal;
                   
                }
            }
        }
    }
}
