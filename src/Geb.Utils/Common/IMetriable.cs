using System;
using System.Collections.Generic;
using System.Text;

namespace Geb.Utils
{
    public interface IMetriable<T>
    {
        double GetDistance(T other);
        int GetDistanceSquare(T other);
    }
}
