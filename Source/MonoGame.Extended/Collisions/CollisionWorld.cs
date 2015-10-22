using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Collisions
{
    public class CollisionWorld : IDisposable, IUpdate
    {
        public CollisionWorld(Vector2 gravity)
        {
            _gravity = gravity;
            _actors = new List<CollisionActor>();
        }

        public void Dispose()
        {
        }

        private readonly Vector2 _gravity;
        private readonly List<CollisionActor> _actors;
        private CollisionGrid _grid;

        public CollisionActor CreateActor(IActorTarget target)
        {
            var actor = new CollisionActor(target);
            _actors.Add(actor);
            return actor;
        }

        public CollisionGrid CreateGrid(int[] data, int columns, int rows, int cellWidth, int cellHeight)
        {
            if (_grid != null)
                throw new InvalidOperationException("Only one collision grid can be created per world");

            _grid = new CollisionGrid(data, columns, rows, cellWidth, cellHeight);
            return _grid;
        }

        public void Update(GameTime gameTime)
        {
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            foreach (var actor in _actors)
            {
                //actor.Velocity += _gravity * deltaTime;
                //actor.Position += actor.Velocity * deltaTime;

                if(_grid != null)
                {
                    var boundingBox = actor.BoundingBox;
                    _grid.CollidesWith(boundingBox, actor.OnCollision);
                }
            }
        }
    }
}