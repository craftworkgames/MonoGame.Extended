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
    public class Screen //: Element<GuiSystem>, IDisposable
    {
        public Screen()
        {
            //Windows = new WindowCollection(this) { ItemAdded = w => _isLayoutRequired = true };
        }

        public virtual void Dispose()
        {
        }

        private Control _content;
        [JsonProperty(Order = 1)]
        public Control Content
        {
            get { return _content; }
            set
            {
                if (_content != value)
                {
                    _content = value;
                    _isLayoutRequired = true;
                }
            }
        }

        //[JsonIgnore]
        //public WindowCollection Windows { get; }

        public float Width { get; private set; }
        public float Height { get; private set; }
        public Size2 Size => new Size2(Width, Height);
        public bool IsVisible { get; set; } = true;

        private bool _isLayoutRequired;
        [JsonIgnore]
        public bool IsLayoutRequired => _isLayoutRequired || Content.IsLayoutRequired;

        public virtual void Update(GameTime gameTime)
        {
            
        }

        public void Show()
        {
            IsVisible = true;
        }

        public void Hide()
        {
            IsVisible = false;
        }

        public T FindControl<T>(string name)
            where T : Control
        {
            return FindControl<T>(Content, name);
        }

        private static T FindControl<T>(Control rootControl, string name)
            where T : Control
        {
            if (rootControl.Name == name)
                return rootControl as T;

            foreach (var childControl in rootControl.Children)
            {
                var control = FindControl<T>(childControl, name);

                if (control != null)
                    return control;
            }

            return null;
        }

        public void Layout(IGuiContext context, Rectangle rectangle)
        {
            Width = rectangle.Width;
            Height = rectangle.Height;

            LayoutHelper.PlaceControl(context, Content, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);

            //foreach (var window in Windows)
            //    LayoutHelper.PlaceWindow(context, window, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);

            _isLayoutRequired = false;
            Content.IsLayoutRequired = false;
        }

        //public override void Draw(IGuiContext context, IGuiRenderer renderer, float deltaSeconds)
        //{
        //    renderer.DrawRectangle(BoundingRectangle, Color.Green);
        //}

        public static Screen FromStream(ContentManager contentManager, Stream stream, params Type[] customControlTypes)
        {
            return FromStream<Screen>(contentManager, stream, customControlTypes);
        }

        public static TScreen FromStream<TScreen>(ContentManager contentManager, Stream stream, params Type[] customControlTypes)
            where TScreen : Screen
        {
            var skinService = new SkinService();
            var serializer = new GuiJsonSerializer(contentManager, customControlTypes)
            {
                Converters =
                {
                    new SkinJsonConverter(contentManager, skinService, customControlTypes),
                    new ControlJsonConverter(skinService, customControlTypes)
                }
            };

            using (var streamReader = new StreamReader(stream))
            using (var jsonReader = new JsonTextReader(streamReader))
            {
                var screen = serializer.Deserialize<TScreen>(jsonReader);
                return screen;
            }
        }

        public static Screen FromFile(ContentManager contentManager, string path, params Type[] customControlTypes)
        {
            using (var stream = TitleContainer.OpenStream(path))
            {
                return FromStream<Screen>(contentManager, stream, customControlTypes);
            }
        }

        public static TScreen FromFile<TScreen>(ContentManager contentManager, string path, params Type[] customControlTypes)
            where TScreen : Screen
        {
            using (var stream = TitleContainer.OpenStream(path))
            {
                return FromStream<TScreen>(contentManager, stream, customControlTypes);
            }
        }
    }
}