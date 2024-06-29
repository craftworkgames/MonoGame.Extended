// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoGame.Extended.Collections;

namespace MonoGame.Extended.Tests.Collections;

public class ObjectPoolTests
{
    private class TestPoolable : IPoolable
    {
        public Action<IPoolable> ReturnAction { get; private set; }
        public IPoolable NextNode { get; set; }
        public IPoolable PreviousNode { get; set; }

        public void Initialize(Action<IPoolable> returnAction)
        {
            ReturnAction = returnAction;
        }

        public void Return()
        {
            ReturnAction(this);
        }
    }

    [Fact]
    public void ObjectPool_ThrowsNullReferenceException_WhenAllItemsReturnedAndNewCalled()
    {
        // Arrange
        var pool = new ObjectPool<TestPoolable>(() => new TestPoolable(), 2);

        // Act & Assert
        var item1 = pool.New();
        var item2 = pool.New();

        // Return all items to the pool
        item1.Return();
        item2.Return();


        var exception = Record.Exception(() => pool.New());
        Assert.Null(exception);
    }
}
