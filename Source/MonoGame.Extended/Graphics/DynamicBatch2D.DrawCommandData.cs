using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics.Batching;
using MonoGame.Extended.Graphics.Effects;

namespace MonoGame.Extended.Graphics
{
    public partial class DynamicBatch2D
    {
        /// <summary>
        ///     Defines a drawing context for two-dimensional geometric objects.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public struct DrawCommandData : IBatchDrawCommandData<DrawCommandData>
        {
            public Texture2D Texture;
            public int TextureKey;

            internal DrawCommandData(Texture2D texture)
            {
                Texture = texture;
                TextureKey = RuntimeHelpers.GetHashCode(texture);
            }

            public void ApplyTo(Effect effect)
            {
                var textureEffect = effect as ITexture2DEffect;
                if (textureEffect != null)
                    textureEffect.Texture = Texture;
            }

            public void SetReferencesToNull()
            {
                Texture = null;
            }

            public bool Equals(ref DrawCommandData other)
            {
                return Texture == other.Texture;
            }

            public int CompareTo(DrawCommandData other)
            {
                return TextureKey.CompareTo(other.TextureKey);
            }
        }
    }
}
