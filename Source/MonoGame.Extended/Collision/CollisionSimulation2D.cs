using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Collision.Detection.Broadphase;
using MonoGame.Extended.Collision.Detection.Broadphase.BoundingVolumes;
using MonoGame.Extended.Collision.Detection.Narrowphase;
using MonoGame.Extended.Collision.Detection.Narrowphase.Shapes;
using MonoGame.Extended.Collision.Response;

namespace MonoGame.Extended.Collision
{
    public class CollisionSimulation2D : SimpleDrawableGameComponent
    {
        private CollisionRenderer2D _renderer;

        internal readonly List<Collider2D> CollidersToAdd = new List<Collider2D>();
        internal readonly List<Collider2D> CollidersToRemove = new List<Collider2D>();
        internal readonly List<Collider2D> Colliders = new List<Collider2D>();

        internal readonly List<BroadphaseColliderProxy2D> ColliderProxies = new List<BroadphaseColliderProxy2D>();

        internal readonly HashSet<BroadphaseCollisionPair2D> BroadphaseCollisionPairsLookup = new HashSet<BroadphaseCollisionPair2D>();
        internal readonly List<BroadphaseCollisionPair2D> BroadphaseCollisionPairs = new List<BroadphaseCollisionPair2D>();
        internal readonly List<NarrowphaseCollisionPair2D> NarrowphaseCollisionPairs = new List<NarrowphaseCollisionPair2D>();

        private readonly List<BroadphaseCollisionDelegate> _broadphaseCollisionSubscribersList = new List<BroadphaseCollisionDelegate>();
        private readonly List<NarrowphaseCollisionDelegate> _narrowphaseCollisionSubscribersList = new List<NarrowphaseCollisionDelegate>();

        public ICollisionBroadphase2D Broadphase { get; }
        public ICollisionNarrowphase2D Narrowphase { get; }
        public ICollisionResponder2D Responder { get; }

        public event BroadphaseCollisionDelegate BroadphaseCollision
        {
            add
            {
                _broadphaseCollisionSubscribersList.Add(value);
            }
            remove
            {
                _broadphaseCollisionSubscribersList.Remove(value);
            }
        }

        public event NarrowphaseCollisionDelegate NarrowphaseCollision
        {
            add
            {
                _narrowphaseCollisionSubscribersList.Add(value);
            }
            remove
            {
                _narrowphaseCollisionSubscribersList.Remove(value);
            }
        }

        public CollisionSimulation2D(ICollisionBroadphase2D broadphase = null, ICollisionNarrowphase2D narrowphase = null, ICollisionResponder2D responder = null)
        {
            Broadphase = broadphase ?? new BruteForceBroadphase2D(this);
            Narrowphase = narrowphase ?? new PassThroughNarrowphase2D(this);
            Responder = responder ?? new DefaultCollisionResponder2D(this);
        }

        public void Initialize(GraphicsDevice graphicsDevice)
        {
            _renderer = new CollisionRenderer2D(this, graphicsDevice);
        }

        public Collider2D CreateCollider(ITransform2D transform, CollisionShape2D shape, BoundingVolumeType2D boundingVolumeType)
        {
            // TODO: Use a pool.
            var collider = new Collider2D(transform, shape, boundingVolumeType);

            CollidersToAdd.Add(collider);

            return collider;
        }

        public void DestroyCollider(Collider2D collider)
        {
            CollidersToRemove.Add(collider);
        }

        public override void Update(GameTime gameTime)
        {
            ProcessCollidersToAdd();
            ProcessCollidersToRemove();
            UpdateColliders();
            FindBroadphaseCollisionPairs(gameTime);
            FindNarrowphaseCollisionPairs(gameTime);
            RespondToNarrowphaseCollisionPairs(gameTime);
        }

        private void ProcessCollidersToAdd()
        {
            foreach (var collider in CollidersToAdd)
            {
                collider.Index = Colliders.Count - 1;
                Colliders.Add(collider);

                var colliderProxy = new BroadphaseColliderProxy2D(collider);
                Broadphase.Add(colliderProxy);
                ColliderProxies.Add(colliderProxy);
            }

            CollidersToAdd.Clear();
        }
            
        private void ProcessCollidersToRemove()
        {
            var lastIndex = Colliders.Count - 1;

            foreach (var collider in CollidersToRemove)
            {
                Colliders[lastIndex].Index = collider.Index;
                Colliders[collider.Index] = Colliders[lastIndex];
                Colliders.RemoveAt(lastIndex);

                var colliderProxy = ColliderProxies[collider.Index];
                ColliderProxies[collider.Index] = ColliderProxies[lastIndex];
                ColliderProxies.RemoveAt(lastIndex);

                Broadphase.Remove(colliderProxy);
            }

            CollidersToRemove.Clear();
        }

        private void UpdateColliders()
        {
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var index = 0; index < Colliders.Count; ++index)
            {
                var colliderProxy = ColliderProxies[index];
                var collider = colliderProxy.Collider;
                collider.UpdateIfNecessary(colliderProxy.WorldBoundingVolume);
                Broadphase.Update(colliderProxy);
            }
        }

        private void FindBroadphaseCollisionPairs(GameTime gameTime)
        {
            BroadphaseCollisionPairs.Clear();
            BroadphaseCollisionPairsLookup.Clear();

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var index = 0; index < ColliderProxies.Count; ++index)
            {
                var colliderProxy = ColliderProxies[index];
                Broadphase.Query(colliderProxy, gameTime, ProcessBroadphaseCollisionPair);
            }
        }

        private void ProcessBroadphaseCollisionPair(ref BroadphaseCollisionPair2D broadphaseCollisionPair)
        {
            if (BroadphaseCollisionPairsLookup.Contains(broadphaseCollisionPair))
            {
                return;
            }

            bool cancelled;
            OnBroadphaseCollision(ref broadphaseCollisionPair, out cancelled);
            if (cancelled)
                return;

            BroadphaseCollisionPairsLookup.Add(broadphaseCollisionPair);
            BroadphaseCollisionPairs.Add(broadphaseCollisionPair);
        }

        internal void OnBroadphaseCollision(ref BroadphaseCollisionPair2D collisionPair, out bool cancelled)
        {
            var firstCollider = collisionPair.FirstCollider;
            var secondCollider = collisionPair.SecondCollider;

            foreach (var @delegate in _broadphaseCollisionSubscribersList)
            {
                @delegate(firstCollider, secondCollider, out cancelled);
                if (cancelled)
                {
                    return;
                }
            }

            firstCollider.OnBroadphaseCollision(collisionPair.SecondCollider, out cancelled);
            if (cancelled)
                return;
            secondCollider.OnBroadphaseCollision(collisionPair.FirstCollider, out cancelled);
        }

        private void FindNarrowphaseCollisionPairs(GameTime gameTime)
        {
            NarrowphaseCollisionPairs.Clear();

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var index = 0; index < BroadphaseCollisionPairs.Count; index++)
            {
                var collisionPair = BroadphaseCollisionPairs[index];
                Narrowphase.Resolve(gameTime, ref collisionPair, ProcessNarrowphaseCollisionPair);
            }
        }

        private void ProcessNarrowphaseCollisionPair(ref NarrowphaseCollisionPair2D narrowphaseCollisionPair)
        {
            bool cancelled;
            OnNarrowphaseCollision(ref narrowphaseCollisionPair, out cancelled);
            if (cancelled)
                return;

            NarrowphaseCollisionPairs.Add(narrowphaseCollisionPair);
        }

        internal void OnNarrowphaseCollision(ref NarrowphaseCollisionPair2D collisionPair, out bool cancelled)
        {
            var firstCollider = collisionPair.FirstCollider;
            var secondCollider = collisionPair.SecondCollider;

            foreach (var @delegate in _broadphaseCollisionSubscribersList)
            {
                @delegate(firstCollider, secondCollider, out cancelled);
                if (cancelled)
                {
                    return;
                }
            }

            firstCollider.OnNarrowphaseCollision(collisionPair.SecondCollider, out cancelled);
            if (cancelled)
                return;
            secondCollider.OnNarrowphaseCollision(collisionPair.FirstCollider, out cancelled);
        }

        private void RespondToNarrowphaseCollisionPairs(GameTime gameTime)
        {
            var responder = Responder;

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var index = 0; index < NarrowphaseCollisionPairs.Count; index++)
            {
                var collisionPair = NarrowphaseCollisionPairs[index];
                responder.RespondTo(gameTime, ref collisionPair);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            _renderer.Begin();

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var index = 0; index < ColliderProxies.Count; index++)
            {
                var colliderProxy = ColliderProxies[index];
                var collider = colliderProxy.Collider;
                Matrix2D worldMatrix;
                collider.Transform.GetWorldMatrix(out worldMatrix);
                _renderer.DrawShape(collider.Shape, ref worldMatrix);
                _renderer.DrawBoundingVolume(colliderProxy.WorldBoundingVolume);
            }
            
            _renderer.End();
        }
    }
}
