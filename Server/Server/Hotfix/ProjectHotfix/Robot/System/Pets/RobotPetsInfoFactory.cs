using ETModel;
using ETModel.Robot;
using CustomFrameWork;
using CustomFrameWork.Component;


namespace ETHotfix.Robot
{
    public static class RobotPetsInfoFactory
    {

        public static RobotPetsInfo Create(Scene clientScene,long id,int configId)
        {
            ReadConfigComponent readConfig = Root.MainFactory.GetCustomComponent<ReadConfigComponent>();
            if(!readConfig.GetJson<Pets_InfoConfigJson>().JsonDic.TryGetValue(configId, out Pets_InfoConfig config))
            {
                Log.Error($"无法找到宠物配置，configId={configId}");
                return null;
            }
            RobotPetsInfo petsInfo = ComponentFactory.CreateWithId<RobotPetsInfo>(id);
            petsInfo.Config = config;
            petsInfo.AddComponent<ClientSceneComponent, Scene>(clientScene);
            petsInfo.AddComponent<NumericComponent>();
            return petsInfo;
        }
    }
}
