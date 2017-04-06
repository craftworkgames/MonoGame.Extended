using System;
using MonoGame.Extended.Particles;
using NUnit.Framework;

namespace MonoGame.Extended.Tests.Particles
{
    [TestFixture]
    public class ParticleBufferTests
    {
        public class AvailableProperty
        {
            [Test]
            public void WhenNoParticlesReleased_ReturnsBufferSize()
            {
                var subject = new ParticleBuffer(100);

                Assert.AreEqual(subject.Available, 100);
            }

            [Test]
            public void WhenSomeParticlesReleased_ReturnsAvailableCount()
            {
                var subject = new ParticleBuffer(100);

                subject.Release(10);
                Assert.AreEqual(subject.Available, 90);
            }

            [Test]
            public void WhenAllParticlesReleased_ReturnsZero()
            {
                var subject = new ParticleBuffer(100);

                subject.Release(100);
                Assert.AreEqual(subject.Available, 0);

            }
        }

        public class CountProperty
        {
            [Test]
            public void WhenNoParticlesReleased_ReturnsZero()
            {
                var subject = new ParticleBuffer(100);
                Assert.AreEqual(subject.Count, 0);
            }

            [Test]
            public void WhenSomeParticlesReleased_ReturnsCount()
            {
                var subject = new ParticleBuffer(100);

                subject.Release(10);
                Assert.AreEqual(subject.Count, 10);

            }

            [Test]
            public void WhenAllParticlesReleased_ReturnsZero()
            {
                var subject = new ParticleBuffer(100);

                subject.Release(100);
                Assert.AreEqual(subject.Count, 100);

            }
        }

        public class ReleaseMethod
        {
            [Test]
            public void WhenPassedReasonableQuantity_ReturnsNumberReleased()
            {
                var subject = new ParticleBuffer(100);

                var count = subject.Release(50);

                Assert.AreEqual(count.Total, 50);
            }

            [Test]
            public void WhenPassedImpossibleQuantity_ReturnsNumberActuallyReleased()
            {
                var subject = new ParticleBuffer(100);

                var count = subject.Release(200);
                Assert.AreEqual(count.Total, 100);
            }
        }

        public class ReclaimMethod
        {
            [Test]
            public void WhenPassedReasonableNumber_ReclaimsParticles()
            {
                var subject = new ParticleBuffer(100);

                subject.Release(100);
                Assert.AreEqual(subject.Count, 100);

                subject.Reclaim(50);
                Assert.AreEqual(subject.Count, 50);
            }
        }

        //public class CopyToMethod
        //{
        //    [Test]
        //    public void WhenBufferIsSequential_CopiesParticlesInOrder()
        //    {
        //        unsafe
        //        {
        //            var subject = new ParticleBuffer(10);
        //            var iterator = subject.Release(5);

        //            do
        //            {
        //                var particle = iterator.Next();
        //                particle->Age = 1f;
        //            }
        //            while (iterator.HasNext);

        //            var destination = new Particle[10];

        //            fixed (Particle* buffer = destination)
        //            {
        //                subject.CopyTo((IntPtr)buffer);
        //            }

        //            Assert.AreEqual(destination[0].Age, 1f, 0.0001);
        //            Assert.AreEqual(destination[1].Age, 1f, 0.0001);
        //            Assert.AreEqual(destination[2].Age, 1f, 0.0001);
        //            Assert.AreEqual(destination[3].Age, 1f, 0.0001);
        //            Assert.AreEqual(destination[4].Age, 1f, 0.0001);
        //        }
        //    }
        //}

        //public class CopyToReverseMethod
        //{
        //    [Test]
        //    public void WhenBufferIsSequential_CopiesParticlesInReverseOrder()
        //    {
        //        unsafe
        //        {
        //            var subject = new ParticleBuffer(10);
        //            var iterator = subject.Release(5);
                    
        //            do
        //            {
        //                var particle = iterator.Next();
        //                particle->Age = 1f;
        //            }
        //            while (iterator.HasNext);

        //            var destination = new Particle[10];

        //            fixed (Particle* buffer = destination)
        //            {
        //                subject.CopyToReverse((IntPtr)buffer);
        //            }

        //            Assert.AreEqual(destination[0].Age, 1f, 0.0001);
        //            Assert.AreEqual(destination[1].Age, 1f, 0.0001);
        //            Assert.AreEqual(destination[2].Age, 1f, 0.0001);
        //            Assert.AreEqual(destination[3].Age, 1f, 0.0001);
        //            Assert.AreEqual(destination[4].Age, 1f, 0.0001);
        //        }
        //    }
        //}

        public class DisposeMethod
        {
            [Test]
            public void IsIdempotent()
            {
                var subject = new ParticleBuffer(100);
                subject.Dispose();
                subject.Dispose();
            }
        }
    }
}