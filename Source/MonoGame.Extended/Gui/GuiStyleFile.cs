using System.Collections.Generic;
using System.IO;
using MonoGame.Extended.Gui.Controls;
using Newtonsoft.Json;

namespace MonoGame.Extended.Gui
{
    public class GuiStyleFile
    {
        private GuiStyleFile(GuiControlStyle[] styles)
        {
            _styles = styles;
        }

        private readonly GuiControlStyle[] _styles;

        public IEnumerable<GuiControlStyle> Styles => _styles;

        public static GuiStyleFile Load(StreamReader streamReader)
        {
            using (var reader = new JsonTextReader(streamReader))
            {
                var serializer = new JsonSerializer()
                {
                    Formatting = Formatting.Indented,
                    TypeNameHandling = TypeNameHandling.Objects,
                    Converters = { new MonoGameColorJsonConverter() }
                };

                var styles = serializer.Deserialize<GuiControlStyle[]>(reader);
                return new GuiStyleFile(styles);
            }
        }
    }
}