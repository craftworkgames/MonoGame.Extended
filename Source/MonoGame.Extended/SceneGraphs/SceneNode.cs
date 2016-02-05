using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Sprites;

namespace MonoGame.Extended.SceneGraphs
{
    public interface ISceneEntity
    {
    }

    public class SceneNode : IMovable, IRotatable, IScalable
    {
        private SceneNode(string name, SceneNode parent, Vector2 position, float rotation, Vector2 scale)
        {
            Name = name;
            Parent = parent;
            Position = position;
            Rotation = rotation;
            Scale = scale;

            _children = new SceneNodeCollection(this);
            _entities = new List<ISceneEntity>();
        }

        private readonly SceneNodeCollection _children;
        private readonly List<ISceneEntity> _entities;

        public string Name { get; set; }
        public Vector2 Position { get; set; }
        public float Rotation { get; set; }
        public Vector2 Scale { get; set; }
        
        public SceneNode Parent { get; private set; }
        public IEnumerable<SceneNode> Children => _children;
        public IEnumerable<ISceneEntity> Entities => _entities;
        
        internal static SceneNode CreateRootNode()
        {
            return new SceneNode(null, null, Vector2.Zero, 0, Vector2.One);
        }

        public SceneNode CreateChildSceneNode(string name, Vector2 position, float rotation, Vector2 scale)
        {
            var sceneNode = new SceneNode(name, this, position, rotation, scale);
            _children.Add(sceneNode);
            return sceneNode;
        }

        public SceneNode CreateChildSceneNode(string name, Vector2 position, float rotation)
        {
            return CreateChildSceneNode(name, position, rotation, Vector2.One);
        }

        public SceneNode CreateChildSceneNode(string name, Vector2 position)
        {
            return CreateChildSceneNode(name, position, 0, Vector2.One);
        }

        public SceneNode CreateChildSceneNode(string name)
        {
            return CreateChildSceneNode(name, Vector2.Zero, 0, Vector2.One);
        }

        public SceneNode CreateChildSceneNode()
        {
            return CreateChildSceneNode(null, Vector2.Zero, 0, Vector2.One);
        }

        public SceneNode CreateChildSceneNode(Vector2 position, float rotation, Vector2 scale)
        {
            return CreateChildSceneNode(null, position, rotation, scale);
        }

        public SceneNode CreateChildSceneNode(Vector2 position, float rotation)
        {
            return CreateChildSceneNode(null, position, rotation, Vector2.One);
        }

        public SceneNode CreateChildSceneNode(Vector2 position)
        {
            return CreateChildSceneNode(null, position, 0, Vector2.One);
        }

        public void RemoveChildSceneNode(int index)
        {
            RemoveChildSceneNode(_children[index]);
        }

        public void RemoveChildSceneNode(SceneNode sceneNode)
        {
            if (sceneNode.Parent != this)
                throw new InvalidOperationException($"{sceneNode} does not belong to parent");

            sceneNode.Parent = null;
            _children.Remove(sceneNode);
        }

        public void Attach(ISceneEntity entity)
        {
            _entities.Add(entity);
        }

        public Matrix GetLocalTransform()
        {
            var rotationMatrix = Matrix.CreateRotationZ(Rotation);
            var scaleMatrix = Matrix.CreateScale(new Vector3(Scale.X, Scale.Y, 1));
            var translationMatrix = Matrix.CreateTranslation(new Vector3(Position.X, Position.Y, 0));
            var tempMatrix = Matrix.Multiply(scaleMatrix, rotationMatrix);
            return Matrix.Multiply(tempMatrix, translationMatrix);
        }

        public Vector2 GetWorldPosition()
        {
            return Parent == null ? Position : Parent.GetWorldPosition() + Position.Rotate(Parent.Rotation);
        }

        public float GetWorldRotation()
        {
            return Parent?.GetWorldRotation() + Rotation ?? Rotation;
        }

        public Vector2 GetWorldScale()
        {
            return Parent?.GetWorldScale() * Scale ?? Scale;
        }

        public override string ToString()
        {
            return $"name: {Name}, position: {Position}, rotation: {Rotation}, scale: {Scale}";
        }
    }
}
