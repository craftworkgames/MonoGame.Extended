using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Shapes;

namespace MonoGame.Extended.Gui.Drawables
{
    public interface IGuiDrawable
    {
        void Draw(SpriteBatch spriteBatch, RectangleF rectangle);
    }
}