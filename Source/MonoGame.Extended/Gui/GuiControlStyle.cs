using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Shapes;

namespace MonoGame.Extended.Gui
{
    public abstract class GuiControlStyle
    {
        public abstract IShapeF BoundingShape { get; }
        public abstract void Draw(GuiDrawableControl control, SpriteBatch spriteBatch);
    }
}