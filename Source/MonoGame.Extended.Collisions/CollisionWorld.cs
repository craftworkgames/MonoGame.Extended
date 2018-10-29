using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Collisions
{
    public class CollisionWorld : IDisposable
    {

        private CollisionGrid _grid;
        private Size2 _size;

        public CollisionWorld()
        {
        }

        public void Dispose()
        {
        }

        public int Collision(Vector2 position)
        {
            if (_grid != null)
            {
                var playerGridPosition = new Point((int)position.X / _grid.CellWidth, (int)position.Y / _grid.CellHeight);

                if (!IsInGrid(playerGridPosition.X, playerGridPosition.Y))
                    return 0;

                var collidable = _grid.GetCellAtIndex(playerGridPosition.X, playerGridPosition.Y);
                return (int)collidable.Flag;
            }
            return 0;
        }

        public CollisionGrid CreateGrid(int[] data, int columns, int rows, int cellWidth, int cellHeight)
        {
            if (_grid != null)
                throw new InvalidOperationException("Only one collision grid can be created per world");

            _grid = new CollisionGrid(data, columns, rows, cellWidth, cellHeight);
            _size = new Size2(columns, rows);
            return _grid;
        }

        private CollisionInfo GetCollisionInfo(ICollidable first, ICollidable second, RectangleF intersectingRectangle)
        {
            var info = new CollisionInfo
            {
                Other = second
            };

            if (intersectingRectangle.Width < intersectingRectangle.Height)
            {
                var d = first.BoundingBox.Center.X < second.BoundingBox.Center.X
                    ? intersectingRectangle.Width
                    : -intersectingRectangle.Width;
                info.PenetrationVector = new Vector2(d, 0);
            }
            else
            {
                var d = first.BoundingBox.Center.Y < second.BoundingBox.Center.Y
                    ? intersectingRectangle.Height
                    : -intersectingRectangle.Height;
                info.PenetrationVector = new Vector2(0, d);
            }

            return info;
        }

        private bool IsInGrid(int x, int y)
        {
            return x >= 0 && x <= _size.Width && y >= 0 && y <= _size.Height;
        }

        /// <summary>
        /// Returns the tile flag at the posiiton
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public int GetTileData(Vector2 position)
        {
            var cell = _grid.GetCellAtPosition(new Vector3(position, 0));
            return cell.Data;
        }
    }
}