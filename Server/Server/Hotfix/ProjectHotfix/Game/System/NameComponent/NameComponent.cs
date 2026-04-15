using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using CustomFrameWork;
using CustomFrameWork.Baseic;
using CustomFrameWork.Component;
using ETModel;
using TencentCloud.Ame.V20190916.Models;
using TencentCloud.Bri.V20190328.Models;


namespace ETHotfix
{
    [EventMethod(typeof(NameComponent), EventSystemType.INIT)]
    public class NameComponentEventOnInit : ITEventMethodOnInit<NameComponent>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="b_Component"></param>
        public void OnInit(NameComponent b_Component)
        {
            b_Component.OnInit();
        }
    }

    [EventMethod(typeof(NameComponent), EventSystemType.LOAD)]
    public class NameComponentEventOnLoad : ITEventMethodOnLoad<NameComponent>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="b_Component"></param>
        public override void OnLoad(NameComponent b_Component)
        {
            b_Component.OnInit();
        }
    }

    public static class NameComponentComponent
    {
        public static void OnInit(this NameComponent b_Component)
        {
            b_Component.Namelist.Clear();

            string configStr = File.ReadAllText($"./dat_word_filter.txt");

            var wordfilterlist = configStr.Split(",");

            for (int i = 0, len = wordfilterlist.Length; i < len; i++)
            {
                var mWordfilter = wordfilterlist[i];

                if (mWordfilter.Trim().Equals("")) continue;

                b_Component.Namelist.Add(mWordfilter);
            }
        }

    }
}