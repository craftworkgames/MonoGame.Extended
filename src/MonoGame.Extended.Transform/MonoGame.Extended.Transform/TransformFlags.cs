// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace MonoGame.Extended.Transform
{
    [Flags]
    internal enum TransformFlags : byte
    {
        WorldMatrixIsDirty = 1 << 0,
        LocalMatrixIsDirty = 1 << 1,
        All = WorldMatrixIsDirty | LocalMatrixIsDirty
    }
}
