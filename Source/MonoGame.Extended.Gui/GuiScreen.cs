using System;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended.Gui.Controls;
using MonoGame.Extended.Gui.Serialization;
using Newtonsoft.Json;

namespace MonoGame.Extended.Gui
{
    public class GuiScreen : GuiElement<GuiSystem>, IDisposable
    {
        public GuiScreen(GuiSkin skin)
        {
            Skin = skin;
            Controls = new GuiControlCollection { ItemAdded = c => IsLayoutRequired = true };
            Windows = new GuiWindowCollection(this) { ItemAdded = w => IsLayoutRequired = true };
        }

        [JsonProperty(Order = 1)]
        public GuiSkin Skin { get; set; }

        [JsonProperty(Order = 2)]
        public GuiControlCollection Controls { get; set; }

        [JsonIgnore]
        public GuiWindowCollection Windows { get; }

        public new float Width { get; private set; }
        public new float Height { get; private set; }
        public new Size2 Size => new Size2(Width, Height);
        public bool IsVisible { get; set; } = true;

        [JsonIgnore]
        public bool IsLayoutRequired { get; private set; }

        public virtual void Update(GameTime gameTime) { }

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

        public void Layout(IGuiContext context, Rectangle rectangle)
        {
            Width = rectangle.Width;
            Height = rectangle.Height;

            foreach (var control in Controls)
                GuiLayoutHelper.PlaceControl(context, control, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);

            foreach (var window in Windows)
                GuiLayoutHelper.PlaceWindow(context, window, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);

            IsLayoutRequired = false;
        }

        public override void Draw(IGuiContext context, IGuiRenderer renderer, float deltaSeconds)
        {
            renderer.DrawRectangle(BoundingRectangle, Color.Green);
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