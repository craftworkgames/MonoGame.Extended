using System.Linq;

namespace MonoGame.Extended.Particles
{
    public class ParticleEffect
    {
        public string Name { get; set; }
        public Emitter[] Emitters { get; set; }

        public ParticleEffect()
        {
            Emitters = new Emitter[0];
        }

        public int ActiveParticles => Emitters.Sum(t => t.ActiveParticles);

        public void FastForward(Vector position, float seconds, float triggerPeriod)
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

        public void Trigger(Vector position)
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