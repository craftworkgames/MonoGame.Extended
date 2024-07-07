// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using MonoGame.Extended.Graphics;

namespace MonoGame.Extended.Content.Pipeline.TextureAtlases;

[ContentTypeWriter]
public class TexturePackerWriter : ContentTypeWriter<TexturePackerProcessorResult>
{
    protected override void Write(ContentWriter writer, TexturePackerProcessorResult result)
    {
        var tpFile = result.Data;
        var imageAssetName = Path.GetFileNameWithoutExtension(tpFile.Meta.Image);

        writer.Write(imageAssetName);
        writer.Write(tpFile.Regions.Count);

        foreach (var region in tpFile.Regions)
        {
            var regionName = Path.GetFileNameWithoutExtension(region.FileName);

            writer.Write(region.Frame.X);
            writer.Write(region.Frame.Y);
            writer.Write(region.Frame.Width);
            writer.Write(region.Frame.Height);
            writer.Write(regionName);
        }
    }

    public override string GetRuntimeType(TargetPlatform targetPlatform)
    {
        return typeof(Texture2DAtlas).AssemblyQualifiedName;
    }

    public override string GetRuntimeReader(TargetPlatform targetPlatform)
    {
        return typeof(ContentReaders.Texture2DAtlasReader).AssemblyQualifiedName;
    }
}
