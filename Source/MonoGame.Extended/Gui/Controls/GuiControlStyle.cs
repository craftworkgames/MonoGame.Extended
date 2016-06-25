namespace MonoGame.Extended.Gui.Controls
{
    public abstract class GuiControlStyle
    {
        protected GuiControlStyle()
        {
            Name = string.Empty;
            Normal = new GuiSpriteStyle();
        }

        public string Name { get; set; }
        public GuiSpriteStyle Normal { get; set; }

        public abstract GuiControl CreateControl(IGuiContentService contentService);

        public override string ToString()
        {
            return Name;
        }
    }
}