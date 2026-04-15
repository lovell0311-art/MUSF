using ETModel;

namespace ETHotfix
{

    [ObjectSystem]
    public class UI51GoldCardComponentAwake : AwakeSystem<UI51GoldCardComponent>
    {
        public override void Awake(UI51GoldCardComponent self)
        {
            self.collector = self.GetParent<UI>().GameObject.GetReferenceCollector();
            self.collector.GetButton("CloseBtn").onClick.AddSingleListener(()=> UIComponent.Instance.Remove(UIType.UI51GoldCard));
            self.collector.GetInputField("Name").onEndEdit.AddSingleListener(value=>self.roleinfo.name=value);
            self.collector.GetInputField("phone").onEndEdit.AddSingleListener(value => 
            {
               
                self.roleinfo.num = value;
            });
            self.collector.GetInputField("Address").onEndEdit.AddSingleListener(value=>self.roleinfo.adr=value);
            self.collector.GetButton("SureBtn").onClick.AddSingleListener(self.SureEnvent);
        }
    }
    /// <summary>
    /// 五一金卡活动
    /// </summary>

    public class UI51GoldCardComponent : Component
    {
        public ReferenceCollector collector;

        public (string name, string num, string adr) roleinfo;
        
        /// <summary>
        /// 确认提交
        /// </summary>
        /// <param name="name"></param>
        /// <param name="num"></param>
        /// <param name="adr"></param>
        public void SureEnvent()
        {
            if (string.IsNullOrEmpty(roleinfo.name))
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint,"请您填写姓名");
                return;
            }
            if (string.IsNullOrEmpty(roleinfo.num))
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "请您填写联系电话");
                return;
            }
            if (roleinfo.num.Length != 11)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "手机号格式不对 请你重新填写");
                return;
            }
            if (string.IsNullOrEmpty(roleinfo.adr))
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "请您填写地址");
                return;
            }
          
        }
    }
}