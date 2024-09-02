// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Graphics;

/// <summary>
/// Represents a nine-patch texture.
/// </summary>
/// <remarks>
///     <para>
///         A nine-patch texture is a specialized texture object used for rendering scalable graphical assets,
///         particularly user interface (UI) elements.It consists of a single texture region subdivided into nine
///         distinct subregions.  When rendered, the four corner subregions remain unscaled, preserving their original
///         dimensions. The top and bottom edge subregions are stretched horizontally, while the left and right edge
///         subregions are stretched vertically.  The central subregion is scaled along both axes to fill the desired
///         dimensions.
///     </para>
///     <para>
///         This approach is highly beneficial for UI components that require dynamic scaling, such as containers for
///         menus, dialog boxes, or other resizable elements.  By leveraging the nine-patch texture, these graphical
///         assets can be seamlessly scaled to different sizes while maintaining their visual integrity and preventing
///         undesirable distortions or stretching artifacts.
///     </para>
/// </remarks>
public class NinePatch
{
    /// <summary>The index representing the top-left patch.</summary>
    public const int TopLeft = 0;

    /// <summary>The index representing the top-middle patch.</summary>
    public const int TopMiddle = 1;

    /// <summary>The index representing the top-right patch.</summary>
    public const int TopRight = 2;

    /// <summary>The index representing the middle-left patch.</summary>
    public const int MiddleLeft = 3;

    /// <summary>The index representing the middle patch.</summary>
    public const int Middle = 4;

    /// <summary>The index representing the middle-right patch.</summary>
    public const int MiddleRight = 5;

    /// <summary>The index representing the bottom-left patch.</summary>
    public const int BottomLeft = 6;

    /// <summary>The index representing the bottom-middle patch.</summary>
    public const int BottomMiddle = 7;

    /// <summary>The index representing the bottom-right patch.</summary>
    public const int BottomRight = 8;

    private readonly Texture2DRegion[] _patches;

    /// <summary>
    /// Gets the name assigned to this nine-patch.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// The size of the border patches around the middle patch.
    /// </summary>
    public Thickness Padding { get; }

    /// <summary>
    /// Gets a read-only span of the texture regions that make up the nine-patch.
    /// </summary>
    /// <remarks>
    /// Elements are in order of top-left, top-middle, top-right, middle-left, middle, middle-right, bottom-left,
    /// bottom-middle, and bottom-right.
    /// </remarks>
    public ReadOnlySpan<Texture2DRegion> Patches => _patches;

    /// <summary>
    /// Initializes a new instance of the <see cref="NinePatch"/> class with the specified patches.
    /// </summary>
    /// <remarks>
    /// The <paramref name="patches"/> array should contain the elements in the order of top-left, top-middle,
    /// top-right, middle-left, middle, middle-right, bottom-left, bottom-middle, and bottom-right.
    /// </remarks>
    /// <param name="patches">An array of nine <see cref="Texture2DRegion"/> objects.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="patches"/> is null.</exception>
    /// <exception cref="ArgumentException">
    /// Thrown if <paramref name="patches"/> does not contain exactly nine elements.
    /// </exception>
    public NinePatch(Texture2DRegion[] patches) : this(patches, null) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="NinePatch"/> class with the specified patches and name.
    /// </summary>
    /// <param name="patches">
    /// An array of nine <see cref="Texture2DRegion"/> objects.
    /// The top, left, bottom and right regions must to be of exactly the same size.
    /// Mid patches can be as small as 1x1.
    /// </param>
    /// <param name="name">
    /// The name of the nine-patch. If null or empty, a default name will be generated based on the texture name of the
    /// top-left patch.
    /// </param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="patches"/> is null.</exception>
    /// <exception cref="ArgumentException">
    /// Thrown if <paramref name="patches"/> does not contain exactly nine elements.
    /// </exception>
    public NinePatch(Texture2DRegion[] patches, string name)
    {
        ArgumentNullException.ThrowIfNull(patches);
        if (patches.Length != 9)
        {
            throw new ArgumentException($"{nameof(patches)} must contain exactly 9 elements.", nameof(patches));
        }

        if (string.IsNullOrEmpty(name))
        {
            name = $"{patches[0].Texture.Name}-nine-patch";
        }

        _patches = patches;

        Size topLeft = patches[NinePatch.TopLeft].Size;
        Size bottomRight = patches[NinePatch.BottomRight].Size;
        Padding = new Thickness(topLeft.Width, topLeft.Height, bottomRight.Width, bottomRight.Height);

        Name = name;
    }
}
