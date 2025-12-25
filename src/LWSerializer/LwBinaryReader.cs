using System;
using System.Collections.Generic;
using System.IO.Hashing;
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
        
        /// <summary>
        /// Writer에 추가적인 쓰기작업이 발생할경우 반환한 주소값이 유효하지않게 될 수 있습니다.
        /// </summary>
        /// <returns></returns>
        public LwNativePointer<byte> ToPtr()
        {
            return new LwNativePointer<byte>(_array);
        }
        
        #region BeginRead
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void* BeginRead(int byteLength) => BeginRead((uint)byteLength);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void* BeginRead(uint byteLength)
        {
            uint alignment = byteLength >= 8 ? 8 : byteLength;
            if ((alignment & (alignment - 1)) != 0) alignment = 1;
            long currentPos = _position;
            long alignedPos = (currentPos + (alignment - 1)) & ~(alignment - 1);
            _position = alignedPos + byteLength;
            return (byte*)_array + alignedPos;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void* Peek(int byteLengthint)
        {
            uint byteLength = (uint)byteLengthint;
            uint alignment = byteLength >= 8 ? 8 : byteLength;
            if ((alignment & (alignment - 1)) != 0) alignment = 1;
            long currentPos = _position;
            long alignedPos = (currentPos + (alignment - 1)) & ~(alignment - 1);
            return (byte*)_array + alignedPos;
        }
        #endregion

        public void Read<T1>(out T1 _1) where T1 : unmanaged
        {
            _1 = *(T1*)BeginRead(Unsafe.SizeOf<T1>());
        }

        public T Read<T>() where T : unmanaged
        {
            return *(T*)BeginRead(Unsafe.SizeOf<T>());
        }

        public void ReadPadding(int byteLen)
        {
            BeginRead(byteLen);
        }

        public int PeekSpanLength<T>() where T : unmanaged
        {
            return Peek<int>();
        }
        
        public void ReadSpan<T>(void* dest) where T : unmanaged
        {
            if (dest == null)
                throw new ArgumentNullException();
            Read(out int length);
            int byteLen = length * Unsafe.SizeOf<T>();
            void* source = BeginRead(byteLen);
            Unsafe.CopyBlock(dest, source, (uint)byteLen);
        }
        
        public void ReadSpan<T>(Span<T> dest) where T : unmanaged
        {
            if (dest == null) 
                throw new ArgumentNullException();
            ReadSpan<T>(Unsafe.AsPointer(ref dest.GetPinnableReference()));
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

        public void ReadRef<T>(out T[] result) where T : ILwSerializable
        {
            Read(out int length);
            result = new  T[length];
            for (int i = 0; i < length; i++)
            {
                var instance = Activator.CreateInstance<T>();
                ReadRef(instance);
                result[i] = instance;
            }
        }
        
        public void ReadRef(ILwSerializable binaryable)
        {
            binaryable.OnNativeRead(this);   
        }
        
        #region Dic
        public void Read<K, V>(out Dictionary<K, V> dic)  
            where K : unmanaged
            where V : unmanaged
        {
            dic = new Dictionary<K, V>();
            Read(out int count);
            for (int i = 0; i < count; i++)
            {
                Read(out K key, out V value);
                dic.Add(key, value);
            }
        }
        
        public void Read<V>(out Dictionary<string, V> dic) where V : unmanaged
        {
            dic = new Dictionary<string, V>();
            Read(out int count);
            for (int i = 0; i < count; i++)
            {
                Read(out string key);
                Read(out V value);
                dic.Add(key, value);
            }
        }
        
        public void ReadRef<K, V>(out Dictionary<K, V> dic)  
            where K : unmanaged
            where V : ILwSerializable
        {
            dic = new Dictionary<K, V>();
            Read(out int count);
            for (int i = 0; i < count; i++)
            {
                Read(out K key);
                var nvalue = Activator.CreateInstance<V>();
                ReadRef(nvalue);
                dic.Add(key, nvalue);
            }
        }
        
        public void ReadRef<V>(out Dictionary<string, V> dic) where V : ILwSerializable
        {
            dic = new Dictionary<string, V>();
            Read(out int count);
            for (int i = 0; i < count; i++)
            {
                Read(out string key);
                var nvalue = Activator.CreateInstance<V>();
                ReadRef(nvalue);
                dic.Add(key, nvalue);
            }
        }
        #endregion

        public T Peek<T>() where T : unmanaged
        {
            return *(T*)Peek(Unsafe.SizeOf<T>());
        }
        
        public ulong GetXxHash64(int seed)
        {
            return XxHash64.HashToUInt64(this.ToPtr().AsSpan((int)_position), seed);
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