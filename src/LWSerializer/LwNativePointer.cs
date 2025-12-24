using System;

namespace LWSerializer
{
    public readonly unsafe struct LwNativePointer<T> where T : unmanaged
    {
        public readonly T* Value;

        public LwNativePointer(void* ptr) => Value = (T*)ptr;
        public LwNativePointer(IntPtr ptr) => Value = (T*)ptr;

        public bool IsNull => Value == null;

        public ref T this[int index] => ref Value[index];

        public ReadOnlySpan<T> AsSpan(int length) => new ReadOnlySpan<T>(Value, length);

        public static implicit operator void*(LwNativePointer<T> ptr) => ptr.Value;
        public static implicit operator IntPtr(LwNativePointer<T> ptr) => (IntPtr)ptr.Value;

        public override string ToString() => $"0x{(IntPtr)Value:X16}";
    }
}