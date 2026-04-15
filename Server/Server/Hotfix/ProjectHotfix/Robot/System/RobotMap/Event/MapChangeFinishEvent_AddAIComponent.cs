using ETModel;
using ETModel.Robot;
using ETModel.Robot.EventType;
using CustomFrameWork;

namespace ETHotfix.Robot
{
    [Event("MapChangeFinish")]
    public class MapChangeFinishEvent_AddAIComponent : AEvent<MapChangeFinish>
    {
        public override void Run(MapChangeFinish args)
        {
            if(args.NewMapId != 0)
            {
                //args.ClientScene.AddComponent<AIComponent,int>(2).AddComponent<ClientSceneComponent, Scene>(args.ClientScene);
            }
        }
    }
}
