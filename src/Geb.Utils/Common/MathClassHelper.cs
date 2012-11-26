using System;
using System.Collections.Generic;
using System.Text;

namespace Geb.Utils
{
    /// <summary>
    /// 与数学有关的扩展方法。
    /// </summary>
    public static class MathHelper
    {
        #region Max 扩展方法

        public static Byte Max(this Byte val1, Byte val2)
        {
            return Math.Max(val1, val2);
        }

        public static Int16 Max(this Int16 val1, Int16 val2)
        {
            return Math.Max(val1, val2);
        }

        public static Int32 Max(this Int32 val1, Int32 val2)
        {
            return Math.Max(val1, val2);
        }

        public static Int64 Max(this Int64 val1, Int64 val2)
        {
            return Math.Max(val1, val2);
        }

        public static Single Max(this Single val1, Single val2)
        {
            return Math.Max(val1, val2);
        }

        public static Double Max(this Double val1, Double val2)
        {
            return Math.Max(val1, val2);
        }

        #endregion

        #region Min 扩展方法

        public static Byte Min(this Byte val1, Byte val2)
        {
            return Math.Min(val1, val2);
        }

        public static Int16 Min(this Int16 val1, Int16 val2)
        {
            return Math.Min(val1, val2);
        }

        public static Int32 Min(this Int32 val1, Int32 val2)
        {
            return Math.Min(val1, val2);
        }

        public static Int64 Min(this Int64 val1, Int64 val2)
        {
            return Math.Min(val1, val2);
        }

        public static Single Min(this Single val1, Single val2)
        {
            return Math.Min(val1, val2);
        }

        public static Double Min(this Double val1, Double val2)
        {
            return Math.Min(val1, val2);
        } 

        #endregion
    }
}
