using Microsoft.Xna.Framework;
using MonoGame.Extended.TextureAtlases;

namespace MonoGame.Extended.Gui.Controls
{
    public abstract class GuiLayoutControl : GuiControl
    {
        protected GuiLayoutControl()
            : this(null)
        {
        }

        protected GuiLayoutControl(TextureRegion2D backgroundRegion) 
            : base(backgroundRegion)
        {
            Origin = Vector2.Zero;
        }

        public abstract void PerformLayout();


    }
}