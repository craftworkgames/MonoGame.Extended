using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Gui.Controls;
using MonoGame.Extended.Shapes;

namespace MonoGame.Extended.Gui.Styles
{
    public interface IGuiDrawable
    {
        Size Size { get; }
        void Draw(SpriteBatch spriteBatch, Rectangle bounds);
    }

    public abstract class GuiControlStyle<T>
        where T : GuiControl
    {
        protected abstract IGuiDrawable GetCurrentDrawable(T control);

        protected virtual Size GetDesiredSize(T control)
        {
            var drawable = GetCurrentDrawable(control);
            return drawable.Size;
        }

        public virtual void Draw(T control, SpriteBatch spriteBatch, Rectangle rectangle)
        {
            var drawable = GetCurrentDrawable(control);
            drawable.Draw(spriteBatch, rectangle);
        }
    }
}