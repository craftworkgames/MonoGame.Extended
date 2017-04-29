using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Particles.Modifiers;
using MonoGame.Extended.Particles.Profiles;
using MonoGame.Extended.TextureAtlases;

namespace MonoGame.Extended.Particles
{
    public unsafe class ParticleEmitter : Transform2D<ParticleEmitter>, IDisposable
    {
        private readonly FastRandom _random = new FastRandom();
        private readonly float _term;
        internal readonly ParticleBuffer Buffer;
        private bool _autoTrigger;

        private float _totalSeconds;

        public ParticleEmitter(TextureRegion2D textureRegion, int capacity, TimeSpan term, Profile profile,
            bool autoTrigger = true)
        {
            if (profile == null)
                throw new ArgumentNullException(nameof(profile));

            _term = (float) term.TotalSeconds;
            _autoTrigger = autoTrigger;

            TextureRegion = textureRegion;
            Buffer = new ParticleBuffer(capacity);
            Offset = Vector2.Zero;
            Profile = profile;
            Modifiers = new IModifier[0];
            ModifierExecutionStrategy = ParticleModifierExecutionStrategy.Serial;
            Parameters = new ParticleReleaseParameters();
        }

        public int ActiveParticles => Buffer.Count;
        public Vector2 Offset { get; set; }
        public IModifier[] Modifiers { get; set; }
        public ParticleModifierExecutionStrategy ModifierExecutionStrategy { get; set; }

        public Profile Profile { get; }
        public ParticleReleaseParameters Parameters { get; set; }
        public TextureRegion2D TextureRegion { get; set; }

        public void Dispose()
        {
            Buffer.Dispose();
            GC.SuppressFinalize(this);
        }

        private void ReclaimExpiredParticles()
        {
            var iterator = Buffer.Iterator;
            var expired = 0;

            while (iterator.HasNext)
            {
                var particle = iterator.Next();

                if (_totalSeconds - particle->Inception < _term)
                    break;

                expired++;
            }

            if (expired != 0)
                Buffer.Reclaim(expired);
        }

        public bool Update(float elapsedSeconds)
        {
            if (_autoTrigger)
            {
                Trigger();
                _autoTrigger = false;
            }

            _totalSeconds += elapsedSeconds;

            if (Buffer.Count == 0)
                return false;

            ReclaimExpiredParticles();

            var iterator = Buffer.Iterator;

            while (iterator.HasNext)
            {
                var particle = iterator.Next();
                particle->Age = (_totalSeconds - particle->Inception)/_term;
                particle->Position = particle->Position + particle->Velocity*elapsedSeconds;
            }

            ModifierExecutionStrategy.ExecuteModifiers(Modifiers, elapsedSeconds, iterator);
            return true;
        }

        public void Trigger()
        {
            Trigger(Position);
        }

        public void Trigger(Vector2 position, float layerDepth = 0)
        {
            var numToRelease = _random.Next(Parameters.Quantity);
            Release(position + Offset, numToRelease, layerDepth);
        }

        public void Trigger(LineSegment line)
        {
            var numToRelease = _random.Next(Parameters.Quantity);
            var lineVector = line.ToVector();

            for (var i = 0; i < numToRelease; i++)
            {
                var offset = lineVector*_random.NextSingle();
                Release(line.Origin + offset, 1);
            }
        }

        private void Release(Vector2 position, int numToRelease, float layerDepth = 0)
        {
            var iterator = Buffer.Release(numToRelease);

            while (iterator.HasNext)
            {
                var particle = iterator.Next();

                Vector2 heading;
                Profile.GetOffsetAndHeading(out particle->Position, out heading);

                particle->Age = 0f;
                particle->Inception = _totalSeconds;
                particle->Position += position;
                particle->TriggerPos = position;

                var speed = _random.NextSingle(Parameters.Speed);

                particle->Velocity = heading*speed;

                _random.NextColor(out particle->Color, Parameters.Color);

                particle->Opacity = _random.NextSingle(Parameters.Opacity);
                var scale = _random.NextSingle(Parameters.Scale);
                particle->Scale = new Vector2(scale, scale);
                particle->Rotation = _random.NextSingle(Parameters.Rotation);
                particle->Mass = _random.NextSingle(Parameters.Mass);
                particle->LayerDepth = layerDepth;
            }
        }

        ~ParticleEmitter()
        {
            Dispose();
        }
    }
}