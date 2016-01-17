using System;
using System.Collections.Generic;
using System.Text;

namespace Geb.Utils
{
    public delegate void TAction();
    public delegate void TAction<T0>(T0 t0);
    public delegate void TAction<T0, T1>(T0 t0, T1 t1);
    public delegate void TAction<T0, T1, T2>(T0 t0, T1 t1, T2 t2);
    public delegate void TAction<T0, T1, T2, T3>(T0 t0, T1 t1, T2 t2, T3 t3);
    public delegate void TAction<T0, T1, T2, T3, T4>(T0 t0, T1 t1, T2 t2, T3 t3, T4 t4);
}
