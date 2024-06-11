﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics
{
    public class TextureRegion
    {
        public string Name { get; }
        public Texture2D Texture { get; protected set; }
        public int X { get; }
        public int Y { get; }
        public int Width { get; }
        public int Height { get; }
        public SizeF Size => new SizeF(Width, Height);
        public object Tag { get; set; }
        public Rectangle Bounds => new Rectangle(X, Y, Width, Height);

        public TextureRegion(Texture2D texture, int x, int y, int width, int height)
            : this(null, texture, x, y, width, height)
        {
        }

        public TextureRegion(Texture2D texture, Rectangle region)
            : this(null, texture, region.X, region.Y, region.Width, region.Height)
        {
        }

        public TextureRegion(string name, Texture2D texture, Rectangle region)
            : this(name, texture, region.X, region.Y, region.Width, region.Height)
        {
        }

        public TextureRegion(Texture2D texture)
            : this(texture.Name, texture, 0, 0, texture.Width, texture.Height)
        {
        }

        public TextureRegion(string name, Texture2D texture, int x, int y, int width, int height)
        {
            Name = name;
            Texture = texture;
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public override string ToString()
        {
            return $"{Name ?? string.Empty} {Bounds}";
        }
    }
}
