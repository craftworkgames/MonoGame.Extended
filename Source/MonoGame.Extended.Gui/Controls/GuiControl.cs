using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.InputListeners;
using MonoGame.Extended.Shapes;
using MonoGame.Extended.TextureAtlases;

namespace MonoGame.Extended.Gui.Controls
{
    public abstract class GuiControl : IMovable, ISizable
    {
        protected GuiControl()
        {
            IsEnabled = true;
        }

        public string Name { get; set; }
        public float Left => Position.X;
        public float Top => Position.Y;
        public float Right => Position.X + Width;
        public float Bottom => Position.Y + Height;
        public float Width => Size.Width;
        public float Height => Size.Height;
        public Vector2 Center => new Vector2(Position.X + Width * 0.5f, Position.Y + Height * 0.5f);
        public RectangleF BoundingRectangle => new RectangleF(Left, Top, Width, Height);
        public bool IsFocused { get; internal set; }
        public TextureRegion2D BackgroundRegion { get; set; }
        public Color BackgroundColor { get; set; } = Color.White;
        public GuiHorizontalAlignment HorizontalAlignment { get; set; } = GuiHorizontalAlignment.Stretch;
        public GuiVerticalAlignment VerticalAlignment { get; set; } = GuiVerticalAlignment.Stretch;
        public GuiThickness Margin { get; set; }
        public GuiThickness Padding { get; set; }
        public GuiControlStyle DisabledStyle { get; set; }
        public GuiControlStyle HoverStyle { get; set; }
        public Vector2 Position { get; set; }
        public SizeF Size { get; set; }
        public GuiControl Parent { get; set; }
        public BitmapFont Font { get; set; }
        public string Text { get; set; } = string.Empty;
        public Color TextColor { get; set; } = Color.White;
        public GuiHorizontalAlignment HorizontalTextAlignment { get; set; } = GuiHorizontalAlignment.Centre;
        public GuiVerticalAlignment VerticalTextAlignment { get; set; } = GuiVerticalAlignment.Centre;

        public Rectangle DestinationRectangle
        {
            get
            {
                var x = (int) Position.X;
                var y = (int) Position.Y;

                if (Parent != null)
                {
                    var parentRectangle = Parent.DestinationRectangle;
                    x += parentRectangle.X;
                    y += parentRectangle.Y;
                }

                return new Rectangle(x, y, (int)Size.Width, (int)Size.Height);
            }
        }

        private bool _isHovered;
        public bool IsHovered
        {
            get { return _isHovered; }
            private set
            {
                if (_isHovered != value)
                {
                    _isHovered = value;

                    if (_isHovered)
                        HoverStyle?.Apply(this);
                    else
                        HoverStyle?.Revert(this);
                }
            }
        }

        private bool _isEnabled;
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                if (_isEnabled != value)
                {
                    _isEnabled = value;

                    if (!_isEnabled)
                        DisabledStyle?.Apply(this);
                    else
                        DisabledStyle?.Revert(this);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {

        }

        public event EventHandler<MouseEventArgs> MouseDown;
        public event EventHandler<MouseEventArgs> MouseUp;

        public virtual void Update(GameTime gameTime)
        {
        }

        public virtual void OnMouseDown(object sender, MouseEventArgs args)
        {
            MouseDown?.Invoke(this, args);
        }

        public virtual void OnMouseUp(object sender, MouseEventArgs args)
        {
            MouseUp?.Invoke(this, args);
        }

        public virtual void OnKeyTyped(object sender, KeyboardEventArgs args)
        {
        }

        public virtual void OnMouseEnter(object sender, MouseEventArgs args)
        {
            if (IsEnabled)
                IsHovered = true;
        }

        public virtual void OnMouseLeave(object sender, MouseEventArgs args)
        {
            if (IsEnabled)
                IsHovered = false;
        }

        public bool Contains(Vector2 point)
        {
            return BoundingRectangle.Contains(point);
        }

        public bool Contains(Point point)
        {
            return BoundingRectangle.Contains(point);
        }
    }
}