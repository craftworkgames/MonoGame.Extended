using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Collisions
{
    public class QuadTreeCollisionComponent : SimpleGameComponent
    {
        private readonly Dictionary<IActorTarget, QuadTreeData> _targetDataDictionary =
            new Dictionary<IActorTarget, QuadTreeData>();

        private readonly QuadTree _collisionTree;

        public QuadTreeCollisionComponent(RectangleF boundary)
        {
            _collisionTree = new QuadTree(boundary);
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var value in _targetDataDictionary.Values)
            {
                var target = value.Target;
                var collisions =_collisionTree.Query(target.BoundingBox);

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
                var data = new QuadTreeData(target);
                _targetDataDictionary.Add(target, data);
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