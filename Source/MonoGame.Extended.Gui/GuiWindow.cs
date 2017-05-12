using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Gui.Controls;

namespace MonoGame.Extended.Gui
{
    public class GuiWindow : GuiElement<GuiScreen>
    {
        public GuiWindow(GuiScreen parent)
        {
            Parent = parent;
        }

        public bool IsInitialized { get; private set; }
        public GuiSkin Skin => Parent.Skin;
        public GuiControlCollection Controls { get; } = new GuiControlCollection();

        protected virtual void Initialize()
        {
        }

        public void Show()
        {
            if (!IsInitialized)
            {
                Initialize();
                IsInitialized = true;
            }

            Parent.Windows.Add(this);
        }

        public void Hide()
        {
            Parent.Windows.Remove(this);
        }

        public override void Draw(IGuiContext context, IGuiRenderer renderer, float deltaSeconds)
        {
            renderer.DrawRectangle(BoundingRectangle, Color.Magenta);
        }
    }
}