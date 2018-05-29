//using System;
//using MonoGame.Extended.Particles;
//using MonoGame.Extended.Particles.Modifiers;
//using Xunit;

//namespace MonoGame.Extended.Tests.Particles
//{
//    internal class AssertionModifier : Modifier
//    {
//        private readonly Predicate<Particle> _predicate;

//        public AssertionModifier(Predicate<Particle> predicate)
//        {
//            _predicate = predicate;
//        }

//        public override unsafe void Update(float elapsedSeconds, ParticleBuffer.ParticleIterator iterator)
//        {
//            while (iterator.HasNext) {
//                var particle = iterator.Next();
//                Assert.IsTrue(_predicate(*particle));
//            }
//        }
//    }
//}