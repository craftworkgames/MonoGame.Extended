using System.ComponentModel;
using Microsoft.Xna.Framework;
using MonoGame.Extended.TextureAtlases;
using Newtonsoft.Json;

namespace MonoGame.Extended.Gui
{
    public abstract class GuiElement<TParent> : IRectangular
        where TParent : IRectangular
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        [JsonIgnore]
        public TParent Parent { get; internal set; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [JsonIgnore]
        public Rectangle BoundingRectangle
        {
            get
            {
                var offset = Vector2.Zero;

                if (Parent != null)
                    offset = Parent.BoundingRectangle.Location.ToVector2();

                return new Rectangle((offset + Position - Size * Origin).ToPoint(), (Point)Size);
            }
        }

        public string Name { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 Origin { get; set; }
        public Color Color { get; set; }
        public TextureRegion2D BackgroundRegion { get; set; }
        public Size2 Size { get; set; }

        public float Width
        {
            get { return Size.Width; }
            set { Size = new Size2(value, Size.Height); }
        }

        public float Height
        {
            get { return Size.Height; }
            set { Size = new Size2(Size.Width, value); }
        }

        public abstract void Draw(IGuiContext context, IGuiRenderer renderer, float deltaSeconds);
    }
}