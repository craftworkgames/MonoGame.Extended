// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System.IO;
using System.Xml;
using MonoGame.Extended.BitmapFonts.BmfTypes;

namespace MonoGame.Extended.BitmapFonts;

internal static class BMFXmlLoader
{
    public static BitmapFontFile Load(Stream stream)
    {
        // XML does not contain the header like binary so we manually create it
        BmfHeader header = new BmfHeader() { B = (byte)'B', M = (byte)'M', F = (byte)'F', Version = 3 };

        XmlDocument document = new XmlDocument();
        document.Load(stream);
        XmlNode root = document.DocumentElement;

        string fontName = LoadFontName(root);
        BmfInfoBlock info = LoadInfoNode(root);
        BmfCommonBlock common = LoadCommonNode(root);
        string[] pages = LoadPageNodes(root);
        BmfCharsBlock[] characters = LoadCharacterNodes(root);
        BmfKerningPairsBlock[] kernings = LoadKerningNodes(root);

        return new BitmapFontFile(header, common, info, fontName, pages, characters, kernings);
    }

    private static string LoadFontName(XmlNode root)
    {
        XmlNode infoNode = root.SelectSingleNode("info");
        return infoNode.GetStringAttribute("face");
    }

    private static BmfInfoBlock LoadInfoNode(XmlNode root)
    {
        BmfInfoBlock info;

        XmlNode node = root.SelectSingleNode("info");
        info.FontSize = node.GetInt16Attribute("size");

        byte smooth = node.GetByteAttribute("smooth");
        byte unicode = node.GetByteAttribute("unicode");
        byte italic = node.GetByteAttribute("italic");
        byte bold = node.GetByteAttribute("bold");
        byte fixedHeight = node.GetByteAttribute("fixedHeight");
        info.BitField = (byte)((smooth << 7) | (unicode << 6) | (italic << 5) | (bold << 4) | (fixedHeight << 3));

        info.CharSet = node.GetByteAttribute("charSet");
        info.StretchH = node.GetUInt16Attribute("stretchH");
        info.AA = node.GetByteAttribute("aa");

        byte[] paddingValues = node.GetByteDelimitedAttribute("padding", 4);
        info.PaddingUp = paddingValues[0];
        info.PaddingRight = paddingValues[1];
        info.PaddingDown = paddingValues[2];
        info.PaddingLeft = paddingValues[3];

        byte[] spacingValues = node.GetByteDelimitedAttribute("spacing", 2);
        info.SpacingHoriz = spacingValues[0];
        info.SpacingVert = spacingValues[1];

        info.Outline = node.GetByteAttribute("outline");

        return info;
    }

    private static BmfCommonBlock LoadCommonNode(XmlNode root)
    {
        BmfCommonBlock common;

        XmlNode node = root.SelectSingleNode("common");
        common.LineHeight = node.GetUInt16Attribute("lineHeight");
        common.Base = node.GetUInt16Attribute("base");
        common.ScaleW = node.GetUInt16Attribute("scaleW");
        common.ScaleH = node.GetUInt16Attribute("scaleH");
        common.Pages = node.GetUInt16Attribute("pages");

        byte packed = node.GetByteAttribute("packed");
        common.BitField = (byte)(packed << 7);

        common.AlphaChnl = node.GetByteAttribute("alphaChnl");
        common.RedChnl = node.GetByteAttribute("redChnl");
        common.GreenChnl = node.GetByteAttribute("greenChnl");
        common.BlueChnl = node.GetByteAttribute("blueChnl");

        return common;
    }

    private static string[] LoadPageNodes(XmlNode root)
    {
        XmlNodeList nodes = root.SelectNodes("pages/page");
        string[] pages = new string[nodes.Count];

        for (int i = 0; i < nodes.Count; ++i)
        {
            XmlNode node = nodes[i];

            int id = node.GetInt32Attribute("id");
            string file = node.GetStringAttribute("file");

            pages[id] = file;
        }

        return pages;
    }

    private static BmfCharsBlock[] LoadCharacterNodes(XmlNode root)
    {
        XmlNodeList characterNodes = root.SelectNodes("chars/char");
        BmfCharsBlock[] characters = new BmfCharsBlock[characterNodes.Count];

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

            characters[i] = character;
        }

        return characters;
    }

    private static BmfKerningPairsBlock[] LoadKerningNodes(XmlNode root)
    {
        XmlNodeList kerningNodes = root.SelectNodes("kernings/kerning");
        BmfKerningPairsBlock[] kernings = new BmfKerningPairsBlock[kerningNodes.Count];

        for (int i = 0; i < kerningNodes.Count; ++i)
        {
            XmlNode node = kerningNodes[i];

            BmfKerningPairsBlock kerning;
            kerning.First = node.GetUInt32Attribute("first");
            kerning.Second = node.GetUInt32Attribute("second");
            kerning.Amount = node.GetInt16Attribute("amount");

            kernings[i] = kerning;
        }

        return kernings;
    }
}
