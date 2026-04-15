using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using CustomFrameWork;
using CustomFrameWork.Baseic;
using UnityEngine;


namespace ETModel
{
    namespace EventType.NSMapEntity
    {
        /// <summary>
        /// 通知 Hotfix 层，自己要离开Map
        /// </summary>
        public class Destory
        {
            public static readonly Destory Instance = new Destory();
            public ETModel.MapEntity self;
        }

        /// <summary>
        /// 进入Map
        /// 事件名 NSMapEntity.EnterMap.类名
        /// </summary>
        public class EnterMap
        {
            public static readonly EnterMap Instance = new EnterMap();
            public ETModel.MapEntity self;
            public ETModel.MapComponent map;
        }

        /// <summary>
        /// 离开Map
        /// 事件名 NSMapEntity.LeaveMap.类名
        /// </summary>
        public class LeaveMap
        {
            public static readonly LeaveMap Instance = new LeaveMap();
            public ETModel.MapEntity self;
            public ETModel.MapComponent map;
        }

    }


    /// <summary>
    /// 在生命周期结束时，会将自己从Map中移除
    /// </summary>
    public class MapEntity : CustomComponent
    {
        private long instanceId { get; set; }
        public long InstanceId => instanceId;
        public MapCellAreaComponent Parent { get; set; } = null;

        public Vector2Int Position = new Vector2Int() { x = 0, y=0 };

        // 当前所在的地图
        public MapComponent Map
        {
            get
            {
                if (this.Parent == null) return null;
                return this.Parent.Parent;
            }
        }

        public override void ContextAwake()
        {
            instanceId = Help_UniqueValueHelper.GetServerUniqueValue();
        }

        public override void Dispose()
        {
            if (IsDisposeable) return;
            // 方法都在Hotfix层，抛个事件让Hotfix去处理
            EventType.NSMapEntity.Destory.Instance.self = this;
            Root.EventSystem.OnRun("NSMapEntity.Destory", EventType.NSMapEntity.Destory.Instance);

            base.Dispose();
            Parent = null;
            instanceId = 0;
            Position.x = 0;
            Position.y = 0;
        }
    }



    public partial class MapComponent
    {
        public Dictionary<long,MapEntity> AllEntities = new Dictionary<long,MapEntity>();
    }

}
