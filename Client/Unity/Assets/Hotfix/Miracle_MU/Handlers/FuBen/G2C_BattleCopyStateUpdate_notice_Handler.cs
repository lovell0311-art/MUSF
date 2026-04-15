using ETModel;

namespace ETHotfix
{
    /// <summary>
    /// 副本 房间变动
    /// </summary>
    [MessageHandler]
    public class G2C_BattleCopyStateUpdate_notice_Handler : AMHandler<G2C_BattleCopyStateUpdate_notice>
    {
        protected override void Run(ETModel.Session session, G2C_BattleCopyStateUpdate_notice message)
        {
            if (UIMainComponent.Instance == null) return;
            var state = message.State;//数组下标 1：恶魔广场 2：血色城堡 
            var waitTime = message.LeftSeconds;//进入状态剩余时间
            SceneName sceneName = SceneComponent.Instance.CurrentSceneName.ToEnum<SceneName>();
            string scenceName = SceneNameExtension.GetSceneName(sceneName);
            switch (state)//副本状态
            {
                case 0://持续关闭状态
                    break;
                case 1://准备 
                    UIMainComponent.Instance.FuBenStartOrEnd = "开启";
                    if (message.MapType == 1)
                    {
                        UIMainComponent.Instance.ChangEMoGuangChangState(waitTime);//恶魔广场 
                        UIMainComponent.Instance.ChangEMoGuangChangState(true);//恶魔广场 
                    }
                    else if (message.MapType == 2)
                    {
                        UIMainComponent.Instance.ChangXueSeChengBaoState(waitTime);
                        UIMainComponent.Instance.ChangXueSeChengBaoState(true);
                    }
                    break;
                case 2://开启
                    UIMainComponent.Instance.FuBenStartOrEnd = "结束";
                   
                    if (sceneName == SceneName.XueSeChengBao || sceneName == SceneName.EMoGuangChang)
                    {
                        if(sceneName == SceneName.XueSeChengBao)
                            UIMainComponent.Instance.ChangeXueSeAstar(true, false);
                        UIMainComponent.Instance.StartFubenCountDown(true,scenceName, waitTime, true);
                    }
                    else
                    {
                        if (message.MapType == 1)
                        {
                            UIMainComponent.Instance.ChangEMoGuangChangState(0);//恶魔广场 
                            UIMainComponent.Instance.ChangEMoGuangChangState(false);//恶魔广场 
                        }
                        else if (message.MapType == 2)
                        {
                            UIMainComponent.Instance.ChangXueSeChengBaoState(0);//血色城堡SA
                            UIMainComponent.Instance.ChangXueSeChengBaoState(false);//血色城堡
                        }
                    }
                    break;
                case 3://结束

                    UIMainComponent.Instance.FuBenStartOrEnd = string.Empty;
                    if (message.MapType == 1)
                    {
                        UIMainComponent.Instance.ChangEMoGuangChangState(0);//恶魔广场 
                        UIMainComponent.Instance.ChangEMoGuangChangState(false);//恶魔广场 
                    }
                    else if (message.MapType == 2)
                    {
                        UIMainComponent.Instance.ChangXueSeChengBaoState(0);//血色城堡
                        UIMainComponent.Instance.ChangXueSeChengBaoState(false);//血色城堡
                        if (sceneName == SceneName.XueSeChengBao)
                        {
                            UIMainComponent.Instance.ChangeXueSeAstar(true, true);
                        }
                    }
                    break;
            }
        }
    }
}
