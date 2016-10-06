namespace MonoGame.Extended.Graphics
{
    public partial class GeometryBuffer
    {
        public struct GeometryItem
        {
            public readonly int StartIndex;
            public readonly int PrimitivesCount;

            public GeometryItem(int startIndex, int primitivesCount)
            {
                StartIndex = startIndex;
                PrimitivesCount = primitivesCount;
            }
        }
    }
}
