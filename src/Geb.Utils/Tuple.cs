using System;
using System.Collections.Generic;
using System.Text;

namespace Geb.Utils
{
    public class Tuple<T1, T2> 
        where T1:new() 
        where T2:new()
    {
        public Tuple() 
        {
            First = new T1();
            Second = new T2();
        }
        public Tuple(T1 first, T2 second)
        {
            First = first;
            Second = second;
        }

        public T1 First { get; set; }
        public T2 Second { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Tuple :");
            sb.AppendLine();
            sb.Append(" - ");
            sb.Append(typeof(T1).Name);
            sb.Append(" : ");
            sb.Append(First);
            sb.AppendLine();
            sb.Append(" - ");
            sb.Append(typeof(T2).Name);
            sb.Append(" : ");
            sb.Append(Second);
            return sb.ToString();
        }
    }

    public class Tuple<T1, T2, T3> : Tuple<T1, T2>
        where T1 : new()
        where T2 : new()
        where T3 : new()
    {
        public Tuple()
        {
            First = new T1();
            Second = new T2();
            Third = new T3();
        }

        public Tuple(T1 first, T2 second, T3 third)
            : base(first, second)
        {
            Third = third;
        }

        public T3 Third { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(base.ToString());
            sb.AppendLine();
            sb.Append(" - ");
            sb.Append(typeof(T3).Name);
            sb.Append(" : ");
            sb.Append(Third);
            return sb.ToString();
        }
    }

    public class Tuple<T1, T2, T3, T4> : Tuple<T1, T2, T3>
        where T1 : new()
        where T2 : new()
        where T3 : new()
        where T4 : new()
    {
        public Tuple() 
        {
            First = new T1();
            Second = new T2();
            Third = new T3();
            Fourth = new T4();
        }
        public Tuple(T1 first, T2 second, T3 third, T4 fourth)
            : base(first, second, third)
        {
            Fourth = fourth;
        }

        public T4 Fourth { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(base.ToString());
            sb.AppendLine();
            sb.Append(" - ");
            sb.Append(typeof(T4).Name);
            sb.Append(" : ");
            sb.Append(Fourth);
            return sb.ToString();
        }
    }

}
