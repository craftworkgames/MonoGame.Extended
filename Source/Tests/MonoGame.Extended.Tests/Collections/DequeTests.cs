using System;
using System.Linq;
using MonoGame.Extended.Collections;
using Xunit;

namespace MonoGame.Extended.Tests.Collections
{
    public class DequeTests
    {
        private class TestDequeElement
        {
            public int Value { get; set; }
        }

        private readonly Random _random;

        public DequeTests()
        {
            _random = new Random();
        }

        [Fact]
        public void Deque_Constructor_Default()
        {
            var deque = new Deque<object>();
            Assert.True(deque.Count == 0);
            Assert.True(deque.Capacity == 0);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(50)]
        public void Deque_Constructor_Collection(int count)
        {
            var elements = new TestDequeElement[count];
            for (var i = 0; i < count; i++)
            {
                elements[i] = new TestDequeElement
                {
                    Value = i
                };
            }
            var deque = new Deque<TestDequeElement>(elements);
            Assert.True(deque.Count == count);
            Assert.True(deque.Capacity == count);
            for (var index = 0; index < deque.Count; index++)
            {
                Assert.True(deque[index].Value == index);
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(50)]
        public void Deque_Constructor_Capacity(int capacity)
        {
            var deque = new Deque<TestDequeElement>(capacity);
            Assert.True(deque.Count == 0);
            Assert.True(deque.Capacity == capacity);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(50)]
        public void Deque_Clear(int count)
        {
            var elements = new TestDequeElement[count];
            for (var i = 0; i < count; i++)
            {
                elements[i] = new TestDequeElement
                {
                    Value = i
                };
            }
            var deque = new Deque<TestDequeElement>(elements);
            deque.Clear();
            Assert.True(deque.Count == 0);
            Assert.True(deque.Capacity >= count);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(50)]
        public void Deque_Trim_And_Clear(int count)
        {
            var elements = new TestDequeElement[count];
            for (var i = 0; i < count; i++)
            {
                elements[i] = new TestDequeElement
                {
                    Value = i
                };
            }
            var deque = new Deque<TestDequeElement>(elements);
            deque.Clear();
            deque.TrimExcess();
            Assert.True(deque.Count == 0);
            Assert.True(deque.Capacity == 0);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(50)]
        public void Deque_Trim_Front(int count)
        {
            var elements = new TestDequeElement[count];
            for (var i = 0; i < count; i++)
            {
                elements[i] = new TestDequeElement
                {
                    Value = i
                };
            }
            var deque = new Deque<TestDequeElement>(elements);

            for (var i = 0; i < count; i++)
            {
                deque.RemoveFromFront(out _);
                deque.Capacity = deque.Count;
                Assert.True(deque.Count == count - 1 - i);
                Assert.True(deque.Capacity == count - 1 - i);
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(50)]
        public void Deque_Trim_Back(int count)
        {
            var elements = new TestDequeElement[count];
            for (var i = 0; i < count; i++)
            {
                elements[i] = new TestDequeElement
                {
                    Value = i
                };
            }
            var deque = new Deque<TestDequeElement>(elements);

            for (var i = 0; i < count; i++)
            {
                deque.RemoveFromBack(out _);
                deque.Capacity = deque.Count;
                Assert.True(deque.Count == count - 1 - i);
                Assert.True(deque.Capacity == count - 1 - i);
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(50)]
        public void Deque_Add_Front(int count)
        {
            var deque = new Deque<TestDequeElement>();
            for (var i = 0; i < count; i++)
            {
                deque.AddToFront(new TestDequeElement
                {
                    Value = i
                });
            }
            Assert.True(deque.Count == count);
            Assert.True(deque.Capacity >= count);
            for (var index = 0; index < deque.Count; index++)
            {
                var element = deque[index];
                Assert.True(element.Value == deque.Count - 1 - index);
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(50)]
        public void Deque_Add_Back(int count)
        {
            var deque = new Deque<TestDequeElement>();
            for (var i = 0; i < count; i++)
            {
                deque.AddToBack(new TestDequeElement
                {
                    Value = i
                });
            }
            Assert.True(deque.Count == count);
            Assert.True(deque.Capacity >= count);
            for (var index = 0; index < deque.Count; index++)
            {
                var element = deque[index];
                Assert.True(element.Value == index);
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(50)]
        public void Deque_Remove_Front(int count)
        {
            var elements = new TestDequeElement[count];
            for (var i = 0; i < count; i++)
            {
                elements[i] = new TestDequeElement
                {
                    Value = i
                };
            }
            var deque = new Deque<TestDequeElement>(elements);

            var index = 0;
            while (deque.RemoveFromFront(out var element))
            {
                Assert.True(element.Value == index);
                index++;
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(50)]
        public void Deque_Remove_Back(int count)
        {
            var elements = new TestDequeElement[count];
            for (var i = 0; i < count; i++)
            {
                elements[i] = new TestDequeElement
                {
                    Value = i
                };
            }
            var deque = new Deque<TestDequeElement>(elements);

            var index = 0;
            while (deque.RemoveFromBack(out var element))
            {
                Assert.True(element.Value == elements.Length - 1 - index);
                index++;
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(50)]
        public void Deque_Get_Front(int count)
        {
            var elements = new TestDequeElement[count];
            for (var i = 0; i < count; i++)
            {
                elements[i] = new TestDequeElement
                {
                    Value = i
                };
            }
            var deque = new Deque<TestDequeElement>(elements);
            var indices = Enumerable.Range(0, count);
            foreach (var index in indices)
            {
                deque.GetFront(out var element);
                deque.RemoveFromFront();
                Assert.True(element.Value == index);
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(50)]
        public void Deque_Get_Back(int count)
        {
            var elements = new TestDequeElement[count];
            for (var i = 0; i < count; i++)
            {
                elements[i] = new TestDequeElement
                {
                    Value = i
                };
            }
            var deque = new Deque<TestDequeElement>(elements);
            var indices = Enumerable.Range(0, count);
            foreach (var index in indices)
            {
                deque.GetBack(out var element);
                deque.RemoveFromBack();
                Assert.True(element.Value == count - 1 - index);
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(50)]
        public void Deque_Get_Index(int count)
        {
            var elements = new TestDequeElement[count];
            for (var i = 0; i < count; i++)
            {
                elements[i] = new TestDequeElement
                {
                    Value = i
                };
            }
            var deque = new Deque<TestDequeElement>(elements);
            var indices = Enumerable.Range(0, count).ToList().Shuffle(_random);
            foreach (var index in indices)
            {
                deque.Get(index, out var element);
                Assert.True(element.Value == index);
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(50)]
        public void Deque_ForEach_Iteration(int count)
        {
            var elements = new TestDequeElement[count];
            for (var i = 0; i < count; i++)
            {
                elements[i] = new TestDequeElement
                {
                    Value = i
                };
            }
            var deque = new Deque<TestDequeElement>(elements);
            var counter = 0;
            foreach (var element in deque)
            {
                Assert.True(element.Value == counter);
                counter++;
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(50)]
        public void Deque_ForEach_Iteration_Modified(int count)
        {
            var elements = new TestDequeElement[count];
            for (var i = 0; i < count; i++)
            {
                elements[i] = new TestDequeElement
                {
                    Value = i
                };
            }
            var deque = new Deque<TestDequeElement>(elements);
            var counter = 0;
            foreach (var element in deque)
            {
                Assert.True(element.Value == counter);
                counter++;
                deque.RemoveFromFront();
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(50)]
        public void Deque_Remove(int count)
        {
            var elements = new TestDequeElement[count];
            for (var i = 0; i < count; i++)
            {
                elements[i] = new TestDequeElement
                {
                    Value = i
                };
            }
            var deque = new Deque<TestDequeElement>(elements);
            var counter = count;
            while (deque.Count > 0)
            {
                var index = _random.Next(0, deque.Count - 1);
                deque.RemoveAt(index);
                counter--;
                Assert.True(deque.Count == counter);
            }
        }
    }
}
