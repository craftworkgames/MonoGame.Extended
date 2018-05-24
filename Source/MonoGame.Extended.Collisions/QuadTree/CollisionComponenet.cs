using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Collisions
{
    /// <summary>
    /// Handles basic collision between actors.
    /// When two actors collide, their OnCollision method is called.
    /// </summary>
    public class CollisionComponent : SimpleGameComponent
    {
        private readonly Dictionary<IActorTarget, QuadtreeData> _targetDataDictionary =
            new Dictionary<IActorTarget, QuadtreeData>();

        private readonly Quadtree _collisionTree;

        public CollisionComponent(RectangleF boundary)
        {
            _collisionTree = new Quadtree(boundary);
        }

        public override void Update(GameTime gameTime)
        {
            // Update bounding box locations.
            foreach (var value in _targetDataDictionary.Values)
            {
                _collisionTree.Remove(value);
                value.BoundingBox = value.Target.BoundingBox;
                _collisionTree.Insert(value);
            }

            // Detect collisions
            foreach (var value in _targetDataDictionary.Values)
            {
                var target = value.Target;
                var collisions =_collisionTree.Query(target.BoundingBox)
                    .Where(data => data.Target != target);

                // Generate list of collision Infos
                foreach (var other in collisions)
                {
                    var collisionInfo = new CollisionInfo
                    {
                        Other = other.Target,
                        PenetrationVector = RectangleF.Intersection(target.BoundingBox, other.Target.BoundingBox).Size
                    };

                    target.OnCollision(collisionInfo);
                }
            }
        }

        public void Insert(IActorTarget target)
        {
            if (!_targetDataDictionary.ContainsKey(target))
            {
                var data = new QuadtreeData(target);
                _targetDataDictionary.Add(target, data);
                _collisionTree.Insert(data);
            }
        }

        public void Remove(IActorTarget target)
        {
            if (_targetDataDictionary.ContainsKey(target))
            {
                var data = _targetDataDictionary[target];
                _collisionTree.Remove(data);
                _targetDataDictionary.Remove(target);
            }
        }
    }
}