using System;

namespace LWBinarySerializer
{
    public readonly unsafe struct NativePointer<T> where T : unmanaged
    {
        public readonly T* Value;

        public NativePointer(void* ptr) => Value = (T*)ptr;
        public NativePointer(IntPtr ptr) => Value = (T*)ptr;

        public bool IsNull => Value == null;

        public ref T this[int index] => ref Value[index];

        public ReadOnlySpan<T> AsSpan(int length) => new ReadOnlySpan<T>(Value, length);

        public static implicit operator void*(NativePointer<T> ptr) => ptr.Value;
        public static implicit operator IntPtr(NativePointer<T> ptr) => (IntPtr)ptr.Value;

        public override string ToString() => $"0x{(IntPtr)Value:X16}";
    }
}