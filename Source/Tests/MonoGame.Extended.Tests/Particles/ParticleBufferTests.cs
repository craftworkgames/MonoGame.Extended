//using System;
//using MonoGame.Extended.Particles;
//using Xunit;

//namespace MonoGame.Extended.Tests.Particles
//{
//    
//    public class ParticleBufferTests
//    {
//        public class AvailableProperty
//        {
//            [Fact]
//            public void WhenNoParticlesReleased_ReturnsBufferSize()
//            {
//                var subject = new ParticleBuffer(100);

//                Assert.Equal(subject.Available, 100);
//            }

//            [Fact]
//            public void WhenSomeParticlesReleased_ReturnsAvailableCount()
//            {
//                var subject = new ParticleBuffer(100);

//                subject.Release(10);
//                Assert.Equal(subject.Available, 90);
//            }

//            [Fact]
//            public void WhenAllParticlesReleased_ReturnsZero()
//            {
//                var subject = new ParticleBuffer(100);

//                subject.Release(100);
//                Assert.Equal(subject.Available, 0);

//            }
//        }

//        public class CountProperty
//        {
//            [Fact]
//            public void WhenNoParticlesReleased_ReturnsZero()
//            {
//                var subject = new ParticleBuffer(100);
//                Assert.Equal(subject.Count, 0);
//            }

//            [Fact]
//            public void WhenSomeParticlesReleased_ReturnsCount()
//            {
//                var subject = new ParticleBuffer(100);

//                subject.Release(10);
//                Assert.Equal(subject.Count, 10);

//            }

//            [Fact]
//            public void WhenAllParticlesReleased_ReturnsZero()
//            {
//                var subject = new ParticleBuffer(100);

//                subject.Release(100);
//                Assert.Equal(subject.Count, 100);

//            }
//        }

//        public class ReleaseMethod
//        {
//            [Fact]
//            public void WhenPassedReasonableQuantity_ReturnsNumberReleased()
//            {
//                var subject = new ParticleBuffer(100);

//                var count = subject.Release(50);

//                Assert.Equal(count.Total, 50);
//            }

//            [Fact]
//            public void WhenPassedImpossibleQuantity_ReturnsNumberActuallyReleased()
//            {
//                var subject = new ParticleBuffer(100);

//                var count = subject.Release(200);
//                Assert.Equal(count.Total, 100);
//            }
//        }

//        public class ReclaimMethod
//        {
//            [Fact]
//            public void WhenPassedReasonableNumber_ReclaimsParticles()
//            {
//                var subject = new ParticleBuffer(100);

//                subject.Release(100);
//                Assert.Equal(subject.Count, 100);

//                subject.Reclaim(50);
//                Assert.Equal(subject.Count, 50);
//            }
//        }

//        //public class CopyToMethod
//        //{
//        //    [Fact]
//        //    public void WhenBufferIsSequential_CopiesParticlesInOrder()
//        //    {
//        //        unsafe
//        //        {
//        //            var subject = new ParticleBuffer(10);
//        //            var iterator = subject.Release(5);

//        //            do
//        //            {
//        //                var particle = iterator.Next();
//        //                particle->Age = 1f;
//        //            }
//        //            while (iterator.HasNext);

//        //            var destination = new Particle[10];

//        //            fixed (Particle* buffer = destination)
//        //            {
//        //                subject.CopyTo((IntPtr)buffer);
//        //            }

//        //            Assert.Equal(destination[0].Age, 1f, 0.0001);
//        //            Assert.Equal(destination[1].Age, 1f, 0.0001);
//        //            Assert.Equal(destination[2].Age, 1f, 0.0001);
//        //            Assert.Equal(destination[3].Age, 1f, 0.0001);
//        //            Assert.Equal(destination[4].Age, 1f, 0.0001);
//        //        }
//        //    }
//        //}

//        //public class CopyToReverseMethod
//        //{
//        //    [Fact]
//        //    public void WhenBufferIsSequential_CopiesParticlesInReverseOrder()
//        //    {
//        //        unsafe
//        //        {
//        //            var subject = new ParticleBuffer(10);
//        //            var iterator = subject.Release(5);
                    
//        //            do
//        //            {
//        //                var particle = iterator.Next();
//        //                particle->Age = 1f;
//        //            }
//        //            while (iterator.HasNext);

//        //            var destination = new Particle[10];

//        //            fixed (Particle* buffer = destination)
//        //            {
//        //                subject.CopyToReverse((IntPtr)buffer);
//        //            }

//        //            Assert.Equal(destination[0].Age, 1f, 0.0001);
//        //            Assert.Equal(destination[1].Age, 1f, 0.0001);
//        //            Assert.Equal(destination[2].Age, 1f, 0.0001);
//        //            Assert.Equal(destination[3].Age, 1f, 0.0001);
//        //            Assert.Equal(destination[4].Age, 1f, 0.0001);
//        //        }
//        //    }
//        //}

//        public class DisposeMethod
//        {
//            [Fact]
//            public void IsIdempotent()
//            {
//                var subject = new ParticleBuffer(100);
//                subject.Dispose();
//                subject.Dispose();
//            }
//        }
//    }
//}