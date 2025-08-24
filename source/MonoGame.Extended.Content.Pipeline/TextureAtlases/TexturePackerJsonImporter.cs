// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using Microsoft.Xna.Framework.Content.Pipeline;
using MonoGame.Extended.Content.TexturePacker;

namespace MonoGame.Extended.Content.Pipeline.TextureAtlases
{
    [ContentImporter(".json", DefaultProcessor = "TexturePackerProcessor", DisplayName = "TexturePacker JSON Importer - MonoGame.Extended")]
    public class TexturePackerJsonImporter : ContentImporter<ContentImporterResult<TexturePackerFileContent>>
    {
        public override ContentImporterResult<TexturePackerFileContent> Import(string filename, ContentImporterContext context)
        {
            var tpFile = TexturePackerFileReader.Read(filename);

            if (tpFile.Meta.Image != null)
            {
                context.AddDependency(tpFile.Meta.Image);
            }
            else if (tpFile.Meta.DataFormat == "monogame-extended")
            {
                // new format: textures are in the textures array
                foreach (var texture in tpFile.Textures)
                {
                    context.AddDependency(texture.FileName);
                }
            }

            return new ContentImporterResult<TexturePackerFileContent>(filename, tpFile);
        }
    }
}
