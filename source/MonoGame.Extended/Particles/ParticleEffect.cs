using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Particles.Serialization;
using MonoGame.Extended.Serialization.Json;

namespace MonoGame.Extended.Particles
{
    public class ParticleEffect : Transform2, IDisposable
    {
        public ParticleEffect(string name = null)
        {
            Name = name;
            Emitters = new List<ParticleEmitter>();
        }

        ~ParticleEffect() => Dispose(false);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if(IsDisposed)
            {
                return;
            }

            if(disposing)
            {
                foreach(var emitter in Emitters)
                {
                    emitter.Dispose();
                }
            }

            IsDisposed = true;
        }

        public string Name { get; set; }
        public List<ParticleEmitter> Emitters { get; set; }
        public int ActiveParticles => Emitters.Sum(t => t.ActiveParticles);

        /// <summary>
        /// Gets a value that indicates whether this instance of the <see cref="ParticleEffect"/> class has been
        /// disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        public void FastForward(Vector2 position, float seconds, float triggerPeriod)
        {
            var time = 0f;
            while (time < seconds)
            {
                Update(triggerPeriod);
                Trigger(position);
                time += triggerPeriod;
            }
        }

        public static ParticleEffect FromFile(ITextureRegionService textureRegionService, string path)
        {
            using (var stream = TitleContainer.OpenStream(path))
            {
                return FromStream(textureRegionService, stream);
            }
        }

        public static ParticleEffect FromStream(ITextureRegionService textureRegionService, Stream stream)
        {
            var options =  ParticleJsonSerializerOptionsProvider.GetOptions(textureRegionService);
            return JsonSerializer.Deserialize<ParticleEffect>(stream, options);
        }

        public void Update(float elapsedSeconds)
        {
            ThrowIfDisposed();

            for (var i = 0; i < Emitters.Count; i++)
                Emitters[i].Update(elapsedSeconds, Position);
        }

        public void Trigger()
        {
            Trigger(Position);
        }

        public void Trigger(Vector2 position, float layerDepth = 0)
        {
            ThrowIfDisposed();
            for (var i = 0; i < Emitters.Count; i++)
                Emitters[i].Trigger(position, layerDepth);
        }

        public void Trigger(LineSegment line, float layerDepth = 0)
        {
            ThrowIfDisposed();
            for (var i = 0; i < Emitters.Count; i++)
                Emitters[i].Trigger(line, layerDepth);
        }

        private void ThrowIfDisposed()
        {
            if(IsDisposed)
            {
                throw new ObjectDisposedException(nameof(ParticleBuffer));
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
