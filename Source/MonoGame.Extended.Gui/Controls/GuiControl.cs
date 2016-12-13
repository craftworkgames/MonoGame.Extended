using System.ComponentModel;
using Microsoft.Xna.Framework;
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
    }
}