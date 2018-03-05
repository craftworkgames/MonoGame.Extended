using System;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Input.InputListeners;
using MonoGame.Extended.TextureAtlases;
using Newtonsoft.Json;

namespace MonoGame.Extended.Gui.Controls
{
    public abstract class Control : Element<Control>, IMovable, ISizable, IRectangular
    {
        protected Control()
        {
            Color = Color.White;
            TextColor = Color.White;
            IsEnabled = true;
            IsVisible = true;

            Origin = Vector2.Zero;
        }

        private Skin _skin;

        [EditorBrowsable(EditorBrowsableState.Never)]
        public Skin Skin
        {
            get { return _skin; }
            set
            {
                if (_skin != value)
                {
                    _skin = value;
                    _skin?.GetStyle(GetType())?.Apply(this);
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public Thickness Margin { get; set; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public Thickness ClipPadding { get; set; }

        public bool IsLayoutRequired { get; set; }

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

        [JsonIgnore]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Vector2 Offset { get; set; }
        public BitmapFont Font { get; set; }
        public Color TextColor { get; set; }
        public Vector2 TextOffset { get; set; }
        public HorizontalAlignment HorizontalAlignment { get; set; } = HorizontalAlignment.Stretch;
        public VerticalAlignment VerticalAlignment { get; set; } = VerticalAlignment.Stretch;
        public HorizontalAlignment HorizontalTextAlignment { get; set; } = HorizontalAlignment.Centre;
        public VerticalAlignment VerticalTextAlignment { get; set; } = VerticalAlignment.Centre;

        private bool _isHovering;

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
            var fontSize = Size2.Empty;

            if (font != null && Text != null)
            {
                var textSize = font.MeasureString(CreateBoxText(Text, font, Width));
                fontSize.Width += textSize.Width;
                fontSize.Height += textSize.Height;
            }

            var buttonSize = Size2.Empty;

            var guiButton = this as Button;
            if (guiButton?.IconRegion != null)
            {
                buttonSize = guiButton.IconRegion.Size;
            }

            desiredSize.Width += Math.Max(fontSize.Width, buttonSize.Width);
            desiredSize.Height += Math.Max(fontSize.Height, buttonSize.Height);

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
                if (_isEnabled != value)
                {
                    _isEnabled = value;
                    DisabledStyle?.ApplyIf(this, !_isEnabled);
                }
            }
        }

        public bool IsVisible { get; set; }
        public ControlStyle HoverStyle { get; set; }

        private ControlStyle _disabledStyle;
        public ControlStyle DisabledStyle
        {
            get { return _disabledStyle; }
            set
            {
                _disabledStyle = value;
                DisabledStyle?.ApplyIf(this, !_isEnabled);
            }
        }

        public virtual void OnScrolled(int delta) { }

        public virtual bool OnKeyTyped(IGuiContext context, KeyboardEventArgs args) { return true; }
        public virtual bool OnKeyPressed(IGuiContext context, KeyboardEventArgs args) { return true; }

        public virtual bool OnFocus(IGuiContext context) { return true; }
        public virtual bool OnUnfocus(IGuiContext context) { return true; }

        public virtual bool OnPointerDown(IGuiContext context, PointerEventArgs args) { return true; }
        public virtual bool OnPointerMove(IGuiContext context, PointerEventArgs args) { return true; }
        public virtual bool OnPointerUp(IGuiContext context, PointerEventArgs args) { return true; }
        
        public virtual bool OnPointerEnter(IGuiContext context, PointerEventArgs args)
        {
            if (IsEnabled && !_isHovering)
            {
                _isHovering = true;
                HoverStyle?.Apply(this);
            }
            return true;
        }

        public virtual bool OnPointerLeave(IGuiContext context, PointerEventArgs args)
        {
            if (IsEnabled && _isHovering)
            {
                _isHovering = false;
                HoverStyle?.Revert(this);
            }
            return true;
        }

        public virtual bool Contains(IGuiContext context, Point point)
        {
            return BoundingRectangle.Contains(point);
        }

        public override void Draw(IGuiContext context, IGuiRenderer renderer, float deltaSeconds)
        {
            DrawBackground(context, renderer, deltaSeconds);
            DrawForeground(context, renderer, deltaSeconds, GetTextInfo(context, CreateBoxText(Text, Font ?? context.DefaultFont, Width), BoundingRectangle, HorizontalTextAlignment, VerticalTextAlignment));
        }

        public bool HasParent(Control control)
        {
            return Parent != null && (Parent == control || Parent.HasParent(control));
        }



        protected TextInfo GetTextInfo(IGuiContext context, string text, Rectangle targetRectangle, HorizontalAlignment horizontalAlignment, VerticalAlignment verticalAlignment, Rectangle? clippingRectangle = null)
        {
            var font = Font ?? context.DefaultFont;
            var textSize = font.GetStringRectangle(text ?? string.Empty, Vector2.Zero).Size;
            var destinationRectangle = LayoutHelper.AlignRectangle(horizontalAlignment, verticalAlignment, textSize, targetRectangle);
            var textPosition = destinationRectangle.Location.ToVector2();
            var textInfo = new TextInfo(text, font, textPosition, textSize, TextColor, clippingRectangle ?? ClippingRectangle);
            return textInfo;
        }

        protected virtual void DrawBackground(IGuiContext context, IGuiRenderer renderer, float deltaSeconds)
        {
            if (BackgroundRegion != null)
                renderer.DrawRegion(BackgroundRegion, BoundingRectangle, Color);
            else if(Color != Color.Transparent)
                renderer.FillRectangle(BoundingRectangle, Color);

            if(BorderThickness != 0)
                renderer.DrawRectangle(BoundingRectangle, BorderColor, BorderThickness);
        }

        protected virtual void DrawForeground(IGuiContext context, IGuiRenderer renderer, float deltaSeconds, TextInfo textInfo)
        {
            if (!string.IsNullOrWhiteSpace(textInfo.Text))
                renderer.DrawText(textInfo.Font, textInfo.Text, textInfo.Position + TextOffset, textInfo.Color, textInfo.ClippingRectangle);
        }

        protected virtual string CreateBoxText(string text, BitmapFont font, float width)
        {
            if (string.IsNullOrEmpty(text) || width <= 0.0f) return text;
            var words = text.Split(' ');
            var currentWidth = 0;

            var blockText = string.Empty;
            foreach (var word in words)
            {
                var spaceMeasurement = font.MeasureString($" {word}");
                currentWidth += (int)spaceMeasurement.Width;

                if (currentWidth > width)
                {
                    var measurement = font.MeasureString(word);
                    blockText += string.IsNullOrEmpty(blockText) ? word : $"\n{word}";
                    currentWidth = (int)measurement.Width;
                }
                else
                {
                    blockText += string.IsNullOrEmpty(blockText) ? word : $" {word}";
                }
            }
            return blockText;
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