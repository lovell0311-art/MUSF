using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ETModel
{
    [AttributeUsage(AttributeTargets.Class)]
    public class MergerSystemAttribute : BaseAttribute
    {
        /// <summary>
        /// 合成方法的优先级别
        /// </summary>
        public int Prioritylev { get; }// 优先级

        public MergerSystemAttribute(int prioritylev)
        {
            Prioritylev = prioritylev;
        }
    }
}