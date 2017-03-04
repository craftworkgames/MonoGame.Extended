using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Effects
{
    public interface ITiledMapEffect : IEffectMatrices, ITextureEffect
    {
        float Alpha { get; set; }
    }
}
