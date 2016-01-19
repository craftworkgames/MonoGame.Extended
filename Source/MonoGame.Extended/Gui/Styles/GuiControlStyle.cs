using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Gui.Controls;
using MonoGame.Extended.Shapes;

namespace MonoGame.Extended.Gui.Styles
{
    public interface IGuiDrawable
    {
        void Draw(SpriteBatch spriteBatch, Rectangle bounds);
    }

    public abstract class GuiControlStyle
    {
        public abstract IShapeF BoundingShape { get; }
        public abstract void Draw(GuiControl control, SpriteBatch spriteBatch);
    }
}