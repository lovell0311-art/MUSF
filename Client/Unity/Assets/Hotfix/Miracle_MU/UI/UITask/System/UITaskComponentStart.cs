using ETModel;
using UnityEditor;


namespace ETHotfix
{
    [ObjectSystem]
    public class UITaskComponentStart : StartSystem<UITaskComponent>
    {
        public override void Start(UITaskComponent self)
        {
            if (self.CurTaskInfo == null)
            {
                self.InitTaskTypeTog();
                //self.GetBattleCopyInfoRequest().Coroutine();
                self.InitActiveMap();
                self.ChangeBtnStatus(self.curTaskStatus);
            }
            self.RegisterEvent();



            /*  if (self.curTog != null)
              {
                  self.curTog.isOn = true;
                  var taskInfo=TaskDatas.GetCurTaskInfo(self.curTaskType);
                  self.ShowCurTaskInfo(taskInfo);
              }*/

        }
    }
}
