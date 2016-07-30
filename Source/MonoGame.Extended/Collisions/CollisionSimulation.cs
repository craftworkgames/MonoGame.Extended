//using System;
//using System.Collections.Generic;
//using Microsoft.Xna.Framework;
//using MonoGame.Extended.Collisions.Broadphase;
//using MonoGame.Extended.Collisions.Drawing;
//using MonoGame.Extended.Collisions.Narrowphase;
//using MonoGame.Extended.Collisions.Response;
//using MonoGame.Extended.Shapes.Explicit;
//
//namespace MonoGame.Extended.Collisions
//{
//
//    //TODO: Move type constraints to delegate class, expose CollisionSimulation2D and CollisionSimulation3D as public classes.
//    public class CollisionSimulation<TShapeVertexType, TTransform, TCollisionBody> : SimpleDrawableGameComponent, ICollisionSimulation
//        where TShapeVertexType : struct, IShapeVertexType<TShapeVertexType>
//        where TTransform : struct, ITransform
//        where TCollisionBody : CollisionBody<TTransform>
//    {
//        internal static ICollisionSimulation Instance;
//
//        internal List<TCollisionBody> BodiesToAdd = new List<TCollisionBody>();
//        internal List<TCollisionBody> BodiesToRemove = new List<TCollisionBody>();
//        internal List<TCollisionBody> Bodies = new List<TCollisionBody>();
//
//        internal List<CollisionFixture<TShapeVertexType, TTransform>> FixturesToAdd = new List<CollisionFixture<TShapeVertexType, TTransform>>();
//        internal List<CollisionFixture<TShapeVertexType, TTransform>> FixturesToRemove = new List<CollisionFixture<TShapeVertexType, TTransform>>();
//        internal List<CollisionFixtureProxy<TShapeVertexType, TTransform>> FixtureProxies = new List<CollisionFixtureProxy<TShapeVertexType, TTransform>>();
//
//        internal HashSet<BroadphaseCollisionPair> BroadphaseCollisionPairsLookup = new HashSet<BroadphaseCollisionPair>();
//        internal List<BroadphaseCollisionPair> BroadphaseCollisionPairs = new List<BroadphaseCollisionPair>();
//        internal List<NarrowphaseCollisionPair> NarrowphaseCollisionPairs = new List<NarrowphaseCollisionPair>();
//
//        public ICollisionBroadphase Broadphase { get; }
//        public ICollisionNarrowphase Narrowphase { get; }
//        public ICollisionResponder Responder { get; }
//        public ICollisionDrawer Drawer { get; set; }
//
//        //TODO: Need factory for collisionBody
//
//        public CollisionSimulation(ICollisionBroadphase broadphase = null, ICollisionNarrowphase narrowphase = null, ICollisionResponder responder = null, ICollisionDrawer drawer = null)
//        {
//            if (Instance != null)
//            {
//                throw new InvalidOperationException(message: "Only one CollisionSimulation class can be instantiated.");
//            }
//            Instance = this;
//
//            Broadphase = broadphase ?? new BruteForceBroadphase();
//            Narrowphase = narrowphase ?? new PassThroughNarrowphase();
//            Responder = responder ?? new EmptyResponder();
//            Drawer = drawer;
//
//            Broadphase.Initialize(ProcessBroadphaseCollisionPair);
//            Narrowphase.Initialize(ProcessNarrowphaseCollisionPair);
//        }
//
//        public TCollisionBody CreateBody(object userObject)
//        {
//            // TODO: Use a pool of body objects.
//            CollisionBody<TTransform> body = new TCollisionBody
//            {
//                CollisionSimulation = this
//            };
//
//            body.Flags |= CollisionBodyFlags.IsBeingAdded;
//            BodiesToAdd.Add(body);
//
//            return body;
//        }
//
//        public void DestroyBody(CollisionBody body)
//        {
//            body.Flags |= CollisionBodyFlags.IsBeingRemoved;
//            BodiesToRemove.Add(body);
//        }
//
//        public CollisionFixture CreateFixture(CollisionBody body, CollisionShape2D shape)
//        {
//            CollisionFixture fixture;
//
//            // TODO: Use a pool of fixture objects.
//            fixture = new CollisionFixture();
//
//            fixture.Flags |= CollisionFixtureFlags.IsBeingAdded;
//            fixture.Shape = shape;
//            fixture.Body = body;
//
//            FixturesToAdd.Add(fixture);
//
//            return fixture;
//        }
//
//        internal void DestroyFixture(CollisionFixture fixture)
//        {
//            fixture.Flags |= CollisionFixtureFlags.IsBeingRemoved;
//            FixturesToRemove.Add(fixture);
//        }
//
//        public override void Update(GameTime gameTime)
//        {
//            ProcessBodiesToAdd();
//            ProcessBodiesToRemove();
//            ProcessFixturesToAdd();
//            ProcessFixturesToRemove();
//
//            BroadphaseCollisionPairs.Clear();
//            Broadphase.Update(gameTime, FixtureProxies);
//
//            NarrowphaseCollisionPairs.Clear();
//            Narrowphase.Update(gameTime, BroadphaseCollisionPairs);
//
//            Responder.Update(gameTime, NarrowphaseCollisionPairs);
//        }
//
//        private void ProcessBodiesToAdd()
//        {
//            foreach (var body in BodiesToAdd)
//            {
//                body.Flags &= ~CollisionBodyFlags.IsBeingAdded;
//                body.Index = Bodies.Count - 1;
//                Bodies.Add(body);
//            }
//            BodiesToAdd.Clear();
//        }
//
//        private void ProcessBodiesToRemove()
//        {
//            foreach (var body in BodiesToRemove)
//            {
//                // reset state will clear the flags
//                body.ResetState();
//                // within the array, swap the body reference to the back of the array; this is important to achieve O(1) removal
//                Bodies[Bodies.Count - 1].Index = body.Index;
//                Bodies.FastRemove(body.Index);
//            }
//            BodiesToRemove.Clear();
//        }
//
//        private void ProcessFixturesToAdd()
//        {
//            foreach (var fixture in FixturesToAdd)
//            {
//                fixture.Flags &= ~CollisionFixtureFlags.IsBeingAdded;
//                fixture.ProxyIndex = FixtureProxies.Count;
//                var fixtureProxy = new CollisionFixtureProxy(fixture);
//                fixture.Shape._update = () =>
//                {
//
//                };
//                FixtureProxies.Add(fixtureProxy);
//            }
//            FixturesToAdd.Clear();
//        }
//
//        private void ProcessFixturesToRemove()
//        {
//            foreach (var fixture in FixturesToRemove)
//            {
//                // reset state will clear the flags
//                fixture.ResetState();
//                FreeFixtures.AddToBack(fixture);
//                FixtureProxies[FixtureProxies.Count - 1].Fixture.ProxyIndex = fixture.ProxyIndex;
//                FixtureProxies.FastRemove(fixture.ProxyIndex);
//            }
//            FixturesToRemove.Clear();
//        }
//
//        private void ProcessBroadphaseCollisionPair(ref BroadphaseCollisionPair broadphaseCollisionPair)
//        {
//            if (BroadphaseCollisionPairsLookup.Contains(broadphaseCollisionPair))
//            {
//                return;
//            }
//
//            BroadphaseCollisionPairsLookup.Add(broadphaseCollisionPair);
//            BroadphaseCollisionPairs.Add(broadphaseCollisionPair);
//        }
//
//        private void ProcessNarrowphaseCollisionPair(ref NarrowphaseCollisionPair narrowphaseCollisionPair)
//        {
//            NarrowphaseCollisionPairs.Add(narrowphaseCollisionPair);
//        }
//
//        public override void Draw(GameTime gameTime)
//        {
//            if (Drawer == null)
//            {
//                return;
//            }
//
//            Drawer.Begin();
//
//            // ReSharper disable once ForCanBeConvertedToForeach
//            for (var index = 0; index < FixtureProxies.Count; index++)
//            {
//                var fixtureProxy = FixtureProxies[index];
//                Drawer.DrawBoundingVolume(ref fixtureProxy.BoundingVolume);
//            }
//
//            Drawer.End();
//        }
//    }
//}
