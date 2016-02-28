using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Maps.Tiled
{
    public abstract class TiledLayer
    {
        protected TiledLayer(string name)
        {
            Name = name;
            Visible = true;
            Properties = new TiledProperties();
        }

        public string Name { get; private set; }
        public bool Visible { get; set; }
        public TiledProperties Properties { get; private set; }

        public abstract void Draw(SpriteBatch spriteBatch, Rectangle visibleRectangle);
    }
}