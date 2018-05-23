using System;
using System.Collections.Generic;
using System.Text;

using System.Reflection;

namespace Geb.Utils
{
    public class Singleton<T>
        where T : class
    {
        protected static Object SyncRoot = new Object();

        protected static T instance;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (instance == null)
                        {
                            ConstructorInfo constructor =
                                typeof(T).GetConstructor(
                                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
                                null, new Type[0], null
                                );

                            if (constructor == null)
                                throw new Exception(String.Format("{0}没有默认构造方法。",typeof(T).ToString()));
                            
                            instance = constructor.Invoke(new object[0]) as T;
                        }
                    }
                }

                return instance;
            }
        }
    }
}
