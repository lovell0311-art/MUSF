using System;
using System.Collections.Generic;
using System.Text;

namespace ETModel
{
    public class DBEquipmentItem : DBBase
    {
        /// <summary>
        /// 所属玩家，对应DBGamePlayer的ID，Player的GameUserId
        /// </summary>
        public long GameUserId { get; set; }
    }
}
