using System;
using System.Collections.Generic;
using System.Linq;
using ETModel;
using ETModel.Robot;
using System.Threading.Tasks;
using CustomFrameWork;

namespace ETHotfix.Robot
{
    public static partial class RobotPetsWindowsComponentSystem
    {
        /// <summary>
        /// 打开宠物面板
        /// </summary>
        public static async Task<bool> OpenPetsWindowsAsync(this RobotPetsWindowsComponent self,ETCancellationToken cancellationToken)
        {
            Scene clientScene = self.GetParent<Scene>();
            Session session = clientScene.GetComponent<SessionComponent>().session;

            IResponse res = await session.Call(new C2G_OpenPetsInterfaceRequest(),cancellationToken);
            if (cancellationToken.IsCancel()) return false;
            if(res.Error != ErrorCode.ERR_Success)
            {
                Log.Warning($"C2G_OpenPetsInterfaceRequest:{res.Error}");
                return false;
            }
            // 清除战斗状态
            if(self.FirstPetsId != 0)
            {
                if (self.PetsDict.TryGetValue(self.FirstPetsId, out RobotPetsInfo petsInfo))
                {
                    petsInfo.IsToWar = false;
                }
                self.FirstPetsId = 0;
            }


            Dictionary<long, RobotPetsInfo> tempPetsDict = new Dictionary<long, RobotPetsInfo>();
            // Swap
            (tempPetsDict, self.PetsDict) = (self.PetsDict, tempPetsDict);

            G2C_OpenPetsInterfaceResponse g2C_OpenPetsInterfaceResponse = res as G2C_OpenPetsInterfaceResponse;
            PetsInfo current = g2C_OpenPetsInterfaceResponse.Current;
            if (current.PetsConfigID != 0)
            {
                RobotPetsInfo petsInfo = null;
                if(!tempPetsDict.TryGetValue(current.PetsID, out petsInfo))
                {
                    petsInfo = RobotPetsInfoFactory.Create(clientScene, current.PetsID, current.PetsConfigID);
                }
                else
                {
                    tempPetsDict.Remove(current.PetsID);
                }
                petsInfo.IsToWar = g2C_OpenPetsInterfaceResponse.IsToWar == 1 ? true : false;
                petsInfo.FromProtoPetsInfo(current);
                self.PetsDict.Add(current.PetsID, petsInfo);
                self.FirstPetsId = current.PetsID;
            }
            foreach (PetsRankInfo petsRankInfo in g2C_OpenPetsInterfaceResponse.List)
            {
                RobotPetsInfo petsInfo = null;
                if (!tempPetsDict.TryGetValue(current.PetsID, out petsInfo))
                {
                    petsInfo = RobotPetsInfoFactory.Create(clientScene, current.PetsID, current.PetsConfigID);
                }
                else
                {
                    tempPetsDict.Remove(current.PetsID);
                }
                petsInfo.IsDeath = petsRankInfo.IsDeath == 1 ? true : false;
                petsInfo.DeathTime = petsRankInfo.DeathTime;
                petsInfo.PetsTrialTime = petsRankInfo.PetsTrialTime;
                NumericComponent numeric = petsInfo.GetComponent<NumericComponent>();
                numeric.SetNoEvent((int)E_GameProperty.Level, petsRankInfo.PetsLevel);
                self.PetsDict.Add(current.PetsID, petsInfo);
            }

            // 清除不存在的数据
            foreach(RobotPetsInfo petsInfo in tempPetsDict.Values.ToArray())
            {
                petsInfo.Dispose();
            }
            tempPetsDict.Clear();
            return true;
        }

        public static async Task<bool> AddAttributePointAsync(this RobotPetsWindowsComponent self,RobotPetsInfo petsInfo, ETCancellationToken cancellationToken)
        {
            Scene clientScene = self.GetParent<Scene>();
            Session session = clientScene.GetComponent<SessionComponent>().session;

            NumericComponent numeric = petsInfo.GetComponent<NumericComponent>();
            int freePoint = numeric.GetAsInt((int)E_GameProperty.FreePoint);
            if (freePoint <= 0) return false;

            RandomSelector<int> selector = new RandomSelector<int>();
            selector.Add(1, petsInfo.Config.Strength);
            selector.Add(2, petsInfo.Config.Willpower);
            selector.Add(3, petsInfo.Config.Agility);
            selector.Add(4, petsInfo.Config.BoneGas);

            Dictionary<int,int> addPointNumber= new Dictionary<int, int>();
            addPointNumber[1] = 0;
            addPointNumber[2] = 0;
            addPointNumber[3] = 0;
            addPointNumber[4] = 0;

            for(int i =0;i<freePoint;++i)
            {
                selector.TryGetValue(out int attrType);
                addPointNumber[attrType] += 1;
            }

            foreach(var kv in addPointNumber)
            {
                IResponse res = await session.Call(new C2G_AddAttributePointRequest()
                {
                    PetsID = petsInfo.Id,
                    PetsAttributeType = kv.Key,
                    PetsAddPoint = kv.Value,
                }, cancellationToken);
                if (cancellationToken.IsCancel()) return false;
                if (res.Error != ErrorCode.ERR_Success)
                {
                    Log.Warning($"C2G_AddAttributePointRequest:{res.Error}");
                    return false;
                }
                G2C_AddAttributePointResponse g2C_AddAttributePointResponse = res as G2C_AddAttributePointResponse;
                if(self.PetsDict.TryGetValue(g2C_AddAttributePointResponse.Info.PetsID,out RobotPetsInfo nwePetsInfo))
                {
                    nwePetsInfo.FromProtoPetsInfo(g2C_AddAttributePointResponse.Info);
                }
            }
            return true;
        }

        public static async Task<bool> GoToWarAsync(this RobotPetsWindowsComponent self, long petsId, ETCancellationToken cancellationToken)
        {
            Scene clientScene = self.GetParent<Scene>();
            Session session = clientScene.GetComponent<SessionComponent>().session;
            IResponse res = await session.Call(new C2G_PetsGoToWarRequest()
            {
                PetsID = petsId,
            }, cancellationToken);
            if (cancellationToken.IsCancel()) return false;
            if (res.Error != ErrorCode.ERR_Success)
            {
                Log.Warning($"C2G_PetsGoToWarRequest:{res.Error}");
                return false;
            }
            if(self.PetsDict.TryGetValue(self.FirstPetsId, out RobotPetsInfo petsInfo))
            {
                petsInfo.IsToWar = false;
            }
            if (self.PetsDict.TryGetValue(petsId, out petsInfo))
            {
                petsInfo.IsToWar = true;
                self.FirstPetsId = petsId;
            }

            return true;
        }
    }
}
