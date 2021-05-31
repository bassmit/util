using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

static unsafe class Mem
{
    public static T* Malloc<T>(Allocator allocator) where T : unmanaged
        => Malloc<T>(1, allocator);

    public static T* Malloc<T>(int amount, Allocator allocator) where T : unmanaged
        => (T*) UnsafeUtility.Malloc(amount * UnsafeUtility.SizeOf<T>(), UnsafeUtility.AlignOf<T>(), allocator);
}