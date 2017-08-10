using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Particles.Modifiers;
using MonoGame.Extended.Particles.Profiles;
using MonoGame.Extended.TextureAtlases;
using Newtonsoft.Json;

namespace MonoGame.Extended.Particles
{
    public unsafe class ParticleEmitter : IDisposable
    {
        private readonly FastRandom _random = new FastRandom();
        private float _totalSeconds;

        [JsonConstructor]
        public ParticleEmitter(string name, TextureRegion2D textureRegion, int capacity, TimeSpan lifeSpan, Profile profile)
        {
            if (profile == null)
                throw new ArgumentNullException(nameof(profile));

            _lifeSpanSeconds = (float)lifeSpan.TotalSeconds;

            Name = name;
            TextureRegion = textureRegion;
            Buffer = new ParticleBuffer(capacity);
            Offset = Vector2.Zero;
            Profile = profile;
            Modifiers = new List<Modifier>();
            ModifierExecutionStrategy = ParticleModifierExecutionStrategy.Serial;
            Parameters = new ParticleReleaseParameters();
        }

        public ParticleEmitter(TextureRegion2D textureRegion, int capacity, TimeSpan lifeSpan, Profile profile)
            : this(null, textureRegion, capacity, lifeSpan, profile)
        {
        }

        public void Dispose()
        {
            Buffer.Dispose();
            GC.SuppressFinalize(this);
        }
        
        ~ParticleEmitter()
        {
            Dispose();
        }

        public string Name { get; set; }
        public int ActiveParticles => Buffer.Count;
        public Vector2 Offset { get; set; }
        public List<Modifier> Modifiers { get; }
        public Profile Profile { get; set; }
        public ParticleReleaseParameters Parameters { get; set; }
        public TextureRegion2D TextureRegion { get; set; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public ParticleModifierExecutionStrategy ModifierExecutionStrategy { get; set; }

        internal ParticleBuffer Buffer;

        public int Capacity
        {
            get { return Buffer.Size; }
            set
            {
                var oldBuffer = Buffer;
                oldBuffer.Dispose();
                Buffer = new ParticleBuffer(value);
            }
        }

        private float _lifeSpanSeconds;
        public TimeSpan LifeSpan
        {
            get { return TimeSpan.FromSeconds(_lifeSpanSeconds); }
            set { _lifeSpanSeconds = (float) value.TotalSeconds; }
        }

        private float _nextAutoTrigger;

        private bool _autoTrigger = true;
        public bool AutoTrigger
        {
            get { return _autoTrigger; }
            set
            {
                _autoTrigger = value;
                _nextAutoTrigger = 0;
            }
        }

        private float _autoTriggerFrequency;
        public float AutoTriggerFrequency
        {
            get { return _autoTriggerFrequency; }
            set
            {
                _autoTriggerFrequency = value;
                _nextAutoTrigger = 0;
            }
        }

        private void ReclaimExpiredParticles()
        {
            var iterator = Buffer.Iterator;
            var expired = 0;

            while (iterator.HasNext)
            {
                var particle = iterator.Next();

                if (_totalSeconds - particle->Inception < _lifeSpanSeconds)
                    break;

                expired++;
            }

            if (expired != 0)
                Buffer.Reclaim(expired);
        }

        public bool Update(float elapsedSeconds, Vector2 position = default(Vector2))
        {
            _totalSeconds += elapsedSeconds;

            if (_autoTrigger)
            {
                _nextAutoTrigger -= elapsedSeconds;

                if (_nextAutoTrigger <= 0)
                {
                    Trigger(position);
                    _nextAutoTrigger = _autoTriggerFrequency;
                }
            }

            if (Buffer.Count == 0)
                return false;

            ReclaimExpiredParticles();

            var iterator = Buffer.Iterator;

            while (iterator.HasNext)
            {
                var particle = iterator.Next();
                particle->Age = (_totalSeconds - particle->Inception) / _lifeSpanSeconds;
                particle->Position = particle->Position + particle->Velocity * elapsedSeconds;
            }

            ModifierExecutionStrategy.ExecuteModifiers(Modifiers, elapsedSeconds, iterator);
            return true;
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
                var offset = lineVector * _random.NextSingle();
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

                particle->Velocity = heading * speed;

                _random.NextColor(out particle->Color, Parameters.Color);

                particle->Opacity = _random.NextSingle(Parameters.Opacity);
                var scale = _random.NextSingle(Parameters.Scale);
                particle->Scale = new Vector2(scale, scale);
                particle->Rotation = _random.NextSingle(Parameters.Rotation);
                particle->Mass = _random.NextSingle(Parameters.Mass);
                particle->LayerDepth = layerDepth;
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}