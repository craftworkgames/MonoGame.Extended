using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Graphics.Effects
{
    public interface IEffectWorldViewProjectionMatrix
    {
        Matrix World { get; set; }
        Matrix View { get; set; }
        Matrix Projection { get; set; }

        void SetWorld(ref Matrix world);
        void SetView(ref Matrix view);
        void SetProjection(ref Matrix projection);
    }
}
