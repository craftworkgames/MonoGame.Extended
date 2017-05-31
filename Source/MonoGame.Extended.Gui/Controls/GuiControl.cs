using System;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Input.InputListeners;
using MonoGame.Extended.TextureAtlases;

namespace MonoGame.Extended.Gui.Controls
{
    public abstract class GuiControl : GuiElement<GuiControl>, IMovable, ISizable, IRectangular
    {
        protected GuiControl()
           : this(skin: null)
        {
        }

        protected GuiControl(GuiSkin skin)
        {
            Skin = skin;
            Color = Color.White;
            TextColor = Color.White;
            IsEnabled = true;
            IsVisible = true;
            Controls = new GuiControlCollection(this);
            Origin = Vector2.Zero;

            var style = skin?.GetStyle(GetType());
            style?.Apply(this);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public GuiSkin Skin { get; }

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

        public Vector2 Offset { get; set; }
        public BitmapFont Font { get; set; }
        public Color TextColor { get; set; }
        public Vector2 TextOffset { get; set; }
        public GuiControlCollection Controls { get; }
        public HorizontalAlignment HorizontalAlignment { get; set; } = HorizontalAlignment.Centre;
        public VerticalAlignment VerticalAlignment { get; set; } = VerticalAlignment.Centre;
        public HorizontalAlignment HorizontalTextAlignment { get; set; } = HorizontalAlignment.Centre;
        public VerticalAlignment VerticalTextAlignment { get; set; } = VerticalAlignment.Centre;

        private string _text;
        public string Text
        {
            get { return _text; }
            set
            {
                if (_text != value)
                {
                    _text = value;
                    OnTextChanged();
                }
            }
        }

        protected virtual void OnTextChanged()
        {
            TextChanged?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler TextChanged;

        public Size2 GetDesiredSize(IGuiContext context, Size2 availableSize)
        {
            var fixedSize = Size;
            var desiredSize = CalculateDesiredSize(context, availableSize);
            var ninePatch = BackgroundRegion as NinePatchRegion2D;

            if (ninePatch != null)
            {
                desiredSize.Width = Math.Max(desiredSize.Width, ninePatch.Padding.Size.Width);
                desiredSize.Height = Math.Max(desiredSize.Height, ninePatch.Padding.Size.Height);
            }
            else if (BackgroundRegion != null)
            {
                desiredSize.Width = Math.Max(desiredSize.Width, BackgroundRegion.Width);
                desiredSize.Height = Math.Max(desiredSize.Height, BackgroundRegion.Height);
            }

            var font = Font ?? context.DefaultFont;

            if (font != null && Text != null)
            {
                var textSize = font.MeasureString(Text);
                desiredSize.Width += textSize.Width;
                desiredSize.Height += textSize.Height;
            }

            desiredSize.Width = Math.Min(desiredSize.Width, availableSize.Width);
            desiredSize.Height = Math.Min(desiredSize.Height, availableSize.Height);

            // ReSharper disable CompareOfFloatsByEqualityOperator
            return new Size2(fixedSize.Width == 0 ? desiredSize.Width : fixedSize.Width, fixedSize.Height == 0 ? desiredSize.Height : fixedSize.Height);
            // ReSharper restore CompareOfFloatsByEqualityOperator
        }

        protected virtual Size2 CalculateDesiredSize(IGuiContext context, Size2 availableSize)
        {
            return Size2.Empty;
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

        public override void Draw(IGuiContext context, IGuiRenderer renderer, float deltaSeconds)
        {
            DrawBackground(context, renderer, deltaSeconds);
            DrawForeground(context, renderer, deltaSeconds, GetTextInfo(context, Text, BoundingRectangle, HorizontalTextAlignment, VerticalTextAlignment));
        }

        protected TextInfo GetTextInfo(IGuiContext context, string text, Rectangle targetRectangle, HorizontalAlignment horizontalAlignment, VerticalAlignment verticalAlignment, Rectangle? clippingRectangle = null)
        {
            var font = Font ?? context.DefaultFont;
            var textSize = font.GetStringRectangle(text ?? string.Empty, Vector2.Zero).Size;
            var destinationRectangle = GuiLayoutHelper.AlignRectangle(horizontalAlignment, verticalAlignment, textSize, targetRectangle);
            var textPosition = destinationRectangle.Location.ToVector2();
            var textInfo = new TextInfo(text, font, textPosition, textSize, TextColor, clippingRectangle ?? ClippingRectangle);
            return textInfo;
        }

        protected virtual void DrawBackground(IGuiContext context, IGuiRenderer renderer, float deltaSeconds)
        {
            if (BackgroundRegion != null)
                renderer.DrawRegion(BackgroundRegion, BoundingRectangle, Color);
            else
                renderer.FillRectangle(BoundingRectangle, Color);
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
                Text = text ?? string.Empty;
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