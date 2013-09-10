using System;
using System.Collections.Generic;
using System.Text;

namespace Geb.Utils
{
    /// <summary>
    /// 代表IList中的一段[Start,End)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct ListSegment<T>
    {
        public IList<T> Data;
        public int Start;
        public int End;
    }
}
