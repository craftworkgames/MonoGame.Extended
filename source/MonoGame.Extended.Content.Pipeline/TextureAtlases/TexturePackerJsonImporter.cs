// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using Microsoft.Xna.Framework.Content.Pipeline;
using MonoGame.Extended.Content.TexturePacker;

namespace MonoGame.Extended.Content.Pipeline.TextureAtlases
{
    [ContentImporter(".json", DefaultProcessor = "TexturePackerProcessor", DisplayName = "TexturePacker JSON Importer - MonoGame.Extended")]
    public class TexturePackerJsonImporter : ContentImporter<TexturePackerFileContent>
    {
        public override TexturePackerFileContent Import(string filename, ContentImporterContext context)
        {
            var tpFile = TexturePackerFileReader.Read(filename);
            context.AddDependency(tpFile.Meta.Image);
            return tpFile;
        }
    }
}
