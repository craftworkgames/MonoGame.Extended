//using System;
//using Microsoft.Xna.Framework;
//using MonoGame.Extended.Particles;
//using MonoGame.Extended.Particles.Modifiers;
//using MonoGame.Extended.Particles.Profiles;
//using Xunit;

//namespace MonoGame.Extended.Tests.Particles
//{
//    
//    public class EmitterTests
//    {
//        public class UpdateMethod
//        {
//            [Fact]
//            public void WhenThereAreParticlesToExpire_DecreasesActiveParticleCount()
//            {
//                var subject = new ParticleEmitter(null, 100, TimeSpan.FromSeconds(1), Profile.Point())
//                {
//                    AutoTrigger = false,
//                    Parameters = new ParticleReleaseParameters
//                    {
//                        Quantity = 1
//                    }
//                };

//                subject.Trigger(new Vector2(0f, 0f));
//                Assert.Equal(subject.ActiveParticles, 1);

//                subject.Update(2f);
//                Assert.Equal(subject.ActiveParticles, 0);
//            }

//            [Fact]
//            public void WhenThereAreParticlesToExpire_DoesNotPassExpiredParticlesToModifiers()
//            {
//                var subject = new ParticleEmitter(null, 100, TimeSpan.FromSeconds(1), Profile.Point())
//                {
//                    Parameters = new ParticleReleaseParameters()
//                    {
//                        Quantity = 1
//                    },
//                    Modifiers =
//                    {
//                        new AssertionModifier(particle => particle.Age <= 1f)
//                    }
//                };

//                subject.Trigger(new Vector2(0f, 0f));
//                subject.Update(0.5f);
//                subject.Trigger(new Vector2(0f, 0f));
//                subject.Update(0.5f);
//                subject.Trigger(new Vector2(0f, 0f));
//                subject.Update(0.5f);
//            }

//            [Fact]
//            public void WhenThereAreNoActiveParticles_GracefullyDoesNothing()
//            {
//                var subject = new ParticleEmitter(null, 100, TimeSpan.FromSeconds(1), Profile.Point()) { AutoTrigger = false };

//                subject.Update(0.5f);
//                Assert.Equal(subject.ActiveParticles, 0);
//            }
//        }

//        public class TriggerMethod
//        {
//            [Fact]
//            public void WhenEnoughHeadroom_IncreasesActiveParticlesCountByReleaseQuantity()
//            {
//                var subject = new ParticleEmitter(null, 100, TimeSpan.FromSeconds(1), Profile.Point())
//                {
//                    Parameters = new ParticleReleaseParameters
//                    {
//                        Quantity = 10
//                    }
//                };
//                Assert.Equal(subject.ActiveParticles, 0);
//                subject.Trigger(new Vector2(0f, 0f));
//                Assert.Equal(subject.ActiveParticles, 10);
//            }

//            [Fact]
//            public void WhenNotEnoughHeadroom_IncreasesActiveParticlesCountByRemainingParticles()
//            {
//                var subject = new ParticleEmitter(null, 15, TimeSpan.FromSeconds(1), Profile.Point())
//                {
//                    Parameters = new ParticleReleaseParameters
//                    {
//                        Quantity = 10
//                    }
//                };

//                subject.Trigger(new Vector2(0f, 0f));
//                Assert.Equal(subject.ActiveParticles, 10);
//                subject.Trigger(new Vector2(0f, 0f));
//                Assert.Equal(subject.ActiveParticles, 15);
//            }

//            [Fact]
//            public void WhenNoRemainingParticles_DoesNotIncreaseActiveParticlesCount()
//            {
//                var subject = new ParticleEmitter(null, 10, TimeSpan.FromSeconds(1), Profile.Point())
//                {
//                    Parameters = new ParticleReleaseParameters
//                    {
//                        Quantity = 10
//                    }
//                };

//                subject.Trigger(new Vector2(0f, 0f));
//                Assert.Equal(subject.ActiveParticles, 10);
//                subject.Trigger(new Vector2(0f, 0f));
//                Assert.Equal(subject.ActiveParticles, 10);
//            }
//        }

//        public class DisposeMethod
//        {
//            [Fact]
//            public void IsIdempotent()
//            {
//                var subject = new ParticleEmitter(null, 10, TimeSpan.FromSeconds(1), Profile.Point());

//                subject.Dispose();
//                subject.Dispose();
//            }
//        }
//    }
//}