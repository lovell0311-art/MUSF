using System;

namespace ETModel
{
    /// <summary>
    /// 通过此类实现逻辑类的单件实例化
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SimpleSingleton<T> where T : new()
    {
        protected static T instance;

        public static T Instance
        {
            get
            {
                if (SimpleSingleton<T>.instance == null)
                {
                    SimpleSingleton<T>.instance = Activator.CreateInstance<T>();
                    if (SimpleSingleton<T>.instance != null)
                    {
                    }
                }
                return instance;
            }
        }

        public static void Release()
        {
            if (SimpleSingleton<T>.instance != null)
            {
                SimpleSingleton<T>.instance = (T)((object)null);
            }
        }

        public virtual void Init()
        {

        }

        public virtual void Disponse()
        {

        }
    }
}
