using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Maps.Tiled
{
    public abstract class TiledLayer : IDisposable
    {
        protected TiledLayer(string name)
        {
            Name = name;
            Properties = new TiledProperties();
            IsVisible = true;
        }

        public abstract void Dispose();

        public bool IsVisible { get; set; }
        public string Name { get; }
        public TiledProperties Properties { get; }

        public abstract void Draw(SpriteBatch spriteBatch, Rectangle? visibleRectangle = null, Color? backgroundColor = null);
    }
}