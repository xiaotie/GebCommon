using System;
using System.Collections.Generic;
using System.Text;

namespace Geb.Utils.Collections
{
    public class Ring<T>
    {
        private T[] _cache;

        public readonly int Size;

        public int HeadIdx { get; private set; }
        public int TailIdx { get; private set; }
        public int Count { get; private set; }

        public Ring(int size)
        {
            _cache = new T[size];
            Size = size;
            HeadIdx = 0;
            TailIdx = 0;
            Count = 0;
        }

        public void Add(T item)
        {
            _cache[TailIdx] = item;

            TailIdx++;
            if (TailIdx == Size) TailIdx = 0;

            Count++;
            if (Count > Size) Count = 1;
        }

        public void AddToHead(T item)
        {
            HeadIdx--;
            if (HeadIdx < 0) HeadIdx = Size - 1;
            _cache[HeadIdx] = item;
            Count++;
            if (Count > Size) Count = 1;
        }
    }
}
