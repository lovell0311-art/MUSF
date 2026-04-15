using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ETModel;
using CustomFrameWork;
using CustomFrameWork.Component;

namespace ETHotfix
{
    public static class GateUserHelper
    {
        /// <summary>
        /// 下线用户，调用时。无需加协程锁（CoroutineLockType.LoginGate）
        /// </summary>
        /// <param name="gateUser"></param>
        /// <returns></returns>
        public static async Task KickGateUser(this GateUser gateUser)
        {
            if(gateUser == null || gateUser.IsDisposeable)
            {
                return;
            }
            using(await CoroutineLockComponent.Instance.Wait(CoroutineLockType.LoginGate,gateUser.UserID))
            {
                if (gateUser == null || gateUser.IsDisposeable)
                {
                    return;
                }
                switch (gateUser.GateUserState)
                {
                    case GateUserState.Disconnect:
                        break;
                    case GateUserState.Gate:
                        {
                            // 删除LoginCenter在线记录
                            var loginCenterList = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfos(AppType.LoginCenter);
                            Session loginCenterSession = Game.Scene.GetComponent<NetInnerComponent>().Get(loginCenterList[0].ServerInnerIP);
                            await loginCenterSession.Call(new LoginCenter2Gate_RemoveLoginRecord()
                            {
                                UserId = gateUser.UserID,
                                GateServerId = OptionComponent.Options.AppId
                            });
                            if (gateUser.ClientSession != null &&
                                gateUser.ClientSession.IsDisposed == false &&
                                gateUser.ClientSession.InstanceId == gateUser.CSessionInstanceId)
                            {
                                SessionGateUserComponent sessionGateUser = gateUser.ClientSession.GetComponent<SessionGateUserComponent>();
                                if(sessionGateUser != null)
                                {
                                    sessionGateUser.GameUserId = 0;
                                    sessionGateUser.UserId = 0;
                                    sessionGateUser.GameAreaId = 0;
                                    sessionGateUser.GameAreaLineId = 0;
                                    gateUser.ClientSession.RemoveComponent<SessionGateUserComponent>();
                                }
                                gateUser.ClientSession.Disconnect().Coroutine();
                                gateUser.ClientSession = null;
                                gateUser.CSessionInstanceId = 0;
                            }
                            Root.MainFactory.GetCustomComponent<GateUserComponent>().RemoveUserByUserId(gateUser.UserID);
                        }
                        break;
                    case GateUserState.Game:
                        {
                            if(gateUser.GatePlayer != null)
                            {
                                // 已经选择角色，进入游戏
                                var ipEndPoint = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfo(gateUser.GameServerId);
                                Session targetSession = Game.Scene.GetComponent<NetInnerComponent>().Get(ipEndPoint.ServerInnerIP);
                                await targetSession.Call(new S2Game_RequestExitGame() { UserId = gateUser.UserID });

                                var mGatePlayer = Root.MainFactory.GetCustomComponent<GatePlayerComponent>().Remove(gateUser.GatePlayer.GameUserId);
                                if (mGatePlayer != null)
                                {
                                    mGatePlayer.Dispose();
                                }
                            }
                            else
                            {
                                // 在选择角色界面，还没选择角色
                                var ipEndPoint = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfo(gateUser.GameServerId);
                                Session targetSession = Game.Scene.GetComponent<NetInnerComponent>().Get(ipEndPoint.ServerInnerIP);
                                await targetSession.Call(new S2Game_RequestExitGame() { UserId = gateUser.UserID });
                            }
                            // 删除LoginCenter在线记录
                            var loginCenterList = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfos(AppType.LoginCenter);
                            Session loginCenterSession = Game.Scene.GetComponent<NetInnerComponent>().Get(loginCenterList[0].ServerInnerIP);
                            await loginCenterSession.Call(new LoginCenter2Gate_RemoveLoginRecord()
                            {
                                UserId = gateUser.UserID,
                                GateServerId = OptionComponent.Options.AppId
                            });
                            if (gateUser.ClientSession != null && 
                                gateUser.ClientSession.IsDisposed == false &&
                                gateUser.ClientSession.InstanceId == gateUser.CSessionInstanceId)
                            {
                                SessionGateUserComponent sessionGateUser = gateUser.ClientSession.GetComponent<SessionGateUserComponent>();
                                if (sessionGateUser != null)
                                {
                                    sessionGateUser.GameUserId = 0;
                                    sessionGateUser.UserId = 0;
                                    sessionGateUser.GameAreaId = 0;
                                    sessionGateUser.GameAreaLineId = 0;
                                    gateUser.ClientSession.RemoveComponent<SessionGateUserComponent>();
                                }
                                gateUser.ClientSession.Disconnect().Coroutine();
                                gateUser.ClientSession = null;
                                gateUser.CSessionInstanceId = 0;
                            }
                            Root.MainFactory.GetCustomComponent<GateUserComponent>().RemoveUserByUserId(gateUser.UserID);
                        }
                        break;
                }
                
            }
        }

        public static async Task<(bool, int)> KickUser(long userId)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.LoginGate, userId))
            {
                GateUser gateUser = Root.MainFactory.GetCustomComponent<GateUserComponent>().GetUserByUserId(userId);
                if (gateUser != null) return (false, 0);    // 玩家在线，不应该调用此方法
                // 删除LoginCenter在线记录
                var loginCenterList = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfos(AppType.LoginCenter);
                Session loginCenterSession = Game.Scene.GetComponent<NetInnerComponent>().Get(loginCenterList[0].ServerInnerIP);
                var loginCenter2S_GetLoginRecord = (LoginCenter2S_GetLoginRecord)await loginCenterSession.Call(new S2LoginCenter_GetLoginRecord()
                {
                    UserId = userId,
                });
                if (loginCenter2S_GetLoginRecord.Error != 0)
                {
                    return (false, loginCenter2S_GetLoginRecord.Error);
                }
                if (loginCenter2S_GetLoginRecord.GateServerId != OptionComponent.Options.AppId)
                {
                    return (false, 0);
                }
                if (loginCenter2S_GetLoginRecord.GameServerId != 0)
                {
                    // 已经选择角色，进入游戏
                    var ipEndPoint = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfo(loginCenter2S_GetLoginRecord.GameServerId);
                    Session targetSession = Game.Scene.GetComponent<NetInnerComponent>().Get(ipEndPoint.ServerInnerIP);
                    await targetSession.Call(new S2Game_RequestExitGame() { UserId = userId });
                }
                await loginCenterSession.Call(new LoginCenter2Gate_RemoveLoginRecord()
                {
                    UserId = userId,
                    GateServerId = OptionComponent.Options.AppId
                });
            }
            return (true, 0);
        }

        /// <summary>
        /// 下线角色，恢复到刚LoginGate状态。调用时，无需加协程锁（CoroutineLockType.LoginGate）
        /// </summary>
        /// <param name="gateUser"></param>
        /// <returns></returns>
        public static async Task KickRole(this GateUser gateUser)
        {
            if (gateUser == null || gateUser.IsDisposeable)
            {
                return;
            }
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.LoginGate, gateUser.UserID))
            {
                if (gateUser == null || gateUser.IsDisposeable)
                {
                    return;
                }
                switch (gateUser.GateUserState)
                {
                    case GateUserState.Disconnect:
                    case GateUserState.Gate:
                        break;
                    case GateUserState.Game:
                        {
                            SessionGateUserComponent sessionGateUser = null;
                            if (gateUser.ClientSession != null &&
                                gateUser.ClientSession.IsDisposed == false &&
                                gateUser.ClientSession.InstanceId == gateUser.CSessionInstanceId)
                            {
                                sessionGateUser = gateUser.ClientSession.GetComponent<SessionGateUserComponent>();
                            }

                            if (gateUser.GatePlayer != null)
                            {
                                // 已经选择角色，进入游戏
                                var ipEndPoint = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfo(gateUser.GameServerId);
                                Session targetSession = Game.Scene.GetComponent<NetInnerComponent>().Get(ipEndPoint.ServerInnerIP);
                                await targetSession.Call(new S2Game_RequestExitGame() { UserId = gateUser.UserID });

                                var mGatePlayer = Root.MainFactory.GetCustomComponent<GatePlayerComponent>().Remove(gateUser.GatePlayer.GameUserId);
                                if (mGatePlayer != null)
                                {
                                    mGatePlayer.Dispose();
                                }
                                gateUser.GatePlayer = null;
                            }
                            else
                            {
                                // 在选择角色界面，还没选择角色
                                var ipEndPoint = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfo(gateUser.GameServerId);
                                Session targetSession = Game.Scene.GetComponent<NetInnerComponent>().Get(ipEndPoint.ServerInnerIP);
                                await targetSession.Call(new S2Game_RequestExitGame() { UserId = gateUser.UserID });
                            }

                            if (sessionGateUser != null)
                            {
                                sessionGateUser.GameUserId = 0;
                            }

                            gateUser.GameAreaId = 0;
                            gateUser.GameAreaLineId = 0;
                            gateUser.GameServerId = 0;
                            gateUser.GateUserState = GateUserState.Gate;
                        }
                        break;
                }

            }

        }

        /// <summary>
        /// 1秒后断开连接。留足够的窗口，把剩余的消息发完
        /// </summary>
        /// <param name="self"></param>
        public static async Task Disconnect(this Session self)
        {
            long instanceId = self.InstanceId;
            await ETModel.ET.TimerComponent.Instance.WaitAsync(1000);
            if (instanceId != self.InstanceId || self.IsDisposed) return;
            self.Dispose();
        }
    }
}
