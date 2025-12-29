using System;
using System.IO.Hashing;

namespace LWSerializer
{
    public static class LwUtility
    {
        private static LwBinaryWriter _sharedWriter = new LwBinaryWriter();
        
        /// <summary>  객체를 바이너리 배열로 반환합니다. </summary>
        public static byte[] To<T>(T serializable)
        {
            _sharedWriter.SetLength(0);
            _sharedWriter.Write(serializable);
            var result = _sharedWriter.ToArray();
            return result;
        }
        
        /// <summary> 바이너리 배열을  객체로 반환합니다</summary>
        public static T From<T>(byte[] bytes)
        {
            T result = default;
            using (var reader = new LwBinaryReader(bytes))
            {
                reader.Read(out result);
            }
            return result;
        }
        
        /// <summary> 배열을 XxHash64로 변환합니다 </summary>
        public static ulong ToXxHash64(byte[] bytes, long seed = 5)
        {
            ulong result;
            using (var reader = new LwBinaryReader(bytes))
            {
                result = reader.GetXxHash64(seed);
            }
            return result;
        }
        
        /// <summary> ILwSerializable을 XxHash64로 변환합니다 </summary>
        public static ulong ToXxHash64(ILwSerializable serializable, long seed = 5)
        {
            ulong result;
            using (var writer = new LwBinaryWriter())
            {
                writer.Write(serializable);
                result = writer.GetXxHash64(seed);
            }
            return result;
        }

        public static ulong ToXxHash64(LwNativePointer<byte> ptr, int length, long seed = 5)
        {
            return XxHash64.HashToUInt64(ptr.AsSpan(length), seed);
        }
    }
}