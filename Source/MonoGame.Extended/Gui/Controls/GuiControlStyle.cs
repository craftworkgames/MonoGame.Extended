namespace MonoGame.Extended.Gui.Controls
{
    public abstract class GuiControlStyle
    {
        public string Name { get; set; }
        public GuiSpriteStyle Normal { get; set; }

        public abstract GuiControl CreateControl(GuiContentService contentService);

        public override string ToString()
        {
            return Name;
        }
    }
}