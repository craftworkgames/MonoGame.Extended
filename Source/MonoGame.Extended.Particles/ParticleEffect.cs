using System.Linq;
using Microsoft.Xna.Framework;

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

        public void Update(float elapsedSeconds)
        {
            foreach (var e in Emitters)
                e.Update(elapsedSeconds);
        }

        public void Trigger(Vector2 position)
        {
            foreach (var e in Emitters)
                e.Trigger(position);
        }

        public void Trigger(LineSegment line)
        {
            foreach (var e in Emitters)
                e.Trigger(line);
        }
    }
}