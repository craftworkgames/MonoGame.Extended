using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Collections;
using MonoGame.Extended.Collisions.Broadphase;
using MonoGame.Extended.Collisions.Narrowphase;
using MonoGame.Extended.Collisions.Response;

namespace MonoGame.Extended.Collisions
{
    public sealed class CollisionSimulation
    {
        internal List<CollisionBody> BodiesToAdd;
        internal List<CollisionBody> BodiesToRemove;
        internal List<CollisionBody> Bodies;
        internal Deque<CollisionBody> FreeBodies;

        internal List<CollisionFixture> FixturesToAdd;
        internal List<CollisionFixture> FixturesToRemove;
        internal List<CollisionFixtureProxy> FixtureProxies;
        internal Deque<CollisionFixture> FreeFixtures;

        internal CollisionShape2D[] Shapes;
        internal Deque<CollisionShape2D> FreeShapes;

        internal HashSet<BroadphaseCollisionPair> BroadphaseCollisionPairsLookup;
        internal List<BroadphaseCollisionPair> BroadphaseCollisionPairs;
        internal List<NarrowphaseCollisionPair> NarrowphaseCollisionPairs;

        public IBroadphaseCollisionDetector Broadphase { get; }
        public INarrowphaseCollisionDetector Narrowphase { get; }
        public ICollisionResponder Responder { get; }

        public CollisionSimulation(IBroadphaseCollisionDetector broadphaseCollisionDetector = null, INarrowphaseCollisionDetector narrowphaseCollisionDetector = null, ICollisionResponder collisionResponder = null)
        {
            Broadphase = broadphaseCollisionDetector ?? new BruteForceBroadphase();
            Narrowphase = narrowphaseCollisionDetector ?? new PassThroughNarrowphase();
            Responder = collisionResponder ?? new EmptyResponder();

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
            Shapes = new CollisionShape2D[5];
            FreeShapes = new Deque<CollisionShape2D>();

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

        public CollisionFixture CreateFixture(CollisionBody body, Vector2[] vertices)
        {
            CollisionFixture fixture;
            CollisionShape2D shape;

            if (FreeFixtures.RemoveFromFront(out fixture))
            {
                FreeShapes.RemoveFromFront(out shape);
            }
            else
            {
                fixture = new CollisionFixture();
                shape = new CollisionShape2D(vertices);
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

        public void Update(GameTime gameTime)
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
        }

        private void ProcessFixturesToAdd()
        {
            foreach (var fixture in FixturesToAdd)
            {
                fixture.Flags &= ~CollisionFixtureFlags.IsBeingAdded;
                fixture.ProxyIndex = FixtureProxies.Count;
                var fixtureProxy = new CollisionFixtureProxy(fixture);
                FixtureProxies.Add(fixtureProxy);
            }
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
    }
}
