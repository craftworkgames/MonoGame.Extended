using System;
using System.Collections.Generic;
using System.IO;
using MonoGame.Extended.BitmapFonts;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended.Collections;
using MonoGame.Extended.Gui.Controls;
using MonoGame.Extended.Gui.Serialization;
using MonoGame.Extended.TextureAtlases;
using Newtonsoft.Json;

namespace MonoGame.Extended.Gui
{
    public class GuiSkin
    {
        public GuiSkin()
        {
            TextureAtlases = new List<TextureAtlas>();
            Fonts = new List<BitmapFont>();
            NinePatches = new List<NinePatchRegion2D>();
            _styles = new KeyedCollection<string, GuiControlStyle>(s => s.Name);
        }

        private readonly KeyedCollection<string, GuiControlStyle> _styles;

        [JsonProperty(Order = 0)]
        public string Name { get; set; }

        [JsonProperty(Order = 1)]
        public IList<TextureAtlas> TextureAtlases { get; set; }

        [JsonProperty(Order = 2)]
        public IList<BitmapFont> Fonts { get; set; }

        [JsonProperty(Order = 3)]
        public IList<NinePatchRegion2D> NinePatches { get; set; }

        [JsonProperty(Order = 4)]
        public BitmapFont DefaultFont => Fonts.FirstOrDefault();

        [JsonProperty(Order = 5)]
        public GuiCursor Cursor { get; set; }

        [JsonProperty(Order = 6)]
        public ICollection<GuiControlStyle> Styles => _styles;

        public GuiControlStyle GetStyle(string name)
        {
            return _styles[name];
        }

        public static GuiSkin FromStream(Stream stream, ContentManager contentManager)
        {
            var skinSerializer = new GuiJsonSerializer(contentManager);

            using (var streamReader = new StreamReader(stream))
            using (var jsonReader = new JsonTextReader(streamReader))
            {
                return skinSerializer.Deserialize<GuiSkin>(jsonReader);
            }
        }


        public T Create<T>(string template, string name = null, string text = null)
            where T : GuiControl, new()
        {
            return Create<T>(template, Vector2.Zero, name, text);
        }

        public T Create<T>(string template, Vector2 position, string name = null, string text = null)
            where T : GuiControl, new()
        {
            var control = new T();
            GetStyle(template).Apply(control);
            control.Name = name;
            control.Position = position;
            control.Text = text;
            return control;
        }

        public T Create<T>(string template, Action<T> onCreate)
            where T : GuiControl, new()
        {
            var control = new T();
            GetStyle(template).Apply(control);
            onCreate(control);
            return control;
        }

        public GuiControl Create(Type type, string template)
        {
            var control = (GuiControl)Activator.CreateInstance(type);
            GetStyle(template).Apply(control);
            return control;
        }
    }
}