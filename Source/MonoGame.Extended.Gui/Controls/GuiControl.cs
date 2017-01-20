using System.ComponentModel;
using Microsoft.Xna.Framework;
using MonoGame.Extended.InputListeners;
using MonoGame.Extended.SceneGraphs;
using MonoGame.Extended.Shapes;
using MonoGame.Extended.TextureAtlases;
using Newtonsoft.Json;

namespace MonoGame.Extended.Gui.Controls
{
    public abstract class GuiControl : ISceneEntity, IMovable, ISizable
    {
        protected GuiControl()
        {
            Color = Color.White;
            TextColor = Color.White;
            IsEnabled = true;
            Children = new GuiControlCollection(this);
            Origin = Vector2.One*0.5f;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [JsonIgnore]
        public GuiControl Parent { get; internal set; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [JsonIgnore]
        public RectangleF BoundingRectangle
            => new RectangleF((Parent != null ? Parent.Position + Position : Position) - Size * Origin, Size);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public GuiThickness Margin { get; set; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsFocused { get; set; }

        public string Name { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 Origin { get; set; }

        public Size2 Size { get; set; }
        public Color Color { get; set; }
        public TextureRegion2D TextureRegion { get; set; }

        public string Text { get; set; }
        public Color TextColor { get; set; }
        public Vector2 TextOffset { get; set; }

        public GuiControlCollection Children { get; }

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

        public virtual void OnMouseEnter(MouseEventArgs args)
        {
            if(IsEnabled)
                HoverStyle?.Apply(this);
        }

        public virtual void OnMouseLeave(MouseEventArgs args)
        {
            if(IsEnabled)
                HoverStyle?.Revert(this);
        }
    }
}