using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Particles.Serialization;
using MonoGame.Extended.Serialization;
using Newtonsoft.Json;

namespace MonoGame.Extended.Particles
{
    public class ParticleEffect : Transform2D<ParticleEffect>
    {
        public ParticleEffect(string name = null, bool autoTrigger = true, float autoTriggerDelay = 0f)
        {
            Name = name;
            AutoTrigger = autoTrigger;
            AutoTriggerDelay = autoTriggerDelay;
            Emitters = new ParticleEmitter[0];
        }

        private float _nextAutoTrigger;

        public string Name { get; set; }
        public bool AutoTrigger { get; set; }
        public float AutoTriggerDelay { get; set; }
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
            var serializer = new ParticleJsonSerializer(textureRegionService);

            using (var streamReader = new StreamReader(stream))
            using (var jsonReader = new JsonTextReader(streamReader))
            {
                return serializer.Deserialize<ParticleEffect>(jsonReader);
            }
        }

        public void Update(float elapsedSeconds)
        {
            if (AutoTrigger)
            {
                _nextAutoTrigger -= elapsedSeconds;

                if (_nextAutoTrigger <= 0)
                {
                    Trigger();
                    _nextAutoTrigger = AutoTriggerDelay;
                }
            }

            for (var i = 0; i < Emitters.Length; i++)
                Emitters[i].Update(elapsedSeconds);
        }

        public void Trigger()
        {
            Trigger(Position);
        }

        public void Trigger(Vector2 position, float layerDepth = 0)
        {
            for (var i = 0; i < Emitters.Length; i++)
                Emitters[i].Trigger(position, layerDepth);
        }

        public void Trigger(LineSegment line)
        {
            for (var i = 0; i < Emitters.Length; i++)
                Emitters[i].Trigger(line);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}