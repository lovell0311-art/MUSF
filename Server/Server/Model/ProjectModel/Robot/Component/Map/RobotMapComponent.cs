using System;
using System.Collections.Generic;
using System.Text;

namespace ETModel.Robot
{
    namespace EventType
    {
        public class MapChangeFinish : IDisposable
        {
            public static readonly MapChangeFinish Instance = new MapChangeFinish();

            public Scene ClientScene;
            public long OldMapId;
            public long NewMapId;

            public void Dispose()
            {
                ClientScene = null;
            }
        }

    }


    public class RobotMapComponent : Entity
    {
        #region TempStruct
        /// <summary>
        /// 临时结构体 
        /// </summary>
        public class MapInfo
        {
            public int width { get; set; }
            public int height { get; set; }
            public int SceneInfoSize { get; set; }
            public int[] SceneInfos { get; set; }

        }
        #endregion

        public Map CurrentMap = null;

        
    }
}
