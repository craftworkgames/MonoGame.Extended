using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics.Effects;

namespace MonoGame.Extended.Tiled.Graphics.Effects
{
    public interface ITiledMapEffect : IEffectMatrices, ITextureEffect
    {
        float Alpha { get; set; }
    }
}
