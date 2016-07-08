using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LilyAcolasia_AI
{
    static class AITuple
    {
        public static AITuple<T1, T2, T3> Create<T1, T2, T3>(T1 item1, T2 item2, T3 item3)
        {
            return new AITuple<T1, T2, T3>(item1, item2, item3);
        }

        public static AITuple<T1, T2, T3, T4> Create<T1, T2, T3, T4>(T1 item1, T2 item2, T3 item3, T4 item4)
        {
            return new AITuple<T1, T2, T3, T4>(item1, item2, item3, item4);
        }

        public static Tuple<T1, T2, T3, T4, T5> Create<T1, T2, T3, T4, T5>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5)
        {
            return new Tuple<T1, T2, T3, T4, T5>(item1, item2, item3, item4, item5);
        }
    }

    public class AITuple<T1, T2, T3>
    {
        private T1 item1;
        private T2 item2;
        private T3 item3;
        public T1 Item1 { get { return item1; } set { this.item1 = value; } }
        public T2 Item2 { get { return item2; } set { this.item2 = value; } }
        public T3 Item3 { get { return item3; } set { this.item3 = value; } }

        public AITuple(T1 item1, T2 item2, T3 item3)
        {
            this.item1 = item1;
            this.item2 = item2;
            this.item3 = item3;
        }
    }

    public class AITuple<T1, T2, T3, T4>
    {
        private T1 item1;
        private T2 item2;
        private T3 item3;
        private T4 item4;
        public T1 Item1 { get { return item1; } set { this.item1 = value; } }
        public T2 Item2 { get { return item2; } set { this.item2 = value; } }
        public T3 Item3 { get { return item3; } set { this.item3 = value; } }
        public T4 Item4 { get { return item4; } set { this.item4 = value; } }


        public AITuple(T1 item1, T2 item2, T3 item3, T4 item4)
        {
            this.item1 = item1;
            this.item2 = item2;
            this.item3 = item3;
            this.item4 = item4;
        }
    }

    public class Tuple<T1, T2, T3, T4, T5>
    {
        private T1 item1;
        private T2 item2;
        private T3 item3;
        private T4 item4;
        private T5 item5;
        public T1 Item1 { get { return item1; } set { this.item1 = value; } }
        public T2 Item2 { get { return item2; } set { this.item2 = value; } }
        public T3 Item3 { get { return item3; } set { this.item3 = value; } }
        public T4 Item4 { get { return item4; } set { this.item4 = value; } }
        public T5 Item5 { get { return item5; } set { this.item5 = value; } }

        public Tuple(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5)
        {
            this.item1 = item1;
            this.item2 = item2;
            this.item3 = item3;
            this.item4 = item4;
            this.item5 = item5;
        }
    }
}
