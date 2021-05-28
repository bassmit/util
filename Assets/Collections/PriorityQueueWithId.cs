using System;
using System.Diagnostics;
using Unity.Assertions;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace Collections
{
    [DebuggerDisplay("Count = {Count}")]
    [DebuggerTypeProxy(typeof(PriorityQueueWithIdDebugView<>))]
    unsafe struct PriorityQueueWithId<T> where T : struct, IComparable<T>
    {
        readonly Allocator _allocator;
        List<Wrapper> _data;
        List<Lookup> _lookup;
        readonly int* _free;

        public PriorityQueueWithId(int intialCapacity, Allocator allocator = Allocator.Persistent)
        {
            _allocator = allocator;
            Assert.IsTrue(intialCapacity > 1);
            _data = new List<Wrapper>(intialCapacity, allocator);
            _lookup = new List<Lookup>(intialCapacity, allocator);
            _free = (int*) Mem.Malloc<int>(allocator);
            *_free = -1;
        }

        public T Top => _data[0].Item;
        public int Count => _data.Length;

        public void LowerKey(T n, int id)
        {
            var i = _lookup[id].DataIndex;
            var w = new Wrapper(n, id);
            Assert.IsTrue(w.CompareTo(_data[i]) < 0);
            Percolate(i, w);
        }

        public int Insert(T n)
        {
            if (*_free == -1)
            {
                _lookup.Add(new Lookup{ Next = -1});
                *_free = _lookup.Length - 1;
            }

            var free = *_free;
            _lookup[free].DataIndex = _data.Length;
            *_free = _lookup[free].Next;
            var wrapped = new Wrapper(n, free);

            _data.Resize(_data.Length + 1);
            Percolate(_data.Length - 1, wrapped);
            return free;
        }

        public T Extract()
        {
            Assert.IsTrue(Count > 0);
            var top = _data[0];

            if (Count > 1)
                Trickle(0, _data.TakeLast());
            else
                _data.Clear();

            _lookup[top.LookupIndex].Next = *_free;
            *_free = top.LookupIndex;

            return top.Item;
        }

        public void Remove(int id)
        {
            _lookup[id].Next = *_free;
            *_free = id;

            if (Count > 1)
                Trickle(_lookup[id].DataIndex, _data.TakeLast());
            else
                _data.Clear();
        }

        public void Clear()
        {
            _data.Clear();
            _lookup.Clear();
            *_free = -1;
        }

        public void Dispose()
        {
            _data.Dispose();
            _lookup.Dispose();
            UnsafeUtility.Free(_free, _allocator);
        }

        void Trickle(int i, Wrapper n)
        {
            if (i == Count)
                return;

            var size = Count;
            var child = Left(i);

            while (child < size)
            {
                if (child + 1 < size && _data[child].CompareTo(_data[child + 1]) > 0)
                    ++child;

                var e = _data[child];
                _data[i] = e;
                _lookup[e.LookupIndex].DataIndex = i;

                i = child;
                child = Left(i);
            }

            Percolate(i, n);
        }

        void Percolate(int i, Wrapper n)
        {
            var parent = Parent(i);

            while (i > 0 && _data[parent].CompareTo(n) > 0)
            {
                var e = _data[parent];
                _data[i] = e;

                _lookup[e.LookupIndex].DataIndex = i;

                i = parent;
                parent = Parent(i);
            }

            _data[i] = n;
            _lookup[n.LookupIndex].DataIndex = i;
        }

        static int Parent(int i) => (i - 1) >> 1;
        static int Left(int i) => (i << 1) + 1;

        // can not get PriorityQueueDebugView to work as a nested class
        internal T DebugGet(int i) => _data[i].Item;

        struct Lookup
        {
            public int DataIndex;
            public int Next;

            public override string ToString()
            {
                return $"Next: {Next}, DataIndex: {DataIndex}";
            }
        }

        struct Wrapper : IComparable<Wrapper>
        {
            public T Item;
            public readonly int LookupIndex;

            public Wrapper(T item, int lookupIndex)
            {
                Item = item;
                LookupIndex = lookupIndex;
            }

            public int CompareTo(Wrapper other) => Item.CompareTo(other.Item);

            public override string ToString()
            {
                return $"LookupIndex: {LookupIndex}, Item: {Item}";
            }
        }
    }

    sealed class PriorityQueueWithIdDebugView<T> where T : struct, IComparable<T>
    {
        PriorityQueueWithId<T> _data;

        public PriorityQueueWithIdDebugView(PriorityQueueWithId<T> data)
        {
            _data = data;
        }

        public System.Collections.Generic.List<T> Items
        {
            get
            {
                var l = new System.Collections.Generic.List<T>();
                for (int i = 0; i < _data.Count; i++)
                    l.Add(_data.DebugGet(i));
                l.Sort();
                return l;
            }
        }
    }
}