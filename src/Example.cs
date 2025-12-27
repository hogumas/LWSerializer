using LWSerializer;

namespace LWBinarySerializer
{
    public struct ExampleStruct
    {
        private int _firstInt;
        private float _firstFloat;
        private bool _bool;
        private decimal _decimal;
    }
    
    public static class Example
    {
        /// <summary> ExampleStruct(unmanaged) 를 직렬화합니다. </summary>
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
}