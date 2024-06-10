// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using MonoGame.Extended.BitmapFonts.BmfTypes;

namespace MonoGame.Extended.BitmapFonts;

internal static class BMFTextLoader
{
    public static BitmapFontFile LoadText(Stream stream)
    {
        using StreamReader reader = new StreamReader(stream);

        // Text does not contain the header like binary so we manually create it
        BmfHeader header = new BmfHeader() { B = (byte)'B', M = (byte)'M', F = (byte)'F', Version = 3 };
        BmfInfoBlock info = default;
        BmfCommonBlock common = default;
        List<BmfCharsBlock> characters = new List<BmfCharsBlock>();
        List<BmfKerningPairsBlock> kernings = new List<BmfKerningPairsBlock>();
        List<string> pages = new List<string>();
        string fontName = string.Empty;

        string line = default;
        while ((line = reader.ReadLine()) != null)
        {
            List<string> tokens = GetTokens(line);
            if (tokens.Count == 0)
            {
                continue;
            }

            switch (tokens[0])
            {
                case "info":
                    (info, fontName) = LoadInfoLine(CollectionsMarshal.AsSpan(tokens)[1..]);
                    break;
                case "common":
                    common = LoadCommonLine(CollectionsMarshal.AsSpan(tokens)[1..]);
                    break;
                case "page":
                    pages.Add(LoadPageLine(CollectionsMarshal.AsSpan(tokens)[1..]));
                    break;
                case "char":
                    characters.Add(LoadCharacterLine(CollectionsMarshal.AsSpan(tokens)[1..]));
                    break;
                case "kerning":
                    kernings.Add(LoadKerningLine(CollectionsMarshal.AsSpan(tokens)[1..]));
                    break;
            }

        }

        return new BitmapFontFile(header, common, info, fontName, pages.ToArray(), characters.ToArray(), kernings.ToArray());
    }


    private static (BmfInfoBlock, string) LoadInfoLine(ReadOnlySpan<string> tokens)
    {
        BmfInfoBlock info = default;
        string fontName = default;

        for (int i = 0; i < tokens.Length; ++i)
        {
            string[] split = tokens[i].Split('=');

            if (split.Length != 2)
            {
                continue;
            }

            switch (split[0])
            {
                case "face":
                    fontName = split[1].Replace("\"", string.Empty);
                    break;
                case "size":
                    info.FontSize = Convert.ToInt16(split[1], CultureInfo.InvariantCulture);
                    break;
                case "smooth":
                    byte smooth = Convert.ToByte(split[1], CultureInfo.InvariantCulture);
                    info.BitField |= (byte)(smooth << 7);
                    break;
                case "unicode":
                    byte unicode = Convert.ToByte(split[1], CultureInfo.InvariantCulture);
                    info.BitField |= (byte)(unicode << 6);
                    break;
                case "italic":
                    byte italic = Convert.ToByte(split[1], CultureInfo.InvariantCulture);
                    info.BitField |= (byte)(italic << 5);
                    break;
                case "bold":
                    byte bold = Convert.ToByte(split[1], CultureInfo.InvariantCulture);
                    info.BitField |= (byte)(bold << 4);
                    break;
                case "fixedHeight":
                    byte fixedHeight = Convert.ToByte(split[1], CultureInfo.InvariantCulture);
                    info.BitField |= (byte)(fixedHeight << 3);
                    break;
                case "stretchH":
                    info.StretchH = Convert.ToUInt16(split[1], CultureInfo.InvariantCulture);
                    break;
                case "aa":
                    info.AA = Convert.ToByte(split[1], CultureInfo.InvariantCulture);
                    break;
                case "padding":
                    string[] paddingValues = split[1].Split(',');
                    if (paddingValues.Length == 4)
                    {
                        info.PaddingUp = Convert.ToByte(paddingValues[0], CultureInfo.InvariantCulture);
                        info.PaddingRight = Convert.ToByte(paddingValues[1], CultureInfo.InvariantCulture);
                        info.PaddingDown = Convert.ToByte(paddingValues[2], CultureInfo.InvariantCulture);
                        info.PaddingLeft = Convert.ToByte(paddingValues[3], CultureInfo.InvariantCulture);
                    }
                    break;
                case "spacing":
                    string[] spacingValues = split[1].Split(',');
                    if (spacingValues.Length == 2)
                    {
                        info.SpacingHoriz = Convert.ToByte(spacingValues[0], CultureInfo.InvariantCulture);
                        info.SpacingVert = Convert.ToByte(spacingValues[1], CultureInfo.InvariantCulture);
                    }
                    break;
                case "outline":
                    info.Outline = Convert.ToByte(split[1], CultureInfo.InvariantCulture);
                    break;
            }
        }

        return (info, fontName);
    }

    private static BmfCommonBlock LoadCommonLine(ReadOnlySpan<string> tokens)
    {
        BmfCommonBlock common = default;

        for (int i = 0; i < tokens.Length; ++i)
        {
            string[] split = tokens[i].Split('=');

            if (split.Length != 2)
            {
                continue;
            }

            switch (split[0])
            {
                case "lineHeight":
                    common.LineHeight = Convert.ToUInt16(split[1], CultureInfo.InvariantCulture);
                    break;
                case "base":
                    common.Base = Convert.ToUInt16(split[1], CultureInfo.InvariantCulture);
                    break;
                case "scaleW":
                    common.ScaleW = Convert.ToUInt16(split[1], CultureInfo.InvariantCulture);
                    break;
                case "scaleH":
                    common.ScaleH = Convert.ToUInt16(split[1], CultureInfo.InvariantCulture);
                    break;
                case "pages":
                    common.Pages = Convert.ToUInt16(split[1], CultureInfo.InvariantCulture);
                    break;
                case "packed":
                    byte packed = Convert.ToByte(split[1], CultureInfo.InvariantCulture);
                    common.BitField |= (byte)(packed << 7);
                    break;
                case "alphaChnl":
                    common.AlphaChnl = Convert.ToByte(split[1], CultureInfo.InvariantCulture);
                    break;
                case "redChnl":
                    common.RedChnl = Convert.ToByte(split[1], CultureInfo.InvariantCulture);
                    break;
                case "greenChnl":
                    common.GreenChnl = Convert.ToByte(split[1], CultureInfo.InvariantCulture);
                    break;
                case "blueChnl":
                    common.BlueChnl = Convert.ToByte(split[1], CultureInfo.InvariantCulture);
                    break;
            }
        }

        return common;
    }

    private static string LoadPageLine(ReadOnlySpan<string> tokens)
    {
        string page = default;

        for (int i = 0; i < tokens.Length; ++i)
        {
            string[] split = tokens[i].Split('=');

            if (split.Length != 2)
            {
                continue;
            }

            if (split[0] == "file")
            {
                page = split[1].Replace("\"", string.Empty);
            }
        }

        return page;
    }

    private static BmfCharsBlock LoadCharacterLine(ReadOnlySpan<string> tokens)
    {
        BmfCharsBlock common = default;

        for (int i = 0; i < tokens.Length; ++i)
        {
            string[] split = tokens[i].Split('=');

            if (split.Length != 2)
            {
                continue;
            }

            switch (split[0])
            {
                case "id":
                    common.ID = Convert.ToUInt32(split[1], CultureInfo.InvariantCulture);
                    break;
                case "x":
                    common.X = Convert.ToUInt16(split[1], CultureInfo.InvariantCulture);
                    break;
                case "y":
                    common.Y = Convert.ToUInt16(split[1], CultureInfo.InvariantCulture);
                    break;
                case "width":
                    common.Width = Convert.ToUInt16(split[1], CultureInfo.InvariantCulture);
                    break;
                case "height":
                    common.Height = Convert.ToUInt16(split[1], CultureInfo.InvariantCulture);
                    break;
                case "xoffset":
                    common.XOffset = Convert.ToInt16(split[1], CultureInfo.InvariantCulture);
                    break;
                case "yoffset":
                    common.YOffset = Convert.ToInt16(split[1], CultureInfo.InvariantCulture);
                    break;
                case "xadvance":
                    common.XAdvance = Convert.ToInt16(split[1], CultureInfo.InvariantCulture);
                    break;
                case "page":
                    common.Page = Convert.ToByte(split[1], CultureInfo.InvariantCulture);
                    break;
                case "chnl":
                    common.Chnl = Convert.ToByte(split[1], CultureInfo.InvariantCulture);
                    break;
            }
        }

        return common;
    }

    private static BmfKerningPairsBlock LoadKerningLine(ReadOnlySpan<string> tokens)
    {
        BmfKerningPairsBlock kerning = default;

        for (int i = 0; i < tokens.Length; ++i)
        {
            string[] split = tokens[i].Split('=');

            if (split.Length != 2)
            {
                continue;
            }

            switch (split[0])
            {
                case "first":
                    kerning.First = Convert.ToUInt32(split[1], CultureInfo.InvariantCulture);
                    break;
                case "second":
                    kerning.Second = Convert.ToUInt32(split[1], CultureInfo.InvariantCulture);
                    break;
                case "amount":
                    kerning.Amount = Convert.ToInt16(split[1], CultureInfo.InvariantCulture);
                    break;
            }
        }

        return kerning;
    }



    private static List<string> GetTokens(string line)
    {
        List<string> tokens = new List<string>();
        StringBuilder currentToken = new StringBuilder();
        bool inQuotes = false;

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];

            if (c == ' ' && !inQuotes)
            {
                if (currentToken.Length > 0)
                {
                    tokens.Add(currentToken.ToString());
                    currentToken.Clear();
                }
            }
            else if (c == '"')
            {
                inQuotes = !inQuotes;
            }
            else
            {
                currentToken.Append(c);
            }
        }

        if (currentToken.Length > 0)
        {
            tokens.Add(currentToken.ToString());
        }

        return tokens;
    }
}
