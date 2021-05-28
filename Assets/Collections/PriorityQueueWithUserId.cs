using System;
using System.Diagnostics;
using Unity.Assertions;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace Collections
{
    [DebuggerDisplay("Count = {Count}")]
    [DebuggerTypeProxy(typeof(PriorityQueueWithUserIdDebugView<,>))]
    struct PriorityQueueWithUserId<T, TID> where T : unmanaged, PriorityQueueWithUserId<T, TID>.IElement where TID : struct, IEquatable<TID>
    {
        List<T> _data;
        UnsafeHashMap<TID, int> _index;

        public PriorityQueueWithUserId(int intialCapacity, Allocator allocator = Allocator.Persistent)
        {
            Assert.IsTrue(intialCapacity > 1);
            _data = new List<T>(intialCapacity, allocator);
            _index = new UnsafeHashMap<TID, int>(intialCapacity, allocator);
            Clear();
        }

        public T Top => _data[0];
        public int Count => _data.Length;
        public bool Contains(TID id) => _index.ContainsKey(id);

        public void Set(T n)
        {
            if (!_index.TryGetValue(n.Id, out var i))
            {
                Assert.IsTrue(!_index.ContainsKey(n.Id));
                _data.Resize(_data.Length + 1);
                Percolate(_data.Length - 1, n);
            }
            else if (n.CompareTo(_data[i]) < 0)
                Percolate(i, n);
        }

        public T Extract()
        {
            Assert.IsTrue(Count > 0);
            var top = _data[0];

            if (Count > 1)
                Trickle(0, _data.TakeLast());
            else
                _data.Clear();

            _index.Remove(top.Id);
            return top;
        }

        public void Remove(TID id)
        {
            Assert.IsTrue(_index.ContainsKey(id));
            Trickle(_index[id], _data.TakeLast());
            _index.Remove(id);
        }

        public void Clear()
        {
            _data.Clear();
            _index.Clear();
        }

        public void Dispose()
        {
            _data.Dispose();
            _index.Dispose();
        }

        void Trickle(int i, T n)
        {
            var size = Count;
            var child = Left(i);

            while (child < size)
            {
                if (child + 1 < size && _data[child].CompareTo(_data[child + 1]) > 0)
                    ++child;

                var e = _data[child];
                _data[i] = e;
                _index[e.Id] = i;
                i = child;
                child = Left(i);
            }

            Percolate(i, n);
        }

        void Percolate(int i, T n)
        {
            var parent = Parent(i);

            while (i > 0 && _data[parent].CompareTo(n) > 0)
            {
                var e = _data[parent];
                _data[i] = e;
                _index[e.Id] = i;
                i = parent;
                parent = Parent(i);
            }

            _data[i] = n;
            _index[n.Id] = i;
        }

        static int Parent(int i) => (i - 1) >> 1;
        static int Left(int i) => (i << 1) + 1;

        public interface IElement : IComparable<T>
        {
            TID Id { get; }
        }

        // can not get PriorityQueueDebugView to work as a nested class
        internal T DebugGet(int i) => _data[i];
    }

    sealed class PriorityQueueWithUserIdDebugView<T, TID> where T : unmanaged, PriorityQueueWithUserId<T, TID>.IElement where TID : struct, IEquatable<TID>
    {
        PriorityQueueWithUserId<T, TID> _data;

        public PriorityQueueWithUserIdDebugView(PriorityQueueWithUserId<T, TID> data)
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