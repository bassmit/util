using Unity.Assertions;
using Unity.Collections.LowLevel.Unsafe;

static class Extensions
{
    public static unsafe T Read<T>(this UnsafeList<T> l, int index) where T : unmanaged
    {
        Assert.IsTrue(index >= 0 && index < l.Length);
        return UnsafeUtility.ReadArrayElement<T>(l.Ptr, index);
    }

    public static unsafe void Write<T>(this UnsafeList<T> l, int index, T value) where T : unmanaged
    {
        Assert.IsTrue(index >= 0 && index < l.Length);
        UnsafeUtility.WriteArrayElement(l.Ptr, index, value);
    }
}