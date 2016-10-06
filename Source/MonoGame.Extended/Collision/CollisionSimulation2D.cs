using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Collision.BoundingVolumes;
using MonoGame.Extended.Collision.Detection.Broadphase;
using MonoGame.Extended.Collision.Detection.Narrowphase;
using MonoGame.Extended.Collision.Response;
using MonoGame.Extended.Collision.Shapes;

namespace MonoGame.Extended.Collision
{
    public class CollisionSimulation2D : SimpleDrawableGameComponent
    {
        private CollisionRenderer2D _renderer;

        internal readonly List<Collider2D> CollidersToAdd = new List<Collider2D>();
        internal readonly List<Collider2D> CollidersToRemove = new List<Collider2D>();
        internal readonly List<Collider2D> Colliders = new List<Collider2D>();

        internal readonly List<BroadphaseColliderProxy2D> ColliderProxies = new List<BroadphaseColliderProxy2D>();

        internal readonly HashSet<BroadphaseCollisionResult2D> BroadphaseCollisionPairsLookup = new HashSet<BroadphaseCollisionResult2D>();
        internal readonly List<BroadphaseCollisionResult2D> BroadphaseResults = new List<BroadphaseCollisionResult2D>();
        internal readonly List<NarrowphaseCollisionResult2D> NarrowphaseResults = new List<NarrowphaseCollisionResult2D>();

        private readonly List<BroadphaseCollisionDelegate> _broadphaseCollisionSubscribersList = new List<BroadphaseCollisionDelegate>();
        private readonly List<CollisionDelegate> _narrowphaseCollisionSubscribersList = new List<CollisionDelegate>();

        public IBroadphase2D Broadphase { get; }
        public INarrowphase2D Narrowphase { get; }
        public ICollisionResponder2D Responder { get; }

        public event BroadphaseCollisionDelegate BroadphaseCollision
        {
            add
            {
                _broadphaseCollisionSubscribersList.Add(value);
            }
            remove
            {
                _broadphaseCollisionSubscribersList.FastRemove(value);
            }
        }

        public event CollisionDelegate NarrowphaseCollision
        {
            add
            {
                _narrowphaseCollisionSubscribersList.Add(value);
            }
            remove
            {
                _narrowphaseCollisionSubscribersList.FastRemove(value);
            }
        }

        public CollisionSimulation2D(IBroadphase2D broadphase = null, INarrowphase2D narrowphase = null, ICollisionResponder2D responder = null)
        {
            Broadphase = broadphase ?? new BruteForceBroadphase2D(this);
            Narrowphase = narrowphase ?? new SeperatingAxisNarrowphase2D(this);
            Responder = responder;
        }

        public void Initialize(GraphicsDevice graphicsDevice)
        {
            _renderer = new CollisionRenderer2D(this, graphicsDevice);
        }

        public Collider2D CreateCollider(object owner, Transform2D transform, CollisionShape2D shape, BoundingVolumeType2D boundingVolumeType)
        {
            // TODO: Use a pool.
            var collider = new Collider2D(owner, transform, shape, boundingVolumeType);

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
            QueryForBroadphaseResults(gameTime);
            QueryForNarrowphaseResults(gameTime);
            RespondToNarrowphaseResults(gameTime);
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

        private void QueryForBroadphaseResults(GameTime gameTime)
        {
            BroadphaseResults.Clear();
            BroadphaseCollisionPairsLookup.Clear();

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var index = 0; index < ColliderProxies.Count; ++index)
            {
                var colliderProxy = ColliderProxies[index];
                Broadphase.Query(colliderProxy, gameTime, ProcessBroadphaseResult);
            }
        }

        private void ProcessBroadphaseResult(ref BroadphaseCollisionResult2D result)
        {
            if (BroadphaseCollisionPairsLookup.Contains(result))
            {
                return;
            }

            if (!OnBroadphaseCollision(ref result))
                return;

            BroadphaseCollisionPairsLookup.Add(result);
            BroadphaseResults.Add(result);
        }

        internal bool OnBroadphaseCollision(ref BroadphaseCollisionResult2D result)
        {
            foreach (var @delegate in _broadphaseCollisionSubscribersList)
            {
                bool cancelled;
                @delegate(ref result, out cancelled);
                if (cancelled)
                {
                    return false;
                }
            }

            return true;
        }

        private void QueryForNarrowphaseResults(GameTime gameTime)
        {
            NarrowphaseResults.Clear();

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var index = 0; index < BroadphaseResults.Count; index++)
            {
                var broadphaseResult = BroadphaseResults[index];

                NarrowphaseCollisionResult2D? narrowphaseResult;
                Narrowphase.Query(ref broadphaseResult, gameTime, out narrowphaseResult);

                if (narrowphaseResult == null)
                {
                    continue;
                }

                var result = narrowphaseResult.Value;
                if (OnNarrowphaseCollision(ref result))
                {
                    NarrowphaseResults.Add(narrowphaseResult.Value);
                }
            }
        }

        internal bool OnNarrowphaseCollision(ref NarrowphaseCollisionResult2D result)
        {
            foreach (var @delegate in _narrowphaseCollisionSubscribersList)
            {
                bool cancelled;
                @delegate(ref result, out cancelled);
                if (cancelled)
                {
                    return false; 
                }
            }

            return true;
        }

        private void RespondToNarrowphaseResults(GameTime gameTime)
        {
            var responder = Responder;
            if (responder == null)
                return;

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var index = 0; index < NarrowphaseResults.Count; index++)
            {
                var result = NarrowphaseResults[index];
                responder.RespondTo(ref result, gameTime);
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
                _renderer.DrawShape(collider.Shape);
                _renderer.DrawBoundingVolume(colliderProxy.WorldBoundingVolume);
            }

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < NarrowphaseResults.Count; i++)
            {
                var narrowphaseResult = NarrowphaseResults[i];

                _renderer.DrawCollision(narrowphaseResult);

                //// ReSharper disable once ForCanBeConvertedToForeach
                //for (var j = 0; j < narrowphaseResult.ContactPoints.Count; j++)
                //{
                //    var contact = narrowphaseResult.ContactPoints[j];
                //    _renderer.DrawContact();
                //}
            }
            
            _renderer.End();
        }
    }
}
