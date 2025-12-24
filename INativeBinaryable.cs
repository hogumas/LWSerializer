namespace LWBinarySerializer
{
    public interface INativeBinaryable
    {
        protected internal void OnNativeWrite(NativeBinaryWriter writer);
        protected internal void OnNativeRead(NativeBinaryReader reader);
    }
}