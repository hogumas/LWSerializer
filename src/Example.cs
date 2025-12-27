using LWSerializer;

namespace LWBinarySerializer
{
    public static class Example_Unmanaged
    {
        public struct ExampleStruct
        {
            public int m_int;
            public float m_float;
            public bool m_bool;
            public decimal m_decimal;
        }
        
        public static byte[] Write(ExampleStruct exampleStruct)
        {
            using (var writer = new LwBinaryWriter())
            {
                writer.Write(exampleStruct);
                return writer.ToArray(); //or writer.ToPtr()
            }
        }

        public static ExampleStruct Read(byte[] bytes)
        {
            ExampleStruct result;
            using (var reader = new LwBinaryReader(bytes))
            {
                reader.Read(out result);
            }
            return result;
        }
    }
    
    public static class Example_Managed
    {
        public class ExampleClass : ILwSerializable
        {
            public int m_int;
            public float m_float;
            public string[] m_arr;
            
            void ILwSerializable.OnNativeWrite(LwBinaryWriter writer)
            {
                writer.Write(m_int, m_float);
                writer.Write(m_arr);
            }

            void ILwSerializable.OnNativeRead(LwBinaryReader reader)
            {
                reader.Read(out m_int, out m_float);
                reader.Read(out m_arr);
            }
        }
        
        public static byte[] Write(ExampleClass exampleStruct)
        {
            using (var writer = new LwBinaryWriter())
            {
                writer.WriteRef(exampleStruct);
                return writer.ToArray(); //or writer.ToPtr()
            }
        }

        public static ExampleClass Read(byte[] bytes)
        {
            ExampleClass result = new ExampleClass();
            using (var reader = new LwBinaryReader(bytes))
            {
                reader.ReadRef(result);
            }
            return result;
        }
    }
}