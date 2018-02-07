using System;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Input.InputListeners;
using MonoGame.Extended.TextureAtlases;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

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
            Controls = new GuiControlCollection(this)
            {
                ItemAdded = x => UpdateRootIsLayoutRequired(),
                ItemRemoved = x => UpdateRootIsLayoutRequired()
            };
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
        public GuiControlCollection Controls { get; }
        public HorizontalAlignment HorizontalAlignment { get; set; } = HorizontalAlignment.Centre;
        public VerticalAlignment VerticalAlignment { get; set; } = VerticalAlignment.Centre;
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

            var guiButton = this as GuiButton;
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

        public virtual bool OnKeyTyped(IGuiContext context, KeyboardEventArgs args) { return true; }
        public virtual bool OnKeyPressed(IGuiContext context, KeyboardEventArgs args) { return true; }

        public virtual bool OnFocus(IGuiContext context) { return true; }
        public virtual bool OnUnfocus(IGuiContext context) { return true; }

        public virtual bool OnPointerDown(IGuiContext context, GuiPointerEventArgs args) { return true; }
        public virtual bool OnPointerMove(IGuiContext context, GuiPointerEventArgs args) { return true; }
        public virtual bool OnPointerUp(IGuiContext context, GuiPointerEventArgs args) { return true; }
        
        public virtual bool OnPointerEnter(IGuiContext context, GuiPointerEventArgs args)
        {
            if (IsEnabled && !_isHovering)
            {
                _isHovering = true;
                HoverStyle?.Apply(this);
            }
            return true;
        }

        public virtual bool OnPointerLeave(IGuiContext context, GuiPointerEventArgs args)
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

        public bool HasParent(GuiControl control)
        {
            return Parent != null && (Parent == control || Parent.HasParent(control));
        }

        public override void SetBinding(string viewProperty, string viewModelProperty)
        {
            SetBindingToRoot(new Binding
            {
                Element = this,
                ViewProperty = viewProperty,
                ViewModelProperty = viewModelProperty
            });
        }

        private void SetBindingToRoot(Binding binding)
        {
            if (Parent != null)
            {
                Parent.SetBindingToRoot(binding);
            }
            else
            {
                _bindings.Add(binding);
            }
        }

        protected List<T> FindControls<T>()
            where T : GuiControl
        {
            return FindControls<T>(Controls);
        }

        protected List<T> FindControls<T>(GuiControlCollection controls)
            where T : GuiControl
        {
            var results = new List<T>();
            foreach (var control in controls)
            {
                if (control is T) results.Add(control as T);
                if (control.Controls.Any()) results = results.Concat(FindControls<T>(control.Controls)).ToList();
            }
            return results;
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

        /// <summary>
        /// Recursive Method to find the root element and update the IsLayoutRequired property.  So that the screen knows that something in the controls
        /// have had a change to their layout.  Also, it will reset the size of the element so that it can get a clean build so that the background patches
        /// can be rendered with the updates.
        /// </summary>
        private void UpdateRootIsLayoutRequired()
        {
            if (Parent == null) IsLayoutRequired = true;
            else Parent.UpdateRootIsLayoutRequired();

            Size = Size2.Empty;
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