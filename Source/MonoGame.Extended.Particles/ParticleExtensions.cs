using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.TextureAtlases;

namespace MonoGame.Extended.Particles
{
    public static class ParticleExtensions
    {
        public static void Draw(this SpriteBatch spriteBatch, ParticleEffect effect)
        {
            for (var i = 0; i < effect.Emitters.Count; i++)
                UnsafeDraw(spriteBatch, effect.Emitters[i]);
        }

        public static void Draw(this SpriteBatch spriteBatch, ParticleEmitter emitter)
        {
            UnsafeDraw(spriteBatch, emitter);
        }

        private static unsafe void UnsafeDraw(SpriteBatch spriteBatch, ParticleEmitter emitter)
        {
            if(emitter.TextureRegion == null)
                return;

            var textureRegion = emitter.TextureRegion;
            var origin = new Vector2(textureRegion.Width/2f, textureRegion.Height/2f);
            var iterator = emitter.Buffer.Iterator;

            while (iterator.HasNext)
            {
                var particle = iterator.Next();
                var color = particle->Color.ToRgb();

                if (spriteBatch.GraphicsDevice.BlendState == BlendState.AlphaBlend)
                    color *= particle->Opacity;
                else
                    color.A = (byte) (particle->Opacity*255);

                var position = new Vector2(particle->Position.X, particle->Position.Y);
                var scale = particle->Scale;
                var particleColor = new Color(color, particle->Opacity);
                var rotation = particle->Rotation;
                var layerDepth = particle->LayerDepth;

                spriteBatch.Draw(textureRegion, position, particleColor, rotation, origin, scale, SpriteEffects.None, layerDepth);
            }
        }
    }
}