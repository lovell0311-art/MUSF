
using System;
using System.Collections.Generic;
using CustomFrameWork.Baseic;
using System.Text;
namespace CustomFrameWork.Baseic
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="K"></typeparam>
    public interface ITEventMethodOnInit<K> : ITEventMethod where K : ACustomComponent
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="b_CustomComponent"></param>
        void OnInit(K b_CustomComponent);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="T1"></typeparam>
    public interface ITEventMethodOnInit<K, T1> : ITEventMethod where K : ACustomComponent
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="b_CustomComponent"></param>
        /// <param name="b_Args1"></param>
        void OnInit(K b_CustomComponent, T1 b_Args1);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    public interface ITEventMethodOnInit<K, T1, T2> : ITEventMethod where K : ACustomComponent
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="b_CustomComponent"></param>
        /// <param name="b_Args1"></param>
        /// <param name="b_Args2"></param>
        void OnInit(K b_CustomComponent, T1 b_Args1, T2 b_Args2);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    public interface ITEventMethodOnInit<K, T1, T2, T3> : ITEventMethod where K : ACustomComponent
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="b_CustomComponent"></param>
        /// <param name="b_Args1"></param>
        /// <param name="b_Args2"></param>
        /// <param name="b_Args3"></param>
        void OnInit(K b_CustomComponent, T1 b_Args1, T2 b_Args2, T3 b_Args3);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    public interface ITEventMethodOnInit<K, T1, T2, T3, T4> : ITEventMethod where K : ACustomComponent
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="b_CustomComponent"></param>
        /// <param name="b_Args1"></param>
        /// <param name="b_Args2"></param>
        /// <param name="b_Args3"></param>
        /// <param name="b_Args4"></param>
        void OnInit(K b_CustomComponent, T1 b_Args1, T2 b_Args2, T3 b_Args3, T4 b_Args4);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    /// <typeparam name="T5"></typeparam>
    public interface ITEventMethodOnInit<K, T1, T2, T3, T4, T5> : ITEventMethod where K : ACustomComponent
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="b_CustomComponent"></param>
        /// <param name="b_Args1"></param>
        /// <param name="b_Args2"></param>
        /// <param name="b_Args3"></param>
        /// <param name="b_Args4"></param>
        /// <param name="b_Args5"></param>
        void OnInit(K b_CustomComponent, T1 b_Args1, T2 b_Args2, T3 b_Args3, T4 b_Args4, T5 b_Args5);
    }
}
