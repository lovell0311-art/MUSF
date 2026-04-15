// using System;
// using System.Collections.Generic;
// using System.Threading.Tasks;

// using CustomFrameWork;
// using CustomFrameWork.Baseic;


// namespace CustomFrameWork.Component
// {
    // public partial class ReadConfigComponent
    // {
        // /// <summary>
        // /// 拉取配置
        // /// </summary>
        // /// <returns></returns>
        // public async Task LoadConfigAsync()
        // {
            // ReadWriteComponent mReadWriteComponent = Root.MainFactory.GetCustomComponent<ReadWriteComponent>();
            
            // {
                // Type[] mTypes = Help_LoadDllHelper.Load("Model").GetTypes();
                // for (int i = 0, len = mTypes.Length; i < len; i++)
                // {
                    // Type type = mTypes[i];
                    // if (type.IsDefined(typeof(ReadConfigAttribute), false))
                    // {
                        // object[] mAttributes = type.GetCustomAttributes(typeof(ReadConfigAttribute), false);
                        // for (int j = 0, jlen = mAttributes.Length; j < jlen; j++)
                        // {
                            // ReadConfigAttribute mEventMethod = mAttributes[j] as ReadConfigAttribute;
                            // if (mEventMethod != null)
                            // {
                                // if (mEventMethod.BindId.Contains(AppType.AllServer) || mEventMethod.BindId.Contains(OptionComponent.Options.AppType))
                                // {
                                    // string path = $"../Config/NewConfig/{mEventMethod.ConfigType.Name}.json"; 
                                    // string mJsonString = await mReadWriteComponent.AddReadAsync(E_ReadWriteLock.CONFIG, path);
                                    // Console.WriteLine(path);
                                    // var mConfigJson = (C_ConfigJson)Activator.CreateInstance(type, mJsonString);
                                    // AddJson(mConfigJson);
                                // }

                                // break;
                            // }
                        // }
                    // }
                // }
            // }
        // }
    // }
// }
