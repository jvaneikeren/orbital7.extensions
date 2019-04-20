﻿using System;
using System.Collections.Generic;
using System.Text;

namespace System
{
    public class SerializableTuple<T1, T2>
    {
        public T1 Item1 { get; set; }

        public T2 Item2 { get; set; }

        public string Tag { get; set; }

        public SerializableTuple()
        {

        }

        public SerializableTuple(T1 item1, T2 item2)
            : this()
        {
            this.Item1 = item1;
            this.Item2 = item2;
        }

        public SerializableTuple(T1 item1, T2 item2, string tag)
            : this(item1, item2)
        {
            this.Tag = tag;
        }

        public override string ToString()
        {
            return string.Format("{0}, {1}",
                this.Item1 != null ? this.Item1.ToString() : "null",
                this.Item2 != null ? this.Item2.ToString() : "null");
        }
    }
}
