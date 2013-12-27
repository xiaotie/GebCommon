#define NET2


#if NET2

namespace System.Runtime.CompilerServices
{
    public class ExtensionAttribute : Attribute { }
}

namespace System
{
    public delegate void Action<T0, T1>(T0 t0, T1 t1);
}

#endif