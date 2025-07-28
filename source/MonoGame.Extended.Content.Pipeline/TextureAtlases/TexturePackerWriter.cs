// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System;
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

        if (tpFile.Meta.DataFormat == "monogame-extended")  // new "MonoGame.Extended" format, recommended
        {
            var texture = tpFile.Textures[0];
            var imageAssetName = Path.ChangeExtension(texture.FileName, null);
            writer.Write(imageAssetName);
            writer.Write(texture.Frames.Count);

            foreach (var kv in texture.Frames)
            {
                var frameKey = kv.Key;
                var frame = kv.Value;

                writer.Write(frame.Frame.X);
                writer.Write(frame.Frame.Y);
                writer.Write(frame.Frame.Width);
                writer.Write(frame.Frame.Height);
                writer.Write(frameKey);

                writer.Write(frame.Rotated);  // angle (0, 90)

                writer.Write(frame.Size != null);  // trimmed sprite: true/false
                if (frame.Size != null)
                {
                    writer.Write(frame.Size.Width);   // sprite size before trimming
                    writer.Write(frame.Size.Height);
                    writer.Write(frame.Offset.X);     // trim offset
                    writer.Write(frame.Offset.Y);
                }
                writer.Write(frame.Pivot != null);  // has pivot point: true/false
                if (frame.Pivot != null)
                {
                    writer.Write(frame.Pivot.X);
                    writer.Write(frame.Pivot.Y);
                }
            }
        }
        else if (tpFile.Regions.Count != 0)  // generic "JSON (Array)" format 
        {
            var imageAssetName = Path.ChangeExtension(tpFile.Meta.Image, null);
            writer.Write(imageAssetName);
            writer.Write(tpFile.Regions.Count);

            foreach (var region in tpFile.Regions)
            {
                var regionName = Path.ChangeExtension(region.FileName, null);

                writer.Write(region.Frame.X);
                writer.Write(region.Frame.Y);
                writer.Write(region.Frame.Width);
                writer.Write(region.Frame.Height);
                writer.Write(regionName);
                writer.Write(0);                    // rotation
                writer.Write(false);                // no trimming
                writer.Write(false);                // no pivot point
            }
        }
        else
        {
            throw new InvalidOperationException("No frames or textures found in TexturePackerFileContent.");
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
