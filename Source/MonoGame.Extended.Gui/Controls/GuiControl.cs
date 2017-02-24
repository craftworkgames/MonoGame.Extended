using System.ComponentModel;
using Microsoft.Xna.Framework;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.InputListeners;
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
            Origin = Vector2.One * 0.5f;
        }

        protected GuiControl(TextureRegion2D textureRegion)
            : this()
        {
            TextureRegion = textureRegion;
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
                var position = Parent != null ? Parent.Position - Parent.Size * Parent.Origin + Position : Position;
                return new Rectangle((position - Size * Origin).ToPoint(), (Point)Size);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public Thickness Margin { get; set; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public Thickness Padding { get; set; }
        
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsFocused { get; set; }

        public string Name { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 Origin { get; set; }

        public Size2 Size { get; set; }
        public Color Color { get; set; }

        public Rectangle ClippingRectangle
        {
            get
            {
                var r = BoundingRectangle;
                return new Rectangle(r.Left + Padding.Left, r.Top + Padding.Top, 
                    r.Width - Padding.Right - Padding.Left, r.Height - Padding.Bottom - Padding.Top);
            }
        }

        private TextureRegion2D _textureRegion;
        public TextureRegion2D TextureRegion
        {
            get { return _textureRegion; }
            set
            {
                if (_textureRegion != value)
                {
                    // if this is the first time a texture region has been set and this control has no size, 
                    // use the size of the texture region
                    if (_textureRegion == null && value != null && Size.IsEmpty)
                        Size = value.Size;

                    _textureRegion = value;
                }
            }
        }

        public BitmapFont Font { get; set; }
        public string Text { get; set; }
        public Color TextColor { get; set; }
        public Vector2 TextOffset { get; set; }
        
        public GuiControlCollection Controls { get; }

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

        public virtual void OnMouseDown(MouseEventArgs args) { }
        public virtual void OnMouseUp(MouseEventArgs args) { }
        public virtual void OnKeyTyped(KeyboardEventArgs args) { }
        public virtual void OnKeyPressed(KeyboardEventArgs args) { }
        
        public virtual void OnMouseEnter(MouseEventArgs args)
        {
            if (IsEnabled)
                HoverStyle?.Apply(this);
        }

        public virtual void OnMouseLeave(MouseEventArgs args)
        {
            if (IsEnabled)
                HoverStyle?.Revert(this);
        }

        public void Draw(IGuiRenderer renderer, float deltaSeconds)
        {
            DrawBackground(renderer, deltaSeconds);

            var font = Font ?? renderer.DefaultFont;
            var textSize = font.GetStringRectangle(Text ?? string.Empty, Vector2.Zero).Size.ToVector2();
            var textPosition = BoundingRectangle.Center.ToVector2() - textSize * 0.5f;
            var textInfo = new TextInfo(Text, font, textPosition, textSize, TextColor, ClippingRectangle);

            DrawText(renderer, deltaSeconds, textInfo);
        }

        protected virtual void DrawBackground(IGuiRenderer renderer, float deltaSeconds)
        {
            renderer.DrawRegion(TextureRegion, BoundingRectangle, Color);
        }

        protected virtual void DrawText(IGuiRenderer renderer, float deltaSeconds, TextInfo textInfo)
        {
            if (!string.IsNullOrWhiteSpace(textInfo.Text))
                renderer.DrawText(textInfo.Font, textInfo.Text, textInfo.Position, textInfo.Color, textInfo.ClippingRectangle);
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