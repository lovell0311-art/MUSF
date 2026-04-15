using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using CustomFrameWork;
using CustomFrameWork.Baseic;
using CustomFrameWork.Component;

namespace ETModel
{
    public static class ReadConfigComponentSystem
    {
        /// <summary>
        /// 拉取配置
        /// </summary>
        /// <returns></returns>
        public static async Task LoadConfigAsync(this ReadConfigComponent b_Component)
        {
            ReadWriteComponent mReadWriteComponent = Root.MainFactory.GetCustomComponent<ReadWriteComponent>();

            {
                ConfigInfoComponent c_component = Root.MainFactory.GetCustomComponent<ConfigInfoComponent>();
                string mDBServerPath = @$"{OptionComponent.Options.ConfigPath}\StartUpConfig\Server_DataConfig.json";
                mDBServerPath = mDBServerPath.Replace("\\", "/");
                string mJsonString = await mReadWriteComponent.AddReadAsync(E_ReadWriteLock.CONFIG, mDBServerPath);

                Server_DataConfigJson mHeroJson = new Server_DataConfigJson(mJsonString);
                b_Component.AddJson(mHeroJson);
            }

            {
                Type[] mTypes = typeof(ReadConfigComponentSystem).Assembly.GetTypes();
                for (int i = 0, len = mTypes.Length; i < len; i++)
                {
                    Type type = mTypes[i];
                    if (type.IsDefined(typeof(ReadConfigAttribute), false))
                    {
                        object[] mAttributes = type.GetCustomAttributes(typeof(ReadConfigAttribute), false);
                        for (int j = 0, jlen = mAttributes.Length; j < jlen; j++)
                        {
                            ReadConfigAttribute mEventMethod = mAttributes[j] as ReadConfigAttribute;
                            if (mEventMethod != null)
                            {
                                if (mEventMethod.BindId.Contains(AppType.AllServer) || mEventMethod.BindId.Contains(OptionComponent.Options.AppType))
                                {
                                    string path = $"{OptionComponent.Options.ConfigPath}/NewConfig/{mEventMethod.ConfigType.Name}.json";
                                    string mJsonString = await mReadWriteComponent.AddReadAsync(E_ReadWriteLock.CONFIG, path);
                                    Log.Debug(path);
                                    var mConfigJson = (C_ConfigJson)Activator.CreateInstance(type, mJsonString);
                                    b_Component.AddJson(mConfigJson);
                                }

                                break;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 拉取配置
        /// </summary>
        /// <returns></returns>
        public static void LoadConfig(this ReadConfigComponent b_Component)
        {
            ReadWriteComponent mReadWriteComponent = Root.MainFactory.GetCustomComponent<ReadWriteComponent>();

            {
                ConfigInfoComponent c_component = Root.MainFactory.GetCustomComponent<ConfigInfoComponent>();
                string mDBServerPath = @$"{OptionComponent.Options.ConfigPath}\StartUpConfig\Server_DataConfig.json";
                mDBServerPath = mDBServerPath.Replace("\\", "/");
                string mJsonString = mReadWriteComponent.AddRead(mDBServerPath);

                Server_DataConfigJson mHeroJson = new Server_DataConfigJson(mJsonString);
                b_Component.AddJson(mHeroJson);
            }

            {
                Type[] mTypes = typeof(ReadConfigComponentSystem).Assembly.GetTypes();
                for (int i = 0, len = mTypes.Length; i < len; i++)
                {
                    Type type = mTypes[i];
                    if (type.IsDefined(typeof(ReadConfigAttribute), false))
                    {
                        object[] mAttributes = type.GetCustomAttributes(typeof(ReadConfigAttribute), false);
                        for (int j = 0, jlen = mAttributes.Length; j < jlen; j++)
                        {
                            ReadConfigAttribute mEventMethod = mAttributes[j] as ReadConfigAttribute;
                            if (mEventMethod != null)
                            {
                                if (mEventMethod.BindId.Contains(AppType.AllServer) || mEventMethod.BindId.Contains(OptionComponent.Options.AppType))
                                {
                                    string path = $"{OptionComponent.Options.ConfigPath}/NewConfig/{mEventMethod.ConfigType.Name}.json";
                                    string mJsonString = mReadWriteComponent.AddRead(path);
                                    Log.Debug(path);
                                    var mConfigJson = (C_ConfigJson)Activator.CreateInstance(type, mJsonString);
                                    b_Component.AddJson(mConfigJson);
                                }

                                break;
                            }
                        }
                    }
                }
            }
        }
    }
}
