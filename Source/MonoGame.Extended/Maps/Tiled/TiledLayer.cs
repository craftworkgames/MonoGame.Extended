using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Maps.Tiled
{
    public abstract class TiledLayer : IDisposable
    {
        protected TiledLayer(string name, int depth)
        {
            Name = name;
            Properties = new TiledProperties();
            IsVisible = true;
            Opacity = 1.0f;
            Depth = depth;
        }

        public abstract void Dispose();

        public string Name { get; }
        public TiledProperties Properties { get; }
        public bool IsVisible { get; set; }
        public float Opacity { get; set; }
        public float Depth { get; private set; }

        public abstract void Draw(SpriteBatch spriteBatch, Rectangle? visibleRectangle = null, Color? backgroundColor = null, GameTime gameTime = null);
    }
}