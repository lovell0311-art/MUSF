using CustomFrameWork;
using CustomFrameWork.Baseic;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using TencentCloud.Cat.V20180409.Models;
using System.Data;
using MySql.Data.MySqlClient;
namespace ETModel
{
    public class MySqlStructure
    {
        public int Payid = 0;
        public int Region = 0;
        public string AccountId = "";
        public int Money = 0;
        public int Money_RMB = 0;
        public int PayStatus = 0;
        public DateTime CreateTime = new DateTime();
    }
    public class MySqlComponent : TCustomComponent<MainFactory>
    {
        //public long UpDataTime = 0;
        public Queue<MySqlStructure> mySqlStructures = new Queue<MySqlStructure>();
        public bool State = false;
        public long TimerId = 0;
        public override void Dispose()
        {
            if (IsDisposeable) return;
            State = false;
            mySqlStructures.Clear();
        }
    }
}