// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace MonoGame.Extended.Tests;

public class MockContentManager : ContentManager
{
    public MockContentManager() : base(new GameServiceContainer())
    {
    }

    public override T Load<T>(string assetName)
    {
        return default(T);
    }
}
