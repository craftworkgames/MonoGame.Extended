using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Particles.Data;

namespace MonoGame.Extended.Particles;

public static class SpriteBatchExtensions
{
    public static void Draw(this SpriteBatch spriteBatch, ParticleEffect effect)
    {
        ArgumentNullException.ThrowIfNull(effect);
        ObjectDisposedException.ThrowIf(effect.IsDisposed, effect);

        for (int i = 0; i < effect.Emitters.Count; i++)
        {
            UnsafeDraw(spriteBatch, effect.Emitters[i]);
        }
    }

    public static void Draw(this SpriteBatch spriteBatch, ParticleEmitter emitter)
    {
        ArgumentNullException.ThrowIfNull(emitter);
        ObjectDisposedException.ThrowIf(emitter.IsDisposed, emitter);
        UnsafeDraw(spriteBatch, emitter);
    }

    private static unsafe void UnsafeDraw(SpriteBatch spriteBatch, ParticleEmitter emitter)
    {
        ArgumentNullException.ThrowIfNull(spriteBatch);

        // Early exit if no texture region assigned
        if (emitter.TextureRegion == null)
        {
            return;
        }

        // Early exit if there are no active particles
        if (emitter.ActiveParticles == 0)
        {
            return;
        }

        Texture2DRegion region = emitter.TextureRegion;
        Texture2D texture = region.Texture;
        Rectangle sourceRect = region.Bounds;
        Vector2 origin = new Vector2(region.Width, region.Height) * 0.5f;

        if (emitter.RenderingOrder == ParticleRenderingOrder.FrontToBack)
        {
            int count = emitter.ActiveParticles;

            Span<IntPtr> particlePtrs = count <= 1024 ?
                                       stackalloc IntPtr[count] :
                                       new IntPtr[count];

            ParticleIterator iterator = emitter.Buffer.Iterator;
            int index = 0;

            while (iterator.HasNext)
            {
                particlePtrs[index++] = (IntPtr)iterator.Next();
            }

            for (int i = count - 1; i >= 0; i--)
            {
                RenderParticle(spriteBatch, (Particle*)particlePtrs[i], texture, sourceRect, origin);
            }
        }
        else
        {
            ParticleIterator iterator = emitter.Buffer.Iterator;

            while (iterator.HasNext)
            {
                Particle* particle = iterator.Next();
                RenderParticle(spriteBatch, particle, texture, sourceRect, origin);
            }
        }
    }

    private static unsafe void RenderParticle(SpriteBatch spriteBatch, Particle* particle, Texture2D texture, Rectangle sourceRect, Vector2 origin)
    {
        HslColor hsl = new HslColor(particle->Color[0], particle->Color[1], particle->Color[2]);
        Color color = HslColor.ToRgb(hsl);

        if (spriteBatch.GraphicsDevice.BlendState == BlendState.AlphaBlend)
        {
            color *= particle->Opacity;
        }
        else
        {
            color.A = (byte)MathHelper.Clamp(particle->Opacity * 255, 0, 255);
        }

        Vector2 position = new Vector2(particle->Position[0], particle->Position[1]);
        float scale = particle->Scale;
        float rotation = particle->Rotation;
        float layerDepth = particle->LayerDepth;

        spriteBatch.Draw(
            texture,
            position,
            sourceRect,
            color,
            rotation,
            origin,
            scale,
            SpriteEffects.None,
            layerDepth
        );
    }
}
