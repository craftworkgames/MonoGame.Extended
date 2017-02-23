using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Tiled.Graphics
{
    public interface ITiledMapEffect : IEffectMatrices
    {
        EffectTechnique CurrentTechnique { get; }
        Texture2D Texture { get; set; }
        float Alpha { get; set; }
    }
}
