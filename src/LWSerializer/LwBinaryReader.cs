using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace LWSerializer
{
    public unsafe class LwBinaryReader : IDisposable
    {
        private IntPtr _array;
        private long _position;
        private GCHandle _handle;

        #region constructor
        public LwBinaryReader(LwNativePointer<byte> span)
        {
            _array = span;
            _position = 0;
        }
        
        public LwBinaryReader(IntPtr binaryData)
        {
            _array = binaryData;
            _position = 0;
        }

        public LwBinaryReader(byte[] binaryData)
        {
            _handle = GCHandle.Alloc(binaryData, GCHandleType.Pinned);
            _array = _handle.AddrOfPinnedObject();
        }
        
        ~LwBinaryReader()
        {
            _dispose();
        }
        #endregion

        public void SetPosition(long position)
        {
            _position = position;
        }
        #region BeginRead

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void* BeginRead(int byteLength) => BeginRead((uint)byteLength);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void* BeginRead(uint byteLength)
        {
            // 1. 쓰기 때와 똑같은 규칙으로 정렬 단위 결정
            uint alignment = byteLength >= 8 ? 8 : byteLength;
            if ((alignment & (alignment - 1)) != 0) alignment = 1;
            long currentPos = _position;
            // 2. 패딩을 포함한 시작 위치 계산 (점프!)
            long alignedPos = (currentPos + (alignment - 1)) & ~(alignment - 1);
            /*// 3. 유효성 검사 (데이터가 충분한지)
            if (alignedPos + byteLength > length)
                throw new IndexOutOfRangeException();*/
            _position = alignedPos + byteLength;
            return (byte*)_array + alignedPos;
        }

        #endregion

        public void Read<T1>(out T1 _1) where T1 : unmanaged
        {
            _1 = *(T1*)BeginRead(Unsafe.SizeOf<T1>());
        }

        public void ReadPadding(int byteLen)
        {
            BeginRead(byteLen);
        }

        /// <summary> 반환 Span값은 참조하지말고 복사해서 사용하세요 </summary>
        public void Read<T>(out ReadOnlySpan<T> result) where T : unmanaged
        {
            Read(out int length);
            int byteLen = length * Unsafe.SizeOf<T>();
            void* dataPtr = BeginRead(byteLen);
            result = new ReadOnlySpan<T>(dataPtr, byteLen);
        }

        public void Read<T>(out T[] result) where T : unmanaged
        {
            Read(out int length);
            int byteLen = length * Unsafe.SizeOf<T>();
            result = new T[length];
            if (length == 0)
                return;
            void* source = BeginRead(byteLen);
            fixed (void* destPtr = &result[0])
            {
                Unsafe.CopyBlock(destPtr, source, (uint)byteLen);
            }
        }

        public void Read(out string str)
        {
            Read(out int byteCount);
            if (byteCount == 0)
            {
                str = "";
                return;
            }
            byte* srcPtr = (byte*)BeginRead(byteCount);
            str = Encoding.UTF8.GetString(srcPtr, byteCount);
        }

        public void ReadRef(ILwSerializable binaryable)
        {
            binaryable.OnNativeRead(this);   
        }

        public void Dispose()
        {
            _dispose();
        }

        private void _dispose()
        {
            if (_handle.IsAllocated)
                _handle.Free();
        }

        #region T1....T7
        public unsafe void Read<T1, T2>(out T1 _1, out T2 _2)
            where T1 : unmanaged
            where T2 : unmanaged
        {
            _1 = *(T1*)BeginRead(Unsafe.SizeOf<T1>());
            _2 = *(T2*)BeginRead(Unsafe.SizeOf<T2>());
        }

        public unsafe void Read<T1, T2, T3>(out T1 _1, out T2 _2, out T3 _3)
            where T1 : unmanaged
            where T2 : unmanaged
            where T3 : unmanaged
        {
            _1 = *(T1*)BeginRead(Unsafe.SizeOf<T1>());
            _2 = *(T2*)BeginRead(Unsafe.SizeOf<T2>());
            _3 = *(T3*)BeginRead(Unsafe.SizeOf<T3>());
        }

        public unsafe void Read<T1, T2, T3, T4>(out T1 _1, out T2 _2, out T3 _3, out T4 _4)
            where T1 : unmanaged
            where T2 : unmanaged
            where T3 : unmanaged
            where T4 : unmanaged
        {
            _1 = *(T1*)BeginRead(Unsafe.SizeOf<T1>());
            _2 = *(T2*)BeginRead(Unsafe.SizeOf<T2>());
            _3 = *(T3*)BeginRead(Unsafe.SizeOf<T3>());
            _4 = *(T4*)BeginRead(Unsafe.SizeOf<T4>());
        }

        public unsafe void Read<T1, T2, T3, T4, T5>(out T1 _1, out T2 _2, out T3 _3, out T4 _4, out T5 _5)
            where T1 : unmanaged
            where T2 : unmanaged
            where T3 : unmanaged
            where T4 : unmanaged
            where T5 : unmanaged
        {
            _1 = *(T1*)BeginRead(Unsafe.SizeOf<T1>());
            _2 = *(T2*)BeginRead(Unsafe.SizeOf<T2>());
            _3 = *(T3*)BeginRead(Unsafe.SizeOf<T3>());
            _4 = *(T4*)BeginRead(Unsafe.SizeOf<T4>());
            _5 = *(T5*)BeginRead(Unsafe.SizeOf<T5>());
        }

        public unsafe void Read<T1, T2, T3, T4, T5, T6>(out T1 _1, out T2 _2, out T3 _3, out T4 _4, out T5 _5,
            out T6 _6)
            where T1 : unmanaged
            where T2 : unmanaged
            where T3 : unmanaged
            where T4 : unmanaged
            where T5 : unmanaged
            where T6 : unmanaged
        {
            _1 = *(T1*)BeginRead(Unsafe.SizeOf<T1>());
            _2 = *(T2*)BeginRead(Unsafe.SizeOf<T2>());
            _3 = *(T3*)BeginRead(Unsafe.SizeOf<T3>());
            _4 = *(T4*)BeginRead(Unsafe.SizeOf<T4>());
            _5 = *(T5*)BeginRead(Unsafe.SizeOf<T5>());
            _6 = *(T6*)BeginRead(Unsafe.SizeOf<T6>());
        }

        public unsafe void Read<T1, T2, T3, T4, T5, T6, T7>(out T1 _1, out T2 _2, out T3 _3, out T4 _4, out T5 _5,
            out T6 _6, out T7 _7)
            where T1 : unmanaged
            where T2 : unmanaged
            where T3 : unmanaged
            where T4 : unmanaged
            where T5 : unmanaged
            where T6 : unmanaged
            where T7 : unmanaged
        {
            _1 = *(T1*)BeginRead(Unsafe.SizeOf<T1>());
            _2 = *(T2*)BeginRead(Unsafe.SizeOf<T2>());
            _3 = *(T3*)BeginRead(Unsafe.SizeOf<T3>());
            _4 = *(T4*)BeginRead(Unsafe.SizeOf<T4>());
            _5 = *(T5*)BeginRead(Unsafe.SizeOf<T5>());
            _6 = *(T6*)BeginRead(Unsafe.SizeOf<T6>());
            _7 = *(T7*)BeginRead(Unsafe.SizeOf<T7>());
        }
        #endregion
    }
}