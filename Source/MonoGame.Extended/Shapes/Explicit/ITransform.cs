using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Shapes.Explicit
{
    public interface ITransform
    {
        void GetMatrix(out Matrix transformMatrix);
    }
}
