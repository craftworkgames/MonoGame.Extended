using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics;

public class TextureRegion
{
    public string Name { get; }
    public Texture2D Texture { get; }
    public int X { get; }
    public int Y { get; }
    public int Width { get; }
    public int Height { get; }
    public Size Size  { get; }
    public object Tag { get; set; }
    public Rectangle Bounds { get; }
    public float TopUV {get;}
    public float RightUV {get;}
    public float BottomUV {get;}
    public float LeftUV {get;}

    public TextureRegion(Texture2D texture)
        : this(texture, null, 0, 0, texture.Width, texture.Height) { }

    public TextureRegion(Texture2D texture, string name)
        : this(texture, name, texture.Bounds.X, texture.Bounds.Y, texture.Bounds.Width, texture.Bounds.Height) { }

    public TextureRegion(Texture2D texture, Rectangle region)
        : this(texture, null, region.X, region.Y, region.Width, region.Height) { }

    public TextureRegion(Texture2D texture, int x, int y, int width, int height)
        : this(texture, null, x, y, width, height) { }

    public TextureRegion(Texture2D texture, string name, Rectangle region)
        : this (texture, name, region.X, region.Y, region.Width, region.Height) { }

    public TextureRegion(Texture2D texture, string name, int x, int y, int width, int height)
    {
        ArgumentNullException.ThrowIfNull(texture);
        if(texture.IsDisposed)
        {
            throw new ObjectDisposedException(nameof(texture));
        }

        if(string.IsNullOrEmpty(name))
        {
            name = $"{texture.Name}({x}, {y}, {width}, {height})";
        }

        Name = name;
        Texture = texture;
        X = x;
        Y = y;
        Width = width;
        Height = height;
        Bounds = new Rectangle(x, y, width, height);
        Size = new Size(width, height);
        TopUV = Bounds.Top / (float)texture.Height;
        RightUV = Bounds.Right / (float)texture.Width;
        BottomUV = Bounds.Bottom / (float)texture.Height;
        LeftUV = Bounds.Left / (float)texture.Width;
    }

    public override string ToString()
    {
        return $"{Name ?? string.Empty} {Bounds}";
    }
}
