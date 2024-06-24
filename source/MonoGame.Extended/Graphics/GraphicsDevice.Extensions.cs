// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.


using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics;

public static class GraphicsDeviceExtensions
{

#if FNA
    // MomoGame compatibility layer

    /// <summary>
    /// Draw geometry by indexing into the vertex buffer.
    /// </summary>
    /// <param name="primitiveType">The type of primitives in the index buffer.</param>
    /// <param name="baseVertex">Used to offset the vertex range indexed from the vertex buffer.</param>
    /// <param name="startIndex">The index within the index buffer to start drawing from.</param>
    /// <param name="primitiveCount">The number of primitives to render from the index buffer.</param>
    public static void DrawIndexedPrimitives(this GraphicsDevice graphicsDevice, PrimitiveType primitiveType, int baseVertex, int startIndex, int primitiveCount)
    {
        int minVertexIndex = 0;
        int numVertices = graphicsDevice.GetVertexBuffers()[0].VertexBuffer.VertexCount;

        graphicsDevice.DrawIndexedPrimitives(primitiveType, baseVertex, minVertexIndex, numVertices, startIndex, primitiveCount);
    }
#endif
}
