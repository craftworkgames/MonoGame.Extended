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

namespace MonoGame.Extended.BitmapFonts.Serialization;

internal static class BMFTextLoader
{
    public static void Load(BitmapFontFile bmfFile, Stream stream)
    {
        using StreamReader reader = new StreamReader(stream);

        // Text does not contain the header like binary so we manually create it
        bmfFile.Header = new BmfHeader() { B = (byte)'B', M = (byte)'M', F = (byte)'F', Version = 3 };

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
                    LoadInfoLine(bmfFile, CollectionsMarshal.AsSpan(tokens)[1..]);
                    break;
                case "common":
                    LoadCommonLine(bmfFile, CollectionsMarshal.AsSpan(tokens)[1..]);
                    break;
                case "page":
                    LoadPageLine(bmfFile, CollectionsMarshal.AsSpan(tokens)[1..]);
                    break;
                case "char":
                    LoadCharacterLine(bmfFile, CollectionsMarshal.AsSpan(tokens)[1..]);
                    break;
                case "kerning":
                    LoadKerningLine(bmfFile, CollectionsMarshal.AsSpan(tokens)[1..]);
                    break;
            }
        }
    }


    private static void LoadInfoLine(BitmapFontFile bmfFile, ReadOnlySpan<string> tokens)
    {
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
                    bmfFile.FontName = split[1].Replace("\"", string.Empty);
                    break;
                case "size":
                    bmfFile.Info.FontSize = Convert.ToInt16(split[1], CultureInfo.InvariantCulture);
                    break;
                case "smooth":
                    byte smooth = Convert.ToByte(split[1], CultureInfo.InvariantCulture);
                    bmfFile.Info.BitField |= (byte)(smooth << 7);
                    break;
                case "unicode":
                    byte unicode = Convert.ToByte(split[1], CultureInfo.InvariantCulture);
                    bmfFile.Info.BitField |= (byte)(unicode << 6);
                    break;
                case "italic":
                    byte italic = Convert.ToByte(split[1], CultureInfo.InvariantCulture);
                    bmfFile.Info.BitField |= (byte)(italic << 5);
                    break;
                case "bold":
                    byte bold = Convert.ToByte(split[1], CultureInfo.InvariantCulture);
                    bmfFile.Info.BitField |= (byte)(bold << 4);
                    break;
                case "fixedHeight":
                    byte fixedHeight = Convert.ToByte(split[1], CultureInfo.InvariantCulture);
                    bmfFile.Info.BitField |= (byte)(fixedHeight << 3);
                    break;
                case "stretchH":
                    bmfFile.Info.StretchH = Convert.ToUInt16(split[1], CultureInfo.InvariantCulture);
                    break;
                case "aa":
                    bmfFile.Info.AA = Convert.ToByte(split[1], CultureInfo.InvariantCulture);
                    break;
                case "padding":
                    string[] paddingValues = split[1].Split(',');
                    if (paddingValues.Length == 4)
                    {
                        bmfFile.Info.PaddingUp = Convert.ToByte(paddingValues[0], CultureInfo.InvariantCulture);
                        bmfFile.Info.PaddingRight = Convert.ToByte(paddingValues[1], CultureInfo.InvariantCulture);
                        bmfFile.Info.PaddingDown = Convert.ToByte(paddingValues[2], CultureInfo.InvariantCulture);
                        bmfFile.Info.PaddingLeft = Convert.ToByte(paddingValues[3], CultureInfo.InvariantCulture);
                    }
                    break;
                case "spacing":
                    string[] spacingValues = split[1].Split(',');
                    if (spacingValues.Length == 2)
                    {
                        bmfFile.Info.SpacingHoriz = Convert.ToByte(spacingValues[0], CultureInfo.InvariantCulture);
                        bmfFile.Info.SpacingVert = Convert.ToByte(spacingValues[1], CultureInfo.InvariantCulture);
                    }
                    break;
                case "outline":
                    bmfFile.Info.Outline = Convert.ToByte(split[1], CultureInfo.InvariantCulture);
                    break;
            }
        }
    }

    private static void LoadCommonLine(BitmapFontFile bmfFile, ReadOnlySpan<string> tokens)
    {
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
                    bmfFile.Common.LineHeight = Convert.ToUInt16(split[1], CultureInfo.InvariantCulture);
                    break;
                case "base":
                    bmfFile.Common.Base = Convert.ToUInt16(split[1], CultureInfo.InvariantCulture);
                    break;
                case "scaleW":
                    bmfFile.Common.ScaleW = Convert.ToUInt16(split[1], CultureInfo.InvariantCulture);
                    break;
                case "scaleH":
                    bmfFile.Common.ScaleH = Convert.ToUInt16(split[1], CultureInfo.InvariantCulture);
                    break;
                case "pages":
                    bmfFile.Common.Pages = Convert.ToUInt16(split[1], CultureInfo.InvariantCulture);
                    break;
                case "packed":
                    byte packed = Convert.ToByte(split[1], CultureInfo.InvariantCulture);
                    bmfFile.Common.BitField |= (byte)(packed << 7);
                    break;
                case "alphaChnl":
                    bmfFile.Common.AlphaChnl = Convert.ToByte(split[1], CultureInfo.InvariantCulture);
                    break;
                case "redChnl":
                    bmfFile.Common.RedChnl = Convert.ToByte(split[1], CultureInfo.InvariantCulture);
                    break;
                case "greenChnl":
                    bmfFile.Common.GreenChnl = Convert.ToByte(split[1], CultureInfo.InvariantCulture);
                    break;
                case "blueChnl":
                    bmfFile.Common.BlueChnl = Convert.ToByte(split[1], CultureInfo.InvariantCulture);
                    break;
            }
        }
    }

    private static void LoadPageLine(BitmapFontFile bmfFile, ReadOnlySpan<string> tokens)
    {
        for (int i = 0; i < tokens.Length; ++i)
        {
            string[] split = tokens[i].Split('=');

            if (split.Length != 2)
            {
                continue;
            }

            if (split[0] == "file")
            {
                string page = split[1].Replace("\"", string.Empty);
                bmfFile.Pages.Add(page);
            }
        }
    }

    private static void LoadCharacterLine(BitmapFontFile bmfFile, ReadOnlySpan<string> tokens)
    {
        BmfCharsBlock character = default;

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
                    character.ID = Convert.ToUInt32(split[1], CultureInfo.InvariantCulture);
                    break;
                case "x":
                    character.X = Convert.ToUInt16(split[1], CultureInfo.InvariantCulture);
                    break;
                case "y":
                    character.Y = Convert.ToUInt16(split[1], CultureInfo.InvariantCulture);
                    break;
                case "width":
                    character.Width = Convert.ToUInt16(split[1], CultureInfo.InvariantCulture);
                    break;
                case "height":
                    character.Height = Convert.ToUInt16(split[1], CultureInfo.InvariantCulture);
                    break;
                case "xoffset":
                    character.XOffset = Convert.ToInt16(split[1], CultureInfo.InvariantCulture);
                    break;
                case "yoffset":
                    character.YOffset = Convert.ToInt16(split[1], CultureInfo.InvariantCulture);
                    break;
                case "xadvance":
                    character.XAdvance = Convert.ToInt16(split[1], CultureInfo.InvariantCulture);
                    break;
                case "page":
                    character.Page = Convert.ToByte(split[1], CultureInfo.InvariantCulture);
                    break;
                case "chnl":
                    character.Chnl = Convert.ToByte(split[1], CultureInfo.InvariantCulture);
                    break;
            }
        }

        bmfFile.Characters.Add(character);
    }

    private static void LoadKerningLine(BitmapFontFile bmfFile, ReadOnlySpan<string> tokens)
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

        bmfFile.Kernings.Add(kerning);
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
