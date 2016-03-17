using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Particles
{
    public class SpriteBatchRenderer
    {
        /// <summary>
        /// Draw a particle effect. This draw function calls spritebatch.Begin() and .End()
        /// </summary>
        public void Draw(SpriteBatch spriteBatch, ParticleEffect effect)
        {
            foreach (var emitter in effect.Emitters)
                Draw(spriteBatch, emitter);
        }

        private unsafe void Draw(SpriteBatch spriteBatch, Emitter emitter)
        {
            var texture = emitter.Texture;
            var origin = new Vector2(texture.Width / 2f, texture.Height / 2f);

            var blendState = emitter.BlendMode == BlendMode.Add
                ? BlendState.Additive
                : BlendState.AlphaBlend;

            //TODO var sortMode = emitter.RenderingOrder == RenderingOrder.BackToFront ?

            //spriteBatch.Begin(SpriteSortMode.Deferred, blendState);

            var iterator = emitter.Buffer.Iterator;
            
            while (iterator.HasNext)
            {
                var particle = iterator.Next();

                var color = particle->Colour.ToRgb();

                if (blendState == BlendState.AlphaBlend)
                    color *= particle->Opacity;
                else
                    color.A = (byte) (particle->Opacity * 255);

                spriteBatch.Draw(texture, 
                    new Vector2(particle->Position.X, particle->Position.Y), 
                    null, null, origin, 
                    particle->Rotation,
                    new Vector2(particle->Scale.X / texture.Width, particle->Scale.Y / texture.Height),
                    new Color(color, particle->Opacity));
                
            }

            //spriteBatch.End();
        }
    }
}