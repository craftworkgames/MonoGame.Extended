using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.InputListeners;
using MonoGame.Extended.Shapes;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;

namespace MonoGame.Extended.Gui.Controls
{
    public abstract class GuiControl : IMovable, ISizable
    {
        private bool _isEnabled;

        private bool _isHovered;

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
        public Vector2 Center => new Vector2(Position.X + Width*0.5f, Position.Y + Height*0.5f);
        public RectangleF BoundingRectangle => new RectangleF(Left, Top, Width, Height);
        public bool IsFocused { get; internal set; }

        public TextureRegion2D BackgroundRegion { get; set; }
        public Color BackgroundColor { get; set; } = Color.White;
        public GuiHorizontalAlignment HorizontalAlignment { get; set; } = GuiHorizontalAlignment.Stretch;
        public GuiVerticalAlignment VerticalAlignment { get; set; } = GuiVerticalAlignment.Stretch;
        public GuiThickness Margin { get; set; }
        public GuiThickness Padding { get; set; }

        public BitmapFont Font { get; set; }
        public string Text { get; set; } = string.Empty;
        public Color TextColor { get; set; } = Color.White;
        public GuiHorizontalAlignment HorizontalTextAlignment { get; set; } = GuiHorizontalAlignment.Centre;
        public GuiVerticalAlignment VerticalTextAlignment { get; set; } = GuiVerticalAlignment.Centre;

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


        public GuiControlStyle DisabledStyle { get; set; }
        public GuiControlStyle HoverStyle { get; set; }
        public Vector2 Position { get; set; }
        public SizeF Size { get; set; }

        public void Draw(SpriteBatch spriteBatch)
        {
            var destinationRectangle = GetDestinationRectangle();

            DrawBackground(spriteBatch, destinationRectangle);
            DrawText(spriteBatch, destinationRectangle);
        }

        private void DrawText(SpriteBatch spriteBatch, Rectangle rectangle)
        {
            if (Font == null)
                return;

            var size = Font.MeasureString(Text);
            var sourceRectangle = new Rectangle(0, 0, size.Width, size.Height);
            var targetRectangle = rectangle;
            targetRectangle = new Rectangle(
                targetRectangle.X + Margin.Left,
                targetRectangle.Y + Margin.Top,
                targetRectangle.Width - Margin.Right - Margin.Left,
                targetRectangle.Height - Margin.Bottom - Margin.Top);
            var destinationRectangle = GuiAlignmentHelper.GetDestinationRectangle(
                HorizontalTextAlignment, VerticalTextAlignment, sourceRectangle, targetRectangle);

            spriteBatch.DrawString(Font, Text, destinationRectangle.Location.ToVector2(), TextColor*(TextColor.A/255f));
        }

        private void DrawBackground(SpriteBatch spriteBatch, Rectangle rectangle)
        {
            if (BackgroundRegion == null)
                return;

            var sourceRectangle = BackgroundRegion.Bounds;
            var targetRectangle = rectangle;
            var destinationRectangle = GuiAlignmentHelper.GetDestinationRectangle(
                HorizontalAlignment, VerticalAlignment, sourceRectangle, targetRectangle);

            var ninePatch = new NinePatch(BackgroundRegion, Padding.Left, Padding.Top, Padding.Right, Padding.Bottom)
            {
                Color = BackgroundColor*(BackgroundColor.A/255f)
            };
            ninePatch.Draw(spriteBatch, destinationRectangle);
        }

        private Rectangle GetDestinationRectangle()
        {
            //var size = ResizeToFit(TextureRegion.Size, new SizeF(Size.Width, Size.Height));
            //var x = Position.X + Size.Width * 0.5f - size.X * 0.5f;
            //var y = Position.Y + Size.Height * 0.5f - size.Y * 0.5f;
            //var controlRectangle = new Rectangle((int)x, (int)y, size.X, size.Y);
            //return controlRectangle;
            return new Rectangle((int) Position.X, (int) Position.Y, (int) Size.Width, (int) Size.Height);
        }

        //private static Point ResizeToFit(Size imageSize, SizeF boxSize)
        //{
        //    var widthScale = boxSize.Width / imageSize.Width;
        //    var heightScale = boxSize.Height / imageSize.Height;
        //    var scale = Math.Min(widthScale, heightScale);
        //    return new Point((int)Math.Round(imageSize.Width * scale), (int)Math.Round(imageSize.Height * scale));
        //}

        public event EventHandler<MouseEventArgs> MouseDown;
        public event EventHandler<MouseEventArgs> MouseUp;

        public virtual void Update(GameTime gameTime)
        {
        }

        public virtual void OnMouseDown(object sender, MouseEventArgs args)
        {
            MouseDown.Raise(this, args);
        }

        public virtual void OnMouseUp(object sender, MouseEventArgs args)
        {
            MouseUp.Raise(this, args);
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