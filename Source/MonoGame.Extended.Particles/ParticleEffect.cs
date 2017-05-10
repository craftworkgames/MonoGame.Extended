using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Particles.Serialization;
using MonoGame.Extended.Serialization;
using Newtonsoft.Json;

namespace MonoGame.Extended.Particles
{
    public class ParticleEffect
    {
        public ParticleEffect()
        {
            Emitters = new ParticleEmitter[0];
        }

        public string Name { get; set; }
        public ParticleEmitter[] Emitters { get; set; }

        public int ActiveParticles => Emitters.Sum(t => t.ActiveParticles);

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
            var skinSerializer = new ParticleJsonSerializer(textureRegionService);

            using (var streamReader = new StreamReader(stream))
            using (var jsonReader = new JsonTextReader(streamReader))
            {
                return skinSerializer.Deserialize<ParticleEffect>(jsonReader);
            }
        }

        public void Update(float elapsedSeconds)
        {
            foreach (var e in Emitters)
                e.Update(elapsedSeconds);
        }

        public void Trigger(Vector2 position, float layerDepth = 0)
        {
            foreach (var e in Emitters)
                e.Trigger(position, layerDepth);
        }

        public void Trigger(LineSegment line)
        {
            foreach (var e in Emitters)
                e.Trigger(line);
        }
    }
}