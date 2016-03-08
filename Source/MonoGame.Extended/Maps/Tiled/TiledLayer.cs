using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Maps.Tiled
{
    public abstract class TiledLayer : IDisposable
    {
        public bool IsVisible { get; set; }

        public string Name { get; }
        public float Opacity { get; set; }
        public TiledProperties Properties { get; }

        protected TiledLayer(string name)
        {
            Name = name;
            Properties = new TiledProperties();
            IsVisible = true;
            Opacity = 1.0f;
        }

        public abstract void Dispose();

        public abstract void Draw(SpriteBatch spriteBatch, Rectangle? visibleRectangle = null, Color? backgroundColor = null);
    }
}