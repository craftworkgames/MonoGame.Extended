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
    public class GuiScreenRoot : GuiControl { }

    public class GuiScreen : GuiElement<GuiSystem>, IDisposable
    {
        private object _bindingContext;
        public GuiScreen(GuiSkin skin)
        {
            Skin = skin;
            Controls = new GuiControlCollection { ItemAdded = c => _isLayoutRequired = true };
            Windows = new GuiWindowCollection(this) { ItemAdded = w => _isLayoutRequired = true };
        }

        [JsonProperty(Order = 1)]
        public GuiSkin Skin { get; set; }

        [JsonProperty(Order = 2)]
        public GuiControlCollection Controls { get; set; }

        [JsonIgnore]
        public GuiWindowCollection Windows { get; }

        [JsonIgnore]
        public override object BindingContext
        {
            get
            {
                return _bindingContext;
            }
            set
            {
                _bindingContext = value;
                foreach (var control in Controls) control.BindingContext = _bindingContext;
            }
        }

        public new float Width { get; private set; }
        public new float Height { get; private set; }
        public new Size2 Size => new Size2(Width, Height);
        public bool IsVisible { get; set; } = true;

        [JsonIgnore]
        public bool IsLayoutRequired { get { return _isLayoutRequired || Controls.Any(x => x.IsLayoutRequired); } }
        private bool _isLayoutRequired = false;

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

            _isLayoutRequired = false;
            foreach (var control in Controls) control.IsLayoutRequired = false;
        }

        public override void Draw(IGuiContext context, IGuiRenderer renderer, float deltaSeconds)
        {
            renderer.DrawRectangle(BoundingRectangle, Color.Green);
        }

        public override void SetBinding(string viewProperty, string viewModelProperty)
        {
            foreach (var control in Controls) control.SetBinding(viewProperty, viewModelProperty);
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

        public static GuiScreen FromStream(ContentManager contentManager, Stream stream, params Type[] customControlTypes)
        {
            return FromStream<GuiScreen>(contentManager, stream, customControlTypes);
        }

        public static TScreen FromStream<TScreen>(ContentManager contentManager, Stream stream, params Type[] customControlTypes)
            where TScreen : GuiScreen
        {
            var skinService = new GuiSkinService();
            var serializer = new GuiJsonSerializer(contentManager, customControlTypes)
            {
                Converters =
                {
                    new GuiSkinJsonConverter(contentManager, skinService, customControlTypes),
                    new GuiControlJsonConverter(skinService, customControlTypes)
                }
            };

            using (var streamReader = new StreamReader(stream))
            using (var jsonReader = new JsonTextReader(streamReader))
            {
                var screen = serializer.Deserialize<TScreen>(jsonReader);
                return screen;
            }
        }

        public static GuiScreen FromFile(ContentManager contentManager, string path, params Type[] customControlTypes)
        {
            using (var stream = TitleContainer.OpenStream(path))
            {
                return FromStream<GuiScreen>(contentManager, stream, customControlTypes);
            }
        }

        public static TScreen FromFile<TScreen>(ContentManager contentManager, string path, params Type[] customControlTypes)
            where TScreen : GuiScreen
        {
            using (var stream = TitleContainer.OpenStream(path))
            {
                return FromStream<TScreen>(contentManager, stream, customControlTypes);
            }
        }
    }
}