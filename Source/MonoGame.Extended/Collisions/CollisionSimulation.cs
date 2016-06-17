using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Collections;
using MonoGame.Extended.Collisions.Broadphase;
using MonoGame.Extended.Collisions.Drawing;
using MonoGame.Extended.Collisions.Narrowphase;
using MonoGame.Extended.Collisions.Response;

namespace MonoGame.Extended.Collisions
{
    public sealed class CollisionSimulation : SimpleDrawableGameComponent, ICollisionSimulation
    {
        internal static ICollisionSimulation Instance;

        internal List<CollisionBody> BodiesToAdd;
        internal List<CollisionBody> BodiesToRemove;
        internal List<CollisionBody> Bodies;
        internal Deque<CollisionBody> FreeBodies;

        internal List<CollisionFixture> FixturesToAdd;
        internal List<CollisionFixture> FixturesToRemove;
        internal List<CollisionFixtureProxy> FixtureProxies;
        internal Deque<CollisionFixture> FreeFixtures;

        internal HashSet<BroadphaseCollisionPair> BroadphaseCollisionPairsLookup;
        internal List<BroadphaseCollisionPair> BroadphaseCollisionPairs;
        internal List<NarrowphaseCollisionPair> NarrowphaseCollisionPairs;

        public ICollisionBroadphase Broadphase { get; }
        public ICollisionNarrowphase Narrowphase { get; }
        public ICollisionResponder Responder { get; }
        public ICollisionDrawer Drawer { get; set; }

        public CollisionSimulation(ICollisionBroadphase broadphase = null, ICollisionNarrowphase narrowphase = null, ICollisionResponder responder = null, ICollisionDrawer drawer = null)
        {
            if (Instance != null)
            {
                throw new InvalidOperationException("Only one CollisionSimulation class can be instantiated.");
            }
            Instance = this;

            Broadphase = broadphase ?? new BruteForceBroadphase();
            Narrowphase = narrowphase ?? new PassThroughNarrowphase();
            Responder = responder ?? new EmptyResponder();
            Drawer = drawer;

            Broadphase.Initialize(ProcessBroadphaseCollisionPair);
            Narrowphase.Initialize(ProcessNarrowphaseCollisionPair);

            BodiesToAdd = new List<CollisionBody>();
            BodiesToRemove = new List<CollisionBody>();
            Bodies = new List<CollisionBody>();
            FreeBodies = new Deque<CollisionBody>();

            FixturesToAdd = new List<CollisionFixture>();
            FixturesToRemove = new List<CollisionFixture>();
            FixtureProxies = new List<CollisionFixtureProxy>();
            FreeFixtures = new Deque<CollisionFixture>();

            BroadphaseCollisionPairsLookup = new HashSet<BroadphaseCollisionPair>();
            BroadphaseCollisionPairs = new List<BroadphaseCollisionPair>();
            NarrowphaseCollisionPairs = new List<NarrowphaseCollisionPair>();
        }

        public CollisionBody CreateBody(object userObject)
        {
            CollisionBody body;

            if (!FreeBodies.RemoveFromFront(out body))
            {
                body = new CollisionBody();
            }

            body.Flags |= CollisionBodyFlags.IsBeingAdded;
            BodiesToAdd.Add(body);

            return body;
        }

        public void DestroyBody(CollisionBody body)
        {
            body.Flags |= CollisionBodyFlags.IsBeingRemoved;
            BodiesToRemove.Add(body);
        }

        public CollisionFixture CreateFixture(CollisionBody body, CollisionShape2D shape)
        {
            CollisionFixture fixture;

            if (FreeFixtures.RemoveFromFront(out fixture))
            {
            }
            else
            {
                fixture = new CollisionFixture();
            }

            fixture.Flags |= CollisionFixtureFlags.IsBeingAdded;
            fixture.Shape = shape;
            fixture.Body = body;

            FixturesToAdd.Add(fixture);

            return fixture;
        }

        internal void DestroyFixture(CollisionFixture fixture)
        {
            fixture.Flags |= CollisionFixtureFlags.IsBeingRemoved;
            FixturesToRemove.Add(fixture);
        }

        public override void Update(GameTime gameTime)
        {
            ProcessBodiesToAdd();
            ProcessBodiesToRemove();
            ProcessFixturesToAdd();
            ProcessFixturesToRemove();

            BroadphaseCollisionPairs.Clear();
            Broadphase.Update(gameTime, FixtureProxies);

            NarrowphaseCollisionPairs.Clear();
            Narrowphase.Update(gameTime, BroadphaseCollisionPairs);

            Responder.Update(gameTime, NarrowphaseCollisionPairs);
        }

        private void ProcessBodiesToAdd()
        {
            foreach (var body in BodiesToAdd)
            {
                body.Flags &= ~CollisionBodyFlags.IsBeingAdded;
                body.Index = Bodies.Count;
                Bodies.Add(body);
            }
            BodiesToAdd.Clear();
        }

        private void ProcessBodiesToRemove()
        {
            foreach (var body in BodiesToRemove)
            {
                // reset state will clear the flags
                body.ResetState();
                FreeBodies.AddToBack(body);
                Bodies[Bodies.Count - 1].Index = body.Index;
                Bodies.FastRemove(body.Index);
            }
            BodiesToRemove.Clear();
        }

        private void ProcessFixturesToAdd()
        {
            foreach (var fixture in FixturesToAdd)
            {
                fixture.Flags &= ~CollisionFixtureFlags.IsBeingAdded;
                fixture.ProxyIndex = FixtureProxies.Count;
                var fixtureProxy = new CollisionFixtureProxy(fixture);
                fixture.Shape._update = () =>
                {

                };
                FixtureProxies.Add(fixtureProxy);
            }
            FixturesToAdd.Clear();
        }

        private void ProcessFixturesToRemove()
        {
            foreach (var fixture in FixturesToRemove)
            {
                // reset state will clear the flags
                fixture.ResetState();
                FreeFixtures.AddToBack(fixture);
                FixtureProxies[FixtureProxies.Count - 1].Fixture.ProxyIndex = fixture.ProxyIndex;
                FixtureProxies.FastRemove(fixture.ProxyIndex);
            }
            FixturesToRemove.Clear();
        }

        private void ProcessBroadphaseCollisionPair(ref BroadphaseCollisionPair broadphaseCollisionPair)
        {
            if (BroadphaseCollisionPairsLookup.Contains(broadphaseCollisionPair))
            {
                return;
            }

            BroadphaseCollisionPairsLookup.Add(broadphaseCollisionPair);
            BroadphaseCollisionPairs.Add(broadphaseCollisionPair);
        }

        private void ProcessNarrowphaseCollisionPair(ref NarrowphaseCollisionPair narrowphaseCollisionPair)
        {
            NarrowphaseCollisionPairs.Add(narrowphaseCollisionPair);
        }

        public override void Draw(GameTime gameTime)
        {
            if (Drawer == null)
            {
                return;
            }

            Drawer.Begin();

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var index = 0; index < FixtureProxies.Count; index++)
            {
                var fixtureProxy = FixtureProxies[index];
                Drawer.DrawBoundingVolume(ref fixtureProxy.BoundingVolume);
            }

            Drawer.End();
        }
    }
}
