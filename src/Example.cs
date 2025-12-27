using LWSerializer;

namespace LWBinarySerializer
{
    
    
    public static class Example_Unmanaged
    {
        public struct ExampleStruct
        {
            private int _firstInt;
            private float _firstFloat;
            private bool _bool;
            private decimal _decimal;
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
            private int _firstInt;
            private float _firstFloat;
            private string[] _arr;
            
            void ILwSerializable.OnNativeWrite(LwBinaryWriter writer)
            {
                writer.Write(_firstInt, _firstFloat);
                writer.Write(_arr);
            }

            void ILwSerializable.OnNativeRead(LwBinaryReader reader)
            {
                reader.Read(out _firstInt, out _firstFloat);
                reader.Read(out _arr);
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