using System.ComponentModel;
using Microsoft.Xna.Framework;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Input.InputListeners;
using MonoGame.Extended.TextureAtlases;
using Newtonsoft.Json;

namespace MonoGame.Extended.Gui.Controls
{
    public abstract class GuiControl : IMovable, ISizable
    {
        protected GuiControl()
        {
            Color = Color.White;
            TextColor = Color.White;
            IsEnabled = true;
            IsVisible = true;
            Controls = new GuiControlCollection(this);
            Origin = Vector2.Zero;
        }

        protected GuiControl(TextureRegion2D backgroundRegion)
            : this()
        {
            BackgroundRegion = backgroundRegion;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [JsonIgnore]
        public GuiControl Parent { get; internal set; }

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

        [EditorBrowsable(EditorBrowsableState.Never)]
        public Thickness Margin { get; set; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public Thickness ClipPadding { get; set; }

        public Rectangle ClippingRectangle
        {
            get
            {
                var r = BoundingRectangle;
                return new Rectangle(r.Left + ClipPadding.Left, r.Top + ClipPadding.Top,
                    r.Width - ClipPadding.Right - ClipPadding.Left, r.Height - ClipPadding.Bottom - ClipPadding.Top);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsFocused { get; set; }

        public string Name { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 Offset { get; set; }
        public Vector2 Origin { get; set; }
        public Color Color { get; set; }
        public BitmapFont Font { get; set; }
        public string Text { get; set; }
        public Color TextColor { get; set; }
        public Vector2 TextOffset { get; set; }
        public GuiControlCollection Controls { get; }
        public HorizontalAlignment HorizontalAlignment { get; set; } = HorizontalAlignment.Stretch;
        public VerticalAlignment VerticalAlignment { get; set; } = VerticalAlignment.Stretch;
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

        public Size2 GetDesiredSize(IGuiContext context, Size2 availableSize)
        {
            return CalculateDesiredSize(context, availableSize);
        }

        protected virtual Size2 CalculateDesiredSize(IGuiContext context, Size2 availableSize)
        {
            var minimumSize = Size2.Empty;
            var ninePatch = BackgroundRegion as NinePatchRegion2D;

            if (ninePatch != null)
            {
                minimumSize.Width += ninePatch.LeftPadding + ninePatch.RightPadding;
                minimumSize.Height += ninePatch.TopPadding + ninePatch.BottomPadding;
            }
            else if(BackgroundRegion != null)
            {
                minimumSize.Width += BackgroundRegion.Width;
                minimumSize.Height += BackgroundRegion.Height;
            }

            var font = Font ?? context.DefaultFont;

            if (font != null && !string.IsNullOrEmpty(Text))
            {
                var textSize = font.MeasureString(Text);
                minimumSize.Width += textSize.Width;
                minimumSize.Height += textSize.Height;
            }

            // ReSharper disable CompareOfFloatsByEqualityOperator
            return new Size2(Size.Width == 0 ? minimumSize.Width : Size.Width, Size.Height == 0 ? minimumSize.Height : Size.Height);
            // ReSharper restore CompareOfFloatsByEqualityOperator
        }

        private bool _isEnabled;
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                _isEnabled = value;
                DisabledStyle?.ApplyIf(this, !_isEnabled);
            }
        }

        public bool IsVisible { get; set; }
        public GuiControlStyle HoverStyle { get; set; }

        private GuiControlStyle _disabledStyle;
        public GuiControlStyle DisabledStyle
        {
            get { return _disabledStyle; }
            set
            {
                _disabledStyle = value;
                DisabledStyle?.ApplyIf(this, !_isEnabled);
            }
        }

        public virtual void OnScrolled(int delta) { }

        public virtual void OnKeyTyped(IGuiContext context, KeyboardEventArgs args) { }
        public virtual void OnKeyPressed(IGuiContext context, KeyboardEventArgs args) { }

        public virtual void OnPointerDown(IGuiContext context, GuiPointerEventArgs args) { }
        public virtual void OnPointerUp(IGuiContext context, GuiPointerEventArgs args) { }
        
        public virtual void OnPointerEnter(IGuiContext context, GuiPointerEventArgs args)
        {
            if (IsEnabled)
                HoverStyle?.Apply(this);
        }

        public virtual void OnPointerLeave(IGuiContext context, GuiPointerEventArgs args)
        {
            if (IsEnabled)
                HoverStyle?.Revert(this);
        }

        public void Draw(IGuiContext context, IGuiRenderer renderer, float deltaSeconds)
        {
            DrawBackground(context, renderer, deltaSeconds);
            DrawForeground(context, renderer, deltaSeconds, GetTextInfo(context, Text, BoundingRectangle, HorizontalAlignment.Centre, VerticalAlignment.Centre));
        }

        protected TextInfo GetTextInfo(IGuiContext context, string text, Rectangle targetRectangle, HorizontalAlignment horizontalAlignment, VerticalAlignment verticalAlignment)
        {
            var font = Font ?? context.DefaultFont;
            var textSize = font.GetStringRectangle(text ?? string.Empty, Vector2.Zero).Size;
            var destinationRectangle = GuiLayoutHelper.GetDestinationRectangle(horizontalAlignment, verticalAlignment, textSize, targetRectangle);
            var textPosition = destinationRectangle.Location.ToVector2();
            var textInfo = new TextInfo(text, font, textPosition, textSize, TextColor, ClippingRectangle);
            return textInfo;
        }

        protected virtual void DrawBackground(IGuiContext context, IGuiRenderer renderer, float deltaSeconds)
        {
            renderer.DrawRegion(BackgroundRegion, BoundingRectangle, Color);
            //renderer.DrawRectangle(BoundingRectangle, Color.Red);
        }

        protected virtual void DrawForeground(IGuiContext context, IGuiRenderer renderer, float deltaSeconds, TextInfo textInfo)
        {
            if (!string.IsNullOrWhiteSpace(textInfo.Text))
                renderer.DrawText(textInfo.Font, textInfo.Text, textInfo.Position + TextOffset, textInfo.Color, textInfo.ClippingRectangle);
        }

        protected struct TextInfo
        {
            public TextInfo(string text, BitmapFont font, Vector2 position, Vector2 size, Color color, Rectangle? clippingRectangle)
            {
                Text = text;
                Font = font;
                Size = size;
                Color = color;
                ClippingRectangle = clippingRectangle;
                Position = position;
            }

            public string Text;
            public BitmapFont Font;
            public Vector2 Size;
            public Color Color;
            public Rectangle? ClippingRectangle;
            public Vector2 Position;
        }
    }
}