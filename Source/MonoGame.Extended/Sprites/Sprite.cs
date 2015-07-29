using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoGame.Extended.Sprites
{
    public class Sprite 
    {
        public Texture2D Texture;
        public Vector2 Position;

        public Rectangle Bounds
        {
            get { return new Rectangle((int)Position.X, (int)Position.Y, Texture.Bounds.Width, Texture.Bounds.Height); }
        }

        public Sprite(Texture2D texture) 
        {
            Texture = texture;
        }

        public Sprite(Texture2D texture, Vector2 position) : this(texture)
        {
            Position = position;
        }
    }
}
