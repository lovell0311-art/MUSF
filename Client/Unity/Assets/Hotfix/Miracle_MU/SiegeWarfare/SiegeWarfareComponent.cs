using UnityEngine;
using ETModel;


namespace ETHotfix
{
    [ObjectSystem]
    public class SiegeWarfareComponentAwake : AwakeSystem<SiegeWarfareComponent>
    {
        public override void Awake(SiegeWarfareComponent self)
        {
            SiegeWarfareComponent.Instance = self;
            self.Awake();
        }
    }
    //[ObjectSystem]
    //public class SiegeWarfareComponentUpdate : UpdateSystem<SiegeWarfareComponent>
    //{
    //    public override void Update(SiegeWarfareComponent self)
    //    {
    //        self.Update();
    //    }
    //}
    /// <summary>
    /// 动画控制组件
    /// </summary>
    public partial class SiegeWarfareComponent : Component
    {
        public static SiegeWarfareComponent Instance;
        RoleEntity roleEntityLocal;

        //public void Update()
        //{
        //    if (Input.GetKeyDown(KeyCode.F))
        //    {
        //        roleEntityLocal?.GetComponent<AnimatorComponent>().SetBoolValue("SiegeSitDown", true);
        //    }
        //    if (Input.GetKeyDown(KeyCode.G))
        //    {
        //        roleEntityLocal?.GetComponent<AnimatorComponent>().SetBoolValue("SiegeSitDown", false);
        //    }
        //}

        public void Awake()
        {
            
            //进入宝座
            TriggerEvents.Instance.RoleActionEnter = (t) =>
            {
                //startTimeKeeping = true;
                IntoThrone(t.gameObject);
            };
            //离开宝座
            TriggerEvents.Instance.RoleActionLevea = (t) =>
            {
                UnitEntity unitEntity = UnitEntityComponent.Instance.Get<UnitEntity>(RoleArchiveInfoManager.Instance.LoadRoleUUID);
                if (t.gameObject.GetInstanceID() == unitEntity.Game_Object.GetInstanceID())
                    UIMainComponent.Instance.SetSitDownBtnShow(false);
                //startTimeKeeping = false;
                //LeveaThrone(t.gameObject).Coroutine();
                //timeKeeping = 0;
                LeveaThrone(t.gameObject).Coroutine();
            }; 
            //停留宝座
            TriggerEvents.Instance.RoleActionStay = (t) =>
            {
                //if (SiegeWarfareData.currole != null) return;
                //if (!SiegeWarfareData.SiegeWarfareIsStart && SiegeWarfareData.currole != null)
                //{
                //    SiegeWarfareData.currole.GetComponent<AnimatorComponent>().SetBoolValue("IsMount", false);
                //    SiegeWarfareData.currole = null;
                //}
                //if (SiegeWarfareData.SiegeWarfareIsStart && SiegeWarfareData.currole != null)
                //    t.parent.eulerAngles = new Vector3(0, 90, 0);
                //timeKeeping += Time.deltaTime;
                //if(timeKeeping >= 1f && startTimeKeeping)
                //{
                //    IntoThrone(t.gameObject).Coroutine();
                //    timeKeeping = 0;
                //    startTimeKeeping = false;
                //}
            };

            TimerComponent.Instance.RegisterTimeCallBack(2000, SetLocalRole);

        }
        public void SetLocalRole()
        {
            roleEntityLocal = UnitEntityComponent.Instance.Get<RoleEntity>(RoleArchiveInfoManager.Instance.LoadRoleUUID);
        }
       /// <summary>
       /// 离开宝座
       /// </summary>
       /// <param name="roleEntity">受击的玩家</param>
        public async ETVoid LeveaThrone(GameObject roleEntity)
        {
            if (!SiegeWarfareData.SiegeWarfareIsStart || SiegeWarfareData.currole == null || SiegeWarfareData.currole.Game_Object == null) return;
            if (roleEntity.GetInstanceID() == SiegeWarfareData.currole.Game_Object.GetInstanceID())
            {
                if (roleEntity.GetInstanceID() == roleEntityLocal.Game_Object.GetInstanceID())
                {
                    UIMainComponent.Instance.SetSitDownBtnShow(false);
                  
                    G2C_LeaveYourSeatResponse g2C_LeaveYourSeat = (G2C_LeaveYourSeatResponse)await SessionComponent.Instance.Session.Call(new C2G_LeaveYourSeatRequest() { });
                    if (g2C_LeaveYourSeat.Error != 0)
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_LeaveYourSeat.Error.GetTipInfo());
                    }
                }
            }
        }
        /// <summary>
        /// 进入宝座
        /// </summary>
        /// <param name="roleEntity"></param>
        /// <returns></returns>
        public void IntoThrone(GameObject roleEntity)
        {
            if (SiegeWarfareData.HavePlayer || !SiegeWarfareData.SiegeWarfareIsStart) return;
            if (roleEntityLocal != null && roleEntity.GetInstanceID() == roleEntityLocal.Game_Object.GetInstanceID())
            {
                UIMainComponent.Instance.SetSitDownBtnShow(true);
                //Log.DebugGreen("攻城战进度进入宝座");
                //G2C_TakeAThroneResponse g2C_TakeAThrone = (G2C_TakeAThroneResponse)await SessionComponent.Instance.Session.Call(new C2G_TakeAThroneRequest() { });
                //if (g2C_TakeAThrone.Error != 0)
                //{
                //    UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_TakeAThrone.Error.GetTipInfo());
                //}
            }
        }
        public override void Dispose()
        {
            if (this.IsDisposed)
                return;
            base.Dispose();
            Instance = null;
        }

    }
}