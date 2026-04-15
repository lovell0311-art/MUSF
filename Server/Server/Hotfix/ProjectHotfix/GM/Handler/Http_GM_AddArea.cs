using System;
using System.Collections.Generic;
using System.Text;
using ETModel;
using ETModel.HttpProto;
using CustomFrameWork;
using CustomFrameWork.Component;
using System.Net;
using System.Threading.Tasks;
using MongoDB.Bson;
using System.Linq;

namespace ETHotfix
{
    [HttpHandler(AppType.GM, "/api/HeQu/")]
    public class Http_GM_HeQu : AHttpHandler
    {

        [Post]  // Url-> /api/HeQu
        public async Task<object> HeQu(HttpListenerRequest req, string param)
        {
            var GMTool = Root.MainFactory.GetCustomComponent<GMKeyComponent>();
            Console.WriteLine(param);
            var data = GMTool.GetRequest(param);
            if (data.TryGetValue("NewArea", out var Key) == false)
            {
                return "Error";
            }
            if (data.TryGetValue("Command", out var Command) == false)
            {
                return "Error";
            }
            switch (Command)
            {
                case "HeBi":
                    {
                        Console.WriteLine(Command);
                        try
                        {
                            var loginCenterList = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfos(AppType.Realm);
                            Session loginCenterSession = Game.Scene.GetComponent<NetInnerComponent>().Get(loginCenterList[0].ServerInnerIP);
                            loginCenterSession.Send(new GM2R_UpdateArwaInfo() { AreaInfo = Key });
                        }
                        catch (Exception e)
                        {
                            return Error(msg: e.ToString());
                        }
                        return "Ok";
                    }
                case "loadAll":
                    {
                        Console.WriteLine(Command);
                        try
                        {
                            var loginCenterList = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetAll();
                            foreach (var loginCenter in loginCenterList)
                            {
                                foreach (var ServerInfo in loginCenter.Value)
                                {
                                    Session loginCenterSession = Game.Scene.GetComponent<NetInnerComponent>().Get(ServerInfo.Value.ServerInnerIP);
                                    if (loginCenterSession != null)
                                        loginCenterSession.Send(new G2All_UpdateCommandInfo() { Command = Command,AreaId = 0 });
                                }
                            }

                        }
                        catch (Exception e)
                        {
                            return Error(msg: e.ToString());
                        }
                        return "Ok";
                    }
                case "LoadGame":
                    {
                        Console.WriteLine(Command);
                        try
                        {
                            var loginCenterList = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfos(AppType.Game);
                            foreach (var loginCenter in loginCenterList)
                            {
                                Session loginCenterSession = Game.Scene.GetComponent<NetInnerComponent>().Get(loginCenter.ServerInnerIP);
                                if (loginCenterSession != null)
                                    loginCenterSession.Send(new G2All_UpdateCommandInfo() { Command = Command, AreaId = 0 });

                            }

                        }
                        catch (Exception e)
                        {
                            return Error(msg: e.ToString());
                        }
                        return "Ok";
                    }
                case "OffAll":
                case "shutdown":
                    {
                        Console.WriteLine(Command);
                        try
                        {
                            if (Key == "0")
                            {
                                var ExitCenterList = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetAll();
                                foreach (var loginCenter in ExitCenterList)
                                {
                                    if (loginCenter.Key == (int)AppType.GM) continue;

                                    foreach (var ServerInfo in loginCenter.Value)
                                    {
                                        Session loginCenterSession = Game.Scene.GetComponent<NetInnerComponent>().Get(ServerInfo.Value.ServerInnerIP);
                                        if (loginCenterSession != null)
                                            loginCenterSession.Send(new G2All_UpdateCommandInfo() { Command = Command, AreaId = 0 });
                                    }

                                }
                                await Task.Delay(2000);
                                System.Environment.Exit(0);
                            }
                            else
                            {
                                List< AppType> ServerTypes = new List<AppType>() { AppType.Game, AppType.MGMT, AppType.DB };
                                if (int.TryParse(Key, out int AreaId))
                                {
                                    foreach (var Type in ServerTypes)
                                    {
                                        var loginCenterList = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfos(Type);
                                        foreach (var loginCenter in loginCenterList)
                                        {
                                            if (loginCenter.ZoneId != AreaId) continue;

                                            Session loginCenterSession = Game.Scene.GetComponent<NetInnerComponent>().Get(loginCenter.ServerInnerIP);
                                            if (loginCenterSession != null)
                                                loginCenterSession.Send(new G2All_UpdateCommandInfo() { Command = Command, AreaId = AreaId });

                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            return Error(msg: e.ToString());
                        }
                        return "Ok";
                    }
                default:
                    Log.Warning($"命令无效：{Command}");
                    break;
            }
            return "Ok";
        }
    }
}
