using System.IO;
using Newtonsoft.Json;

namespace MonoGame.Extended.Gui.Serialization
{
    public class GuiSkinSerializer
    {
        public void Serialize(TextWriter writer, GuiSkin skin)
        {
            using (var jsonWriter = new JsonTextWriter(writer))
            {
                var jsonSerializer = new JsonSerializer { Formatting = Formatting.Indented };
                jsonSerializer.Serialize(jsonWriter, skin);
            }
        }

        public GuiSkin Deserialize(TextReader reader)
        {
            using (var jsonReader = new JsonTextReader(reader))
            {
                var jsonSerializer = new JsonSerializer { Formatting = Formatting.Indented };
                return jsonSerializer.Deserialize<GuiSkin>(jsonReader);
            }
        }
    }
}