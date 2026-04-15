using CustomFrameWork.Baseic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CustomFrameWork
{
    public class Root
    {
        private static CustomCreateBuilder mCreateBuilder;
        public static CustomCreateBuilder CreateBuilder
        {
            get
            {
                if (mCreateBuilder == null)
                {
                    mCreateBuilder = Activator.CreateInstance<CustomCreateBuilder>();
                }
                return mCreateBuilder;
            }
        }

        private static MainFactory mMainFactory;
        public static MainFactory MainFactory
        {
            get
            {
                if (mMainFactory == null)
                {
                    mMainFactory = Activator.CreateInstance<MainFactory>();
                }
                return mMainFactory;
            }
        }

        private static CustomEventSystem mEventSystem;
        public static CustomEventSystem EventSystem
        {
            get
            {
                if (mEventSystem == null)
                {
                    mEventSystem = Activator.CreateInstance<CustomEventSystem>();
                }
                return mEventSystem;
            }
        }

        private static Assembly mHotfixAssembly;
        public static Assembly HotfixAssembly
        {
            get
            {
                return mHotfixAssembly;
            }
        }
        public static void SetHotfixAssembly(Assembly b_Assembly)
        {
            mHotfixAssembly = b_Assembly;
        }

        public static void Close()
        {
            mMainFactory?.Dispose();
            mMainFactory = null;
            mCreateBuilder?.Dispose();
            mCreateBuilder = null;
            mEventSystem.Dispose();
            mEventSystem = null;
            

            mHotfixAssembly = null;

        }
    }
}
