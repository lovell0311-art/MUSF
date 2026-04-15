using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork.Component;
using TencentCloud.Tdmq.V20200217.Models;
using Newtonsoft.Json;
using CustomFrameWork;

namespace ETModel
{
    /// <summary>
    /// C宠物.xlsx-宠物
    /// </summary>
    public partial class Pets_InfoConfig
    {
        public int Ran2 { get; set; }
        public override void InitExpand()
        {
            Ran2 = Ran * 2;
        }
    }
}