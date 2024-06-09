// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.BitmapFonts;

public sealed class BitmapFont
{
    public string Face { get; }
    public int Size { get; }
    public int LineHeight { get; }
    public Dictionary<int, BitmapFontCharacter> Characters { get; }


    public BitmapFont(string face, int size, int lineHeight)
    {
        Face = face;
        Size = size;
        LineHeight = lineHeight;
        Characters = new Dictionary<int, BitmapFontCharacter>();
    }
}
