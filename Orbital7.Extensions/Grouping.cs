﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orbital7.Extensions
{
    public class Grouping<TKey, TElement> : IGrouping<TKey, TElement>
    {
        private readonly TKey key;
        private readonly List<TElement> values;

        public Grouping(TKey key, List<TElement> values)
        {
            if (values == null)
                throw new ArgumentNullException("values");
            this.key = key;
            this.values = values;
        }

        public TKey Key
        {
            get { return key; }
        }

        public void Add(TElement element)
        {
            values.Add(element);
        }

        public IEnumerator<TElement> GetEnumerator()
        {
            return values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
