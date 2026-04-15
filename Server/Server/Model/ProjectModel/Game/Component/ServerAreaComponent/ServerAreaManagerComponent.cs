using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using CustomFrameWork;
using CustomFrameWork.Baseic;

namespace ETModel
{
    public class ServerAreaManagerComponent : TCustomComponent<MainFactory>
    {
        /// <summary>
        /// 当前所有区服
        /// </summary>
        public readonly Dictionary<int, C_ServerArea> GameAreaOnServerDic = new Dictionary<int, C_ServerArea>();
        public readonly Dictionary<int, int> IdMappingDic = new Dictionary<int, int>();
        /// <summary>
        /// 服务器停止中...
        /// </summary>
        public bool ServerStopping = false;
        public override void Awake()
        {
            RemoveAllGameArea();
            ServerStopping = false;
        }
        public override void Dispose()
        {
            if (IsDisposeable) return;

            RemoveAllGameArea();
            base.Dispose();
        }
        /// <summary>
        /// 移除所有区服
        /// </summary>
        public void RemoveAllGameArea()
        {
            if (GameAreaOnServerDic.Count > 0)
            {
                List<C_ServerArea> mAllGameAreaComponents = GameAreaOnServerDic.Values.ToList();
                for (int i = 0, len = mAllGameAreaComponents.Count; i < len; i++)
                    mAllGameAreaComponents[i].Dispose();
                GameAreaOnServerDic.Clear();
            }
            if (IdMappingDic.Count > 0)
            {
                IdMappingDic.Clear();
            }
        }

        /// <summary>
        /// 获取区服
        /// </summary>
        /// <param name="b_GameAreaId">区服Id</param>
        /// <returns>返回区服</returns>
        public C_ServerArea GetGameArea(int b_GameAreaId)
        {
            if (IdMappingDic.TryGetValue(b_GameAreaId, out var mMappingId) == false)
            {
                return null;
            }
            if (GameAreaOnServerDic.TryGetValue(mMappingId, out C_ServerArea mGameAreaComponent))
            {
                return mGameAreaComponent;
            }

            return null;
        }
        /// <summary>
        /// 移除目标区服
        /// </summary>
        /// <param name="b_GameAreaId">区服Id</param>
        public void RemoveGameArea(int b_GameAreaId)
        {
            if (GameAreaOnServerDic.TryGetValue(b_GameAreaId, out C_ServerArea mGameAreaComponent))
            {
                GameAreaOnServerDic.Remove(b_GameAreaId);
                mGameAreaComponent.Dispose();
            }
        }
    }
}