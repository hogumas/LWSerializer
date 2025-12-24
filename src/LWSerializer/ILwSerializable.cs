namespace LWSerializer
{
    public interface ILwSerializable
    {
        protected internal void OnNativeWrite(LwBinaryWriter writer);
        protected internal void OnNativeRead(LwBinaryReader reader);
    }
}