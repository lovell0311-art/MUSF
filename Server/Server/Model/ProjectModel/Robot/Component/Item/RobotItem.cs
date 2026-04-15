using System;
using System.Collections.Generic;
using System.Text;

namespace ETModel.Robot
{
    public class RobotItem : Entity
    {
        public ItemConfig Config;
        public EItemType Type => (EItemType)(Config.Id / 10000);
        public long GameUserId;
        public EItemInComponent InComponent;
        public int PosId;
        public int PosX;
        public int PosY;
    }
}
