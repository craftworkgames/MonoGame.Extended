using System.ComponentModel;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Gui.Keepers;
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
            BackgroundColor = Color.White;
            TextColor = Color.White;
            IsEnabled = true;
            Children = new GuiControlCollection(this);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [JsonIgnore]
        public GuiControl Parent { get; internal set; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [JsonIgnore]
        public RectangleF BoundingRectangle => new RectangleF(Parent != null ? Parent.Position + Position : Position, Size);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public GuiThickness Margin { get; set; }

        public string Name { get; set; }
        public Vector2 Position { get; set; }
        public SizeF Size { get; set; }
        public Color BackgroundColor { get; set; }
        public TextureRegion2D BackgroundRegion { get; set; }
        public string Text { get; set; }
        public Color TextColor { get; set; }

        public GuiControlCollection Children { get; }

        private bool _isEnabled;
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                _isEnabled = value;
                SetStyle(!_isEnabled ? _disabledStyle : null);
            }
        }

        private GuiControlStyle _disabledStyle;
        public GuiControlStyle DisabledStyle
        {
            get { return _disabledStyle; }
            set
            {
                _disabledStyle = value;
                SetStyle(!_isEnabled ? _disabledStyle : null);
            }
        }

        public GuiControlStyle HoverStyle { get; set; }

        public virtual void MouseMoved(MouseEventArgs args)
        {
            if(IsEnabled)
                SetStyle(BoundingRectangle.Contains(args.Position) ? HoverStyle : null);
        }

        private GuiControlStyle _currentStyle;

        protected void SetStyle(GuiControlStyle style)
        {
            if (_currentStyle != style)
            {
                _currentStyle?.Revert(this);
                _currentStyle = style;
                _currentStyle?.Apply(this);
            }
        }


    }
}