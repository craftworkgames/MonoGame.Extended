using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Shapes;

namespace MonoGame.Extended.Gui.Wip
{
    public interface IGuiDrawable
    {
        void Draw(SpriteBatch spriteBatch, RectangleF rectangle);
    }
}