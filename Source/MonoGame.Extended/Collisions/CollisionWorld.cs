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
            _bodies = new List<CollisionBody>();
        }

        public void Dispose()
        {
        }

        private readonly Vector2 _gravity;
        private readonly List<CollisionBody> _bodies;
        private CollisionGrid _grid;

        public CollisionBody CreateBody(IDynamicCollidable collidable)
        {
            var body = new CollisionBody(collidable);
            _bodies.Add(body);
            return body;
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

            foreach (var body in _bodies)
            {
                body.Velocity += _gravity * deltaTime;
                body.Position += body.Velocity * deltaTime;

                if(_grid != null)
                {
                    var boundingBox = body.GetAxisAlignedBoundingBox();
                    _grid.CollidesWith(boundingBox, body.OnCollision);
                }
            }
        }
    }
}