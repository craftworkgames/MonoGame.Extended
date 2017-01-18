#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices.ComTypes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended.Shapes;

#endregion

namespace MonoGame.Extended.Tiled
{
    public abstract class TiledMapObject
    {
        public int Identifier { get; }
        public string Name { get; set; }
        public string Type { get; }
        public TiledMapProperties Properties { get; }
        public bool IsVisible { get; set; }
        public float Opacity { get; set; }
        public float Rotation { get; set; }
        public Vector2 Position { get; }
        public IShapeF Shape { get; protected set; }
        public float Width { get; }
        public float Height { get; }
        public SizeF Size => Shape.BoundingRectangle.Size;
        public RectangleF BoundingRectangle => Shape.BoundingRectangle;

        internal TiledMapObject(ContentReader input)
        {
            Identifier = input.ReadInt32();
            Name = input.ReadString();
            Type = input.ReadString();
            Position = new Vector2(input.ReadSingle(), input.ReadSingle());
            Width = input.ReadSingle();
            Height= input.ReadSingle();
            Rotation = input.ReadSingle();
            IsVisible = input.ReadBoolean();

            Properties = new TiledMapProperties();
            input.ReadTiledMapProperties(Properties);
        }

        public override string ToString()
        {
            return $"{Identifier}";
        }
    }
}