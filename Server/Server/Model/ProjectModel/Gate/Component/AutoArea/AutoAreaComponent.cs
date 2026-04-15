using CustomFrameWork.Baseic;
using CustomFrameWork;
using System;
using System.Collections.Generic;
using System.Text;
using TencentCloud.Ecm.V20190719.Models;

namespace ETModel
{
    public class AutoInfo
    {
        public int Id;
        public string Name;
        /// <summary>
        /// 0:异常1正常2开区准备中已经处理了数据库信息
        /// </summary>
        public int State;
    }
    public sealed class AutoAreaComponent : TCustomComponent<MainFactory>
    {
        public Dictionary<int, AutoInfo> keyValuePairs = new Dictionary<int, AutoInfo>();
        public long _timerId = 0;
        public override void Dispose()
        {
            keyValuePairs.Clear();
            base.Dispose();
        }
    }
    public sealed class JointAreaSignalDetectionComponent : TCustomComponent<MainFactory>
    {
        public bool OneOfCompatibility = false;
        public long _timerId = 0;
        public override void Dispose()
        {
            OneOfCompatibility = false;
            base.Dispose();
        }
    }
}
