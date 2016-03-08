﻿using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.SceneGraphs
{
    public class SceneGraph
    {
        public SceneNode RootNode { get; }

        public SceneGraph()
        {
            RootNode = new SceneNode();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            RootNode?.Draw(spriteBatch);
        }

        public SceneNode GetSceneNodeAt(float x, float y)
        {
            var node = RootNode;

            while (node != null && node.GetBoundingRectangle().Contains(x, y))
            {
                var childNode = node.Children.FirstOrDefault(c => c.GetBoundingRectangle().Contains(x, y));

                if (childNode != null)
                {
                    node = childNode;
                }
                else
                {
                    return node;
                }
            }

            return null;
        }

        public SceneNode GetSceneNodeAt(Vector2 position)
        {
            return GetSceneNodeAt(position.X, position.Y);
        }
    }
}