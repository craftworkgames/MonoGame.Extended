using System;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Gui.Controls;
using MonoGame.Extended.Gui.Serialization;
using Newtonsoft.Json;

namespace MonoGame.Extended.Gui
{
    public class GuiScreen : IDisposable
    {
        public GuiScreen(GuiSkin skin)
        {
            Skin = skin;
            Controls = new GuiControlCollection();
        }

        [JsonProperty(Order = 1)]
        public GuiSkin Skin { get; set; }

        [JsonProperty(Order = 2)]
        public GuiControlCollection Controls { get; }

        public float Width { get; private set; }
        public float Height { get; private set; }
        public Size2 Size => new Size2(Width, Height);
        public bool IsVisible { get; set; } = true;

        public virtual void Initialize() { }

        public void Show()
        {
            IsVisible = true;
        }

        public void Hide()
        {
            IsVisible = false;
        }

        public T FindControl<T>(string name)
            where T : GuiControl
        {
            return FindControl<T>(Controls, name);
        }

        private static T FindControl<T>(GuiControlCollection controls, string name)
            where T : GuiControl
        {
            foreach (var control in controls)
            {
                if (control.Name == name)
                    return control as T;

                if (control.Controls.Any())
                {
                    var childControl = FindControl<T>(control.Controls, name);

                    if (childControl != null)
                        return childControl;
                }
            }

            return null;
        }

        public void Layout(IGuiContext context, RectangleF rectangle)
        {
            Width = rectangle.Width;
            Height = rectangle.Height;

            Initialize();

            foreach (var control in Controls)
                GuiLayoutHelper.PlaceControl(context, control, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public static GuiScreen FromStream(ContentManager contentManager, Stream stream)
        {
            var skinService = new GuiSkinService();
            var serializer = new GuiJsonSerializer(contentManager)
            {
                Converters =
                {
                    new GuiSkinJsonConverter(contentManager, skinService),
                    new GuiControlJsonConverter(skinService)
                }
            };

            using (var streamReader = new StreamReader(stream))
            using (var jsonReader = new JsonTextReader(streamReader))
            {
                var screen = serializer.Deserialize<GuiScreen>(jsonReader);
                return screen;
            }
        }

        public static GuiScreen FromFile(ContentManager contentManager, string path)
        {
            using (var stream = TitleContainer.OpenStream(path))
            {
                return FromStream(contentManager, stream);
            }
        }
    }
}