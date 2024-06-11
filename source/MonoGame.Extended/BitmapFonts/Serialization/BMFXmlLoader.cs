// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System.IO;
using System.Xml;
using MonoGame.Extended.BitmapFonts.BmfTypes;

namespace MonoGame.Extended.BitmapFonts.Serialization;

public static class BmfXmlLoader
{
    public static void Load(BmfFile bmfFile, Stream stream)
    {
        // XML does not contain the header like binary so we manually create it
        bmfFile.Header = new BmfHeader() { B = (byte)'B', M = (byte)'M', F = (byte)'F', Version = 3 };

        XmlDocument document = new XmlDocument();
        document.Load(stream);
        XmlNode root = document.DocumentElement;

        LoadInfoNode(bmfFile, root);
        LoadCommonNode(bmfFile, root);
        LoadPageNodes(bmfFile, root);
        LoadCharacterNodes(bmfFile, root);
        LoadKerningNodes(bmfFile, root);
    }

    private static void LoadInfoNode(BmfFile bmfFile, XmlNode root)
    {
        BmfInfoBlock info;

        XmlNode node = root.SelectSingleNode("info");
        bmfFile.FontName = node.GetStringAttribute("face");
        bmfFile.Info.FontSize = node.GetInt16Attribute("size");

        byte smooth = node.GetByteAttribute("smooth");
        byte unicode = node.GetByteAttribute("unicode");
        byte italic = node.GetByteAttribute("italic");
        byte bold = node.GetByteAttribute("bold");
        byte fixedHeight = node.GetByteAttribute("fixedHeight");
        bmfFile.Info.BitField = (byte)(smooth << 7 | unicode << 6 | italic << 5 | bold << 4 | fixedHeight << 3);

        bmfFile.Info.CharSet = node.GetByteAttribute("charSet");
        bmfFile.Info.StretchH = node.GetUInt16Attribute("stretchH");
        bmfFile.Info.AA = node.GetByteAttribute("aa");

        byte[] paddingValues = node.GetByteDelimitedAttribute("padding", 4);
        bmfFile.Info.PaddingUp = paddingValues[0];
        bmfFile.Info.PaddingRight = paddingValues[1];
        bmfFile.Info.PaddingDown = paddingValues[2];
        bmfFile.Info.PaddingLeft = paddingValues[3];

        byte[] spacingValues = node.GetByteDelimitedAttribute("spacing", 2);
        bmfFile.Info.SpacingHoriz = spacingValues[0];
        bmfFile.Info.SpacingVert = spacingValues[1];

        bmfFile.Info.Outline = node.GetByteAttribute("outline");
    }

    private static void LoadCommonNode(BmfFile bmfFile, XmlNode root)
    {
        BmfCommonBlock common;

        XmlNode node = root.SelectSingleNode("common");
        bmfFile.Common.LineHeight = node.GetUInt16Attribute("lineHeight");
        bmfFile.Common.Base = node.GetUInt16Attribute("base");
        bmfFile.Common.ScaleW = node.GetUInt16Attribute("scaleW");
        bmfFile.Common.ScaleH = node.GetUInt16Attribute("scaleH");
        bmfFile.Common.Pages = node.GetUInt16Attribute("pages");

        byte packed = node.GetByteAttribute("packed");
        bmfFile.Common.BitField = (byte)(packed << 7);

        bmfFile.Common.AlphaChnl = node.GetByteAttribute("alphaChnl");
        bmfFile.Common.RedChnl = node.GetByteAttribute("redChnl");
        bmfFile.Common.GreenChnl = node.GetByteAttribute("greenChnl");
        bmfFile.Common.BlueChnl = node.GetByteAttribute("blueChnl");
    }

    private static void LoadPageNodes(BmfFile bmfFile, XmlNode root)
    {
        XmlNodeList nodes = root.SelectNodes("pages/page");

        for (int i = 0; i < nodes.Count; ++i)
        {
            XmlNode node = nodes[i];

            string file = node.GetStringAttribute("file");
            bmfFile.Pages.Add(file);
        }
    }

    private static void LoadCharacterNodes(BmfFile bmfFile, XmlNode root)
    {
        XmlNodeList characterNodes = root.SelectNodes("chars/char");

        for (int i = 0; i < characterNodes.Count; ++i)
        {
            XmlNode node = characterNodes[i];

            BmfCharsBlock character;
            character.ID = node.GetUInt32Attribute("id");
            character.X = node.GetUInt16Attribute("x");
            character.Y = node.GetUInt16Attribute("y");
            character.Width = node.GetUInt16Attribute("width");
            character.Height = node.GetUInt16Attribute("height");
            character.XOffset = node.GetInt16Attribute("xoffset");
            character.YOffset = node.GetInt16Attribute("yoffset");
            character.XAdvance = node.GetInt16Attribute("xadvance");
            character.Page = node.GetByteAttribute("page");
            character.Chnl = node.GetByteAttribute("chnl");

            bmfFile.Characters.Add(character);
        }
    }

    private static void LoadKerningNodes(BmfFile bmfFile, XmlNode root)
    {
        XmlNodeList kerningNodes = root.SelectNodes("kernings/kerning");

        for (int i = 0; i < kerningNodes.Count; ++i)
        {
            XmlNode node = kerningNodes[i];

            BmfKerningPairsBlock kerning;
            kerning.First = node.GetUInt32Attribute("first");
            kerning.Second = node.GetUInt32Attribute("second");
            kerning.Amount = node.GetInt16Attribute("amount");

            bmfFile.Kernings.Add(kerning);
        }
    }
}
