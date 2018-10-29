using MonoGame.Extended.TextureAtlases;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonoGame.Extended.Gui.Controls
{
    public class Image : ContentControl
    {
        public Image()
        {
            BackgroundColor = Microsoft.Xna.Framework.Color.Transparent;
        }

        public Image(TextureRegion2D image)
            : base()
        {
            Image = image;
            BackgroundColor = Microsoft.Xna.Framework.Color.Transparent;
        }

        public string Text { get { return (string)Content; } set { Content = value; } }

        public override Size GetContentSize(IGuiContext context)
        {
            if (!ImageSize.IsEmpty)
                return new Size((int)ImageSize.Width, (int)ImageSize.Height);
            else if (Image != null)
                return new Size((int)Image.Size.Width, (int)Image.Size.Height);
            else if (!Size.IsEmpty)
                return new Size((int)Size.Width, (int)Size.Height);
            else if (BackgroundRegion!= null)
                return new Size((int)BackgroundRegion.Width, (int)BackgroundRegion.Height);
            else
                return new Size();
        }
    }
}