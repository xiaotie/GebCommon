using System;
using System.Collections.Generic;
using System.Text;

namespace Geb.Utils.Actions
{
    public delegate void Action();
    public delegate void Action<T0>(T0 t0);
    public delegate void Action<T0, T1>(T0 t0, T1 t1);
    public delegate void Action<T0, T1,T2>(T0 t0, T1 t1,T2 t2);
    public delegate void Action<T0, T1, T2, T3>(T0 t0, T1 t1, T2 t2, T3 t3);
    public delegate void Action<T0, T1, T2, T3, T4>(T0 t0, T1 t1, T2 t2, T3 t3,T4 t4);
}
