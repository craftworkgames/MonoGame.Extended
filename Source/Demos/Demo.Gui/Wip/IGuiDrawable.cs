using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Shapes;

namespace Demo.Gui.Wip
{
    public interface IGuiDrawable
    {
        void Draw(SpriteBatch spriteBatch, RectangleF rectangle);
    }
}